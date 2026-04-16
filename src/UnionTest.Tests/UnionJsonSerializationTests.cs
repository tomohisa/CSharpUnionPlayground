using System.Text.Json;
using UnionTest;
using Xunit;

namespace UnionTest.Tests;

public class UnionJsonSerializationTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private static void AssertSerializeSucceeds<TUnion>(TUnion original)
    {
        var json = JsonSerializer.Serialize(original, JsonOptions);
        Assert.NotNull(json);
        Assert.NotEmpty(json);
    }

    private static void AssertDeserializeValueIsNull<TUnion>(TUnion original)
    {
        var json = JsonSerializer.Serialize(original, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<TUnion>(json, JsonOptions);
        Assert.NotNull(deserialized);
        var valueProp = deserialized!.GetType().GetProperty("Value");
        Assert.NotNull(valueProp);
        var innerVal = valueProp!.GetValue(deserialized);
        Assert.Null(innerVal);
    }

    // --- Pattern 1: Different primitive properties ---

    [Fact]
    public void PersonName_Serialize_Succeeds()
    {
        PersonInfo p = new PersonName("Taro", "Yamada");
        AssertSerializeSucceeds(p);
    }

    [Fact]
    public void PersonName_Deserialize_ValueIsNull()
    {
        PersonInfo p = new PersonName("Taro", "Yamada");
        AssertDeserializeValueIsNull(p);
    }

    [Fact]
    public void Age_Serialize_Succeeds()
    {
        PersonInfo p = new Age(30);
        AssertSerializeSucceeds(p);
    }

    [Fact]
    public void Age_Deserialize_ValueIsNull()
    {
        PersonInfo p = new Age(30);
        AssertDeserializeValueIsNull(p);
    }

    [Fact]
    public void Email_Serialize_Succeeds()
    {
        PersonInfo p = new Email("test@example.com");
        AssertSerializeSucceeds(p);
    }

    [Fact]
    public void Email_Deserialize_ValueIsNull()
    {
        PersonInfo p = new Email("test@example.com");
        AssertDeserializeValueIsNull(p);
    }

    // --- Pattern 2: Collections and nested types ---

    [Fact]
    public void StringTagList_Serialize_Succeeds()
    {
        ComplexData c = new StringTagList(["alpha", "beta", "gamma"]);
        AssertSerializeSucceeds(c);
    }

    [Fact]
    public void StringTagList_Deserialize_ValueIsNull()
    {
        ComplexData c = new StringTagList(["alpha", "beta", "gamma"]);
        AssertDeserializeValueIsNull(c);
    }

    [Fact]
    public void Coordinate_Serialize_Succeeds()
    {
        ComplexData c = new Coordinate(1.5, 2.5, 3.5);
        AssertSerializeSucceeds(c);
    }

    [Fact]
    public void Coordinate_Deserialize_ValueIsNull()
    {
        ComplexData c = new Coordinate(1.5, 2.5, 3.5);
        AssertDeserializeValueIsNull(c);
    }

    [Fact]
    public void Metadata_Serialize_Succeeds()
    {
        ComplexData c = new Metadata(new Dictionary<string, string> { ["key1"] = "val1", ["key2"] = "val2" });
        AssertSerializeSucceeds(c);
    }

    [Fact]
    public void Metadata_Deserialize_ValueIsNull()
    {
        ComplexData c = new Metadata(new Dictionary<string, string> { ["key1"] = "val1", ["key2"] = "val2" });
        AssertDeserializeValueIsNull(c);
    }

    // --- Pattern 3: record class vs record struct ---

    [Fact]
    public void ClassRecord_Serialize_Succeeds()
    {
        MixedUnion m = new ClassRecord("Alice", 42);
        AssertSerializeSucceeds(m);
    }

    [Fact]
    public void ClassRecord_Deserialize_ValueIsNull()
    {
        MixedUnion m = new ClassRecord("Alice", 42);
        AssertDeserializeValueIsNull(m);
    }

    [Fact]
    public void StructRecord_Serialize_Succeeds()
    {
        MixedUnion m = new StructRecord(3.14, true);
        AssertSerializeSucceeds(m);
    }

    [Fact]
    public void StructRecord_Deserialize_ValueIsNull()
    {
        MixedUnion m = new StructRecord(3.14, true);
        AssertDeserializeValueIsNull(m);
    }

    [Fact]
    public void NullableRecord_WithNulls_Serialize_Succeeds()
    {
        MixedUnion m = new NullableRecord(null, null);
        AssertSerializeSucceeds(m);
    }

    [Fact]
    public void NullableRecord_WithNulls_Deserialize_ValueIsNull()
    {
        MixedUnion m = new NullableRecord(null, null);
        AssertDeserializeValueIsNull(m);
    }

    [Fact]
    public void NullableRecord_WithValues_Serialize_Succeeds()
    {
        MixedUnion m = new NullableRecord("Hello", 99);
        AssertSerializeSucceeds(m);
    }

    [Fact]
    public void NullableRecord_WithValues_Deserialize_ValueIsNull()
    {
        MixedUnion m = new NullableRecord("Hello", 99);
        AssertDeserializeValueIsNull(m);
    }

    // --- Pattern 4: Inherited records ---

    [Fact]
    public void BaseAnimal_Serialize_Succeeds()
    {
        AnimalUnion a = new BaseAnimal("Generic");
        AssertSerializeSucceeds(a);
    }

    [Fact]
    public void BaseAnimal_Deserialize_ValueIsNull()
    {
        AnimalUnion a = new BaseAnimal("Generic");
        AssertDeserializeValueIsNull(a);
    }

    [Fact]
    public void DogAnimal_Serialize_Succeeds()
    {
        AnimalUnion a = new DogAnimal("Rex", "Labrador");
        AssertSerializeSucceeds(a);
    }

    [Fact]
    public void DogAnimal_Deserialize_ValueIsNull()
    {
        AnimalUnion a = new DogAnimal("Rex", "Labrador");
        AssertDeserializeValueIsNull(a);
    }

    [Fact]
    public void CatAnimal_Serialize_Succeeds()
    {
        AnimalUnion a = new CatAnimal("Whiskers", true);
        AssertSerializeSucceeds(a);
    }

    [Fact]
    public void CatAnimal_Deserialize_ValueIsNull()
    {
        AnimalUnion a = new CatAnimal("Whiskers", true);
        AssertDeserializeValueIsNull(a);
    }

    // --- Pattern 5: Empty, single, many properties ---

    [Fact]
    public void EmptyRecord_Serialize_Succeeds()
    {
        VariedUnion v = new EmptyRecord();
        AssertSerializeSucceeds(v);
    }

    [Fact]
    public void EmptyRecord_Deserialize_ValueIsNull()
    {
        VariedUnion v = new EmptyRecord();
        AssertDeserializeValueIsNull(v);
    }

    [Fact]
    public void SingleProp_Serialize_Succeeds()
    {
        VariedUnion v = new SingleProp("only");
        AssertSerializeSucceeds(v);
    }

    [Fact]
    public void SingleProp_Deserialize_ValueIsNull()
    {
        VariedUnion v = new SingleProp("only");
        AssertDeserializeValueIsNull(v);
    }

    [Fact]
    public void ManyProps_Serialize_Succeeds()
    {
        var guid = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var dt = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        VariedUnion v = new ManyProps("aaa", 123, 4.56, true, dt, guid);
        AssertSerializeSucceeds(v);
    }

    [Fact]
    public void ManyProps_Deserialize_ValueIsNull()
    {
        var guid = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var dt = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        VariedUnion v = new ManyProps("aaa", 123, 4.56, true, dt, guid);
        AssertDeserializeValueIsNull(v);
    }

    // --- Pattern 6: Generic-like types ---

    [Fact]
    public void StringResult_Serialize_Succeeds()
    {
        ResultUnion r = new StringResult("hello");
        AssertSerializeSucceeds(r);
    }

    [Fact]
    public void StringResult_Deserialize_ValueIsNull()
    {
        ResultUnion r = new StringResult("hello");
        AssertDeserializeValueIsNull(r);
    }

    [Fact]
    public void IntResult_Serialize_Succeeds()
    {
        ResultUnion r = new IntResult(42);
        AssertSerializeSucceeds(r);
    }

    [Fact]
    public void IntResult_Deserialize_ValueIsNull()
    {
        ResultUnion r = new IntResult(42);
        AssertDeserializeValueIsNull(r);
    }

    [Fact]
    public void ListResult_Serialize_Succeeds()
    {
        ResultUnion r = new ListResult([10, 20, 30]);
        AssertSerializeSucceeds(r);
    }

    [Fact]
    public void ListResult_Deserialize_ValueIsNull()
    {
        ResultUnion r = new ListResult([10, 20, 30]);
        AssertDeserializeValueIsNull(r);
    }
}
