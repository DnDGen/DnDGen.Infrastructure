using DnDGen.Core.Selectors.Collections;
using EventGen;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.Core.Tests.Selectors.Collections
{
    [TestFixture]
    public class CollectionSelectorEventDecoratorTests
    {
        private ICollectionSelector decorator;
        private Mock<ICollectionSelector> mockInnerSelector;
        private Mock<GenEventQueue> mockEventQueue;

        [SetUp]
        public void Setup()
        {
            mockInnerSelector = new Mock<ICollectionSelector>();
            mockEventQueue = new Mock<GenEventQueue>();
            decorator = new CollectionSelectorEventDecorator(mockInnerSelector.Object, mockEventQueue.Object);
        }

        [Test]
        public void ReturnInnerCollection()
        {
            var innerCollection = new[] { "item 3", "item 4" };
            mockInnerSelector.Setup(s => s.SelectFrom("table name", "item")).Returns(innerCollection);

            var collection = decorator.SelectFrom("table name", "item");
            Assert.That(collection, Is.EqualTo(innerCollection));
        }

        //INFO: This is a quick action, and logs too many events if we log events
        [Test]
        public void DoNotLogEventsForCollectionSelection()
        {
            var innerGroup = new[] { "item 3", "item 4" };
            mockInnerSelector.Setup(s => s.SelectFrom("table name", "item")).Returns(innerGroup);

            var group = decorator.SelectFrom("table name", "item");
            Assert.That(group, Is.EqualTo(innerGroup));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ReturnInnerAll()
        {
            var allCollections = new Dictionary<string, IEnumerable<string>>();
            allCollections["group 1"] = new[] { "item 1", "item 2" };
            allCollections["group 2"] = new[] { "item 3", "item 4" };

            mockInnerSelector.Setup(s => s.SelectAllFrom("table name")).Returns(allCollections);

            var collections = decorator.SelectAllFrom("table name");
            Assert.That(collections, Is.EqualTo(allCollections));
        }

        [Test]
        public void LogEventsForAllSelection()
        {
            var allCollections = new Dictionary<string, IEnumerable<string>>();
            allCollections["group 1"] = new[] { "item 1", "item 2" };
            allCollections["group 2"] = new[] { "item 3", "item 4" };

            mockInnerSelector.Setup(s => s.SelectAllFrom("table name")).Returns(allCollections);

            var collections = decorator.SelectAllFrom("table name");
            Assert.That(collections, Is.EqualTo(allCollections));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Selecting all collections from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Selected 2 collections from table name"), Times.Once);
        }

        [Test]
        public void ReturnInnerRandomString()
        {
            mockInnerSelector.Setup(s => s.SelectRandomFrom("table name", "item")).Returns("random item");

            var randomItem = decorator.SelectRandomFrom("table name", "item");
            Assert.That(randomItem, Is.EqualTo("random item"));
        }

        //INFO: This is a quick action, and logs too many events if we log events
        [Test]
        public void LogEventsForRandomStringSelection()
        {
            mockInnerSelector.Setup(s => s.SelectRandomFrom("table name", "item")).Returns("random item");

            var randomItem = decorator.SelectRandomFrom("table name", "item");
            Assert.That(randomItem, Is.EqualTo("random item"));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ReturnInnerRandomEntry()
        {
            var collection = new[] { 9266, 90210, 42 };
            mockInnerSelector.Setup(s => s.SelectRandomFrom(collection)).Returns(600);

            var randomItem = decorator.SelectRandomFrom(collection);
            Assert.That(randomItem, Is.EqualTo(600));
        }

        //INFO: This is a quick action, and logs too many events if we log events
        [Test]
        public void LogEventsForRandomEntrySelection()
        {
            var collection = new[] { 9266, 90210, 42 };
            mockInnerSelector.Setup(s => s.SelectRandomFrom(collection)).Returns(600);

            var randomItem = decorator.SelectRandomFrom(collection);
            Assert.That(randomItem, Is.EqualTo(600));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ReturnInnerCollectionName()
        {
            mockInnerSelector.Setup(s => s.FindCollectionOf("table name", "item", "group 1", "group 2")).Returns("inner group");

            var group = decorator.FindCollectionOf("table name", "item", "group 1", "group 2");
            Assert.That(group, Is.EqualTo("inner group"));
        }

        [Test]
        public void LogEventsForCollectionNameSelection()
        {
            mockInnerSelector.Setup(s => s.FindCollectionOf("table name", "item")).Returns("inner group");

            var collectionName = decorator.FindCollectionOf("table name", "item");
            Assert.That(collectionName, Is.EqualTo("inner group"));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Finding collection to which item belongs from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", $"item belongs to inner group"), Times.Once);
        }

        [Test]
        public void LogEventsForFilteredCollectionNameSelection()
        {
            mockInnerSelector.Setup(s => s.FindCollectionOf("table name", "item", "group 1", "group 2")).Returns("inner group");

            var collectionName = decorator.FindCollectionOf("table name", "item", "group 1", "group 2");
            Assert.That(collectionName, Is.EqualTo("inner group"));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Finding collection to which item belongs from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", $"item belongs to inner group"), Times.Once);
        }

        [Test]
        public void ReturnIsCollection()
        {
            mockInnerSelector.Setup(s => s.IsCollection("table name", "collection name")).Returns(true);
            var isCollection = decorator.IsCollection("table name", "collection name");
            Assert.That(isCollection, Is.True);
        }

        [Test]
        public void ReturnIsNotCollection()
        {
            mockInnerSelector.Setup(s => s.IsCollection("table name", "collection name")).Returns(false);
            var isCollection = decorator.IsCollection("table name", "collection name");
            Assert.That(isCollection, Is.False);
        }

        //INFO: This is a quick action, and logs too many events if we log events
        [Test]
        public void LogEventsForIsCollection()
        {
            mockInnerSelector.Setup(s => s.IsCollection("table name", "collection name")).Returns(true);

            var isCollection = decorator.IsCollection("table name", "collection name");
            Assert.That(isCollection, Is.True);
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        //INFO: This is a quick action, and logs too many events if we log events
        [Test]
        public void LogEventsForIsNotCollection()
        {
            mockInnerSelector.Setup(s => s.IsCollection("table name", "collection name")).Returns(false);

            var isCollection = decorator.IsCollection("table name", "collection name");
            Assert.That(isCollection, Is.False);
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Test]
        public void ReturnExplodedCollection()
        {
            var explodedCollection = new[] { "thing 1", "thing 2" };
            mockInnerSelector.Setup(s => s.Explode("table name", "collection name")).Returns(explodedCollection);

            var collection = decorator.Explode("table name", "collection name");
            Assert.That(collection, Is.EqualTo(explodedCollection));
        }

        [Test]
        public void LogEventsForExplodeCollection()
        {
            var explodedCollection = new[] { "thing 1", "thing 2" };
            mockInnerSelector.Setup(s => s.Explode("table name", "collection name")).Returns(explodedCollection);

            var collection = decorator.Explode("table name", "collection name");
            Assert.That(collection, Is.EqualTo(explodedCollection));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Exploding collection name from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Exploded collection name into 2 entries"), Times.Once);
        }

        [Test]
        public void ReturnFlattenedCollection()
        {
            var collections = new Dictionary<string, IEnumerable<string>>();
            var keys = new List<string>();

            var flattenedCollection = new[] { "thing 1", "thing 2" };
            mockInnerSelector.Setup(s => s.Flatten(collections, keys)).Returns(flattenedCollection);

            var collection = decorator.Flatten(collections, keys);
            Assert.That(collection, Is.EqualTo(flattenedCollection));
        }

        [Test]
        public void LogEventsForFlattenCollection()
        {
            var collections = new Dictionary<string, IEnumerable<string>>();
            collections["first"] = new[] { "asdf", "asdfadf", "asdfadf", "asdfadf" };
            collections["second"] = new[] { "afadf", "asdfadfasd", "asdfadf", "asdfadf", "asdfadf" };
            collections["third"] = new[] { "asdfadsf", "dfasdfasd", "asdfadf", "asdfadf", "asdfadf", "asdfadf" };

            var keys = new List<string>();
            var count = 7;

            while (count-- > 0)
                keys.Add(count.ToString());

            var flattenedCollection = new[] { "thing 1", "thing 2" };
            mockInnerSelector.Setup(s => s.Flatten(collections, keys)).Returns(flattenedCollection);

            var collection = decorator.Flatten(collections, keys);
            Assert.That(collection, Is.EqualTo(flattenedCollection));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Flattening 3 collections with 7 keys"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Flattened 3 collections into 2 entries"), Times.Once);
        }
    }
}