using System.Text.Json;
using UnionTest;
using Xunit;

namespace UnionTest.Tests;

// ================================================================
// Union型の直接シリアライズ/デシリアライズ テスト
//
// 注意: これらのテストは「本来あるべきラウンドトリップ」を期待
// していますが、現時点では C# 15 Preview の union 型の System.Text.Json
// サポートが不完全なため、*_Deserialize_RoundTrip 系のテストは全て
// 失敗します（Value プロパティが null に戻ってしまう）。
// シリアライズ自体は成功します。
// ================================================================

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

    private static void AssertRoundTrip<TUnion>(TUnion original, Func<TUnion, bool> verify)
    {
        var json = JsonSerializer.Serialize(original, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<TUnion>(json, JsonOptions);
        Assert.NotNull(deserialized);
        Assert.True(verify(deserialized!), "Round-trip did not restore the original union case.");
    }

    // --- Pattern 1: Different primitive properties ---

    [Fact]
    public void PersonName_Serialize_Succeeds()
    {
        PersonInfo p = new PersonName("Taro", "Yamada");
        AssertSerializeSucceeds(p);
    }

    // NOTE: 現時点では失敗します。Union型を直接デシリアライズしても
    // Value プロパティが null になり、元の PersonName に戻りません。
    [Fact]
    public void PersonName_Deserialize_RoundTrip()
    {
        PersonInfo p = new PersonName("Taro", "Yamada");
        AssertRoundTrip(p, d => d.Value is PersonName { First: "Taro", Last: "Yamada" });
    }

    [Fact]
    public void Age_Serialize_Succeeds()
    {
        PersonInfo p = new Age(30);
        AssertSerializeSucceeds(p);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void Age_Deserialize_RoundTrip()
    {
        PersonInfo p = new Age(30);
        AssertRoundTrip(p, d => d.Value is Age { Years: 30 });
    }

    [Fact]
    public void Email_Serialize_Succeeds()
    {
        PersonInfo p = new Email("test@example.com");
        AssertSerializeSucceeds(p);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void Email_Deserialize_RoundTrip()
    {
        PersonInfo p = new Email("test@example.com");
        AssertRoundTrip(p, d => d.Value is Email { Address: "test@example.com" });
    }

    // --- Pattern 2: Collections and nested types ---

    [Fact]
    public void StringTagList_Serialize_Succeeds()
    {
        ComplexData c = new StringTagList(["alpha", "beta", "gamma"]);
        AssertSerializeSucceeds(c);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void StringTagList_Deserialize_RoundTrip()
    {
        ComplexData c = new StringTagList(["alpha", "beta", "gamma"]);
        AssertRoundTrip(c, d => d.Value is StringTagList t && t.Tags.Count == 3 && t.Tags[0] == "alpha");
    }

    [Fact]
    public void Coordinate_Serialize_Succeeds()
    {
        ComplexData c = new Coordinate(1.5, 2.5, 3.5);
        AssertSerializeSucceeds(c);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void Coordinate_Deserialize_RoundTrip()
    {
        ComplexData c = new Coordinate(1.5, 2.5, 3.5);
        AssertRoundTrip(c, d => d.Value is Coordinate { X: 1.5, Y: 2.5, Z: 3.5 });
    }

    [Fact]
    public void Metadata_Serialize_Succeeds()
    {
        ComplexData c = new Metadata(new Dictionary<string, string> { ["key1"] = "val1", ["key2"] = "val2" });
        AssertSerializeSucceeds(c);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void Metadata_Deserialize_RoundTrip()
    {
        ComplexData c = new Metadata(new Dictionary<string, string> { ["key1"] = "val1", ["key2"] = "val2" });
        AssertRoundTrip(c, d => d.Value is Metadata m && m.Properties.Count == 2 && m.Properties["key1"] == "val1");
    }

    // --- Pattern 3: record class vs record struct ---

    [Fact]
    public void ClassRecord_Serialize_Succeeds()
    {
        MixedUnion m = new ClassRecord("Alice", 42);
        AssertSerializeSucceeds(m);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void ClassRecord_Deserialize_RoundTrip()
    {
        MixedUnion m = new ClassRecord("Alice", 42);
        AssertRoundTrip(m, d => d.Value is ClassRecord { Name: "Alice", Id: 42 });
    }

    [Fact]
    public void StructRecord_Serialize_Succeeds()
    {
        MixedUnion m = new StructRecord(3.14, true);
        AssertSerializeSucceeds(m);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void StructRecord_Deserialize_RoundTrip()
    {
        MixedUnion m = new StructRecord(3.14, true);
        AssertRoundTrip(m, d => d.Value is StructRecord { Value: 3.14, Flag: true });
    }

    [Fact]
    public void NullableRecord_WithNulls_Serialize_Succeeds()
    {
        MixedUnion m = new NullableRecord(null, null);
        AssertSerializeSucceeds(m);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void NullableRecord_WithNulls_Deserialize_RoundTrip()
    {
        MixedUnion m = new NullableRecord(null, null);
        AssertRoundTrip(m, d => d.Value is NullableRecord { Label: null, Count: null });
    }

    [Fact]
    public void NullableRecord_WithValues_Serialize_Succeeds()
    {
        MixedUnion m = new NullableRecord("Hello", 99);
        AssertSerializeSucceeds(m);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void NullableRecord_WithValues_Deserialize_RoundTrip()
    {
        MixedUnion m = new NullableRecord("Hello", 99);
        AssertRoundTrip(m, d => d.Value is NullableRecord { Label: "Hello", Count: 99 });
    }

    // --- Pattern 4: Inherited records ---

    [Fact]
    public void BaseAnimal_Serialize_Succeeds()
    {
        AnimalUnion a = new BaseAnimal("Generic");
        AssertSerializeSucceeds(a);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void BaseAnimal_Deserialize_RoundTrip()
    {
        AnimalUnion a = new BaseAnimal("Generic");
        AssertRoundTrip(a, d => d.Value is BaseAnimal { Name: "Generic" });
    }

    [Fact]
    public void DogAnimal_Serialize_Succeeds()
    {
        AnimalUnion a = new DogAnimal("Rex", "Labrador");
        AssertSerializeSucceeds(a);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void DogAnimal_Deserialize_RoundTrip()
    {
        AnimalUnion a = new DogAnimal("Rex", "Labrador");
        AssertRoundTrip(a, d => d.Value is DogAnimal { Name: "Rex", Breed: "Labrador" });
    }

    [Fact]
    public void CatAnimal_Serialize_Succeeds()
    {
        AnimalUnion a = new CatAnimal("Whiskers", true);
        AssertSerializeSucceeds(a);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void CatAnimal_Deserialize_RoundTrip()
    {
        AnimalUnion a = new CatAnimal("Whiskers", true);
        AssertRoundTrip(a, d => d.Value is CatAnimal { Name: "Whiskers", Indoor: true });
    }

    // --- Pattern 5: Empty, single, many properties ---

    [Fact]
    public void EmptyRecord_Serialize_Succeeds()
    {
        VariedUnion v = new EmptyRecord();
        AssertSerializeSucceeds(v);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void EmptyRecord_Deserialize_RoundTrip()
    {
        VariedUnion v = new EmptyRecord();
        AssertRoundTrip(v, d => d.Value is EmptyRecord);
    }

    [Fact]
    public void SingleProp_Serialize_Succeeds()
    {
        VariedUnion v = new SingleProp("only");
        AssertSerializeSucceeds(v);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void SingleProp_Deserialize_RoundTrip()
    {
        VariedUnion v = new SingleProp("only");
        AssertRoundTrip(v, d => d.Value is SingleProp { Solo: "only" });
    }

    [Fact]
    public void ManyProps_Serialize_Succeeds()
    {
        var guid = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var dt = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        VariedUnion v = new ManyProps("aaa", 123, 4.56, true, dt, guid);
        AssertSerializeSucceeds(v);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void ManyProps_Deserialize_RoundTrip()
    {
        var guid = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        var dt = new DateTime(2025, 6, 15, 10, 30, 0, DateTimeKind.Utc);
        VariedUnion v = new ManyProps("aaa", 123, 4.56, true, dt, guid);
        AssertRoundTrip(v, d => d.Value is ManyProps mp
            && mp.A == "aaa" && mp.B == 123 && mp.C == 4.56 && mp.D == true && mp.E == dt && mp.F == guid);
    }

    // --- Pattern 6: Generic-like types ---

    [Fact]
    public void StringResult_Serialize_Succeeds()
    {
        ResultUnion r = new StringResult("hello");
        AssertSerializeSucceeds(r);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void StringResult_Deserialize_RoundTrip()
    {
        ResultUnion r = new StringResult("hello");
        AssertRoundTrip(r, d => d.Value is StringResult { Value: "hello" });
    }

    [Fact]
    public void IntResult_Serialize_Succeeds()
    {
        ResultUnion r = new IntResult(42);
        AssertSerializeSucceeds(r);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void IntResult_Deserialize_RoundTrip()
    {
        ResultUnion r = new IntResult(42);
        AssertRoundTrip(r, d => d.Value is IntResult { Value: 42 });
    }

    [Fact]
    public void ListResult_Serialize_Succeeds()
    {
        ResultUnion r = new ListResult([10, 20, 30]);
        AssertSerializeSucceeds(r);
    }

    // NOTE: 現時点では失敗します（Union型直接デシリアライズの既知の制約）
    [Fact]
    public void ListResult_Deserialize_RoundTrip()
    {
        ResultUnion r = new ListResult([10, 20, 30]);
        AssertRoundTrip(r, d => d.Value is ListResult l && l.Items.Count == 3 && l.Items[1] == 20);
    }
}
