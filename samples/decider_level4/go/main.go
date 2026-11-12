package main

import (
	"crypto/rand"
	"encoding/hex"
	"encoding/json"
	"errors"
	"example.com/dcb-school/dcb"
	d "example.com/dcb-school/domain"
	"fmt"
	"io"
	"net/http"
	"os"
	"strings"
	"sync"
	"time"
)

type httpError struct {
	code    int
	message string
}

func (e httpError) Error() string { return e.message }
func bad()                        { panic(httpError{400, "InvalidRequest"}) }

var schemas map[string]any

func validate(body any, schema map[string]any) {
	if ref, ok := schema["$ref"].(string); ok {
		validate(body, schemas[strings.TrimPrefix(ref, "#/components/schemas/")].(map[string]any))
		return
	}
	if choices, ok := schema["oneOf"].([]any); ok {
		obj, ok := body.(map[string]any)
		if !ok {
			bad()
		}
		kind, ok := obj["$type"].(string)
		if !ok {
			bad()
		}
		found := false
		for _, c := range choices {
			if c.(map[string]any)["$ref"] == "#/components/schemas/"+kind {
				found = true
			}
		}
		if !found {
			bad()
		}
		validate(body, schemas[kind].(map[string]any))
		return
	}
	switch schema["type"] {
	case "object":
		obj, ok := body.(map[string]any)
		if !ok {
			bad()
		}
		properties := schema["properties"].(map[string]any)
		for _, key := range schema["required"].([]any) {
			if _, ok := obj[key.(string)]; !ok {
				bad()
			}
		}
		for k, v := range obj {
			child, ok := properties[k]
			if !ok {
				bad()
			}
			validate(v, child.(map[string]any))
		}
	case "string":
		s, ok := body.(string)
		if !ok {
			bad()
		}
		if c, ok := schema["const"]; ok && s != c {
			bad()
		}
		if values, ok := schema["enum"].([]any); ok {
			found := false
			for _, v := range values {
				if s == v {
					found = true
				}
			}
			if !found {
				bad()
			}
		}
		if _, ok := schema["minLength"]; ok && (strings.TrimSpace(s) == "" || len([]rune(s)) > 128) {
			bad()
		}
	case "integer":
		n, ok := body.(float64)
		if !ok || n < float64(-2147483648) || n > float64(2147483647) || n != float64(int32(n)) {
			bad()
		}
	}
}

type ticket struct {
	created time.Time
	command dcb.PreparedCommand[d.Rejection]
}

