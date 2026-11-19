# DnDGen.Infrastructure

This provides common, core objects for the generators, including the Percentile and Collection selectors, with dependencies

[![Build Status](https://github.com/DnDGen/DnDGen.Infrastructure/actions/workflows/release.yml/badge.svg?branch=main)](https://github.com/DnDGen/DnDGen.Infrastructure/actions/workflows/release.yml?query=branch%3Amain)

## What It Is

DnDGen's generators (TreasureGen, CreatureGen, and the rest) keep their data in XML tables embedded in their assemblies, and pick from them at random. Infrastructure is what reads those tables and does the picking. It gives you:

- **Collection tables**: named lists of entries, such as the weapons a generator knows about.
- **Percentile tables**: a result for every roll from 1 to 100.
- **Selectors** that read those tables and choose from them, at random where that makes sense, with the dice rolled by [DnDGen.RollGen](https://github.com/DnDGen/DnDGen.RollGen).
- **Data selections**: your own types, stored in a table entry as text split into sections, and read back as objects.
- **Ninject modules** that wire all of it up, in your kernel.

## Installing

```
dotnet add package DnDGen.Infrastructure
```

Version 8 and later need .NET 10. It depends on [DnDGen.RollGen](https://github.com/DnDGen/DnDGen.RollGen) and on Ninject 3, and both come with the package. Earlier versions, and what changed in each, are on the [releases page](https://github.com/DnDGen/DnDGen.Infrastructure/releases).

## Using It

### Loading the modules

Everything is resolved from a Ninject kernel. `LoadModules` adds Infrastructure's modules, and RollGen's, which it depends on. It is safe to call twice, or on a kernel that already has RollGen's modules.

```csharp
var kernel = new StandardKernel();
InfrastructureModuleLoader.LoadModules(kernel);

var selector = kernel.Get<ICollectionSelector>();
```

### Where tables live

A table is an XML file in your assembly, embedded as a resource:

```xml
<ItemGroup>
  <EmbeddedResource Include="Tables\Collections\ReadmeCollectionTable.xml" />
</ItemGroup>
```

You find a table by the name of the assembly that embeds it and the table's name, which is the file's name without `.xml`. Folders don't matter: the table is the one resource in that assembly whose name ends in `.<table name>.xml`, so table names need to be unique within an assembly. The assembly is loaded by name, so it needs to be one your application can load.

Tables are read once per kernel and then cached, so changing an embedded table needs a new build, and a new kernel in tests.

### Collection tables

A collection table has `collections` as its root. Each `collection` has exactly one `name` and any number of `entry` elements. Collection names are unique within a table.

```xml
<?xml version="1.0" encoding="utf-8" ?>

<collections>
  <collection>
    <name>Weapons</name>
    <entry>Sword</entry>
    <entry>Axe</entry>
    <entry>Bow</entry>
  </collection>
  <collection>
    <name>Armor</name>
    <entry>Shield</entry>
    <entry>Helmet</entry>
  </collection>
  <collection>
    <name>Coins</name>
    <entry>Gold@2d6</entry>
    <entry>Silver@10</entry>
  </collection>
  <collection>
    <name>Monsters</name>
    <entry>Goblin@1</entry>
    <entry>Ogre@4</entry>
  </collection>
</collections>
```

`ICollectionSelector` reads them. The samples below read the table above, which is in the assembly named `MyGenerator`, as `ReadmeCollectionTable`.

```csharp
var assemblyName = "MyGenerator";
var tableName = "ReadmeCollectionTable";
var selector = kernel.Get<ICollectionSelector>();

var weapons = selector.SelectFrom(assemblyName, tableName, "Weapons");            // Sword, Axe, Bow
var table = selector.SelectAllFrom(assemblyName, tableName);                      // every collection, by name
var exists = selector.IsCollection(assemblyName, tableName, "Weapons");           // true
var missing = selector.IsCollection(assemblyName, tableName, "Potions");          // false

var collectionName = selector.FindCollectionOf(assemblyName, tableName, "Shield"); // Armor
var filteredName = selector.FindCollectionOf(assemblyName, tableName, "Shield", "Weapons", "Armor"); // Armor

var weapon = selector.SelectRandomFrom(assemblyName, tableName, "Weapons");       // one of Sword, Axe, Bow
var anyOf = selector.SelectRandomFrom(weapons);                                   // one of the entries you pass in

var byRarity = selector.SelectRandomFrom(common: weapons, rare: table["Armor"]);  // mostly a weapon, sometimes armor

var flattened = CollectionHelper.FlattenCollection(table, ["Weapons", "Armor"]);  // Sword, Axe, Bow, Shield, Helmet
```

- `SelectFrom` throws an `ArgumentException` if the collection isn't in the table.
- `FindCollectionOf` returns the first collection, in table order, that has the entry. If you list collection names after the entry, only those are looked in. It throws an `ArgumentException` if none has it.
- `SelectRandomFrom` picks one entry with equal chance, and throws an `ArgumentException` for an empty collection.
- `SelectRandomFrom` with rarities rolls 1 to 100: 1 to 60 picks from `common`, 61 to 90 from `uncommon`, 91 to 99 from `rare`, and 100 from `veryRare`. A rarity you leave out, or pass empty, is skipped, and the roll starts at the first rarity that has entries. It throws if every rarity is empty.
- `FlattenCollection` joins the entries of the collections whose names you list, without repeats.

### Percentile tables

A percentile table has `percentiles` as its root. Each `percentile` has a `lower` and `upper` roll, and the `content` those rolls give. Together they must cover every roll from 1 to 100 exactly once.

```xml
<?xml version="1.0" encoding="utf-8" ?>

<percentiles>
  <percentile>
    <lower>1</lower>
    <content>Copper@2d6</content>
    <upper>70</upper>
  </percentile>
  <percentile>
    <lower>71</lower>
    <content>Silver@1d6</content>
    <upper>95</upper>
  </percentile>
  <percentile>
    <lower>96</lower>
    <content>Gold@1d4</content>
    <upper>100</upper>
  </percentile>
</percentiles>
```

`IPercentileSelector` rolls a percentile and gives you what the table has for it.

```csharp
var percentiles = kernel.Get<IPercentileSelector>();
var tableName = "ReadmePercentileTable";

var coin = percentiles.SelectFrom(assemblyName, tableName);       // Copper@2d6 70% of the time, Silver@1d6 25%, Gold@1d4 5%
var coins = percentiles.SelectAllFrom(assemblyName, tableName);   // Copper@2d6, Silver@1d6, Gold@1d4, without repeats

var succeeded = percentiles.SelectFrom(.7);                       // true 30% of the time
var rolledEnough = percentiles.SelectFrom(70);                    // true on a roll of 70 or more
```

- `SelectFrom<T>` and `SelectAllFrom<T>` convert the content to `T`, such as an `int` for a table of numbers.
- `SelectFrom(double threshold)` is a chance, from 0 to 1. A roll at or below it is false, so .7 is false on a 70 and true on a 71. At 1 or more it is always false, and at 0 or less always true.
- `SelectFrom(int rollThreshold)` is a roll to meet. 70 is false on a 69 and true on a 70. Above 100 it is always false, and at 0 or less always true.

### Types and amounts

Many tables hold a type and an amount: "Gold, 2d6". `TypeAndAmountDataSelection` is that entry written as `Type@Roll`, and its selectors roll the amount for you.

```csharp
var coinSelector = kernel.Get<ICollectionTypeAndAmountSelector>();
var coins = coinSelector.SelectFrom(assemblyName, "ReadmeCollectionTable", "Coins");
// Gold with a Roll of 2d6 and an Amount from 2 to 12, then Silver with a Roll of 10 and an Amount of 10

var coinPercentiles = kernel.Get<IPercentileTypeAndAmountSelector>();
var coin = coinPercentiles.SelectFrom(assemblyName, "ReadmePercentileTable");
// Copper, Silver, or Gold, with its Amount rolled
```

`Roll` is any expression RollGen understands. `Amount` is the result, rounded to a whole number, and `AmountAsDouble` is the result as it was rolled.

### Your own data selections

To store something richer than a string in a table, make it a `DataSelection<T>`: say how it splits into sections, and how those sections make it again.

```csharp
public class Monster : DataSelection<Monster>
{
    public string Name { get; set; }
    public int Level { get; set; }

    public override Func<string[], Monster> MapTo => Map;
    public override Func<Monster, string[]> MapFrom => Map;
    public override int SectionCount => 2;

    public static Monster Map(string[] data) => new() { Name = data[0], Level = int.Parse(data[1]) };
    public static string[] Map(Monster monster) => [monster.Name, monster.Level.ToString()];
}
```

Bind it once, and you get selectors for it. An entry in the table is its sections joined by `@`, so `Goblin@1` is a goblin of level 1. Override `Separator` to join with another character.

```csharp
kernel.BindDataSelection<Monster>();

var monsters = kernel.Get<ICollectionDataSelector<Monster>>()
    .SelectFrom(assemblyName, "ReadmeCollectionTable", "Monsters");       // Goblin level 1, Ogre level 4
```

`IPercentileDataSelector<Monster>` is bound too, and reads a percentile table the same way. `DataHelper` does the splitting and joining by itself, for when you need it outside a table:

```csharp
var monster = DataHelper.Parse<Monster>("Goblin@1");      // Goblin, level 1
var text = DataHelper.Parse(monster);                     // Goblin@1

var sections = DataHelper.Parse("Goblin#1", '#');         // Goblin, 1
var joined = DataHelper.Parse(["Goblin", "1"], '#');      // Goblin#1
```

The parsers throw an `ArgumentException` if the data doesn't have `SectionCount` sections.

### Building things as you need them

A generator often needs a new instance of something each time. `JustInTimeFactory` asks the kernel for it, and is bound by `LoadModules`. If you bound something with a name, pass the name.

```csharp
kernel.Bind<string>().ToConstant("hello").Named("greeting");

var factory = kernel.Get<JustInTimeFactory>();
var selector = factory.Build<ICollectionSelector>();
var greeting = factory.Build<string>("greeting");
```

### Writing your own selector

`CollectionMapper` and `PercentileMapper` are what read a table into memory: a collection table becomes a dictionary of collection names to entries, and a percentile table a dictionary of every roll, 1 to 100, to its content. They are public so you can build a selector of your own on them, and they come from the kernel with the tables cached.

```csharp
public class WeaponSelector(CollectionMapper mapper)
{
    public IEnumerable<string> SelectAll(string assemblyName) => mapper.Map(assemblyName, "ReadmeCollectionTable")["Weapons"];
}
```

```csharp
var selector = kernel.Get<WeaponSelector>();
var weapons = selector.SelectAll(assemblyName);           // Sword, Axe, Bow
```

`AssemblyLoader` finds the assembly a table is in, from its name. The default loads it by name. Bind your own `AssemblyLoader` if your tables are in assemblies that need loading another way.

## Reference

Every public member of every public type. Where a type has a generic constraint, such as `where T : DataSelection<T>, new()`, its description says so.

### `InfrastructureModuleLoader`

Loads Infrastructure's Ninject modules, and RollGen's.

| Constructor | Description |
|---|---|
| `InfrastructureModuleLoader()` | Creates a loader. `LoadModules` is static, so you don't need one. |

| Method | Description |
|---|---|
| `LoadModules(IKernel kernel)` | Loads RollGen's modules, then Infrastructure's, into the kernel. Skips any module the kernel already has, so it is safe to call twice. |

### `BindingExtensions`

| Method | Description |
|---|---|
| `BindDataSelection<T>(this IKernel kernel)` | Binds `ICollectionDataSelector<T>` and `IPercentileDataSelector<T>` for your data selection, if they aren't bound yet, and returns the kernel. `T` is a `DataSelection<T>` with a public parameterless constructor. |

### `CollectionHelper`

| Method | Description |
|---|---|
| `FlattenCollection(Dictionary<string, IEnumerable<string>> collections, IEnumerable<string> keys)` | The entries of the collections named in `keys`, without repeats. |

### `DataHelper`

Splits and joins the sections of a data selection's text. The separator is `@` unless you give another.

| Method | Description |
|---|---|
| `Parse<T>(string rawData, Func<string[], T> map, char separator = '@')` | Splits the text, and maps its sections to `T`. Throws an `ArgumentException` if there aren't `SectionCount` sections. `T` is a `DataSelection<T>` with a public parameterless constructor. |
| `Parse(string rawData, char separator = '@')` | Splits the text into its sections. |
| `Parse(string[] splitData, char separator = '@')` | Joins the sections into text. |
| `Parse<T>(T data, Func<T, string[]> map, char separator = '@')` | Maps `T` to sections, and joins them. Throws an `ArgumentException` if there aren't `SectionCount` sections. `T` is a `DataSelection<T>`. |
| `Parse<T>(string rawData)` | Splits the text by the separator of `T`, and builds a `T` with its `MapTo`. `T` is a `DataSelection<T>` with a public parameterless constructor. |
| `Parse<T>(T data)` | Joins the sections from `MapFrom` with the separator of `T`. `T` is a `DataSelection<T>`. |

### `DataSelection<T>`

The base of your own data selections. You implement the abstract properties.

| Property | Description |
|---|---|
| `MapTo` | Abstract. Builds a `T` from its sections. |
| `MapFrom` | Abstract. Breaks a `T` into its sections. |
| `SectionCount` | Abstract. How many sections the text has. |
| `Separator` | Virtual. The character between sections: `@`. |

### `TypeAndAmountDataSelection`

A type and an amount, written `Type@Roll`.

| Constructor | Description |
|---|---|
| `TypeAndAmountDataSelection()` | Creates an empty selection. |

| Method | Description |
|---|---|
| `Map(string[] splitData)` | Builds a selection from its `Type` and `Roll` sections. |
| `Map(TypeAndAmountDataSelection selection)` | Breaks a selection into its `Type` and `Roll` sections. |

| Property | Description |
|---|---|
| `Type` | What the amount is of. |
| `Amount` | `AmountAsDouble`, rounded to a whole number. |
| `AmountAsDouble` | The result of rolling `Roll`. The selectors set it. |
| `Roll` | A RollGen expression for the amount, such as `2d6`. |
| `MapTo` | Maps sections to a selection, with `Map`. |
| `MapFrom` | Maps a selection to sections, with `Map`. |
| `SectionCount` | 2. |

### `ICollectionSelector`

Reads collection tables. Implemented for you.

| Method | Description |
|---|---|
| `SelectFrom(string assemblyName, string tableName, string collectionName)` | The entries of the collection. Throws an `ArgumentException` if it isn't in the table. |
| `SelectAllFrom(string assemblyName, string tableName)` | Every collection in the table, by name. |
| `FindCollectionOf(string assemblyName, string tableName, string entry, params string[] filteredCollectionNames)` | The first collection, in table order, with the entry, only looking in the named collections if you name any. Throws an `ArgumentException` if there is none. |
| `SelectRandomFrom<T>(IEnumerable<T> collection)` | One of the entries, with equal chance. Throws an `ArgumentException` if the collection is empty. |
| `SelectRandomFrom(string assemblyName, string tableName, string collectionName)` | One of the collection's entries, with equal chance. |
| `SelectRandomFrom<T>(IEnumerable<T> common = null, IEnumerable<T> uncommon = null, IEnumerable<T> rare = null, IEnumerable<T> veryRare = null)` | One entry, from a rarity chosen by a roll of 1 to 100: 60% common, 30% uncommon, 9% rare, 1% very rare, with empty rarities skipped. |
| `IsCollection(string assemblyName, string tableName, string collectionName)` | Whether the table has the collection. |

### `ICollectionDataSelector<T>`

Reads collection tables of a data selection `T`, where `T` is a `DataSelection<T>`. Bound by `BindDataSelection<T>`.

| Method | Description |
|---|---|
| `SelectFrom(string assemblyName, string tableName, string collectionName)` | The collection's entries, as `T`. |
| `SelectOneFrom(string assemblyName, string tableName, string collectionName)` | The collection's only entry. Throws if it doesn't have exactly one. |
| `SelectAllFrom(string assemblyName, string tableName)` | Every collection in the table, by name, as `T`. |
| `IsCollection(string assemblyName, string tableName, string collectionName)` | Whether the table has the collection. |
| `SelectRandomFrom(string assemblyName, string tableName, string collectionName)` | One of the collection's entries, as `T`, with equal chance. |

### `ICollectionTypeAndAmountSelector`

`ICollectionDataSelector<TypeAndAmountDataSelection>`, with each `Amount` rolled.

| Method | Description |
|---|---|
| `SelectFrom(string assemblyName, string tableName, string collectionName)` | The collection's entries. |
| `SelectOneFrom(string assemblyName, string tableName, string collectionName)` | The collection's only entry. Throws if it doesn't have exactly one. |
| `SelectAllFrom(string assemblyName, string tableName)` | Every collection in the table, by name. |
| `IsCollection(string assemblyName, string tableName, string collectionName)` | Whether the table has the collection. |
| `SelectRandomFrom(string assemblyName, string tableName, string collectionName)` | One of the collection's entries, with equal chance. |

### `IPercentileSelector`

Reads percentile tables, and rolls chances. Implemented for you.

| Method | Description |
|---|---|
| `SelectFrom(string assemblyName, string tableName)` | The content for a percentile roll. |
| `SelectAllFrom(string assemblyName, string tableName)` | Every different content in the table. |
| `SelectFrom<T>(string assemblyName, string tableName)` | The content for a percentile roll, converted to `T`. |
| `SelectAllFrom<T>(string assemblyName, string tableName)` | Every different content in the table, converted to `T`. |
| `SelectFrom(double threshold)` | Whether a percentile roll beats a chance from 0 to 1. A roll at or below it is false. Always false at 1 or more, and true at 0 or less. |
| `SelectFrom(int rollThreshold)` | Whether a percentile roll meets the threshold or more. Always false above 100, and true at 0 or less. |

### `IPercentileDataSelector<T>`

Reads percentile tables of a data selection `T`, where `T` is a `DataSelection<T>`. Bound by `BindDataSelection<T>`.

| Method | Description |
|---|---|
| `SelectFrom(string assemblyName, string tableName)` | The content for a percentile roll, as `T`. |
| `SelectAllFrom(string assemblyName, string tableName)` | Every different content in the table, as `T`. |

### `IPercentileTypeAndAmountSelector`

`IPercentileDataSelector<TypeAndAmountDataSelection>`, with each `Amount` rolled.

| Method | Description |
|---|---|
| `SelectFrom(string assemblyName, string tableName)` | The content for a percentile roll. |
| `SelectAllFrom(string assemblyName, string tableName)` | Every different content in the table. |

### `JustInTimeFactory`

Asks the kernel for something when you need it. Bound by `LoadModules`.

| Method | Description |
|---|---|
| `Build<T>()` | Gets a `T` from the kernel. |
| `Build<T>(string name)` | Gets the `T` the kernel has bound to the name. |

### `CollectionMapper`

Reads a collection table into memory. Bound, with its result cached, so you can build a selector on it.

| Method | Description |
|---|---|
| `Map(string assemblyName, string tableName)` | Every collection in the table, by name. Throws if the table isn't there or isn't a well-formed collection table. |

### `PercentileMapper`

Reads a percentile table into memory. Bound, with its result cached, so you can build a selector on it.

| Method | Description |
|---|---|
| `Map(string assemblyName, string tableName)` | The content for every roll from 1 to 100. Throws if the table isn't there, isn't well formed, or doesn't cover 1 to 100 exactly once. |

### `AssemblyLoader`

Finds the assembly a table is in. The default loads it by name.

| Method | Description |
|---|---|
| `GetAssembly(string name)` | The assembly with that name. |
