using System.Text.Json;
using UnionTest;
using Xunit;

namespace UnionTest.Tests;

public class UnionJsonDerivedTypeTests
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    // === Approach 2: Union serialize -> deserialize as Payment ===
    // Union serialization does not include $type discriminator, so
    // deserializing as abstract Payment base type throws.

    [Fact]
    public void PayCash_UnionSerialize_Succeeds()
    {
        PaymentUnion original = new PayCash(100.50m);
        var json = JsonSerializer.Serialize(original, JsonOptions);
        Assert.NotNull(json);
        Assert.NotEmpty(json);
    }

    [Fact]
    public void PayCash_UnionSerialize_PaymentDeserialize_Throws()
    {
        PaymentUnion original = new PayCash(100.50m);
        var json = JsonSerializer.Serialize(original, JsonOptions);
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<Payment>(json, JsonOptions));
    }

    [Fact]
    public void PayCard_UnionSerialize_PaymentDeserialize_Throws()
    {
        PaymentUnion original = new PayCard("4111-1111-1111-1111", 250.00m);
        var json = JsonSerializer.Serialize(original, JsonOptions);
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<Payment>(json, JsonOptions));
    }

    [Fact]
    public void PayCrypto_UnionSerialize_PaymentDeserialize_Throws()
    {
        PaymentUnion original = new PayCrypto("0xABC123", 0.5m, "ETH");
        var json = JsonSerializer.Serialize(original, JsonOptions);
        Assert.ThrowsAny<Exception>(() => JsonSerializer.Deserialize<Payment>(json, JsonOptions));
    }

    // === Approach 2b: Serialize as base type Payment, deserialize as Payment ===

    [Fact]
    public void PayCash_SerializeAsPayment_DeserializeAsPayment()
    {
        Payment original = new PayCash(99.99m);
        var json = JsonSerializer.Serialize<Payment>(original, JsonOptions);

        var deserialized = JsonSerializer.Deserialize<Payment>(json, JsonOptions);
        Assert.NotNull(deserialized);

        PaymentUnion u = deserialized switch
        {
            PayCash c => new PaymentUnion(c),
            PayCard c => new PaymentUnion(c),
            PayCrypto c => new PaymentUnion(c),
            _ => throw new InvalidOperationException()
        };
        Assert.True(u.Value is PayCash { Amount: 99.99m });
    }

    [Fact]
    public void PayCard_SerializeAsPayment_DeserializeAsPayment()
    {
        Payment original = new PayCard("5500-0000-0000-0004", 500m);
        var json = JsonSerializer.Serialize<Payment>(original, JsonOptions);

        var deserialized = JsonSerializer.Deserialize<Payment>(json, JsonOptions);
        Assert.NotNull(deserialized);

        PaymentUnion u = deserialized switch
        {
            PayCash c => new PaymentUnion(c),
            PayCard c => new PaymentUnion(c),
            PayCrypto c => new PaymentUnion(c),
            _ => throw new InvalidOperationException()
        };
        Assert.True(u.Value is PayCard { Amount: 500m });
    }

    [Fact]
    public void PayCrypto_SerializeAsPayment_DeserializeAsPayment()
    {
        Payment original = new PayCrypto("0xDEF", 1.23m, "BTC");
        var json = JsonSerializer.Serialize<Payment>(original, JsonOptions);

        var deserialized = JsonSerializer.Deserialize<Payment>(json, JsonOptions);
        Assert.NotNull(deserialized);

        PaymentUnion u = deserialized switch
        {
            PayCash c => new PaymentUnion(c),
            PayCard c => new PaymentUnion(c),
            PayCrypto c => new PaymentUnion(c),
            _ => throw new InvalidOperationException()
        };
        Assert.True(u.Value is PayCrypto { Currency: "BTC" });
    }

    // === Approach 3: interface with [JsonDerivedType] ===

    [Fact]
    public void MsgText_SerializeAsIMessage_DeserializeAsIMessage()
    {
        IMessage original = new MsgText("Alice", "Hello!");
        var json = JsonSerializer.Serialize<IMessage>(original, JsonOptions);

        var deserialized = JsonSerializer.Deserialize<IMessage>(json, JsonOptions);
        Assert.NotNull(deserialized);

        MessageUnion u = deserialized switch
        {
            MsgText t => new MessageUnion(t),
            MsgImage i => new MessageUnion(i),
            MsgFile f => new MessageUnion(f),
            _ => throw new InvalidOperationException()
        };
        Assert.True(u.Value is MsgText { Sender: "Alice", Body: "Hello!" });
    }

    [Fact]
    public void MsgImage_SerializeAsIMessage_DeserializeAsIMessage()
    {
        IMessage original = new MsgImage("Bob", "https://img.example.com/1.png", 800, 600);
        var json = JsonSerializer.Serialize<IMessage>(original, JsonOptions);

        var deserialized = JsonSerializer.Deserialize<IMessage>(json, JsonOptions);
        Assert.NotNull(deserialized);

        MessageUnion u = deserialized switch
        {
            MsgText t => new MessageUnion(t),
            MsgImage i => new MessageUnion(i),
            MsgFile f => new MessageUnion(f),
            _ => throw new InvalidOperationException()
        };
        Assert.True(u.Value is MsgImage { Sender: "Bob", Width: 800 });
    }

    [Fact]
    public void MsgFile_SerializeAsIMessage_DeserializeAsIMessage()
    {
        IMessage original = new MsgFile("Charlie", "report.pdf", 1024000);
        var json = JsonSerializer.Serialize<IMessage>(original, JsonOptions);

        var deserialized = JsonSerializer.Deserialize<IMessage>(json, JsonOptions);
        Assert.NotNull(deserialized);

        MessageUnion u = deserialized switch
        {
            MsgText t => new MessageUnion(t),
            MsgImage i => new MessageUnion(i),
            MsgFile f => new MessageUnion(f),
            _ => throw new InvalidOperationException()
        };
        Assert.True(u.Value is MsgFile { FileName: "report.pdf", SizeBytes: 1024000 });
    }

    // === Approach 4: Concrete child type direct serialization ===

    [Fact]
    public void OrderNew_DirectSerializeDeserialize()
    {
        var original = new OrderNew(1, "Laptop", 2);
        var json = JsonSerializer.Serialize(original, JsonOptions);

        var deserialized = JsonSerializer.Deserialize<OrderNew>(json, JsonOptions);
        Assert.NotNull(deserialized);

        OrderStatusUnion u = new OrderStatusUnion(deserialized!);
        Assert.True(u.Value is OrderNew { OrderId: 1, Product: "Laptop", Quantity: 2 });
    }

    [Fact]
    public void OrderShipped_DirectSerializeDeserialize()
    {
        var original = new OrderShipped(1, new DateTime(2025, 7, 1), "TRACK123");
        var json = JsonSerializer.Serialize(original, JsonOptions);

        var deserialized = JsonSerializer.Deserialize<OrderShipped>(json, JsonOptions);
        Assert.NotNull(deserialized);

        OrderStatusUnion u = new OrderStatusUnion(deserialized!);
        Assert.True(u.Value is OrderShipped { TrackingNumber: "TRACK123" });
    }

    [Fact]
    public void OrderDelivered_DirectSerializeDeserialize()
    {
        var original = new OrderDelivered(1, new DateTime(2025, 7, 5));
        var json = JsonSerializer.Serialize(original, JsonOptions);

        var deserialized = JsonSerializer.Deserialize<OrderDelivered>(json, JsonOptions);
        Assert.NotNull(deserialized);

        OrderStatusUnion u = new OrderStatusUnion(deserialized!);
        Assert.True(u.Value is OrderDelivered { OrderId: 1 });
    }
}
