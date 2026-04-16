using System.Text.Json.Serialization;

namespace UnionTest;

// ======================================================
// アプローチ1: union型自体に[JsonDerivedType]を付与
// → コンパイルエラー: union は struct なので
//   [JsonDerivedType] (class/interface のみ) を付けられない
// ======================================================

// ======================================================
// アプローチ2: 共通基底 abstract record に[JsonDerivedType]を付け、
//              union のケースとして派生型を使う
// ======================================================

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(PayCash), "cash")]
[JsonDerivedType(typeof(PayCard), "card")]
[JsonDerivedType(typeof(PayCrypto), "crypto")]
public abstract record Payment;

public record PayCash(decimal Amount) : Payment;
public record PayCard(string CardNumber, decimal Amount) : Payment;
public record PayCrypto(string WalletAddress, decimal Amount, string Currency) : Payment;

public union PaymentUnion(PayCash, PayCard, PayCrypto);

// ======================================================
// アプローチ3: interface に[JsonDerivedType]を付け、
//              ケース型が実装する
// ======================================================

[JsonPolymorphic(TypeDiscriminatorPropertyName = "$type")]
[JsonDerivedType(typeof(MsgText), "text")]
[JsonDerivedType(typeof(MsgImage), "image")]
[JsonDerivedType(typeof(MsgFile), "file")]
public interface IMessage
{
    string Sender { get; }
}

public record MsgText(string Sender, string Body) : IMessage;
public record MsgImage(string Sender, string Url, int Width, int Height) : IMessage;
public record MsgFile(string Sender, string FileName, long SizeBytes) : IMessage;

public union MessageUnion(MsgText, MsgImage, MsgFile);

// ======================================================
// アプローチ4: 属性なし子型（前回と同じ）を基底なしで
//              子型を直接シリアライズ/デシリアライズして
//              union に再代入する方法
// ======================================================

public record OrderNew(int OrderId, string Product, int Quantity);
public record OrderShipped(int OrderId, DateTime ShippedAt, string TrackingNumber);
public record OrderDelivered(int OrderId, DateTime DeliveredAt);

public union OrderStatusUnion(OrderNew, OrderShipped, OrderDelivered);
