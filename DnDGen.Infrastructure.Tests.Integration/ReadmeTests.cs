using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.IoC;
using DnDGen.Infrastructure.IoC.Modules;
using DnDGen.Infrastructure.Mappers.Collections;
using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace DnDGen.Infrastructure.Tests.Integration
{
    [TestFixture]
    public partial class ReadmeTests : IntegrationTests
    {
        private const string CollectionTableName = "ReadmeCollectionTable";
        private const string PercentileTableName = "ReadmePercentileTable";
        private const BindingFlags DeclaredPublicMembers = BindingFlags.Public | BindingFlags.Instance | BindingFlags.Static | BindingFlags.DeclaredOnly;

        private static readonly Type[] PublicTypes = typeof(InfrastructureModuleLoader).Assembly.GetExportedTypes();
        private static readonly Dictionary<Type, string> TypeKeywords = new()
        {
            [typeof(bool)] = "bool",
            [typeof(char)] = "char",
            [typeof(double)] = "double",
            [typeof(int)] = "int",
            [typeof(object)] = "object",
            [typeof(string)] = "string",
        };

        private string[] _readme;

        [OneTimeSetUp]
        public void ReadReadme()
        {
            var path = Path.Combine(TestContext.CurrentContext.TestDirectory, "README.md");
            _readme = File.ReadAllLines(path);
        }

        [Test]
        public void LoadModules()
        {
            var moduleKernel = new StandardKernel();
            InfrastructureModuleLoader.LoadModules(moduleKernel);
            InfrastructureModuleLoader.LoadModules(moduleKernel);

            var selector = moduleKernel.Get<ICollectionSelector>();
            Assert.That(selector, Is.Not.Null);
        }

        [Test]
        [Repeat(100)]
        public void CollectionSelection()
        {
            var selector = GetNewInstanceOf<ICollectionSelector>();

            var weapons = selector.SelectFrom(assemblyName, CollectionTableName, "Weapons");
            Assert.That(weapons, Is.EqualTo(["Sword", "Axe", "Bow"]));

            var table = selector.SelectAllFrom(assemblyName, CollectionTableName);
            Assert.That(table.Keys, Is.EquivalentTo(["Weapons", "Armor", "Coins", "Monsters"]));
            Assert.That(table["Armor"], Is.EqualTo(["Shield", "Helmet"]));

            Assert.That(selector.IsCollection(assemblyName, CollectionTableName, "Weapons"), Is.True);
            Assert.That(selector.IsCollection(assemblyName, CollectionTableName, "Potions"), Is.False);

            var collectionName = selector.FindCollectionOf(assemblyName, CollectionTableName, "Shield");
            Assert.That(collectionName, Is.EqualTo("Armor"));

            var filteredName = selector.FindCollectionOf(assemblyName, CollectionTableName, "Shield", "Weapons", "Armor");
            Assert.That(filteredName, Is.EqualTo("Armor"));

            var randomWeapon = selector.SelectRandomFrom(assemblyName, CollectionTableName, "Weapons");
            Assert.That(randomWeapon, Is.AnyOf("Sword", "Axe", "Bow"));

            var randomFromCollection = selector.SelectRandomFrom(weapons);
            Assert.That(randomFromCollection, Is.AnyOf("Sword", "Axe", "Bow"));

            var randomByRarity = selector.SelectRandomFrom(common: weapons, rare: table["Armor"]);
            Assert.That(randomByRarity, Is.AnyOf("Sword", "Axe", "Bow", "Shield", "Helmet"));

            var flattened = CollectionHelper.FlattenCollection(table, ["Weapons", "Armor"]);
            Assert.That(flattened, Is.EqualTo(["Sword", "Axe", "Bow", "Shield", "Helmet"]));
        }

        [Test]
        [Repeat(100)]
        public void PercentileSelection()
        {
            var selector = GetNewInstanceOf<IPercentileSelector>();

            var coin = selector.SelectFrom(assemblyName, PercentileTableName);
            Assert.That(coin, Is.AnyOf("Copper@2d6", "Silver@1d6", "Gold@1d4"));

            var coins = selector.SelectAllFrom(assemblyName, PercentileTableName);
            Assert.That(coins, Is.EquivalentTo(["Copper@2d6", "Silver@1d6", "Gold@1d4"]));

            Assert.That(selector.SelectFrom(1.0), Is.False);
            Assert.That(selector.SelectFrom(0.0), Is.True);
            Assert.That(selector.SelectFrom(101), Is.False);
            Assert.That(selector.SelectFrom(0), Is.True);
            Assert.That(() => selector.SelectFrom(0.7), Throws.Nothing);
            Assert.That(() => selector.SelectFrom(70), Throws.Nothing);
        }

        [Test]
        [Repeat(100)]
        public void TypeAndAmountSelection()
        {
            var collectionSelector = GetNewInstanceOf<ICollectionTypeAndAmountSelector>();

            var coins = collectionSelector.SelectFrom(assemblyName, CollectionTableName, "Coins").ToArray();
            Assert.That(coins.Select(c => c.Type), Is.EqualTo(["Gold", "Silver"]));
            Assert.That(coins[0].Roll, Is.EqualTo("2d6"));
            Assert.That(coins[0].Amount, Is.InRange(2, 12));
            Assert.That(coins[1].Roll, Is.EqualTo("10"));
            Assert.That(coins[1].Amount, Is.EqualTo(10));

            var percentileSelector = GetNewInstanceOf<IPercentileTypeAndAmountSelector>();

            var coin = percentileSelector.SelectFrom(assemblyName, PercentileTableName);
            Assert.That(coin.Type, Is.AnyOf("Copper", "Silver", "Gold"));
            Assert.That(coin.Amount, Is.InRange(1, 12));
        }

        [Test]
        public void DataSelectionSelection()
        {
            kernel.BindDataSelection<Monster>();

            var selector = kernel.Get<ICollectionDataSelector<Monster>>();
            var monsters = selector.SelectFrom(assemblyName, CollectionTableName, "Monsters").ToArray();

            Assert.That(monsters, Has.Length.EqualTo(2));
            Assert.That(monsters[0].Name, Is.EqualTo("Goblin"));
            Assert.That(monsters[0].Level, Is.EqualTo(1));
            Assert.That(monsters[1].Name, Is.EqualTo("Ogre"));
            Assert.That(monsters[1].Level, Is.EqualTo(4));
        }

        [Test]
        public void DataHelperParsing()
        {
            var monster = DataHelper.Parse<Monster>("Goblin@1");
            Assert.That(monster.Name, Is.EqualTo("Goblin"));
            Assert.That(monster.Level, Is.EqualTo(1));
            Assert.That(DataHelper.Parse(monster), Is.EqualTo("Goblin@1"));

            Assert.That(DataHelper.Parse("Goblin#1", '#'), Is.EqualTo(["Goblin", "1"]));
            Assert.That(DataHelper.Parse(["Goblin", "1"], '#'), Is.EqualTo("Goblin#1"));
        }

        [Test]
        public void JustInTimeFactory()
        {
            var factoryKernel = new StandardKernel();
            InfrastructureModuleLoader.LoadModules(factoryKernel);
            factoryKernel.Bind<string>().ToConstant("hello").Named("greeting");

            var factory = factoryKernel.Get<JustInTimeFactory>();

            Assert.That(factory.Build<ICollectionSelector>(), Is.Not.Null);
            Assert.That(factory.Build<string>("greeting"), Is.EqualTo("hello"));
        }

        [Test]
        public void HandRolledSelector()
        {
            var selector = kernel.Get<WeaponSelector>();
            var weapons = selector.SelectAll(assemblyName);

            Assert.That(weapons, Is.EqualTo(["Sword", "Axe", "Bow"]));
        }

        [Test]
        public void CollectionTable_ReadmeMatchesEmbeddedFile()
        {
            var shown = GetCodeBlock("xml", "<collections>");
            Assert.That(shown, Is.EqualTo(ReadEmbeddedTable(CollectionTableName)));
        }

        [Test]
        public void PercentileTable_ReadmeMatchesEmbeddedFile()
        {
            var shown = GetCodeBlock("xml", "<percentiles>");
            Assert.That(shown, Is.EqualTo(ReadEmbeddedTable(PercentileTableName)));
        }

        [Test]
        public void Constructors_AreAllDocumented()
        {
            var constructors = PublicTypes
                .SelectMany(type => type.GetConstructors().Select(constructor => $"{WriteType(type)}.{WriteMember(constructor)}"));

            Assert.That(GetDocumented("Constructor"), Is.EquivalentTo(constructors));
        }

        [Test]
        public void Methods_AreAllDocumented()
        {
            var methods = PublicTypes
                .SelectMany(type => type.GetMethods(DeclaredPublicMembers)
                    .Where(method => !method.IsSpecialName)
                    .Select(method => $"{WriteType(type)}.{WriteMember(method)}"));

            Assert.That(GetDocumented("Method"), Is.EquivalentTo(methods));
        }

        [Test]
        public void Properties_AreAllDocumented()
        {
            var fields = PublicTypes.SelectMany(type => type.GetFields(DeclaredPublicMembers).Select(field => $"{WriteType(type)}.{field.Name}"));
            var properties = PublicTypes.SelectMany(type => type.GetProperties(DeclaredPublicMembers).Select(property => $"{WriteType(type)}.{property.Name}"));

            Assert.That(GetDocumented("Property"), Is.EquivalentTo(fields.Concat(properties)));
        }

        private string[] GetDocumented(string kind) => [.. GetTableRows(kind)
            .SelectMany(row => CodeSpanPartialRegex().Matches(row.Cells[0]).Select(match => $"{row.Type}.{match.Groups[1].Value}"))];

        private IEnumerable<(string Type, string[] Cells)> GetTableRows(string kind)
        {
            var type = string.Empty;
            string[] header = [];

            foreach (var line in GetLinesOutsideCodeBlocks(_readme))
            {
                if (line.StartsWith("### "))
                    type = line.Trim('#', ' ', '`');

                if (!line.StartsWith('|'))
                {
                    header = [];
                    continue;
                }

                string[] cells = [.. line.Trim().Trim('|').Split('|').Select(cell => cell.Trim())];

                if (header.Length == 0)
                    header = cells;
                else if (header[0] == kind && !cells[0].StartsWith("---"))
                    yield return (type, cells);
            }
        }

        private string GetCodeBlock(string language, string contains)
        {
            var blocks = new List<List<string>>();
            List<string> current = null;

            foreach (var line in _readme)
            {
                if (current == null && line.StartsWith($"```{language}"))
                    current = [];
                else if (current != null && line.StartsWith("```"))
                {
                    blocks.Add(current);
                    current = null;
                }
                else
                    current?.Add(line);
            }

            var matching = blocks.Where(block => block.Any(line => line.Contains(contains))).ToArray();
            Assert.That(matching, Has.Length.EqualTo(1), $"The README needs one {language} block containing {contains}");

            return Normalize(string.Join("\n", matching[0]));
        }

        private static string ReadEmbeddedTable(string tableName)
        {
            var assembly = typeof(ReadmeTests).Assembly;
            var resource = assembly.GetManifestResourceNames().Single(name => name.EndsWith($".{tableName}.xml"));

            using var reader = new StreamReader(assembly.GetManifestResourceStream(resource));
            return Normalize(reader.ReadToEnd());
        }

        private static string Normalize(string text) => text.TrimStart('﻿').Replace("\r\n", "\n").TrimEnd();

        private static string WriteMember(MethodBase member)
        {
            var name = member is ConstructorInfo ? WriteTypeName(member.DeclaringType) : member.Name;

            if (member.IsGenericMethod)
                name += $"<{string.Join(", ", member.GetGenericArguments().Select(WriteType))}>";

            var parameters = member.GetParameters().Select(WriteParameter);

            return $"{name}({string.Join(", ", parameters)})";
        }

        private static string WriteParameter(ParameterInfo parameter)
        {
            var written = $"{WriteType(parameter.ParameterType)} {parameter.Name}";

            if (parameter.IsDefined(typeof(ParamArrayAttribute)))
                written = $"params {written}";

            if (parameter.Position == 0 && parameter.Member.IsDefined(typeof(ExtensionAttribute)))
                written = $"this {written}";

            if (parameter.HasDefaultValue)
                written += $" = {WriteValue(parameter.DefaultValue)}";

            return written;
        }

        private static string WriteType(Type type)
        {
            if (Nullable.GetUnderlyingType(type) is Type underlyingType)
                return $"{WriteType(underlyingType)}?";

            if (type.IsArray)
                return $"{WriteType(type.GetElementType())}[]";

            if (type.IsGenericType)
                return $"{WriteTypeName(type)}<{string.Join(", ", type.GetGenericArguments().Select(WriteType))}>";

            return TypeKeywords.GetValueOrDefault(type, type.Name);
        }

        private static string WriteTypeName(Type type) => type.IsGenericType ? type.Name[..type.Name.IndexOf('`')] : type.Name;

        private static string WriteValue(object value) => value switch
        {
            null => "null",
            bool boolean => boolean ? "true" : "false",
            char character => $"'{character.ToString().Replace("\\", "\\\\")}'",
            string text => $"\"{text}\"",
            _ => Convert.ToString(value, CultureInfo.InvariantCulture),
        };

        private static IEnumerable<string> GetLinesOutsideCodeBlocks(IEnumerable<string> lines)
        {
            var inCodeBlock = false;

            foreach (var line in lines)
            {
                if (line.StartsWith("```"))
                    inCodeBlock = !inCodeBlock;
                else if (!inCodeBlock)
                    yield return line;
            }
        }

        [GeneratedRegex("`([^`]+)`")]
        private static partial Regex CodeSpanPartialRegex();

        internal class Monster : DataSelection<Monster>
        {
            public string Name { get; set; }
            public int Level { get; set; }

            public override Func<string[], Monster> MapTo => Map;
            public override Func<Monster, string[]> MapFrom => Map;
            public override int SectionCount => 2;

            public static Monster Map(string[] data) => new() { Name = data[0], Level = int.Parse(data[1]) };
            public static string[] Map(Monster monster) => [monster.Name, monster.Level.ToString()];
        }

        internal class WeaponSelector(CollectionMapper mapper)
        {
            public IEnumerable<string> SelectAll(string assemblyName) => mapper.Map(assemblyName, CollectionTableName)["Weapons"];
        }
    }
}
