using DnDGen.Infrastructure.Selectors.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Integration.Selectors.Collections
{
    [TestFixture]
    public class CollectionSelectorTests : IntegrationTests
    {
        private ICollectionSelector collectionsSelector;
        private Stopwatch stopwatch;

        [SetUp]
        public void Setup()
        {
            collectionsSelector = GetNewInstanceOf<ICollectionSelector>();
            stopwatch = new Stopwatch();
        }

        [TestCase("")]
        [TestCase("name", "entry 1", "entry 2")]
        [TestCase("collection", "entry 2", "entry 3", "collection", "sub-collection")]
        [TestCase("sub-collection", "entry 3", "entry 4", "sub-collection")]
        public void SelectFromTable(string name, params string[] collection)
        {
            var selectedCollection = collectionsSelector.SelectFrom("CollectionTable", name);
            Assert.That(selectedCollection, Is.EquivalentTo(collection));
        }

        [Test]
        public void SelectAllFromTable()
        {
            var selectedCollections = collectionsSelector.SelectAllFrom("CollectionTable");
            Assert.That(selectedCollections, Has.Count.EqualTo(4)
                .And.ContainKey(string.Empty)
                .And.ContainKey("name")
                .And.ContainKey("collection")
                .And.ContainKey("sub-collection"));
            Assert.That(selectedCollections[string.Empty], Is.Empty);
            Assert.That(selectedCollections["name"], Is.EquivalentTo(new[] { "entry 1", "entry 2" }));
            Assert.That(selectedCollections["collection"], Is.EquivalentTo(new[] { "entry 2", "entry 3", "collection", "sub-collection" }));
            Assert.That(selectedCollections["sub-collection"], Is.EquivalentTo(new[] { "entry 3", "entry 4", "sub-collection" }));
        }

        [Test]
        public void FindCollectionOfNameFromTable()
        {
            var collectionName = collectionsSelector.FindCollectionOf("CollectionTable", "entry 3", "collection", "sub-collection");
            Assert.That(collectionName, Is.EqualTo("collection"));
        }

        [TestCase("name", "entry 1", "entry 2")]
        [TestCase("collection", "entry 2", "entry 3", "collection", "sub-collection")]
        [TestCase("sub-collection", "entry 3", "entry 4", "sub-collection")]
        public void SelectRandomFromTable(string name, params string[] collection)
        {
            var entry = collectionsSelector.SelectRandomFrom("CollectionTable", name);
            Assert.That(new[] { entry }, Is.SubsetOf(collection));
        }

        [Test]
        public void IsCollectionInTable()
        {
            var isCollection = collectionsSelector.IsCollection("CollectionTable", "sub-collection");
            Assert.That(isCollection, Is.True);
        }

        [Test]
        public void IsNotCollectionInTable()
        {
            var isCollection = collectionsSelector.IsCollection("CollectionTable", "entry 3");
            Assert.That(isCollection, Is.False);
        }

        [Test]
        public void ExplodeFromTable()
        {
            var explodedCollection = collectionsSelector.Explode("CollectionTable", "collection");
            Assert.That(explodedCollection, Is.Unique.And.EquivalentTo(new[]
            {
                "entry 2",
                "entry 3",
                "entry 4",
                "collection",
                "sub-collection",
            }));
        }

        [Test]
        public void ExplodeFromTableIntoOtherTable()
        {
            var explodedCollection = collectionsSelector.Explode("CollectionTable", "collection");
            var allCollections = collectionsSelector.SelectAllFrom("OtherCollectionTable");

            var executedExplodedCollection = allCollections.Where(kvp => explodedCollection.Contains(kvp.Key))
                .Select(kvp => kvp.Value)
                .SelectMany(v => v)
                .Distinct()
                .ToArray();

            Assert.That(executedExplodedCollection, Is.EquivalentTo(new[]
            {
                "other entry 2.1",
                "other entry 2.2",
                "other entry 3.1",
                "other entry 3.2",
                "other entry 4.1",
                "other entry 4.2",
                "other entry 2c",
                "other entry 3c",
                "other entry 3sc",
                "other entry 4sc",
            }));
        }

        [TestCase("EncounterGen-CreatureGroups", "Magic", 265)]
        [TestCase("EncounterGen-CreatureGroups", "Night", 1484)]
        [TestCase("EncounterGen-CreatureGroups", "Wilderness", 573)]
        [TestCase("EncounterGen-CreatureGroups", "ColdCivilized", 611)]
        [TestCase("EncounterGen-CreatureGroups", "Land", 93)]
        [TestCase("EncounterGen-CreatureGroups", "TemperateForest", 98)]
        [TestCase("EncounterGen-CreatureGroups", "TemperateAquatic", 32)]
        [TestCase("EncounterGen-CreatureGroups", "WarmPlains", 37)]
        [TestCase("EncounterGen-CreatureGroups", "Day", 1422)]
        [TestCase("EncounterGen-CreatureGroups", "Aquatic", 2)]
        [TestCase("EncounterGen-CreatureGroups", "Underground", 68)]
        [TestCase("EncounterGen-CreatureGroups", "UndergroundAquatic", 3)]
        [TestCase("CreatureGen-CreatureGroups", "Fortitude", 485)]
        [TestCase("CreatureGen-CreatureGroups", "Reflex", 437)]
        [TestCase("CreatureGen-CreatureGroups", "Will", 318)]
        public void Explode_IsEfficient(string table, string entry, int count)
        {
            var timeLimit = Math.Max(0.1, count / 10_000d);

            stopwatch.Restart();
            var explodedCollection = collectionsSelector.Explode(table, entry);
            stopwatch.Stop();

            Assert.That(explodedCollection, Is.Not.Empty.And.Unique);
            Assert.That(explodedCollection.Count, Is.EqualTo(count));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(timeLimit));
        }

        [TestCase("EncounterGen-CreatureGroups", "Magic", 265)]
        [TestCase("EncounterGen-CreatureGroups", "Night", 1484)]
        [TestCase("EncounterGen-CreatureGroups", "Wilderness", 573)]
        [TestCase("EncounterGen-CreatureGroups", "ColdCivilized", 611)]
        [TestCase("EncounterGen-CreatureGroups", "Land", 93)]
        [TestCase("EncounterGen-CreatureGroups", "TemperateForest", 98)]
        [TestCase("EncounterGen-CreatureGroups", "TemperateAquatic", 32)]
        [TestCase("EncounterGen-CreatureGroups", "WarmPlains", 37)]
        [TestCase("EncounterGen-CreatureGroups", "Day", 1422)]
        [TestCase("EncounterGen-CreatureGroups", "Aquatic", 2)]
        [TestCase("EncounterGen-CreatureGroups", "Underground", 68)]
        [TestCase("EncounterGen-CreatureGroups", "UndergroundAquatic", 3)]
        [TestCase("CreatureGen-CreatureGroups", "Fortitude", 485)]
        [TestCase("CreatureGen-CreatureGroups", "Reflex", 437)]
        [TestCase("CreatureGen-CreatureGroups", "Will", 318)]
        public void Explode_Cached_IsEfficient(string table, string entry, int count)
        {
            collectionsSelector.Explode(table, entry);

            stopwatch.Restart();
            var explodedCollection = collectionsSelector.Explode(table, entry);
            stopwatch.Stop();

            Assert.That(explodedCollection, Is.Not.Empty.And.Unique);
            Assert.That(explodedCollection.Count, Is.EqualTo(count));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(0.01));
        }

        [Test]
        public void HeavySelectAllIsEfficient()
        {
            stopwatch.Restart();
            var allCollections = collectionsSelector.SelectAllFrom("EncounterGroups");
            stopwatch.Stop();

            Assert.That(allCollections, Is.Not.Empty.And.Unique);
            Assert.That(allCollections.Count, Is.EqualTo(1484));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(0.1));
        }

        [TestCase("EncounterGen-CreatureGroups", "Magic", 572)]
        [TestCase("EncounterGen-CreatureGroups", "Night", 2538)]
        [TestCase("EncounterGen-CreatureGroups", "Wilderness", 1115)]
        [TestCase("EncounterGen-CreatureGroups", "ColdCivilized", 953)]
        [TestCase("EncounterGen-CreatureGroups", "Land", 133)]
        [TestCase("EncounterGen-CreatureGroups", "TemperateForest", 150)]
        [TestCase("EncounterGen-CreatureGroups", "TemperateAquatic", 45)]
        [TestCase("EncounterGen-CreatureGroups", "WarmPlains", 66)]
        [TestCase("EncounterGen-CreatureGroups", "Day", 2412)]
        [TestCase("EncounterGen-CreatureGroups", "Aquatic", 6)]
        [TestCase("EncounterGen-CreatureGroups", "Underground", 91)]
        [TestCase("EncounterGen-CreatureGroups", "UndergroundAquatic", 6)]
        public void HeavySeparatedExplodeAndFlattenIsEfficient(string table, string entry, int count)
        {
            var timeLimit = Math.Max(0.1, count / 10_000d);

            stopwatch.Restart();
            var flattenedCollection = ExplodeAndFlatten(table, entry, "EncounterGroups");
            stopwatch.Stop();

            Assert.That(flattenedCollection, Is.Not.Empty.And.Unique);
            Assert.That(flattenedCollection.Count, Is.EqualTo(count));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(timeLimit));
        }

        [TestCase("EncounterGen-CreatureGroups", "Magic", 572)]
        [TestCase("EncounterGen-CreatureGroups", "Night", 2538)]
        [TestCase("EncounterGen-CreatureGroups", "Wilderness", 1115)]
        [TestCase("EncounterGen-CreatureGroups", "ColdCivilized", 953)]
        [TestCase("EncounterGen-CreatureGroups", "Land", 133)]
        [TestCase("EncounterGen-CreatureGroups", "TemperateForest", 150)]
        [TestCase("EncounterGen-CreatureGroups", "TemperateAquatic", 45)]
        [TestCase("EncounterGen-CreatureGroups", "WarmPlains", 66)]
        [TestCase("EncounterGen-CreatureGroups", "Day", 2412)]
        [TestCase("EncounterGen-CreatureGroups", "Aquatic", 6)]
        [TestCase("EncounterGen-CreatureGroups", "Underground", 91)]
        [TestCase("EncounterGen-CreatureGroups", "UndergroundAquatic", 6)]
        public void HeavySeparatedExplodeAndFlatten_Cached_IsEfficient(string table, string entry, int count)
        {
            ExplodeAndFlatten(table, entry, "EncounterGroups");

            stopwatch.Restart();
            var flattenedCollection = ExplodeAndFlatten(table, entry, "EncounterGroups");
            stopwatch.Stop();

            Assert.That(flattenedCollection, Is.Not.Empty.And.Unique);
            Assert.That(flattenedCollection.Count, Is.EqualTo(count));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(0.01));
        }

        private IEnumerable<string> ExplodeAndFlatten(string explodeTableName, string entry, string flattenTableName)
        {
            var explodedCollection = collectionsSelector.Explode(explodeTableName, entry);
            var allCollections = collectionsSelector.SelectAllFrom(flattenTableName);
            var flattenedCollection = collectionsSelector.Flatten(allCollections, explodedCollection);

            return flattenedCollection;
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(10)]
        public void CreateWeighted_IsFast_ViaAll(int collectionQuantity)
        {
            stopwatch.Restart();
            var collection = GetWeightedAppearances(
                allSkin: Enumerable.Range(1, collectionQuantity).Select(i => $"Skin {i}"),
                allHair: Enumerable.Range(1, collectionQuantity).Select(i => $"Hair {i}"),
                allEyes: Enumerable.Range(1, collectionQuantity).Select(i => $"Eyes {i}"),
                allOther: Enumerable.Range(1, collectionQuantity).Select(i => $"Other {i}")
            );
            stopwatch.Stop();

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(.1).Within(.01));
            Assert.That(collection, Is.Not.Empty);
            Assert.That(collection, Is.All.Not.Empty);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        public void CreateWeighted_IsFast_ViaWeighting(int collectionQuantity)
        {
            stopwatch.Restart();
            var collection = GetWeightedAppearances(
                commonSkin: Enumerable.Range(1, collectionQuantity).Select(i => $"Common Skin {i}"),
                uncommonSkin: Enumerable.Range(1, collectionQuantity).Select(i => $"Uncommon Skin {i}"),
                rareSkin: Enumerable.Range(1, collectionQuantity).Select(i => $"Rare Skin {i}")
            );
            stopwatch.Stop();

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(.1).Within(.01));
            Assert.That(collection, Is.Not.Empty);
            Assert.That(collection, Is.All.Not.Empty);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(5)]
        [TestCase(10)]
        public void CreateWeighted_IsFast_ViaMultipleWeighting(int collectionQuantity)
        {
            stopwatch.Restart();
            var collection = GetWeightedAppearances(
                commonSkin: Enumerable.Range(1, collectionQuantity).Select(i => $"Common Skin {i}"),
                commonHair: Enumerable.Range(1, collectionQuantity).Select(i => $"Common Hair {i}"),
                uncommonSkin: Enumerable.Range(1, collectionQuantity).Select(i => $"Uncommon Skin {i}"),
                uncommonHair: Enumerable.Range(1, collectionQuantity).Select(i => $"Uncommon Hair {i}"),
                rareSkin: Enumerable.Range(1, collectionQuantity).Select(i => $"Rare Skin {i}"),
                rareHair: Enumerable.Range(1, collectionQuantity).Select(i => $"Rare Hair {i}")
            );
            stopwatch.Stop();

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(.1).Within(.01));
            Assert.That(collection, Is.Not.Empty);
            Assert.That(collection, Is.All.Not.Empty);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(4, IgnoreReason = "1 million items in the weighted colletion")]
        [TestCase(5, IgnoreReason = "2.5 million items in the weighted colletion")]
        public void CreateWeighted_IsFast_ViaAllWeighting(int collectionQuantity)
        {
            stopwatch.Restart();
            var collection = GetWeightedAppearances(
                commonSkin: Enumerable.Range(1, collectionQuantity).Select(i => $"Common Skin {i}"),
                commonHair: Enumerable.Range(1, collectionQuantity).Select(i => $"Common Hair {i}"),
                commonEyes: Enumerable.Range(1, collectionQuantity).Select(i => $"Common Eyes {i}"),
                commonOther: Enumerable.Range(1, collectionQuantity).Select(i => $"Common Other {i}"),
                uncommonSkin: Enumerable.Range(1, collectionQuantity).Select(i => $"Uncommon Skin {i}"),
                uncommonHair: Enumerable.Range(1, collectionQuantity).Select(i => $"Uncommon Hair {i}"),
                uncommonEyes: Enumerable.Range(1, collectionQuantity).Select(i => $"Uncommon Eyes {i}"),
                uncommonOther: Enumerable.Range(1, collectionQuantity).Select(i => $"Uncommon Other {i}"),
                rareSkin: Enumerable.Range(1, collectionQuantity).Select(i => $"Rare Skin {i}"),
                rareHair: Enumerable.Range(1, collectionQuantity).Select(i => $"Rare Hair {i}"),
                rareEyes: Enumerable.Range(1, collectionQuantity).Select(i => $"Rare Eyes {i}"),
                rareOther: Enumerable.Range(1, collectionQuantity).Select(i => $"Rare Other {i}")
            );
            stopwatch.Stop();

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(.1).Within(.01));
            Assert.That(collection, Is.Not.Empty);
            Assert.That(collection, Is.All.Not.Empty);
        }

        //Copied from CreatureGen
        private enum Rarity
        {
            Common,
            Uncommon,
            Rare,
            VeryRare
        }

        private IEnumerable<string> GetWeightedAppearances(
            IEnumerable<string> allSkin = null, IEnumerable<string> commonSkin = null, IEnumerable<string> uncommonSkin = null, IEnumerable<string> rareSkin = null,
            IEnumerable<string> allHair = null, IEnumerable<string> commonHair = null, IEnumerable<string> uncommonHair = null, IEnumerable<string> rareHair = null,
            IEnumerable<string> allEyes = null, IEnumerable<string> commonEyes = null, IEnumerable<string> uncommonEyes = null, IEnumerable<string> rareEyes = null,
            IEnumerable<string> allOther = null, IEnumerable<string> commonOther = null, IEnumerable<string> uncommonOther = null, IEnumerable<string> rareOther = null)
        {
            var appearances = Build(allSkin, commonSkin, uncommonSkin, rareSkin, (string.Empty, Rarity.Common))
                .SelectMany(a => Build(allHair, commonHair, uncommonHair, rareHair, a))
                .SelectMany(a => Build(allEyes, commonEyes, uncommonEyes, rareEyes, a))
                .SelectMany(a => Build(allOther, commonOther, uncommonOther, rareOther, a))
                .Where(a => !string.IsNullOrEmpty(a.Appearance));

            var commonAppearances = appearances.Where(a => a.Rarity == Rarity.Common).Select(a => a.Appearance);
            var uncommonAppearances = appearances.Where(a => a.Rarity == Rarity.Uncommon).Select(a => a.Appearance);
            var rareAppearances = appearances.Where(a => a.Rarity == Rarity.Rare).Select(a => a.Appearance);
            var veryRareAppearances = appearances.Where(a => a.Rarity == Rarity.VeryRare).Select(a => a.Appearance);

            return collectionsSelector.CreateWeighted(commonAppearances, uncommonAppearances, rareAppearances, veryRareAppearances);
        }

        private IEnumerable<(string Appearance, Rarity Rarity)> Build(
            IEnumerable<string> all, IEnumerable<string> common, IEnumerable<string> uncommon, IEnumerable<string> rare,
            (string Appearance, Rarity Rarity) prototype)
        {
            if (all?.Any() == true)
                return all.Select(a =>
                    (GetAppearancePrototype(prototype.Appearance, a),
                    GetRarity(prototype.Rarity, Rarity.Common)));

            common ??= new[] { string.Empty };
            uncommon ??= new[] { string.Empty };
            rare ??= new[] { string.Empty };

            if (common.Concat(uncommon).Concat(rare).Any(a => !string.IsNullOrEmpty(a)) == false)
                return new[] { prototype };

            var builtCommon = common.Select(a =>
                (GetAppearancePrototype(prototype.Appearance, a),
                GetRarity(prototype.Rarity, Rarity.Common)));
            var builtUncommon = uncommon.Select(a =>
                (GetAppearancePrototype(prototype.Appearance, a),
                GetRarity(prototype.Rarity, Rarity.Uncommon)));
            var builtRare = rare.Select(a =>
                (GetAppearancePrototype(prototype.Appearance, a),
                GetRarity(prototype.Rarity, Rarity.Rare)));

            return builtCommon.Concat(builtUncommon).Concat(builtRare);
        }

        private string GetAppearancePrototype(string source, string additional)
        {
            if (string.IsNullOrEmpty(source) && string.IsNullOrEmpty(additional))
                return string.Empty;

            if (string.IsNullOrEmpty(source))
                return additional;

            if (string.IsNullOrEmpty(additional))
                return source;

            return $"{source}; {additional}";
        }

        private Rarity GetRarity(Rarity source, Rarity additional)
        {
            if (source == Rarity.VeryRare || additional == Rarity.VeryRare)
                return Rarity.VeryRare;

            if (source != Rarity.Common && source == additional)
                return (Rarity)((int)source + 1);

            return (Rarity)Math.Max((int)source, (int)additional);
        }
    }
}
