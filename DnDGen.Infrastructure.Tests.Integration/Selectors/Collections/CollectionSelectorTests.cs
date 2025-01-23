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
        private ICollectionSelector collectionSelector;
        private Stopwatch stopwatch;

        [SetUp]
        public void Setup()
        {
            collectionSelector = GetNewInstanceOf<ICollectionSelector>();
            stopwatch = new Stopwatch();
        }

        [TestCase("")]
        [TestCase("name", "entry 1", "entry 2")]
        [TestCase("collection", "entry 2", "entry 3", "collection", "sub-collection")]
        [TestCase("sub-collection", "entry 3", "entry 4", "sub-collection")]
        public void SelectFromTable(string name, params string[] collection)
        {
            var selectedCollection = collectionSelector.SelectFrom(assemblyName, "CollectionTable", name);
            Assert.That(selectedCollection, Is.EquivalentTo(collection));
        }

        [Test]
        public void SelectAllFromTable()
        {
            var selectedCollections = collectionSelector.SelectAllFrom(assemblyName, "CollectionTable");
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
            var collectionName = collectionSelector.FindCollectionOf(assemblyName, "CollectionTable", "entry 3", "collection", "sub-collection");
            Assert.That(collectionName, Is.EqualTo("collection"));
        }

        [TestCase("name", "entry 1", "entry 2")]
        [TestCase("collection", "entry 2", "entry 3", "collection", "sub-collection")]
        [TestCase("sub-collection", "entry 3", "entry 4", "sub-collection")]
        public void SelectRandomFromTable(string name, params string[] collection)
        {
            var entry = collectionSelector.SelectRandomFrom(assemblyName, "CollectionTable", name);
            Assert.That(new[] { entry }, Is.SubsetOf(collection));
        }

        [Test]
        public void IsCollectionInTable()
        {
            var isCollection = collectionSelector.IsCollection(assemblyName, "CollectionTable", "sub-collection");
            Assert.That(isCollection, Is.True);
        }

        [Test]
        public void IsNotCollectionInTable()
        {
            var isCollection = collectionSelector.IsCollection(assemblyName, "CollectionTable", "entry 3");
            Assert.That(isCollection, Is.False);
        }

        [Test]
        public void ExplodeFromTable()
        {
            var explodedCollection = collectionSelector.Explode(assemblyName, "CollectionTable", "collection");
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
            var explodedCollection = collectionSelector.Explode(assemblyName, "CollectionTable", "collection");
            var allCollections = collectionSelector.SelectAllFrom(assemblyName, "OtherCollectionTable");

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
            var explodedCollection = collectionSelector.Explode(assemblyName, table, entry);
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
            collectionSelector.Explode(assemblyName, table, entry);

            stopwatch.Restart();
            var explodedCollection = collectionSelector.Explode(assemblyName, table, entry);
            stopwatch.Stop();

            Assert.That(explodedCollection, Is.Not.Empty.And.Unique);
            Assert.That(explodedCollection.Count, Is.EqualTo(count));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(0.01));
        }

        [Test]
        public void HeavySelectAllIsEfficient()
        {
            stopwatch.Restart();
            var allCollections = collectionSelector.SelectAllFrom(assemblyName, "EncounterGroups");
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
            var explodedCollection = collectionSelector.Explode(assemblyName, explodeTableName, entry);
            var allCollections = collectionSelector.SelectAllFrom(assemblyName, flattenTableName);
            var flattenedCollection = collectionSelector.Flatten(allCollections, explodedCollection);

            return flattenedCollection;
        }
    }
}