func main() {
	spec, err := os.ReadFile(os.Getenv("API_SPEC"))
	if err != nil {
		panic(err)
	}
	var doc map[string]any
	if json.Unmarshal(spec, &doc) != nil {
		panic("spec")
	}
	schemas = doc["components"].(map[string]any)["schemas"].(map[string]any)
	var journal dcb.JsonJournal = dcb.NewMemoryJsonJournal()
	if path := os.Getenv("DECIDER_PERSISTENT"); path != "" {
		journal = dcb.NewSqliteJournal(path, "FULL")
	}
	store := dcb.NewJsonEventStore(journal, d.DescribeEvent, func(e d.Event) string {
		b, e2 := json.Marshal(e)
		if e2 != nil {
			panic(e2)
		}
		return string(b)
	}, func(s string) d.Event {
		e, e2 := d.DecodeEvent([]byte(s))
		if e2 != nil {
			panic(e2)
		}
		return e
	})
	defer store.Close()
	projection := d.NewSchoolProjection()
	store.Subscribe(projection.Accept)
	executor := dcb.DcbExecutor[d.Command, d.Event, d.Rejection]{Store: store}
	handler := d.SchoolHandler{}
	tickets := map[string]ticket{}
	var gate sync.Mutex
	ttl := 60 * time.Second
	if s := os.Getenv("TICKET_TTL_SECONDS"); s != "" {
		ttl, err = time.ParseDuration(s + "s")
		if err != nil {
			panic(err)
		}
	}
	expire := func() {
		for id, t := range tickets {
			if time.Since(t.created) >= ttl {
				delete(tickets, id)
			}
		}
	}
	result := func(t dcb.PreparedCommand[d.Rejection]) (int, any) {
		r := t.Commit()
		code := 200
		if r.Status == "Rejected" {
			code = 422
		} else if r.Status != "Committed" {
			code = 409
		}
		return code, map[string]any{"status": r.Status, "conflicts": r.Conflicts, "appended": r.Appended, "position": r.Position, "projectionPosition": projection.Checkpoint(), "errors": r.Errors}
	}
	handlerHTTP := http.HandlerFunc(func(w http.ResponseWriter, r *http.Request) {
		code := 200
		var body any
		defer func() {
			if e := recover(); e != nil {
				if he, ok := e.(httpError); ok {
					code = he.code
					body = map[string]any{"error": he.message}
				} else {
					fmt.Fprintln(os.Stderr, e)
					code = 500
					body = map[string]any{"error": "InternalError"}
				}
			}
			w.Header().Set("Content-Type", "application/json")
			w.WriteHeader(code)
			_ = json.NewEncoder(w).Encode(body)
		}()
		route := r.URL.Path
		get := route == "/health" || route == "/school" || route == "/status" || strings.HasPrefix(route, "/students/")
		post := route == "/commands" || route == "/tickets" || strings.HasPrefix(route, "/tickets/") && strings.HasSuffix(route, "/commit") && len(strings.Split(route, "/")) == 4
		if !get && !post {
			panic(httpError{404, "NotFound"})
		}
		method := "POST"
		if get {
			method = "GET"
		}
		if r.Method != method {
			w.Header().Set("Allow", method)
			panic(httpError{405, "MethodNotAllowed"})
		}
		if get {
			store.Refresh()
			if store.ListenerFaults() != 0 {
				panic("projection unavailable")
			}
			switch route {
			case "/health":
				body = map[string]any{"status": "ok"}
			case "/status":
				body = map[string]any{"eventPosition": store.Read(dcb.Query{Items: []dcb.QueryItem{}}).After, "projectionPosition": projection.Checkpoint(), "listenerFaults": store.ListenerFaults()}
			case "/school":
				p, s := projection.Snapshot()
				body = map[string]any{"position": p, "state": s}
			default:
				p, s, ok := projection.FindStudent(strings.TrimPrefix(route, "/students/"))
				if !ok {
					panic(httpError{404, "NotFound"})
				}
				body = map[string]any{"position": p, "student": s}
			}
		} else if route == "/commands" || route == "/tickets" {
			if strings.TrimSpace(strings.Split(r.Header.Get("Content-Type"), ";")[0]) != "application/json" {
				panic(httpError{415, "UnsupportedMediaType"})
			}
			raw, e := io.ReadAll(http.MaxBytesReader(w, r.Body, 1024*1024))
			if e != nil {
				var large *http.MaxBytesError
				if errors.As(e, &large) {
					panic(httpError{413, "PayloadTooLarge"})
				}
				bad()
			}
			var input any
			if json.Unmarshal(raw, &input) != nil {
				bad()
			}
			validate(input, schemas["CommandRequest"].(map[string]any))
			obj := input.(map[string]any)
			commandRaw, _ := json.Marshal(obj["command"])
			c, e := d.DecodeCommand(commandRaw)
			if e != nil {
				bad()
			}
			t := executor.Prepare(obj["commandId"].(string), c, handler)
			if route == "/commands" || !t.WasRead {
				code, body = result(t)
			} else {
				func() {
					gate.Lock()
					defer gate.Unlock()
					expire()
					if len(tickets) >= 1024 {
						panic(httpError{429, "TooManyTickets"})
					}
					key := make([]byte, 16)
					if _, e := rand.Read(key); e != nil {
						panic(e)
					}
					id := hex.EncodeToString(key)
					tickets[id] = ticket{time.Now(), t}
					code = 201
					body = map[string]any{"ticket": id, "position": t.After}
				}()
			}
		} else {
			var t ticket
			func() {
				gate.Lock()
				defer gate.Unlock()
				expire()
				id := strings.Split(route, "/")[2]
				var ok bool
				t, ok = tickets[id]
				if !ok {
					panic(httpError{404, "NotFound"})
				}
				delete(tickets, id)
			}()
			code, body = result(t.command)
		}
	})
	port := os.Getenv("PORT")
	if port == "" {
		port = "8080"
	}
	server := &http.Server{Addr: "127.0.0.1:" + port, Handler: handlerHTTP, ReadHeaderTimeout: 5 * time.Second, ReadTimeout: 30 * time.Second, WriteTimeout: 30 * time.Second}
	if e := server.ListenAndServe(); e != nil && e != http.ErrServerClosed {
		panic(e)
	}
}
