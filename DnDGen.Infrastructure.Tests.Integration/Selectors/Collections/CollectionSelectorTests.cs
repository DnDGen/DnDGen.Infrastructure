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

        [TestCase("Magic", 265)]
        [TestCase("Night", 1484)]
        [TestCase("Wilderness", 573)]
        [TestCase("ColdCivilized", 611)]
        [TestCase("Land", 93)]
        [TestCase("TemperateForest", 98)]
        [TestCase("TemperateAquatic", 32)]
        [TestCase("WarmPlains", 37)]
        [TestCase("Day", 1422)]
        [TestCase("Aquatic", 2)]
        [TestCase("Underground", 68)]
        [TestCase("UndergroundAquatic", 3)]
        public void Explode_IsEfficient(string entry, int count)
        {
            var timeLimit = Math.Max(0.1, count / 10_000d);

            stopwatch.Restart();
            var explodedCollection = collectionsSelector.Explode("CreatureGroups", entry);
            stopwatch.Stop();

            Assert.That(explodedCollection, Is.Not.Empty.And.Unique);
            Assert.That(explodedCollection.Count, Is.EqualTo(count));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(timeLimit));
        }

        [TestCase("Magic", 265)]
        [TestCase("Night", 1484)]
        [TestCase("Wilderness", 573)]
        [TestCase("ColdCivilized", 611)]
        [TestCase("Land", 93)]
        [TestCase("TemperateForest", 98)]
        [TestCase("TemperateAquatic", 32)]
        [TestCase("WarmPlains", 37)]
        [TestCase("Day", 1422)]
        [TestCase("Aquatic", 2)]
        [TestCase("Underground", 68)]
        [TestCase("UndergroundAquatic", 3)]
        public void Explode_Cached_IsEfficient(string entry, int count)
        {
            collectionsSelector.Explode("CreatureGroups", entry);

            stopwatch.Restart();
            var explodedCollection = collectionsSelector.Explode("CreatureGroups", entry);
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

        [TestCase("Magic", 572)]
        [TestCase("Night", 2538)]
        [TestCase("Wilderness", 1115)]
        [TestCase("ColdCivilized", 953)]
        [TestCase("Land", 133)]
        [TestCase("TemperateForest", 150)]
        [TestCase("TemperateAquatic", 45)]
        [TestCase("WarmPlains", 66)]
        [TestCase("Day", 2412)]
        [TestCase("Aquatic", 6)]
        [TestCase("Underground", 91)]
        [TestCase("UndergroundAquatic", 6)]
        public void HeavySeparatedExplodeAndFlattenIsEfficient(string entry, int count)
        {
            var timeLimit = Math.Max(0.1, count / 10_000d);

            stopwatch.Restart();
            var flattenedCollection = ExplodeAndFlatten("CreatureGroups", entry, "EncounterGroups");
            stopwatch.Stop();

            Assert.That(flattenedCollection, Is.Not.Empty.And.Unique);
            Assert.That(flattenedCollection.Count, Is.EqualTo(count));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(timeLimit));
        }

        [TestCase("Magic", 572)]
        [TestCase("Night", 2538)]
        [TestCase("Wilderness", 1115)]
        [TestCase("ColdCivilized", 953)]
        [TestCase("Land", 133)]
        [TestCase("TemperateForest", 150)]
        [TestCase("TemperateAquatic", 45)]
        [TestCase("WarmPlains", 66)]
        [TestCase("Day", 2412)]
        [TestCase("Aquatic", 6)]
        [TestCase("Underground", 91)]
        [TestCase("UndergroundAquatic", 6)]
        public void HeavySeparatedExplodeAndFlatten_Cached_IsEfficient(string entry, int count)
        {
            ExplodeAndFlatten("CreatureGroups", entry, "EncounterGroups");

            stopwatch.Restart();
            var flattenedCollection = ExplodeAndFlatten("CreatureGroups", entry, "EncounterGroups");
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
    }
}
