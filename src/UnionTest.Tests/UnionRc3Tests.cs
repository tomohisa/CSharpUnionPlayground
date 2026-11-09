using System.Text.Json;
using System.Text.Json.Serialization;
using UnionTest;
using Xunit;

namespace UnionTest.Tests;

public union Reading(int, string, bool);

public class UnionRc3Tests
{
    [Theory]
    [InlineData("42", 42)]
    [InlineData("\"hello\"", "hello")]
    [InlineData("true", true)]
    public void DistinctTokenKinds_RoundTrip(string json, object expected)
    {
        var reading = JsonSerializer.Deserialize<Reading>(json);
        Assert.Equal(expected, reading.Value);
        Assert.Equal(json, JsonSerializer.Serialize(reading));
    }

    [Fact]
    public void DefaultUnion_RoundTripsAsNull()
    {
        Assert.Equal("null", JsonSerializer.Serialize(default(Reading)));
        Assert.Null(JsonSerializer.Deserialize<Reading>("null").Value);
    }

    [Fact]
    public void ObjectCase_SerializesWithoutWrapperOrDiscriminator()
    {
        PersonInfo info = new Age(30);
        Assert.Equal("{\"Years\":30}", JsonSerializer.Serialize(info));
    }

    [Fact]
    public void MultipleObjectCases_RequireClassifier()
    {
        Assert.Throws<JsonException>(() => JsonSerializer.Deserialize<PersonInfo>("{\"Years\":30}"));
    }

    [Fact]
    public void StructuralClassifier_RestoresNestedUnionCases()
    {
        var options = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            TypeClassifiers = { new JsonUnionTypeStructuralClassifier() }
        };
        var original = new ContainerList("team", [new(new Age(30)), new(new PersonName("Taro", "Yamada")), new(new Email("a@b.com"))]);
        var json = JsonSerializer.Serialize(original, options);
        var copy = JsonSerializer.Deserialize<ContainerList>(json, options)!;
        Assert.Equal(original.Group, copy.Group);
        Assert.Equal(original.People.Select(p => p.Value), copy.People.Select(p => p.Value));
    }
}
