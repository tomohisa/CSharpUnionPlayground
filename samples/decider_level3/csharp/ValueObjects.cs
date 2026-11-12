using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using Microsoft.Extensions.Options;
namespace SchoolDomain;

// Reference types with private constructors avoid an invalid default(struct).
public sealed record StudentId
{
    public string Value {get;}
    private StudentId(string value)=>Value=value;
    public static StudentId FromEvent(string value)=>new(value);
    public static StudentId Create(string value)=>!string.IsNullOrWhiteSpace(value)?new(value):throw new ArgumentException("InvalidId");
}
public sealed record ClassId
{
    public string Value {get;}
    private ClassId(string value)=>Value=value;
    public static ClassId FromEvent(string value)=>new(value);
    public static ClassId Create(string value)=>!string.IsNullOrWhiteSpace(value)?new(value):throw new ArgumentException("InvalidId");
}
public sealed record SeatCapacity
{
    public const int Minimum=1,Maximum=10;
    public int Value {get;}
    private SeatCapacity(int value)=>Value=value;
    public static SeatCapacity FromEvent(int value)=>new(value);
    public static SeatCapacity Create(int value)=>value is >=Minimum and <=Maximum?new(value):throw new ArgumentOutOfRangeException(nameof(value));
}
public sealed record AssessmentScore
{
    public const int Minimum=0,Maximum=100;
    public int Value {get;}
    private AssessmentScore(int value)=>Value=value;
    public static AssessmentScore FromEvent(int value)=>new(value);
    public static AssessmentScore Create(int value)=>value is >=Minimum and <=Maximum?new(value):throw new ArgumentOutOfRangeException(nameof(value));
}
public sealed record SchoolDate
{
    public string Value {get;}
    private SchoolDate(string value)=>Value=value;
    public static SchoolDate FromEvent(string value)=>new(value);
    public static bool TryCreate(string? value,out SchoolDate? date)
    {
        bool valid=DateOnly.TryParseExact(value,"yyyy-MM-dd",CultureInfo.InvariantCulture,DateTimeStyles.None,out var parsed);
        date=valid?new(value!):null;return valid;
    }
    public static SchoolDate Create(string value)=>TryCreate(value,out var date)?date!:throw new ArgumentException("InvalidDate");
}
[AttributeUsage(AttributeTargets.Property)]
public sealed class SchoolDateAttribute:ValidationAttribute
{
    public override bool IsValid(object? value)=>value is string text && SchoolDate.TryCreate(text,out _);
}
[AttributeUsage(AttributeTargets.Property)]
public sealed class SeatCapacityAttribute:ValidationAttribute
{
    public override bool IsValid(object? value)=>value is int n && n is >=SeatCapacity.Minimum and <=SeatCapacity.Maximum;
}
[AttributeUsage(AttributeTargets.Property)]
public sealed class AssessmentScoreAttribute:ValidationAttribute
{
    public override bool IsValid(object? value)=>value is int n && n is >=AssessmentScore.Minimum and <=AssessmentScore.Maximum;
}
// The platform generator reads property attributes at compile time; no TryValidateObject/reflection.
[OptionsValidator]
internal partial class CommandValidator:IValidateOptions<RegisterStudent>,IValidateOptions<PromoteStudent>,IValidateOptions<OpenClass>,
    IValidateOptions<Enroll>,IValidateOptions<Drop>,IValidateOptions<StartClass>,IValidateOptions<Attend>,IValidateOptions<Assess>,IValidateOptions<CompleteClass>;

public sealed class ValidatedCommand
{
    public Command Value {get;}
    private ValidatedCommand(Command value)=>Value=value;
    private static readonly CommandValidator Validator=new();
    public static ImmutableArray<Rejection> Validate(Command command)
    {
        if(command is null)return [Rejection.InvalidCommand];
        var result=command switch
        {
            RegisterStudent c=>Validator.Validate(null,c),PromoteStudent c=>Validator.Validate(null,c),OpenClass c=>Validator.Validate(null,c),
            Enroll c=>Validator.Validate(null,c),Drop c=>Validator.Validate(null,c),StartClass c=>Validator.Validate(null,c),
            Attend c=>Validator.Validate(null,c),Assess c=>Validator.Validate(null,c),CompleteClass c=>Validator.Validate(null,c)
        };
        return result.Succeeded?[]:[..result.Failures.Select(f=>Enum.Parse<Rejection>(f[(f.LastIndexOf(':')+1)..].Trim())).Distinct()];
    }
    public static ValidatedCommand Create(Command command)
    {
        var errors=Validate(command);if(!errors.IsEmpty)throw new ArgumentException(string.Join(",",errors));
        // Smart constructors are reusable independently of command/JSON validation.
        switch(command)
        {
            case RegisterStudent c:StudentId.Create(c.StudentId);break;
            case PromoteStudent c:StudentId.Create(c.StudentId);break;
            case OpenClass c:ClassId.Create(c.ClassId);SeatCapacity.Create(c.Capacity);break;
            case Enroll c:ClassId.Create(c.ClassId);StudentId.Create(c.StudentId);break;
            case Drop c:ClassId.Create(c.ClassId);StudentId.Create(c.StudentId);break;
            case StartClass c:ClassId.Create(c.ClassId);SchoolDate.Create(c.StartedOn);break;
            case Attend c:ClassId.Create(c.ClassId);StudentId.Create(c.StudentId);break;
            case Assess c:ClassId.Create(c.ClassId);StudentId.Create(c.StudentId);AssessmentScore.Create(c.Score);break;
            case CompleteClass c:ClassId.Create(c.ClassId);break;
        }
        return new(command);
    }
}
