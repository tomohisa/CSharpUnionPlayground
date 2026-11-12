using System.Collections.Immutable;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;
using Playground.Dcb;
namespace SchoolDomain;

public sealed record ProjectionReport(int Size,int Updates,int Rows,double SeedSeconds,double UpdateSeconds,double GrowthSeconds,double SnapshotSeconds,long UpdateWork,long GrowthWork);
public static class ProjectionTests
{
    private static Recorded<Event> Envelope(int position,Event e)=>new(position,"scale","projection-test",[],e);
    private static void Require(bool value){if(!value)throw new Exception("projection scaling invariant failed");}
    public static void Run(int size)
    {
        const int updates=1000;
        var p=new SchoolProjection();int position=0;
        void Send(Event e){Require(p.Accept([Envelope(++position,e)]));}
        var timer=Stopwatch.StartNew();
        Send(new ClassOpened("a",Course.Foundation,size+updates+1));
        for(int i=0;i<size;i++){var id="s"+i;Send(new StudentRegistered(id,Grade.Year3));Send(new StudentEnrolled("a",id,Course.Foundation));}
        double seed=timer.Elapsed.TotalSeconds;
        var old=p.Snapshot().State;int before=p.Checkpoint;long work=p.Work;
        Require(!p.Accept([Envelope(before+1,new StudentPromoted("s0",Grade.Year1)),Envelope(before+2,new AttendanceRecorded("a","s0"))]));
        Require(p.Checkpoint==before && p.Work==work && p.Snapshot().State.Students.Single(s=>s.StudentId=="s0").Grade==Grade.Year3);
        Require(!p.Accept([Envelope(before+1,new StudentPromoted("s0",Grade.Year1)),Envelope(before+3,new StudentPromoted("s0",Grade.Year1))]));
        Require(p.Checkpoint==before && p.Work==work);
        Require(p.Accept([Envelope(1,new ClassOpened("ignored",Course.Foundation,1))]) && p.Work==work);
        int stableRows=p.RowCount;
        for(int i=0;i<100;i++){Send(new StudentDropped("a","s1"));Send(new StudentEnrolled("a","s1",Course.Foundation));}
        var order=Decider.Members(p.Snapshot().State.Classes[0]);
        Require(p.RowCount==stableRows && order.Length==size && order[0]=="s0" && order[1]=="s2" && order[^1]=="s1");
        timer.Restart();work=p.Work;
        for(int i=size;i<size+updates;i++){var id="s"+i;Send(new StudentRegistered(id,Grade.Year3));Send(new StudentEnrolled("a",id,Course.Foundation));}
        double growth=timer.Elapsed.TotalSeconds;long growthWork=p.Work-work;
        Require(growthWork==8L*updates); // Constant touched-row operations, irrespective of roster/table size.
        Require(Decider.Members(old.Classes[0]).Length==size && old.Students.Length==size);
        Send(new ClassStarted("a","2026-04-01"));Send(new StudyStarted("a","s0"));
        timer.Restart();work=p.Work;
        for(int i=0;i<updates;i++)Send(new AttendanceRecorded("a","s0"));
        double update=timer.Elapsed.TotalSeconds;long updateWork=p.Work-work;
        Require(updateWork==3L*updates);
        timer.Restart();var snapshot=p.Snapshot().State;double query=timer.Elapsed.TotalSeconds;
        Require(snapshot.Students.Length==size+updates && Decider.Members(snapshot.Classes[0]).Length==size+updates);
        Require(snapshot.Enrollments.Single(e=>e.StudentId=="s0").Progress is Studying {Attendance:updates});
        Console.WriteLine(JsonSerializer.Serialize(new ProjectionReport(size,updates,p.RowCount,seed,update,growth,query,updateWork,growthWork),ProjectionJson.Default.ProjectionReport));
    }
}
[JsonSourceGenerationOptions(PropertyNamingPolicy=JsonKnownNamingPolicy.CamelCase)]
[JsonSerializable(typeof(ProjectionReport))]
internal partial class ProjectionJson:JsonSerializerContext;
