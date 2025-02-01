using DnDGen.Infrastructure.Selectors.Collections;
using NUnit.Framework;
using System.Diagnostics;

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
        public void HeavySelectAllIsEfficient()
        {
            stopwatch.Restart();
            var allCollections = collectionSelector.SelectAllFrom(assemblyName, "EncounterGroups");
            stopwatch.Stop();

            Assert.That(allCollections, Is.Not.Empty.And.Unique);
            Assert.That(allCollections.Count, Is.EqualTo(1484));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(0.1));
        }
    }
}
