namespace UnionTest;

// --- パターン1: プリミティブ型のプロパティが異なる子型 ---
public record PersonName(string First, string Last);
public record Age(int Years);
public record Email(string Address);

public union PersonInfo(PersonName, Age, Email);

// --- パターン2: コレクション・ネスト型を含む子型 ---
public record StringTagList(List<string> Tags);
public record Coordinate(double X, double Y, double Z);
public record Metadata(Dictionary<string, string> Properties);

public union ComplexData(StringTagList, Coordinate, Metadata);

// --- パターン3: record class vs record struct の混在 ---
public record class ClassRecord(string Name, int Id);
public record struct StructRecord(double Value, bool Flag);
public record class NullableRecord(string? Label, int? Count);

public union MixedUnion(ClassRecord, StructRecord, NullableRecord);

// --- パターン4: 継承を含む record 型 ---
public record BaseAnimal(string Name);
public record DogAnimal(string Name, string Breed) : BaseAnimal(Name);
public record CatAnimal(string Name, bool Indoor) : BaseAnimal(Name);

public union AnimalUnion(BaseAnimal, DogAnimal, CatAnimal);

// --- パターン5: 空・単一プロパティ・多数プロパティの子型 ---
public record EmptyRecord();
public record SingleProp(string Solo);
public record ManyProps(string A, int B, double C, bool D, DateTime E, Guid F);

public union VariedUnion(EmptyRecord, SingleProp, ManyProps);

// --- パターン6: ジェネリック型を含む子型 ---
public record StringResult(string Value);
public record IntResult(int Value);
public record ListResult(List<int> Items);

public union ResultUnion(StringResult, IntResult, ListResult);
