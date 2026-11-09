using System.Text.Json;
using System.Text.Json.Serialization;
using UnionTest;
using Xunit;

namespace UnionTest.Tests;

public class UnionStructuralRemedyTests
{
    private static readonly JsonSerializerOptions StrictOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
        RespectRequiredConstructorParameters = true,
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Disallow,
        TypeClassifiers = { new JsonUnionTypeStructuralClassifier() }
    };

    private static void RoundTrip<T>(T original)
    {
        var json = JsonSerializer.Serialize(original, StrictOptions);
        var restored = JsonSerializer.Deserialize<T>(json, StrictOptions);
        Assert.Equal(original, restored);
    }

    [Fact]
    public void RequiredPropertiesAndUnmappedMembers_DistinguishAnimalCases()
    {
        RoundTrip<AnimalUnion>(new BaseAnimal("Generic"));
        RoundTrip<AnimalUnion>(new DogAnimal("Rex", "Labrador"));
        RoundTrip<AnimalUnion>(new CatAnimal("Misty", true));
    }

    [Fact]
    public void RequiredPropertiesAndUnmappedMembers_DistinguishEmptyAndNonemptyCases()
    {
        RoundTrip<VariedUnion>(new EmptyRecord());
        RoundTrip<VariedUnion>(new SingleProp("only"));
        RoundTrip<VariedUnion>(new ManyProps("a", 1, 2.5, true, DateTime.UnixEpoch, Guid.Empty));
    }

    [Fact]
    public void RequiredPropertiesAndUnmappedMembers_DistinguishPaymentCases()
    {
        RoundTrip<PaymentUnion>(new PayCash(100m));
        RoundTrip<PaymentUnion>(new PayCard("4111", 100m));
        RoundTrip<PaymentUnion>(new PayCrypto("wallet", 1m, "ETH"));
        RoundTrip(new ContainerWithPayment("ORDER-1", new PaymentUnion(new PayCash(100m))));
    }

    [Fact]
    public void IdenticalPropertyNames_StillCannotBeClassified()
    {
        Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<ResultUnion>("{\"value\":42}", StrictOptions));
    }

    [Fact]
    public void UnionToAbstractBase_StillNeedsDiscriminator()
    {
        PaymentUnion original = new PayCash(100m);
        var json = JsonSerializer.Serialize(original, StrictOptions);
        Assert.DoesNotContain("$type", json);
        Assert.Throws<NotSupportedException>(() => JsonSerializer.Deserialize<Payment>(json, StrictOptions));
    }
}
