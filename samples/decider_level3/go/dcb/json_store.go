package dcb

import (
	"bytes"
	"database/sql"
	"encoding/binary"
	"encoding/hex"
	"encoding/json"
	_ "github.com/mattn/go-sqlite3"
	"os"
	"path/filepath"
	"sort"
	"strings"
	"sync"
)

// Journals contain JSON and generic metadata, never school types.
type JsonEvent struct {
	Position                                    int
	CommandId, EventType, PayloadJson, TagsJson string
}
type JsonJournal interface {
	Read(Query, int) (int, []JsonEvent)
	Append(string, Query, int, []JsonEvent) (bool, []JsonEvent)
	Load([]JsonEvent)
	Close()
	EngineVersion() string
}

func must(e error) {
	if e != nil {
		panic(e)
	}
}
func Keys(keys []string) []byte {
	b := bytes.NewBuffer([]byte{})
	for _, k := range keys {
		must(binary.Write(b, binary.BigEndian, uint32(len([]byte(k)))))
		b.WriteString(k)
	}
	return b.Bytes()
}
func tagKey(t Tag) string { return hex.EncodeToString(Keys(append([]string{t.Category}, t.Keys...))) }
func decodeTags(s string) []Tag {
	var ts []Tag
	must(json.Unmarshal([]byte(s), &ts))
	if ts == nil {
		panic("tags must be an array")
	}
	seen := map[string]bool{}
	for _, t := range ts {
		k := tagKey(t)
		if seen[k] || t.Keys == nil {
			panic("invalid tag")
		}
		seen[k] = true
	}
	return ts
}

type MemoryJsonJournal struct {
	gate        sync.Mutex
	log         []JsonEvent
	tags, types map[string][]int
}

func NewMemoryJsonJournal() *MemoryJsonJournal {
	return &MemoryJsonJournal{tags: map[string][]int{}, types: map[string][]int{}}
}
func (m *MemoryJsonJournal) selectPositions(q Query, after int) []int {
	if after < 0 || after > len(m.log) {
		panic("invalid position")
	}
	all := map[int]bool{}
	items := q.Items
	if len(items) == 0 {
		items = []QueryItem{{}}
	}
	for _, it := range items {
		var sets [][]int
		for _, t := range it.Tags {
			sets = append(sets, m.tags[tagKey(t)])
		}
		if len(it.Types) > 0 {
			var ps []int
			for _, t := range it.Types {
				ps = append(ps, m.types[t]...)
			}
			sets = append(sets, ps)
		}
		if len(sets) == 0 {
			for p := after + 1; p <= len(m.log); p++ {
				all[p] = true
			}
			continue
		}
		selected := map[int]bool{}
		for _, p := range sets[0] {
			if p > after {
				selected[p] = true
			}
		}
		for _, ps := range sets[1:] {
			next := map[int]bool{}
			for _, p := range ps {
				if selected[p] {
					next[p] = true
				}
			}
			selected = next
		}
		for p := range selected {
			all[p] = true
		}
	}
	out := make([]int, 0, len(all))
	for p := range all {
		out = append(out, p)
	}
	sort.Ints(out)
	return out
}
func (m *MemoryJsonJournal) Read(q Query, after int) (int, []JsonEvent) {
	m.gate.Lock()
	defer m.gate.Unlock()
	out := []JsonEvent{}
	for _, p := range m.selectPositions(q, after) {
		out = append(out, m.log[p-1])
	}
	return len(m.log), out
}
func (m *MemoryJsonJournal) Append(id string, q Query, after int, events []JsonEvent) (bool, []JsonEvent) {
	m.gate.Lock()
	defer m.gate.Unlock()
	if id == "" || len(events) == 0 {
		panic("invalid append")
	}
	if len(m.selectPositions(q, after)) > 0 {
		return false, nil
	}
	rows := append([]JsonEvent{}, events...)
	for i := range rows {
		rows[i].Position = len(m.log) + i + 1
		rows[i].CommandId = id
		decodeTags(rows[i].TagsJson)
	}
	for _, e := range rows {
		m.log = append(m.log, e)
		m.types[e.EventType] = append(m.types[e.EventType], e.Position)
		for _, t := range decodeTags(e.TagsJson) {
			k := tagKey(t)
			m.tags[k] = append(m.tags[k], e.Position)
		}
	}
	return true, rows
}
func (m *MemoryJsonJournal) Load(events []JsonEvent) {
	m.gate.Lock()
	defer m.gate.Unlock()
	if len(m.log) > 0 {
		panic("load requires empty journal")
	}
	for i, e := range events {
		if e.Position != i+1 {
			panic("invalid position")
		}
		decodeTags(e.TagsJson)
	}
	for _, e := range events {
		m.log = append(m.log, e)
		m.types[e.EventType] = append(m.types[e.EventType], e.Position)
		for _, t := range decodeTags(e.TagsJson) {
			k := tagKey(t)
			m.tags[k] = append(m.tags[k], e.Position)
		}
	}
}
func (m *MemoryJsonJournal) Close()                {}
func (m *MemoryJsonJournal) EngineVersion() string { return "memory-json-v1" }

