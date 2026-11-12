package dcb

import (
	"sync"
	"testing"
)

type counter struct {
	Key   string
	Delta int
}

func counterTag(key string) Tag { return Tag{Category: "counter", Keys: []string{key}} }
func counterQuery(key string) Query {
	return Query{Items: []QueryItem{{Tags: []Tag{counterTag(key)}, Types: []string{}}}}
}
func describeCounter(e counter) EventDescriptor {
	return EventDescriptor{EventType: "CounterDelta", Tags: []Tag{counterTag(e.Key), {Category: "scope", Keys: []string{"test"}}}}
}
func TestDomainFreeLibrary(t *testing.T) {
	check := func(ok bool) {
		t.Helper()
		if !ok {
			t.Fatal("DCB library probe failed")
		}
	}
	store := NewStore(describeCounter, func(e counter) counter { return e })
	p := NewProjection(0, func(s int, e counter) int { return s + e.Delta }, func(s int) int { return s })
	store.Subscribe(p.Accept)
	store.Subscribe(func(events []Recorded[counter]) bool {
		if len(events) > 0 {
			panic("expected listener failure")
		}
		return true
	})
	first := store.Append("first", counterQuery("a"), 0, []counter{{"a", 1}, {"a", 2}})
	position, total := p.Snapshot()
	check(first.Committed && len(first.Events) == 2 && store.ListenerFaults() == 1 && position == 2 && total == 3)
	check(store.Append("other", counterQuery("b"), 0, []counter{{"b", 9}}).Committed)
	check(!store.Append("stale", counterQuery("a"), 0, []counter{{"a", 99}}).Committed)
	all := store.Read(Query{Items: []QueryItem{}})
	position, total = p.Snapshot()
	check(all.After == 3 && position == 3 && total == 12)
	check(len(store.Read(Query{Items: []QueryItem{{Tags: []Tag{counterTag("a"), {Category: "scope", Keys: []string{"test"}}}, Types: []string{}}}}).Events) == 2)
	check(len(store.Read(Query{Items: []QueryItem{counterQuery("a").Items[0], counterQuery("b").Items[0]}}).Events) == 3)
	check(len(store.Read(Query{Items: []QueryItem{{Tags: []Tag{counterTag("a")}, Types: []string{"OtherType"}}}}).Events) == 0)
	check(p.Accept(all.Events))
	_, total = p.Snapshot()
	check(total == 12)
	gap := NewProjection(0, func(s int, e counter) int { return s + e.Delta }, func(s int) int { return s })
	check(!gap.Accept([]Recorded[counter]{all.Events[0], all.Events[2]}))
	position, total = gap.Snapshot()
	check(position == 0 && total == 0)
	late := NewProjection(0, func(s int, e counter) int { return s + e.Delta }, func(s int) int { return s })
	store.Subscribe(late.Accept)
	_, total = late.Snapshot()
	check(total == 12)
	processor := CommandProcessor[counter, counter, string]{Store: store, QueryFor: func(c counter) Query { return counterQuery(c.Key) }, Decide: func(_ []Recorded[counter], c counter) Preparation[counter, string] {
		if c.Delta > 0 {
			return Preparation[counter, string]{Events: []counter{c}, Errors: []string{}}
		}
		return Preparation[counter, string]{Events: []counter{}, Errors: []string{"NegativeDelta"}}
	}, MaxConflicts: 3}
	ticket := processor.Prepare("retry", counter{"a", 1})
	store.Append("interfere", counterQuery("a"), 3, []counter{{"a", 1}})
	result := processor.Commit(ticket)
	position, total = p.Snapshot()
	_, lateTotal := late.Snapshot()
	check(result.Status == "Committed" && result.Conflicts == 1 && position == 5 && total == 14 && lateTotal == 14)
	check(processor.Commit(processor.Prepare("reject", counter{"a", -1})).Status == "Rejected")
	restored := NewStore(describeCounter, func(e counter) counter { return e })
	restored.Load(store.Read(Query{Items: []QueryItem{}}).Events)
	check(restored.Read(Query{Items: []QueryItem{}}).After == 5)
	refused := false
	func() {
		defer func() {
			if recover() != nil {
				refused = true
			}
		}()
		bad := all.Events[0]
		bad.Tags = []Tag{}
		NewStore(describeCounter, func(e counter) counter { return e }).Load([]Recorded[counter]{bad})
	}()
	check(refused)
	racing := NewStore(describeCounter, func(e counter) counter { return e })
	var ready, done sync.WaitGroup
	ready.Add(2)
	done.Add(2)
	start := make(chan struct{})
	results := make(chan bool, 2)
	for i := 0; i < 2; i++ {
		go func() {
			defer done.Done()
			read := racing.Read(counterQuery("a"))
			ready.Done()
			<-start
			results <- racing.Append("race", counterQuery("a"), read.After, []counter{{"a", 1}}).Committed
		}()
	}
	ready.Wait()
	close(start)
	done.Wait()
	close(results)
	winners := 0
	for ok := range results {
		if ok {
			winners++
		}
	}
	check(winners == 1 && racing.Read(Query{Items: []QueryItem{}}).After == 1)
}

type counterHandler struct{}

func (counterHandler) Validate(n int) []string {
	if n > 0 {
		return []string{}
	}
	return []string{"positive delta required"}
}
func (counterHandler) QueryFor(n int) Query {
	if n <= 0 {
		panic("validation did not precede query")
	}
	return counterQuery("executor")
}
func (counterHandler) Handle(_ []Recorded[counter], n int) Preparation[counter, string] {
	return Preparation[counter, string]{Events: []counter{{Key: "executor", Delta: n}}, Errors: []string{}}
}
func TestExecutorWithUnrelatedCommand(t *testing.T) {
	store := NewStore(describeCounter, func(e counter) counter { return e })
	var executor Executor[int, counter, string] = DcbExecutor[int, counter, string]{Store: store}
	if executor.Execute("valid", 5, counterHandler{}).Appended != 1 {
		t.Fatal("valid command")
	}
	if executor.Execute("invalid", 0, counterHandler{}).Status != "Rejected" || store.Read(Query{}).After != 1 {
		t.Fatal("invalid command advanced")
	}
}
