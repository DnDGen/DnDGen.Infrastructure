using DnDGen.Core.Selectors.Collections;
using DnDGen.Core.Tests;
using EventGen;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Integration.DnDGen.Core.Selectors.Collections
{
    [TestFixture]
    public class CollectionSelectorTests : IntegrationTests
    {
        [Inject]
        public ICollectionSelector CollectionsSelector { get; set; }
        [Inject]
        public ClientIDManager ClientIdManager { get; set; }
        [Inject]
        public GenEventQueue EventQueue { get; set; }

        [SetUp]
        public void Setup()
        {
            var clientId = Guid.NewGuid();
            ClientIdManager.SetClientID(clientId);
        }

        [TestCase("")]
        [TestCase("name", "entry 1", "entry 2")]
        [TestCase("collection", "entry 2", "entry 3", "collection", "sub-collection")]
        [TestCase("sub-collection", "entry 3", "entry 4", "sub-collection")]
        public void SelectFromTable(string name, params string[] collection)
        {
            var selectedCollection = CollectionsSelector.SelectFrom("CollectionTable", name);
            Assert.That(selectedCollection, Is.EquivalentTo(collection));
        }

        [Test]
        public void SelectAllFromTable()
        {
            var selectedCollections = CollectionsSelector.SelectAllFrom("CollectionTable");
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
            var collectionName = CollectionsSelector.FindCollectionOf("CollectionTable", "entry 3", "collection", "sub-collection");
            Assert.That(collectionName, Is.EqualTo("collection"));
        }

        [TestCase("name", "entry 1", "entry 2")]
        [TestCase("collection", "entry 2", "entry 3", "collection", "sub-collection")]
        [TestCase("sub-collection", "entry 3", "entry 4", "sub-collection")]
        public void SelectRandomFromTable(string name, params string[] collection)
        {
            var entry = CollectionsSelector.SelectRandomFrom("CollectionTable", name);
            Assert.That(new[] { entry }, Is.SubsetOf(collection));
        }

        [Test]
        public void IsCollectionInTable()
        {
            var isCollection = CollectionsSelector.IsCollection("CollectionTable", "sub-collection");
            Assert.That(isCollection, Is.True);
        }

        [Test]
        public void IsNotCollectionInTable()
        {
            var isCollection = CollectionsSelector.IsCollection("CollectionTable", "entry 3");
            Assert.That(isCollection, Is.False);
        }

        [Test]
        public void ExplodeFromTable()
        {
            var explodedCollection = CollectionsSelector.Explode("CollectionTable", "collection");
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
            var explodedCollection = CollectionsSelector.Explode("CollectionTable", "collection");
            var allCollections = CollectionsSelector.SelectAllFrom("OtherCollectionTable");

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
            var explodedCollection = CollectionsSelector.Explode("CreatureGroups", "Night");
            Assert.That(explodedCollection, Is.Not.Empty);
            Assert.That(explodedCollection.Count, Is.EqualTo(1484));
            Assert.That(explodedCollection, Is.Unique);
            AssertEventSpacing();
        }

        private void AssertEventSpacing()
        {
            var dequeuedEvents = EventQueue.DequeueAllForCurrentThread();

            if (!dequeuedEvents.Any())
                Assert.Fail("No events were logged");

            Assert.That(dequeuedEvents, Is.Ordered.By("When"));

            var times = dequeuedEvents.Select(e => e.When);
            var checkpointEvent = dequeuedEvents.First();
            var checkpoint = checkpointEvent.When;
            var finalEvent = dequeuedEvents.Last();
            var finalCheckPoint = finalEvent.When;

            while (finalCheckPoint > checkpoint)
            {
                var oneSecondAfterCheckpoint = checkpoint.AddSeconds(1);

                var failedEvent = dequeuedEvents.First(e => e.When > checkpoint);
                var failureMessage = $"{GetMessage(checkpointEvent)}\n{GetMessage(failedEvent)}";
                Assert.That(times, Has.Some.InRange(checkpoint.AddTicks(1), oneSecondAfterCheckpoint), failureMessage);

                checkpointEvent = dequeuedEvents.Last(e => e.When <= oneSecondAfterCheckpoint);
                checkpoint = checkpointEvent.When;
            }
        }

        private string GetMessage(GenEvent genEvent)
        {
            return $"[{genEvent.When.ToLongTimeString()}] {genEvent.Source}: {genEvent.Message}";
        }

        [Test]
        public void HeavySelectAllIsEfficient()
        {
            var allCollections = CollectionsSelector.SelectAllFrom("EncounterGroups");
            Assert.That(allCollections, Is.Not.Empty);
            Assert.That(allCollections.Count, Is.EqualTo(1484));
            Assert.That(allCollections, Is.Unique);
            AssertEventSpacing();
        }

        [Test]
        public void HeavySeparatedExplodeAndFlattenIsEfficient()
        {
            var flattenedCollection = ExplodeAndFlatten("CreatureGroups", "Night", "EncounterGroups");

            AssertEventSpacing();

            Assert.That(flattenedCollection, Is.Not.Empty);
            Assert.That(flattenedCollection, Is.Unique);
            Assert.That(flattenedCollection.Count, Is.EqualTo(2538));
        }

        private IEnumerable<string> ExplodeAndFlatten(string explodeTableName, string entry, string flattenTableName)
        {
            var explodedCollection = CollectionsSelector.Explode(explodeTableName, entry);
            var allCollections = CollectionsSelector.SelectAllFrom(flattenTableName);
            var flattenedCollection = CollectionsSelector.Flatten(allCollections, explodedCollection);

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

            AssertEventSpacing();
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