type SqliteJournal struct {
	gate    sync.Mutex
	db      *sql.DB
	stmts   map[string]*sql.Stmt
	version string
}

func NewSqliteJournal(path, durability string) *SqliteJournal {
	if durability != "FULL" && durability != "NORMAL" {
		panic("invalid durability")
	}
	must(os.MkdirAll(filepath.Dir(path), 0755))
	db, e := sql.Open("sqlite3", path)
	must(e)
	db.SetMaxOpenConns(1)
	s := &SqliteJournal{db: db, stmts: map[string]*sql.Stmt{}}
	for _, q := range []string{"PRAGMA journal_mode=WAL", "PRAGMA synchronous=" + durability, "PRAGMA foreign_keys=ON", "PRAGMA busy_timeout=5000", "PRAGMA cache_size=-2048", "PRAGMA wal_autocheckpoint=1000", "CREATE TABLE IF NOT EXISTS events(position INTEGER PRIMARY KEY,command_id TEXT NOT NULL,event_type TEXT NOT NULL,payload_json TEXT NOT NULL,tags_json TEXT NOT NULL)", "CREATE TABLE IF NOT EXISTS event_tags(position INTEGER NOT NULL REFERENCES events(position),category TEXT NOT NULL,keys_key BLOB NOT NULL,PRIMARY KEY(position,category,keys_key))", "CREATE INDEX IF NOT EXISTS tags_match ON event_tags(category,keys_key,position)", "CREATE INDEX IF NOT EXISTS events_type_position ON events(event_type,position)"} {
		s.exec(q)
	}
	var v int
	must(db.QueryRow("PRAGMA user_version").Scan(&v))
	if v != 0 && v != 1 {
		panic("unsupported schema version")
	}
	s.exec("PRAGMA user_version=1")
	must(db.QueryRow("SELECT sqlite_version()").Scan(&s.version))
	return s
}
func (s *SqliteJournal) stmt(q string) *sql.Stmt {
	p := s.stmts[q]
	if p == nil {
		var e error
		p, e = s.db.Prepare(q)
		must(e)
		s.stmts[q] = p
	}
	return p
}
func (s *SqliteJournal) exec(q string, args ...any) {
	var e error
	if len(args) == 0 {
		_, e = s.db.Exec(q)
	} else {
		_, e = s.stmt(q).Exec(args...)
	}
	must(e)
}
func (s *SqliteJournal) high() int {
	var n int
	must(s.stmt("SELECT COALESCE(MAX(position),0) FROM events").QueryRow().Scan(&n))
	return n
}
func selection(q Query, low, high int, exists bool) (string, []any) {
	args := []any{}
	param := func(v any) string { args = append(args, v); return "?" }
	rng := func() string { return "position>" + param(low) + " AND position<=" + param(high) }
	all := len(q.Items) == 0
	for _, i := range q.Items {
		if len(i.Tags) == 0 && len(i.Types) == 0 {
			all = true
		}
	}
	condition := ""
	if all {
		condition = rng()
	} else {
		alts := []string{}
		for _, it := range q.Items {
			terms := []string{}
			for _, t := range it.Tags {
				terms = append(terms, "SELECT position FROM event_tags WHERE category="+param(t.Category)+" AND keys_key="+param(Keys(t.Keys))+" AND "+rng())
			}
			if len(it.Types) > 0 {
				ps := []string{}
				for _, t := range it.Types {
					ps = append(ps, param(t))
				}
				terms = append(terms, "SELECT position FROM events WHERE event_type IN ("+strings.Join(ps, ",")+") AND "+rng())
			}
			alts = append(alts, "SELECT position FROM ("+strings.Join(terms, " INTERSECT ")+")")
		}
		condition = "position IN (" + strings.Join(alts, " UNION ") + ")"
	}
	cols, end := "position,command_id,event_type,payload_json,tags_json", " ORDER BY position"
	if exists {
		cols, end = "1", " LIMIT 1"
	}
	return "SELECT " + cols + " FROM events WHERE " + condition + end, args
}
func (s *SqliteJournal) Read(q Query, after int) (int, []JsonEvent) {
	s.gate.Lock()
	defer s.gate.Unlock()
	s.exec("BEGIN")
	done := false
	defer func() {
		if !done {
			s.exec("ROLLBACK")
		}
	}()
	high := s.high()
	if after < 0 || after > high {
		panic("invalid position")
	}
	query, args := selection(q, after, high, false)
	rs, e := s.stmt(query).Query(args...)
	must(e)
	out := []JsonEvent{}
	for rs.Next() {
		var row JsonEvent
		must(rs.Scan(&row.Position, &row.CommandId, &row.EventType, &row.PayloadJson, &row.TagsJson))
		out = append(out, row)
	}
	must(rs.Err())
	must(rs.Close())
	s.exec("COMMIT")
	done = true
	return high, out
}
func (s *SqliteJournal) Append(id string, q Query, after int, events []JsonEvent) (bool, []JsonEvent) {
	s.gate.Lock()
	defer s.gate.Unlock()
	s.exec("BEGIN IMMEDIATE")
	done := false
	defer func() {
		if !done {
			s.exec("ROLLBACK")
		}
	}()
	high := s.high()
	if id == "" || len(events) == 0 || after < 0 || after > high {
		panic("invalid append")
	}
	query, args := selection(q, after, high, true)
	var one int
	e := s.stmt(query).QueryRow(args...).Scan(&one)
	if e == nil {
		s.exec("ROLLBACK")
		done = true
		return false, nil
	}
	if e != sql.ErrNoRows {
		must(e)
	}
	rows := append([]JsonEvent{}, events...)
	for i := range rows {
		r := &rows[i]
		r.Position = high + i + 1
		r.CommandId = id
		s.exec("INSERT INTO events VALUES(?,?,?,?,?)", r.Position, r.CommandId, r.EventType, r.PayloadJson, r.TagsJson)
		for _, t := range decodeTags(r.TagsJson) {
			s.exec("INSERT INTO event_tags VALUES(?,?,?)", r.Position, t.Category, Keys(t.Keys))
		}
	}
	s.exec("COMMIT")
	done = true
	return true, rows
}
func (s *SqliteJournal) Load(events []JsonEvent) {
	s.gate.Lock()
	defer s.gate.Unlock()
	s.exec("BEGIN IMMEDIATE")
	done := false
	defer func() {
		if !done {
			s.exec("ROLLBACK")
		}
	}()
	if s.high() != 0 {
		panic("load requires empty journal")
	}
	for i, e := range events {
		if e.Position != i+1 {
			panic("invalid position")
		}
		s.exec("INSERT INTO events VALUES(?,?,?,?,?)", e.Position, e.CommandId, e.EventType, e.PayloadJson, e.TagsJson)
		for _, t := range decodeTags(e.TagsJson) {
			s.exec("INSERT INTO event_tags VALUES(?,?,?)", e.Position, t.Category, Keys(t.Keys))
		}
	}
	s.exec("COMMIT")
	done = true
}
func (s *SqliteJournal) Close() {
	for _, p := range s.stmts {
		must(p.Close())
	}
	must(s.db.Close())
}
func (s *SqliteJournal) EngineVersion() string { return s.version }

