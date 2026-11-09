using System.Text.Json;
using UnionTest;
using Xunit;

namespace UnionTest.Tests;

// RC3: 標準設定では複数の object ケースを判別できず JsonException になります。
// RoundTrip テストは期待する動作を引き続き検証するため、失敗を残しています。
// 型判別器を設定した成功例は UnionRc3Tests を参照してください。

// --- テスト用モデル（親クラスが union 型プロパティを持つ） ---

public record ContainerSingle(string Label, PersonInfo Info);

public record ContainerMultiple(int Id, PersonInfo Primary, ComplexData Data);

public record ContainerNested(string Title, ContainerSingle Inner);

public record ContainerList(string Group, List<PersonInfo> People);

public record ContainerWithPayment(string OrderId, PaymentUnion Payment);

public class UnionAsPropertyTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // --- シリアライズ成功ケース ---

    [Fact]
    public void ContainerSingle_Serialize_Succeeds()
    {
        var c = new ContainerSingle("profile", new PersonInfo(new PersonName("Taro", "Yamada")));
        var json = JsonSerializer.Serialize(c, JsonOptions);
        Assert.NotNull(json);
        Assert.NotEmpty(json);
    }

    [Fact]
    public void ContainerMultiple_Serialize_Succeeds()
    {
        var c = new ContainerMultiple(
            1,
            new PersonInfo(new Age(30)),
            new ComplexData(new Coordinate(1.0, 2.0, 3.0)));
        var json = JsonSerializer.Serialize(c, JsonOptions);
        Assert.NotNull(json);
        Assert.NotEmpty(json);
    }

    [Fact]
    public void ContainerNested_Serialize_Succeeds()
    {
        var inner = new ContainerSingle("inner", new PersonInfo(new Email("a@b.com")));
        var outer = new ContainerNested("outer", inner);
        var json = JsonSerializer.Serialize(outer, JsonOptions);
        Assert.NotNull(json);
        Assert.NotEmpty(json);
    }

    [Fact]
    public void ContainerList_Serialize_Succeeds()
    {
        var c = new ContainerList("team", new List<PersonInfo>
        {
            new(new PersonName("Alice", "A")),
            new(new Age(25)),
            new(new Email("c@d.com"))
        });
        var json = JsonSerializer.Serialize(c, JsonOptions);
        Assert.NotNull(json);
        Assert.NotEmpty(json);
    }

    [Fact]
    public void ContainerWithPayment_Serialize_Succeeds()
    {
        var c = new ContainerWithPayment("ORDER-1", new PaymentUnion(new PayCash(100m)));
        var json = JsonSerializer.Serialize(c, JsonOptions);
        Assert.NotNull(json);
        Assert.NotEmpty(json);
    }

    // --- ラウンドトリップ: 親クラスのデシリアライズ後も子 union の Value が復元されるべき ---

    [Fact]
    public void ContainerSingle_Deserialize_RoundTrip()
    {
        var original = new ContainerSingle("profile", new PersonInfo(new PersonName("Taro", "Yamada")));
        var json = JsonSerializer.Serialize(original, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<ContainerSingle>(json, JsonOptions);

        Assert.NotNull(deserialized);
        Assert.Equal("profile", deserialized!.Label);
        Assert.True(
            deserialized.Info.Value is PersonName { First: "Taro", Last: "Yamada" },
            "Container's union property did not round-trip to PersonName.");
    }

    [Fact]
    public void ContainerMultiple_Deserialize_RoundTrip()
    {
        var original = new ContainerMultiple(
            1,
            new PersonInfo(new Age(30)),
            new ComplexData(new Coordinate(1.0, 2.0, 3.0)));
        var json = JsonSerializer.Serialize(original, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<ContainerMultiple>(json, JsonOptions);

        Assert.NotNull(deserialized);
        Assert.Equal(1, deserialized!.Id);
        Assert.True(deserialized.Primary.Value is Age { Years: 30 },
            "Primary union property did not round-trip to Age.");
        Assert.True(deserialized.Data.Value is Coordinate { X: 1.0, Y: 2.0, Z: 3.0 },
            "Data union property did not round-trip to Coordinate.");
    }

    [Fact]
    public void ContainerNested_Deserialize_RoundTrip()
    {
        var inner = new ContainerSingle("inner", new PersonInfo(new Email("a@b.com")));
        var original = new ContainerNested("outer", inner);
        var json = JsonSerializer.Serialize(original, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<ContainerNested>(json, JsonOptions);

        Assert.NotNull(deserialized);
        Assert.Equal("outer", deserialized!.Title);
        Assert.Equal("inner", deserialized.Inner.Label);
        Assert.True(deserialized.Inner.Info.Value is Email { Address: "a@b.com" },
            "Nested container's union property did not round-trip to Email.");
    }

    [Fact]
    public void ContainerList_Deserialize_RoundTrip()
    {
        var original = new ContainerList("team", new List<PersonInfo>
        {
            new(new PersonName("Alice", "A")),
            new(new Age(25)),
            new(new Email("c@d.com"))
        });
        var json = JsonSerializer.Serialize(original, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<ContainerList>(json, JsonOptions);

        Assert.NotNull(deserialized);
        Assert.Equal("team", deserialized!.Group);
        Assert.Equal(3, deserialized.People.Count);
        Assert.True(deserialized.People[0].Value is PersonName { First: "Alice", Last: "A" });
        Assert.True(deserialized.People[1].Value is Age { Years: 25 });
        Assert.True(deserialized.People[2].Value is Email { Address: "c@d.com" });
    }

    [Fact]
    public void ContainerWithPayment_Deserialize_RoundTrip()
    {
        var original = new ContainerWithPayment("ORDER-1", new PaymentUnion(new PayCash(100m)));
        var json = JsonSerializer.Serialize(original, JsonOptions);
        var deserialized = JsonSerializer.Deserialize<ContainerWithPayment>(json, JsonOptions);

        Assert.NotNull(deserialized);
        Assert.Equal("ORDER-1", deserialized!.OrderId);
        Assert.True(deserialized.Payment.Value is PayCash { Amount: 100m },
            "Container's PaymentUnion property did not round-trip to PayCash.");
    }
}
