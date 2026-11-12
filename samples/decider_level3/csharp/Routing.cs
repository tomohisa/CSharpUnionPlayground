using System.Collections.Immutable;
using Playground.Dcb;
namespace SchoolDomain;
public static class Routing
{
    public static Tag Student(string id) => new("student",[id]);
    public static Tag Class(string id) => new("class",[id]);
    public static Tag Enrollment(string room,string student) => new("enrollment",[room,student]);
    private static Query One(params Tag[] tags) => new([.. tags.Select(t => new QueryItem([t],[]))]);
    public static Query QueryFor(Command command) => command switch
    {
        RegisterStudent c => One(Student(c.StudentId)),
        PromoteStudent c => One(Student(c.StudentId)),
        OpenClass c => One(Class(c.ClassId)),
        Enroll c => One(Student(c.StudentId),Class(c.ClassId)),
        Drop c => One(Class(c.ClassId)),
        StartClass c => One(Class(c.ClassId)),
        Attend c => One(Class(c.ClassId)),
        Assess c => One(Class(c.ClassId)),
        CompleteClass c => One(Class(c.ClassId)),
    };
    public static EventDescriptor Describe(Event e) => e switch
    {
        StudentRegistered x => new("StudentRegistered",[Student(x.StudentId)]),
        StudentPromoted x => new("StudentPromoted",[Student(x.StudentId)]),
        ClassOpened x => new("ClassOpened",[Class(x.ClassId)]),
        StudentEnrolled x => new("StudentEnrolled",[Student(x.StudentId),Class(x.ClassId),Enrollment(x.ClassId,x.StudentId)]),
        StudentDropped x => new("StudentDropped",[Student(x.StudentId),Class(x.ClassId),Enrollment(x.ClassId,x.StudentId)]),
        ClassStarted x => new("ClassStarted",[Class(x.ClassId)]),
        AttendanceRecorded x => new("AttendanceRecorded",[Student(x.StudentId),Class(x.ClassId),Enrollment(x.ClassId,x.StudentId)]),
        ResultRecorded x => new("ResultRecorded",[Student(x.StudentId),Class(x.ClassId),Enrollment(x.ClassId,x.StudentId)]),
        ClassCompleted x => new("ClassCompleted",[Class(x.ClassId)]),
        StudyStarted x => new("StudyStarted",[Student(x.StudentId),Class(x.ClassId),Enrollment(x.ClassId,x.StudentId)]),
    };
}
