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

        [Test]
        public void HeavyExplodeIsEfficient()
        {
            stopwatch.Restart();
            var explodedCollection = collectionsSelector.Explode("CreatureGroups", "Night");
            stopwatch.Stop();

            Assert.That(explodedCollection, Is.Not.Empty);
            Assert.That(explodedCollection.Count, Is.EqualTo(1484));
            Assert.That(explodedCollection, Is.Unique);

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(0.1484));
        }

        [Test]
        public void HeavySelectAllIsEfficient()
        {
            stopwatch.Restart();
            var allCollections = collectionsSelector.SelectAllFrom("EncounterGroups");
            stopwatch.Stop();

            Assert.That(allCollections, Is.Not.Empty);
            Assert.That(allCollections.Count, Is.EqualTo(1484));
            Assert.That(allCollections, Is.Unique);

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(0.1));
        }

        [Test]
        public void HeavySeparatedExplodeAndFlattenIsEfficient()
        {
            stopwatch.Restart();
            var flattenedCollection = ExplodeAndFlatten("CreatureGroups", "Night", "EncounterGroups");
            stopwatch.Stop();

            Assert.That(flattenedCollection, Is.Not.Empty);
            Assert.That(flattenedCollection, Is.Unique);
            Assert.That(flattenedCollection.Count, Is.EqualTo(2538));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(0.1));
        }

        private IEnumerable<string> ExplodeAndFlatten(string explodeTableName, string entry, string flattenTableName)
        {
            var explodedCollection = collectionsSelector.Explode(explodeTableName, entry);
            var allCollections = collectionsSelector.SelectAllFrom(flattenTableName);
            var flattenedCollection = collectionsSelector.Flatten(allCollections, explodedCollection);

            return flattenedCollection;
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
        public void HeavyMultipleSeparatedExplodeAndFlattenIsEfficient(string entry, int count)
        {
            var timeLimit = Math.Max(0.1, count / 1000d);

            stopwatch.Restart();
            var encounters = ExplodeAndFlatten("CreatureGroups", entry, "EncounterGroups");
            stopwatch.Stop();

            Assert.That(encounters.Count, Is.EqualTo(count));
            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(timeLimit));
        }
    }
}
