namespace SchoolDomain
open Playground.Dcb
open System.Diagnostics
open System.Text.Json
module ProjectionTests =
    let run size =
        let updates=1000
        let p=SchoolProjection()
        let mutable position=0
        let envelope position payload:Recorded<Event>={Position=position;CommandId="scale";EventType="projection-test";Tags=[];Payload=payload}
        let require value=if not value then failwith "projection scaling invariant failed"
        let send event=position<-position+1;require(p.Accept [envelope position event])
        let timer=Stopwatch.StartNew()
        send(Event.ClassOpened("a",Foundation,size+updates+1))
        for i in 0..size-1 do
            let id="s"+string i
            send(Event.StudentRegistered(id,Year3));send(Event.StudentEnrolled("a",id,Foundation))
        let seedSeconds=timer.Elapsed.TotalSeconds
        let _,old=p.Snapshot()
        let before,work=p.Checkpoint,p.Work
        require(not(p.Accept [envelope (before+1) (Event.StudentPromoted("s0",Year1));envelope (before+2) (Event.AttendanceRecorded("a","s0"))]))
        require(p.Checkpoint=before && p.Work=work && ((snd(p.Snapshot())).Students |> List.find(fun s->s.StudentId="s0")).Grade=Year3)
        require(not(p.Accept [envelope (before+1) (Event.StudentPromoted("s0",Year1));envelope (before+3) (Event.StudentPromoted("s0",Year1))]))
        require(p.Checkpoint=before && p.Work=work)
        require(p.Accept [envelope 1 (Event.ClassOpened("ignored",Foundation,1))] && p.Work=work)
        let stableRows=p.RowCount
        for _ in 1..100 do send(Event.StudentDropped("a","s1"));send(Event.StudentEnrolled("a","s1",Foundation))
        let order=Decider.members (snd(p.Snapshot())).Classes.Head
        require(p.RowCount=stableRows && order.Length=size && order[0]="s0" && order[1]="s2" && List.last order="s1")
        timer.Restart()
        let work=p.Work
        for i in size..size+updates-1 do
            let id="s"+string i
            send(Event.StudentRegistered(id,Year3));send(Event.StudentEnrolled("a",id,Foundation))
        let growthSeconds,growthWork=timer.Elapsed.TotalSeconds,p.Work-work
        require(growthWork<=10L*int64 updates && (Decider.members old.Classes.Head).Length=size && old.Students.Length=size)
        send(Event.ClassStarted("a","2026-04-01"));send(Event.StudyStarted("a","s0"))
        timer.Restart()
        let work=p.Work
        for _ in 1..updates do send(Event.AttendanceRecorded("a","s0"))
        let updateSeconds,updateWork=timer.Elapsed.TotalSeconds,p.Work-work
        require(updateWork=3L*int64 updates)
        timer.Restart()
        let _,snapshot=p.Snapshot()
        let snapshotSeconds=timer.Elapsed.TotalSeconds
        require(snapshot.Students.Length=size+updates && (Decider.members snapshot.Classes.Head).Length=size+updates)
        require((snapshot.Enrollments |> List.find(fun e->e.StudentId="s0")).Progress=Progress.Studying updates)
        printfn "%s" (JsonSerializer.Serialize {|size=size;updates=updates;rows=p.RowCount;seedSeconds=seedSeconds;updateSeconds=updateSeconds;growthSeconds=growthSeconds;snapshotSeconds=snapshotSeconds;updateWork=updateWork;growthWork=growthWork|})