type JsonEventStore[E any] struct {
	gate                                         sync.Mutex
	journal                                      JsonJournal
	describe                                     func(E) EventDescriptor
	encode                                       func(E) string
	decode                                       func(string) E
	listeners                                    []func([]Recorded[E]) bool
	published, Faults                            int
	Encoded, Decoded, EncodedBytes, DecodedBytes int64
}

func NewJsonEventStore[E any](j JsonJournal, d func(E) EventDescriptor, e func(E) string, r func(string) E) *JsonEventStore[E] {
	return &JsonEventStore[E]{journal: j, describe: d, encode: e, decode: r}
}
func (s *JsonEventStore[E]) encodeEvent(id string, e E) JsonEvent {
	m := s.describe(e)
	if m.EventType == "" {
		panic("event type required")
	}
	body := s.encode(e)
	s.Encoded++
	s.EncodedBytes += int64(len(body))
	raw, err := json.Marshal(m.Tags)
	must(err)
	return JsonEvent{CommandId: id, EventType: m.EventType, PayloadJson: body, TagsJson: string(raw)}
}
func (s *JsonEventStore[E]) decodeAll(rows []JsonEvent) []Recorded[E] {
	out := make([]Recorded[E], 0, len(rows))
	for _, e := range rows {
		p := s.decode(e.PayloadJson)
		ts := decodeTags(e.TagsJson)
		meta := s.describe(p)
		if !(Query{Items: []QueryItem{{Tags: meta.Tags, Types: []string{meta.EventType}}}}).Matches(e.EventType, ts) || len(ts) != len(meta.Tags) {
			panic("invalid stored metadata")
		}
		s.Decoded++
		s.DecodedBytes += int64(len(e.PayloadJson))
		out = append(out, Recorded[E]{e.Position, e.CommandId, e.EventType, ts, p})
	}
	return out
}
func (s *JsonEventStore[E]) Read(q Query) ReadResult[E] {
	s.gate.Lock()
	defer s.gate.Unlock()
	high, rows := s.journal.Read(q, 0)
	return ReadResult[E]{high, s.decodeAll(rows)}
}
func (s *JsonEventStore[E]) refresh() {
	if len(s.listeners) == 0 {
		return
	}
	high, rows := s.journal.Read(Query{}, s.published)
	events := s.decodeAll(rows)
	active := s.listeners[:0]
	for _, l := range s.listeners {
		if l(events) {
			active = append(active, l)
		} else {
			s.Faults++
		}
	}
	s.listeners = active
	s.published = high
}
func (s *JsonEventStore[E]) Refresh() { s.gate.Lock(); defer s.gate.Unlock(); s.refresh() }
func (s *JsonEventStore[E]) Append(id string, q Query, after int, batch []E) AppendResult[E] {
	s.gate.Lock()
	defer s.gate.Unlock()
	rows := []JsonEvent{}
	for _, e := range batch {
		rows = append(rows, s.encodeEvent(id, e))
	}
	ok, rows := s.journal.Append(id, q, after, rows)
	if !ok {
		return AppendResult[E]{false, []Recorded[E]{}}
	}
	if len(s.listeners) > 0 {
		s.refresh()
	} else {
		s.published = rows[len(rows)-1].Position
	}
	return AppendResult[E]{true, s.decodeAll(rows)}
}
func (s *JsonEventStore[E]) Subscribe(l func([]Recorded[E]) bool) {
	s.gate.Lock()
	defer s.gate.Unlock()
	if len(s.listeners) > 0 {
		s.refresh()
	}
	high, rows := s.journal.Read(Query{}, 0)
	if !l(s.decodeAll(rows)) {
		panic("replay failed")
	}
	s.listeners = append(s.listeners, l)
	s.published = high
}
func (s *JsonEventStore[E]) Load(events []Recorded[E]) {
	s.gate.Lock()
	defer s.gate.Unlock()
	if len(s.listeners) > 0 {
		panic("load before subscribe")
	}
	rows := []JsonEvent{}
	for i, e := range events {
		row := s.encodeEvent(e.CommandId, e.Payload)
		if e.Position != i+1 || e.CommandId == "" || row.EventType != e.EventType || len(decodeTags(row.TagsJson)) != len(e.Tags) || !(Query{Items: []QueryItem{{Tags: e.Tags}}}).Matches(row.EventType, decodeTags(row.TagsJson)) {
			panic("invalid import")
		}
		row.Position = e.Position
		rows = append(rows, row)
	}
	s.journal.Load(rows)
}
func (s *JsonEventStore[E]) journalHigh() int      { high, _ := s.journal.Read(Query{}, 0); return high }
func (s *JsonEventStore[E]) Close()                { s.journal.Close() }
func (s *JsonEventStore[E]) EngineVersion() string { return s.journal.EngineVersion() }
func (s *JsonEventStore[E]) ListenerFaults() int   { return s.Faults }
