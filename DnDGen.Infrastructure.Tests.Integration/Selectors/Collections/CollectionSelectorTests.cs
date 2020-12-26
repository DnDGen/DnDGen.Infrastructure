using DnDGen.Infrastructure.Selectors.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Integration.Selectors.Collections
{
    [TestFixture]
    public class CollectionSelectorTests : IntegrationTests
    {
        private ICollectionSelector collectionsSelector;

        [SetUp]
        public void Setup()
        {
            collectionsSelector = GetNewInstanceOf<ICollectionSelector>();
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
            Assert.That(selectedCollections.Count, Is.EqualTo(4));
            Assert.That(selectedCollections.Keys, Contains.Item(string.Empty));
            Assert.That(selectedCollections.Keys, Contains.Item("name"));
            Assert.That(selectedCollections.Keys, Contains.Item("collection"));
            Assert.That(selectedCollections.Keys, Contains.Item("sub-collection"));
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
            Assert.That(explodedCollection, Is.EquivalentTo(new[]
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

        [Test]
        public void HeavyExplodeIsEfficient()
        {
            var explodedCollection = collectionsSelector.Explode("CreatureGroups", "Night");
            Assert.That(explodedCollection, Is.Not.Empty);
            Assert.That(explodedCollection.Count, Is.EqualTo(1484));
            Assert.That(explodedCollection, Is.Unique);
        }

        [Test]
        public void HeavySelectAllIsEfficient()
        {
            var allCollections = collectionsSelector.SelectAllFrom("EncounterGroups");
            Assert.That(allCollections, Is.Not.Empty);
            Assert.That(allCollections.Count, Is.EqualTo(1484));
            Assert.That(allCollections, Is.Unique);
        }

        [Test]
        public void HeavySeparatedExplodeAndFlattenIsEfficient()
        {
            var flattenedCollection = ExplodeAndFlatten("CreatureGroups", "Night", "EncounterGroups");

            Assert.That(flattenedCollection, Is.Not.Empty);
            Assert.That(flattenedCollection, Is.Unique);
            Assert.That(flattenedCollection.Count, Is.EqualTo(2538));
        }

        private IEnumerable<string> ExplodeAndFlatten(string explodeTableName, string entry, string flattenTableName)
        {
            var explodedCollection = collectionsSelector.Explode(explodeTableName, entry);
            var allCollections = collectionsSelector.SelectAllFrom(flattenTableName);
            var flattenedCollection = collectionsSelector.Flatten(allCollections, explodedCollection);

            return flattenedCollection;
        }

        [Test]
        public void HeavyMultipleSeparatedExplodeAndFlattenIsEfficient()
        {
            var magicEncounters = ExplodeAndFlatten("CreatureGroups", "Magic", "EncounterGroups");
            var nightEncounters = ExplodeAndFlatten("CreatureGroups", "Night", "EncounterGroups");
            var wildernessEncounters = ExplodeAndFlatten("CreatureGroups", "Wilderness", "EncounterGroups");
            var coldCivilizedEncounters = ExplodeAndFlatten("CreatureGroups", "ColdCivilized", "EncounterGroups");
            var landEncounters = ExplodeAndFlatten("CreatureGroups", "Land", "EncounterGroups");
            var temperateForestEncounters = ExplodeAndFlatten("CreatureGroups", "TemperateForest", "EncounterGroups");
            var temperateAquaticEncounters = ExplodeAndFlatten("CreatureGroups", "TemperateAquatic", "EncounterGroups");
            var warmPlainsEncounters = ExplodeAndFlatten("CreatureGroups", "WarmPlains", "EncounterGroups");
            var dayEncounters = ExplodeAndFlatten("CreatureGroups", "Day", "EncounterGroups");
            var aquaticEncounters = ExplodeAndFlatten("CreatureGroups", "Aquatic", "EncounterGroups");
            var undergroundEncounters = ExplodeAndFlatten("CreatureGroups", "Underground", "EncounterGroups");
            var undergroundAquaticEncounters = ExplodeAndFlatten("CreatureGroups", "UndergroundAquatic", "EncounterGroups");

            Assert.That(magicEncounters.Count, Is.EqualTo(572), "magic");
            Assert.That(nightEncounters.Count, Is.EqualTo(2538), "night");
            Assert.That(wildernessEncounters.Count, Is.EqualTo(1115), "wilderness");
            Assert.That(coldCivilizedEncounters.Count, Is.EqualTo(953), "cold civilized");
            Assert.That(landEncounters.Count, Is.EqualTo(133), "land");
            Assert.That(temperateForestEncounters.Count, Is.EqualTo(150), "temperate forest");
            Assert.That(temperateAquaticEncounters.Count, Is.EqualTo(45), "temperate aquatic");
            Assert.That(warmPlainsEncounters.Count, Is.EqualTo(66), "warm plains");
            Assert.That(dayEncounters.Count, Is.EqualTo(2412), "day");
            Assert.That(aquaticEncounters.Count, Is.EqualTo(6), "aquatic");
            Assert.That(undergroundEncounters.Count, Is.EqualTo(91), "underground");
            Assert.That(undergroundAquaticEncounters.Count, Is.EqualTo(6), "underground aquatic");
        }
    }
}
