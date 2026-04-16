using System.Text.Json;
using UnionTest;
using Xunit;

namespace UnionTest.Tests;

// ================================================================
// Union型をプロパティとして保持する親クラスのJSONシリアライズテスト
//
// 親クラスに union 型のプロパティが含まれる場合も、その union 型を
// 直接シリアライズするのと同じ制約を受けます:
//   - シリアライズは成功する
//   - デシリアライズすると union プロパティの Value が null になる
//     （＝元の子ケースに戻らない）
//
// NOTE: 下記の *_Deserialize_RoundTrip 系テストは現時点では失敗します。
// 本来は親クラスごとラウンドトリップしてほしいですが、union 型の
// System.Text.Json サポートが不完全なためです。
// ================================================================

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

    // NOTE: 現時点では失敗します。親クラスのデシリアライズ自体は成功しますが、
    // union プロパティ (Info) の Value が null になり、元の PersonName に戻りません。
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

    // NOTE: 現時点では失敗します（複数の union プロパティどちらも Value が null になります）
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

    // NOTE: 現時点では失敗します（ネストされた親クラス内の union も Value が null になります）
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

    // NOTE: 現時点では失敗します（List 内の各 union 要素の Value も null になります）
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

    // NOTE: 現時点では失敗します（PaymentUnion も同じく Value が null になります。
    // $type ディスクリミネータが出力されないため Payment 基底型でのデシリアライズもできません）
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
