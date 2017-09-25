using DnDGen.Core.Selectors.Collections;
using DnDGen.Core.Tests;
using EventGen;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Integration.DnDGen.Core.Selectors.Collections
{
    [TestFixture]
    public class CollectionSelectorTests : IntegrationTests
    {
        [Inject]
        public ICollectionsSelector CollectionsSelector { get; set; }
        [Inject]
        public ClientIDManager ClientIdManager { get; set; }

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
            var executedExplodedCollection = CollectionsSelector.ExplodeInto("CollectionTable", "collection", "OtherCollectionTable").ToArray();
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
    }
}
