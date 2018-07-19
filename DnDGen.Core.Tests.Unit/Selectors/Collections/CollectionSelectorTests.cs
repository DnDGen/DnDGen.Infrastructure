using DnDGen.Core.Mappers.Collections;
using DnDGen.Core.Selectors.Collections;
using EventGen;
using Moq;
using NUnit.Framework;
using RollGen;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Core.Tests.Unit.Selectors.Collections
{
    [TestFixture]
    public class CollectionSelectorTests
    {
        private const string TableName = "table name";

        private ICollectionSelector selector;
        private Mock<CollectionMapper> mockMapper;
        private Mock<Dice> mockDice;
        private Mock<GenEventQueue> mockEventQueue;
        private Dictionary<string, IEnumerable<string>> allCollections;

        [SetUp]
        public void Setup()
        {
            mockMapper = new Mock<CollectionMapper>();
            mockDice = new Mock<Dice>();
            mockEventQueue = new Mock<GenEventQueue>();
            selector = new CollectionSelector(mockMapper.Object, mockDice.Object, mockEventQueue.Object);
            allCollections = new Dictionary<string, IEnumerable<string>>();

            mockMapper.Setup(m => m.Map(TableName)).Returns(allCollections);
        }

        [Test]
        public void SelectCollection()
        {
            allCollections["entry"] = Enumerable.Empty<string>();
            var collection = selector.SelectFrom(TableName, "entry");
            Assert.That(collection, Is.EqualTo(allCollections["entry"]));
        }

        [Test]
        public void SelectAllCollections()
        {
            var collections = selector.SelectAllFrom(TableName);
            Assert.That(collections, Is.EqualTo(allCollections));
        }

        [Test]
        public void IfEntryNotPresentInTable_ThrowException()
        {
            Assert.That(() => selector.SelectFrom(TableName, "entry"), Throws.Exception.With.Message.EqualTo("entry is not a valid collection in the table table name"));
        }

        [Test]
        public void SelectRandomItemFromCollection()
        {
            var collection = new[] { "item 1", "item 2", "item 3" };
            mockDice.Setup(d => d.Roll(1).d(3).AsSum()).Returns(2);

            var item = selector.SelectRandomFrom(collection);
            Assert.That(item, Is.EqualTo("item 2"));
        }

        [Test]
        public void SelectRandomItemFromTable()
        {
            allCollections["entry"] = new[] { "item 1", "item 2", "item 3" };
            mockDice.Setup(d => d.Roll(1).d(3).AsSum()).Returns(2);

            var item = selector.SelectRandomFrom(TableName, "entry");
            Assert.That(item, Is.EqualTo("item 2"));
        }

        [Test]
        public void CannotSelectRandomFromEmptyCollection()
        {
            var collection = Enumerable.Empty<string>();
            Assert.That(() => selector.SelectRandomFrom(collection), Throws.ArgumentException.With.Message.EqualTo("Cannot select random from an empty collection"));
        }

        [Test]
        public void CannotSelectRandomFromEmptyTable()
        {
            allCollections["entry"] = Enumerable.Empty<string>();
            Assert.That(() => selector.SelectRandomFrom(TableName, "entry"), Throws.ArgumentException.With.Message.EqualTo("Cannot select random from an empty collection"));
        }

        [Test]
        public void CannotSelectRandomFromInvalidEntry()
        {
            Assert.That(() => selector.SelectRandomFrom(TableName, "entry"), Throws.Exception.With.Message.EqualTo("entry is not a valid collection in the table table name"));
        }

        [Test]
        public void SelectRandomFromNonStringCollection()
        {
            var collection = new[] { 9266, 90210, 42, 600, 1337 };

            mockDice.Setup(d => d.Roll(1).d(5).AsSum()).Returns(2);

            var entry = selector.SelectRandomFrom(collection);
            Assert.That(entry, Is.EqualTo(90210));
        }

        [Test]
        public void FindCollectionContainingEntry()
        {
            allCollections["entry"] = new[] { "first", "fourth" };
            allCollections["other entry"] = new[] { "third", "fourth" };
            allCollections["wrong entry"] = new[] { "fifth", "second" };

            var collectionName = selector.FindCollectionOf(TableName, "fourth");
            Assert.That(collectionName, Is.EqualTo("entry"));
        }

        [Test]
        public void DoNotFindCollectionContainingEntry()
        {
            allCollections["entry"] = new[] { "first", "fourth" };
            allCollections["other entry"] = new[] { "third", "fourth" };
            allCollections["wrong entry"] = new[] { "fifth", "second" };

            Assert.That(() => selector.FindCollectionOf(TableName, "sixth"), Throws.ArgumentException.With.Message.EqualTo("No collection in table name contains sixth"));
        }

        [Test]
        public void FindCollectionContainingEntryWithFilteredCollectionNames()
        {
            allCollections["entry"] = new[] { "first", "second" };
            allCollections["other entry"] = new[] { "third", "fourth" };
            allCollections["wrong entry"] = new[] { "fifth", "fourth" };

            var group = selector.FindCollectionOf(TableName, "fourth", "entry", "other entry");
            Assert.That(group, Is.EqualTo("other entry"));
        }

        [Test]
        public void FindCollectionContainingEntryThrowsExceptionIfNotInFilteredCollectionNames()
        {
            allCollections["entry"] = new[] { "first", "second" };
            allCollections["other entry"] = new[] { "third", "fifth" };
            allCollections["wrong entry"] = new[] { "third", "fourth" };

            Assert.That(() => selector.FindCollectionOf(TableName, "fourth", "entry", "other entry"), Throws.ArgumentException.With.Message.EqualTo("No collection from the 2 filters in table name contains fourth"));
        }

        [Test]
        public void IsCollection()
        {
            allCollections["entry"] = new[] { "first", "second" };
            var isCollection = selector.IsCollection(TableName, "entry");
            Assert.That(isCollection, Is.True);
        }

        [Test]
        public void IsNotCollection()
        {
            allCollections["entry"] = new[] { "first", "second" };
            var isCollection = selector.IsCollection(TableName, "other entry");
            Assert.That(isCollection, Is.False);
        }

        [Test]
        public void EntryIsNotCollection()
        {
            allCollections["entry"] = new[] { "first", "second" };
            var isCollection = selector.IsCollection(TableName, "first");
            Assert.That(isCollection, Is.False);
        }

        [Test]
        public void EntryIsCollection()
        {
            allCollections["entry"] = new[] { "first", "second" };
            allCollections["first"] = new[] { "first", "third" };

            var isCollection = selector.IsCollection(TableName, "first");
            Assert.That(isCollection, Is.True);
        }

        [Test]
        public void ExplodeCollectionWithoutSubCollections()
        {
            allCollections["entry"] = new[] { "first", "second", "third" };

            var explodedCollection = selector.Explode(TableName, "entry");
            Assert.That(explodedCollection, Is.EquivalentTo(allCollections["entry"]));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving entry from table name"), Times.Once);
        }

        [Test]
        public void ExplodeCollectionWithSubCollections()
        {
            allCollections["entry"] = new[] { "first", "second", "third" };
            allCollections["second"] = new[] { "sub 1", "sub 2" };

            var explodedCollection = selector.Explode(TableName, "entry");
            Assert.That(explodedCollection, Contains.Item("first"));
            Assert.That(explodedCollection, Does.Not.Contain("second"));
            Assert.That(explodedCollection, Contains.Item("sub 1"));
            Assert.That(explodedCollection, Contains.Item("sub 2"));
            Assert.That(explodedCollection, Contains.Item("third"));
            Assert.That(explodedCollection.Count, Is.EqualTo(4));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving entry from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving second from table name"), Times.Once);
        }

        [Test]
        public void ExplodedCollectionsAreDistinctBetweenSubCollections()
        {
            allCollections["entry"] = new[] { "first", "second", "third" };
            allCollections["second"] = new[] { "sub 1", "sub 2" };
            allCollections["third"] = new[] { "sub 1", "sub 3" };

            var explodedCollection = selector.Explode(TableName, "entry");
            Assert.That(explodedCollection, Is.Unique);
            Assert.That(explodedCollection, Contains.Item("first"));
            Assert.That(explodedCollection, Does.Not.Contain("second"));
            Assert.That(explodedCollection, Contains.Item("sub 1"));
            Assert.That(explodedCollection, Contains.Item("sub 2"));
            Assert.That(explodedCollection, Contains.Item("sub 3"));
            Assert.That(explodedCollection, Does.Not.Contain("third"));
            Assert.That(explodedCollection.Count, Is.EqualTo(4));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(3));
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving entry from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving second from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving third from table name"), Times.Once);
        }

        [Test]
        public void ExplodedCollectionsAreDistinctBetweenCollectionAndSubCollection()
        {
            allCollections["entry"] = new[] { "first", "second", "third", "sub 1", "sub 3" };
            allCollections["second"] = new[] { "sub 1", "sub 2" };
            allCollections["third"] = new[] { "second", "sub 3" };

            var explodedCollection = selector.Explode(TableName, "entry");
            Assert.That(explodedCollection, Is.Unique);
            Assert.That(explodedCollection, Contains.Item("first"));
            Assert.That(explodedCollection, Does.Not.Contain("second"));
            Assert.That(explodedCollection, Contains.Item("sub 1"));
            Assert.That(explodedCollection, Contains.Item("sub 2"));
            Assert.That(explodedCollection, Contains.Item("sub 3"));
            Assert.That(explodedCollection, Does.Not.Contain("third"));
            Assert.That(explodedCollection.Count, Is.EqualTo(4));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(4));
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving entry from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving second from table name"), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving third from table name"), Times.Once);
        }

        [Test]
        public void ExplodeCollectionWithSubCollectionsAndSelfAsSubCollection()
        {
            allCollections["entry"] = new[] { "first", "second", "entry" };
            allCollections["second"] = new[] { "sub 1", "sub 2" };

            var explodedCollection = selector.Explode(TableName, "entry");
            Assert.That(explodedCollection, Contains.Item("first"));
            Assert.That(explodedCollection, Does.Not.Contain("second"));
            Assert.That(explodedCollection, Contains.Item("sub 1"));
            Assert.That(explodedCollection, Contains.Item("sub 2"));
            Assert.That(explodedCollection, Contains.Item("entry"));
            Assert.That(explodedCollection.Count, Is.EqualTo(4));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving entry from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving second from table name"), Times.Once);
        }

        [Test]
        public void ExplodeCollectionPreservingDuplicatesWithoutSubCollections()
        {
            allCollections["entry"] = new[] { "first", "second", "third" };

            var explodedCollection = selector.ExplodeAndPreserveDuplicates(TableName, "entry");
            Assert.That(explodedCollection, Is.EquivalentTo(allCollections["entry"]));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving entry from table name"), Times.Once);
        }

        [Test]
        public void ExplodeCollectionPreservingDuplicatesWithSubCollections()
        {
            allCollections["entry"] = new[] { "first", "second", "third" };
            allCollections["second"] = new[] { "sub 1", "sub 2" };

            var explodedCollection = selector.ExplodeAndPreserveDuplicates(TableName, "entry");
            Assert.That(explodedCollection, Contains.Item("first"));
            Assert.That(explodedCollection, Does.Not.Contain("second"));
            Assert.That(explodedCollection, Contains.Item("sub 1"));
            Assert.That(explodedCollection, Contains.Item("sub 2"));
            Assert.That(explodedCollection, Contains.Item("third"));
            Assert.That(explodedCollection.Count, Is.EqualTo(4));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving entry from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving second from table name"), Times.Once);
        }

        [Test]
        public void ExplodedCollectionsPreservingDuplicatesAreNotDistinctBetweenSubCollections()
        {
            allCollections["entry"] = new[] { "first", "second", "third" };
            allCollections["second"] = new[] { "sub 1", "sub 2" };
            allCollections["third"] = new[] { "sub 1", "sub 3" };

            var explodedCollection = selector.ExplodeAndPreserveDuplicates(TableName, "entry");
            Assert.That(explodedCollection, Is.Not.Unique);
            Assert.That(explodedCollection, Contains.Item("first"));
            Assert.That(explodedCollection, Does.Not.Contain("second"));
            Assert.That(explodedCollection, Contains.Item("sub 1"));
            Assert.That(explodedCollection.Count(e => e == "sub 1"), Is.EqualTo(2));
            Assert.That(explodedCollection, Contains.Item("sub 2"));
            Assert.That(explodedCollection, Contains.Item("sub 3"));
            Assert.That(explodedCollection, Does.Not.Contain("third"));
            Assert.That(explodedCollection.Count, Is.EqualTo(5));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(3));
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving entry from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving second from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving third from table name"), Times.Once);
        }

        [Test]
        public void ExplodedCollectionsPreservingDuplicatesAreNotDistinctBetweenCollectionAndSubCollection()
        {
            allCollections["entry"] = new[] { "first", "second", "third", "sub 1", "sub 3" };
            allCollections["second"] = new[] { "sub 1", "sub 2" };
            allCollections["third"] = new[] { "second", "sub 3" };

            var explodedCollection = selector.ExplodeAndPreserveDuplicates(TableName, "entry");
            Assert.That(explodedCollection, Is.Not.Unique);
            Assert.That(explodedCollection, Contains.Item("first"));
            Assert.That(explodedCollection, Does.Not.Contain("second"));
            Assert.That(explodedCollection, Contains.Item("sub 1"));
            Assert.That(explodedCollection.Count(e => e == "sub 1"), Is.EqualTo(3));
            Assert.That(explodedCollection, Contains.Item("sub 2"));
            Assert.That(explodedCollection.Count(e => e == "sub 2"), Is.EqualTo(2));
            Assert.That(explodedCollection, Contains.Item("sub 3"));
            Assert.That(explodedCollection.Count(e => e == "sub 3"), Is.EqualTo(2));
            Assert.That(explodedCollection, Does.Not.Contain("third"));
            Assert.That(explodedCollection.Count, Is.EqualTo(8));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(4));
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving entry from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving second from table name"), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving third from table name"), Times.Once);
        }

        [Test]
        public void ExplodeCollectionPreservingDuplicatesWithSubCollectionsAndSelfAsSubCollection()
        {
            allCollections["entry"] = new[] { "first", "second", "entry" };
            allCollections["second"] = new[] { "sub 1", "sub 2" };

            var explodedCollection = selector.ExplodeAndPreserveDuplicates(TableName, "entry");
            Assert.That(explodedCollection, Contains.Item("first"));
            Assert.That(explodedCollection, Does.Not.Contain("second"));
            Assert.That(explodedCollection, Contains.Item("sub 1"));
            Assert.That(explodedCollection, Contains.Item("sub 2"));
            Assert.That(explodedCollection, Contains.Item("entry"));
            Assert.That(explodedCollection.Count, Is.EqualTo(4));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving entry from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", "Recursively retrieving second from table name"), Times.Once);
        }

        [Test]
        public void FlattenCollection()
        {
            var otherCollections = new Dictionary<string, IEnumerable<string>>();
            otherCollections["first"] = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            otherCollections["sub 1"] = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            otherCollections["sub 2"] = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            otherCollections["fourth"] = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            otherCollections["third"] = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            var keys = new[]
            {
                "first",
                "second",
                "sub 1",
                "sub 2",
                "third",
            };

            var flattenedCollection = selector.Flatten(otherCollections, keys);
            Assert.That(flattenedCollection, Is.SupersetOf(otherCollections["first"]), "first");
            Assert.That(flattenedCollection, Is.SupersetOf(otherCollections["sub 1"]), "sub 1");
            Assert.That(flattenedCollection, Is.SupersetOf(otherCollections["sub 2"]), "sub 2");
            Assert.That(flattenedCollection, Is.SupersetOf(otherCollections["third"]), "third");
            Assert.That(flattenedCollection, Is.Not.SupersetOf(otherCollections["fourth"]), "fourth");
            Assert.That(flattenedCollection, Does.Not.Contain("first"));
            Assert.That(flattenedCollection, Does.Not.Contain("second"));
            Assert.That(flattenedCollection, Does.Not.Contain("sub 1"));
            Assert.That(flattenedCollection, Does.Not.Contain("sub 2"));
            Assert.That(flattenedCollection, Does.Not.Contain("third"));
            Assert.That(flattenedCollection, Does.Not.Contain("fourth"));
        }

        [Test]
        public void FlattenCollectionDistinctly()
        {
            var otherCollections = new Dictionary<string, IEnumerable<string>>();
            otherCollections["first"] = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            otherCollections["sub 1"] = new[] { Guid.NewGuid().ToString(), "repeat" };
            otherCollections["sub 2"] = new[] { Guid.NewGuid().ToString(), "repeat" };
            otherCollections["fourth"] = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };
            otherCollections["third"] = new[] { Guid.NewGuid().ToString(), Guid.NewGuid().ToString() };

            var keys = new[]
            {
                "first",
                "second",
                "sub 1",
                "sub 2",
                "third",
            };

            var flattenedCollection = selector.Flatten(otherCollections, keys);
            Assert.That(flattenedCollection, Is.SupersetOf(otherCollections["first"]), "first");
            Assert.That(flattenedCollection, Is.SupersetOf(otherCollections["sub 1"]), "sub 1");
            Assert.That(flattenedCollection, Is.SupersetOf(otherCollections["sub 2"]), "sub 2");
            Assert.That(flattenedCollection, Is.SupersetOf(otherCollections["third"]), "third");
            Assert.That(flattenedCollection, Is.Not.SupersetOf(otherCollections["fourth"]), "fourth");
            Assert.That(flattenedCollection, Does.Not.Contain("first"));
            Assert.That(flattenedCollection, Does.Not.Contain("second"));
            Assert.That(flattenedCollection, Does.Not.Contain("sub 1"));
            Assert.That(flattenedCollection, Does.Not.Contain("sub 2"));
            Assert.That(flattenedCollection, Does.Not.Contain("third"));
            Assert.That(flattenedCollection, Does.Not.Contain("fourth"));
            Assert.That(flattenedCollection.Count(i => i == "repeat"), Is.EqualTo(1));
        }

        [Test]
        public void CreateEmptyWeightedCollectionWithDefault()
        {
            var weightedCollection = selector.CreateWeighted();
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Empty);
        }

        [Test]
        public void CreateEmptyWeightedCollection()
        {
            var common = new List<string>();
            var uncommon = new List<string>();
            var rare = new List<string>();
            var veryRare = new List<string>();

            var weightedCollection = selector.CreateWeighted(common, uncommon, rare, veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Empty);
        }

        [Test]
        public void CreateWeightedCollection()
        {
            var common = new[] { "common" };
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(common, uncommon, rare, veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(9));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(30));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(60));
            Assert.That(weightedCollection.Count(), Is.EqualTo(100));
        }

        [Test]
        public void CreateWeightedCollectionWithNullCommon()
        {
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(uncommon: uncommon, rare: rare, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(9));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(90));
            Assert.That(weightedCollection.Count(), Is.EqualTo(100));
        }

        [Test]
        public void CreateWeightedCollectionWithNullUncommon()
        {
            var common = new[] { "common" };
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(common, rare: rare, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(9));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(90));
            Assert.That(weightedCollection.Count(), Is.EqualTo(100));
        }

        [Test]
        public void CreateWeightedCollectionWithNullRare()
        {
            var common = new[] { "common" };
            var uncommon = new[] { "uncommon" };
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(common, uncommon, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(39));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(60));
            Assert.That(weightedCollection.Count(), Is.EqualTo(100));
        }

        [Test]
        public void CreateWeightedCollectionWithNullVeryRare()
        {
            var common = new[] { "common" };
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };

            var weightedCollection = selector.CreateWeighted(common, uncommon, rare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(3));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(6));
            Assert.That(weightedCollection.Count(), Is.EqualTo(10));
        }

        [TestCaseSource(typeof(WeightTestData), "Two")]
        public void CreateWeightedCollectionWithDuplicateRareAgainstVeryRare(int rareQuantity, int veryRareQuantity)
        {
            var rare = Enumerable.Range(1, rareQuantity).Select(i => $"rare {i}");
            var veryRare = Enumerable.Range(1, veryRareQuantity).Select(i => $"very rare {i}");

            var weightedCollection = selector.CreateWeighted(rare: rare, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(veryRare, Is.SubsetOf(weightedCollection));
            Assert.That(rare, Is.SubsetOf(weightedCollection));

            foreach (var veryRareEntry in veryRare)
                Assert.That(weightedCollection.Count(i => i == veryRareEntry), Is.EqualTo(1), veryRareEntry);

            var rareDuplicationCount = Convert.ToInt32(Math.Ceiling(veryRareQuantity / (double)rareQuantity * 9));

            foreach (var rareEntry in rare)
                Assert.That(weightedCollection.Count(i => i == rareEntry), Is.EqualTo(rareDuplicationCount), rareEntry);

            var expectedCount = rare.Count() * rareDuplicationCount + veryRareQuantity;
            Assert.That(weightedCollection.Count(), Is.EqualTo(expectedCount));

            var rareWeight = weightedCollection.Count(i => rare.Contains(i)) / (double)weightedCollection.Count();
            Assert.That(rareWeight, Is.AtLeast(.9));
        }

        private class WeightTestData
        {
            private static IEnumerable<int> quantities = new[]
            {
                1, 2, 9, 10, 11, 42, 96
            };

            public static IEnumerable Two
            {
                get
                {
                    foreach (var value1 in quantities)
                    {
                        foreach (var value2 in quantities)
                        {
                            yield return new TestCaseData(value1, value2);
                        }
                    }
                }
            }

            public static IEnumerable Three
            {
                get
                {
                    foreach (var value1 in quantities)
                    {
                        foreach (var value2 in quantities)
                        {
                            foreach (var value3 in quantities)
                            {
                                yield return new TestCaseData(value1, value2, value3);
                            }
                        }
                    }
                }
            }

            public static IEnumerable Four
            {
                get
                {
                    foreach (var value1 in quantities)
                    {
                        foreach (var value2 in quantities)
                        {
                            foreach (var value3 in quantities)
                            {
                                foreach (var value4 in quantities)
                                {
                                    yield return new TestCaseData(value1, value2, value3, value4);
                                }
                            }
                        }
                    }
                }
            }
        }

        [TestCaseSource(typeof(WeightTestData), "Two")]
        public void CreateWeightedCollectionWithDuplicateUncommonAgainstRare(int uncommonQuantity, int rareQuantity)
        {
            var uncommon = Enumerable.Range(1, uncommonQuantity).Select(i => $"uncommon {i}");
            var rare = Enumerable.Range(1, rareQuantity).Select(i => $"rare {i}");

            var weightedCollection = selector.CreateWeighted(uncommon: uncommon, rare: rare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(rare, Is.SubsetOf(weightedCollection));
            Assert.That(uncommon, Is.SubsetOf(weightedCollection));

            foreach (var rareEntry in rare)
                Assert.That(weightedCollection.Count(i => i == rareEntry), Is.EqualTo(1), rareEntry);

            var uncommonDuplicationCount = Convert.ToInt32(Math.Ceiling(rareQuantity / (double)uncommonQuantity * 9));

            foreach (var uncommonEntry in uncommon)
                Assert.That(weightedCollection.Count(i => i == uncommonEntry), Is.EqualTo(uncommonDuplicationCount), uncommonEntry);

            var expectedCount = uncommon.Count() * uncommonDuplicationCount + rareQuantity;
            Assert.That(weightedCollection.Count(), Is.EqualTo(expectedCount));

            var uncommonWeight = weightedCollection.Count(i => uncommon.Contains(i)) / (double)weightedCollection.Count();
            Assert.That(uncommonWeight, Is.AtLeast(.9));
        }

        [TestCaseSource(typeof(WeightTestData), "Two")]
        public void CreateWeightedCollectionWithDuplicateUncommonAgainstVeryRare(int uncommonQuantity, int veryRareQuantity)
        {
            var uncommon = Enumerable.Range(1, uncommonQuantity).Select(i => $"uncommon {i}");
            var veryRare = Enumerable.Range(1, veryRareQuantity).Select(i => $"very rare {i}");

            var weightedCollection = selector.CreateWeighted(uncommon, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(veryRare, Is.SubsetOf(weightedCollection));
            Assert.That(uncommon, Is.SubsetOf(weightedCollection));

            foreach (var veryRareEntry in veryRare)
                Assert.That(weightedCollection.Count(i => i == veryRareEntry), Is.EqualTo(1), veryRareEntry);

            var uncommonDuplicationCount = Convert.ToInt32(Math.Ceiling(Math.Round(veryRareQuantity / (double)uncommonQuantity * 99, 3)));

            foreach (var uncommonEntry in uncommon)
                Assert.That(weightedCollection.Count(i => i == uncommonEntry), Is.EqualTo(uncommonDuplicationCount), uncommonEntry);

            var expectedCount = uncommon.Count() * uncommonDuplicationCount + veryRareQuantity;
            Assert.That(weightedCollection.Count(), Is.EqualTo(expectedCount));

            var uncommonWeight = weightedCollection.Count(i => uncommon.Contains(i)) / (double)weightedCollection.Count();
            Assert.That(uncommonWeight, Is.AtLeast(.99));
        }

        [TestCaseSource(typeof(WeightTestData), "Three")]
        public void CreateWeightedCollectionWithDuplicateUncommonAgainstRareAndVeryRare(int uncommonQuantity, int rareQuantity, int veryRareQuantity)
        {
            var uncommon = Enumerable.Range(1, uncommonQuantity).Select(i => $"uncommon {i}");
            var rare = Enumerable.Range(1, rareQuantity).Select(i => $"rare {i}");
            var veryRare = Enumerable.Range(1, veryRareQuantity).Select(i => $"very rare {i}");

            var weightedCollection = selector.CreateWeighted(uncommon: uncommon, rare: rare, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(veryRare, Is.SubsetOf(weightedCollection));
            Assert.That(uncommon, Is.SubsetOf(weightedCollection));

            foreach (var veryRareEntry in veryRare)
                Assert.That(weightedCollection.Count(i => i == veryRareEntry), Is.EqualTo(1), veryRareEntry);

            var rareDuplicationCount = Convert.ToInt32(Math.Ceiling(veryRareQuantity / (double)rareQuantity * 9));

            foreach (var rareEntry in rare)
                Assert.That(weightedCollection.Count(i => i == rareEntry), Is.EqualTo(rareDuplicationCount), rareEntry);

            var uncommonDuplicationCount = Convert.ToInt32(Math.Ceiling((veryRareQuantity + rareQuantity * rareDuplicationCount) / (double)uncommonQuantity * 9));

            foreach (var uncommonEntry in uncommon)
                Assert.That(weightedCollection.Count(i => i == uncommonEntry), Is.EqualTo(uncommonDuplicationCount), uncommonEntry);

            var expectedCount = uncommon.Count() * uncommonDuplicationCount + rare.Count() * rareDuplicationCount + veryRareQuantity;
            Assert.That(weightedCollection.Count(), Is.EqualTo(expectedCount));

            var uncommonWeight = weightedCollection.Count(i => uncommon.Contains(i)) / (double)weightedCollection.Count();
            Assert.That(uncommonWeight, Is.AtLeast(.9));
        }

        [TestCaseSource(typeof(WeightTestData), "Two")]
        public void CreateWeightedCollectionWithDuplicateCommonAgainstRare(int commonQuantity, int rareQuantity)
        {
            var common = Enumerable.Range(1, commonQuantity).Select(i => $"common {i}");
            var rare = Enumerable.Range(1, rareQuantity).Select(i => $"rare {i}");

            var weightedCollection = selector.CreateWeighted(common, rare: rare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(rare, Is.SubsetOf(weightedCollection));
            Assert.That(common, Is.SubsetOf(weightedCollection));

            foreach (var rareEntry in rare)
                Assert.That(weightedCollection.Count(i => i == rareEntry), Is.EqualTo(1), rareEntry);

            var commonDuplicationCount = Convert.ToInt32(Math.Ceiling(rareQuantity / (double)commonQuantity * 9));

            foreach (var commonEntry in common)
                Assert.That(weightedCollection.Count(i => i == commonEntry), Is.EqualTo(commonDuplicationCount), commonEntry);

            var expectedCount = common.Count() * commonDuplicationCount + rareQuantity;
            Assert.That(weightedCollection.Count(), Is.EqualTo(expectedCount));

            var commonWeight = weightedCollection.Count(i => common.Contains(i)) / (double)weightedCollection.Count();
            Assert.That(commonWeight, Is.AtLeast(.9));
        }

        [TestCaseSource(typeof(WeightTestData), "Two")]
        public void CreateWeightedCollectionWithDuplicateCommonAgainstVeryRare(int commonQuantity, int veryRareQuantity)
        {
            var common = Enumerable.Range(1, commonQuantity).Select(i => $"common {i}");
            var veryRare = Enumerable.Range(1, veryRareQuantity).Select(i => $"very rare {i}");

            var weightedCollection = selector.CreateWeighted(common, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(veryRare, Is.SubsetOf(weightedCollection));
            Assert.That(common, Is.SubsetOf(weightedCollection));

            foreach (var veryRareEntry in veryRare)
                Assert.That(weightedCollection.Count(i => i == veryRareEntry), Is.EqualTo(1), veryRareEntry);

            var commonDuplicationCount = Convert.ToInt32(Math.Ceiling(Math.Round(veryRareQuantity / (double)commonQuantity * 99, 3)));

            foreach (var commonEntry in common)
                Assert.That(weightedCollection.Count(i => i == commonEntry), Is.EqualTo(commonDuplicationCount), commonEntry);

            var expectedCount = common.Count() * commonDuplicationCount + veryRareQuantity;
            Assert.That(weightedCollection.Count(), Is.EqualTo(expectedCount));

            var commonWeight = weightedCollection.Count(i => common.Contains(i)) / (double)weightedCollection.Count();
            Assert.That(commonWeight, Is.AtLeast(.99));
        }

        [TestCaseSource(typeof(WeightTestData), "Two")]
        public void CreateWeightedCollectionWithDuplicateCommonAgainstUncommon(int commonQuantity, int uncommonQuantity)
        {
            var common = Enumerable.Range(1, commonQuantity).Select(i => $"common {i}");
            var uncommon = Enumerable.Range(1, uncommonQuantity).Select(i => $"uncommon {i}");

            var weightedCollection = selector.CreateWeighted(common, uncommon);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(uncommon, Is.SubsetOf(weightedCollection));
            Assert.That(common, Is.SubsetOf(weightedCollection));

            foreach (var uncommonEntry in uncommon)
                Assert.That(weightedCollection.Count(i => i == uncommonEntry), Is.EqualTo(1), uncommonEntry);

            var commonDuplicationCount = Convert.ToInt32(Math.Ceiling(uncommonQuantity / (double)commonQuantity * 2));

            foreach (var commonEntry in common)
                Assert.That(weightedCollection.Count(i => i == commonEntry), Is.EqualTo(commonDuplicationCount), commonEntry);

            var expectedCount = common.Count() * commonDuplicationCount + uncommonQuantity;
            Assert.That(weightedCollection.Count(), Is.EqualTo(expectedCount));

            var commonWeight = weightedCollection.Count(i => common.Contains(i)) / (double)weightedCollection.Count();
            Assert.That(commonWeight, Is.AtLeast(2 / 3d));
        }

        [TestCaseSource(typeof(WeightTestData), "Three")]
        public void CreateWeightedCollectionWithDuplicateCommonAgainstRareAndVeryRare(int commonQuantity, int rareQuantity, int veryRareQuantity)
        {
            var common = Enumerable.Range(1, commonQuantity).Select(i => $"common {i}");
            var rare = Enumerable.Range(1, rareQuantity).Select(i => $"rare {i}");
            var veryRare = Enumerable.Range(1, veryRareQuantity).Select(i => $"very rare {i}");

            var weightedCollection = selector.CreateWeighted(common, rare: rare, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(veryRare, Is.SubsetOf(weightedCollection));
            Assert.That(common, Is.SubsetOf(weightedCollection));

            foreach (var veryRareEntry in veryRare)
                Assert.That(weightedCollection.Count(i => i == veryRareEntry), Is.EqualTo(1), veryRareEntry);

            var rareDuplicationCount = Convert.ToInt32(Math.Ceiling(veryRareQuantity / (double)rareQuantity * 9));

            foreach (var rareEntry in rare)
                Assert.That(weightedCollection.Count(i => i == rareEntry), Is.EqualTo(rareDuplicationCount), rareEntry);

            var commonDuplicationCount = Convert.ToInt32(Math.Ceiling((veryRareQuantity + rareQuantity * rareDuplicationCount) / (double)commonQuantity * 9));

            foreach (var commonEntry in common)
                Assert.That(weightedCollection.Count(i => i == commonEntry), Is.EqualTo(commonDuplicationCount), commonEntry);

            var expectedCount = common.Count() * commonDuplicationCount + rare.Count() * rareDuplicationCount + veryRareQuantity;
            Assert.That(weightedCollection.Count(), Is.EqualTo(expectedCount));

            var commonWeight = weightedCollection.Count(i => common.Contains(i)) / (double)weightedCollection.Count();
            Assert.That(commonWeight, Is.AtLeast(.9));
        }

        [Test]
        public void CreateWeightedCollectionOfJustCommon()
        {
            var common = new[] { "common" };

            var weightedCollection = selector.CreateWeighted(common);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateWeightedCollectionOfJustUncommon()
        {
            var uncommon = new[] { "uncommon" };

            var weightedCollection = selector.CreateWeighted(uncommon);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateWeightedCollectionOfJustRare()
        {
            var rare = new[] { "rare" };

            var weightedCollection = selector.CreateWeighted(rare: rare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateWeightedCollectionOfJustVeryRare()
        {
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(), Is.EqualTo(1));
        }

        [Test]
        public void CreateWeightedCollectionOfCommonAndUncommon()
        {
            var common = new[] { "common" };
            var uncommon = new[] { "uncommon" };

            var weightedCollection = selector.CreateWeighted(common, uncommon);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(2));
            Assert.That(weightedCollection.Count(), Is.EqualTo(3));
        }

        [Test]
        public void CreateWeightedCollectionOfCommonAndRare()
        {
            var common = new[] { "common" };
            var rare = new[] { "rare" };

            var weightedCollection = selector.CreateWeighted(common, rare: rare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(9));
            Assert.That(weightedCollection.Count(), Is.EqualTo(10));
        }

        [Test]
        public void CreateWeightedCollectionOfCommonAndVeryRare()
        {
            var common = new[] { "common" };
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(common, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(99));
            Assert.That(weightedCollection.Count(), Is.EqualTo(100));
        }

        [Test]
        public void CreateWeightedCollectionOfUncommonAndRare()
        {
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };

            var weightedCollection = selector.CreateWeighted(uncommon: uncommon, rare: rare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(9));
            Assert.That(weightedCollection.Count(), Is.EqualTo(10));
        }

        [Test]
        public void CreateWeightedCollectionOfUncommonAndVeryRare()
        {
            var uncommon = new[] { "uncommon" };
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(uncommon: uncommon, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(99));
            Assert.That(weightedCollection.Count(), Is.EqualTo(100));
        }

        [Test]
        public void CreateWeightedCollectionOfRareAndVeryRare()
        {
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(rare: rare, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(9));
            Assert.That(weightedCollection.Count(), Is.EqualTo(10));
        }

        [Test]
        public void CreateWeightedCollectionOfCommonAndUncommonAndRare()
        {
            var common = new[] { "common" };
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };

            var weightedCollection = selector.CreateWeighted(common, uncommon, rare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(3));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(6));
            Assert.That(weightedCollection.Count(), Is.EqualTo(10));
        }

        [Test]
        public void CreateWeightedCollectionOfCommonAndUncommonAndVeryRare()
        {
            var common = new[] { "common" };
            var uncommon = new[] { "uncommon" };
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(common, uncommon, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(39));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(60));
            Assert.That(weightedCollection.Count(), Is.EqualTo(100));
        }

        [Test]
        public void CreateWeightedCollectionOfCommonAndRareAndVeryRare()
        {
            var common = new[] { "common" };
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(common, rare: rare, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection, Contains.Item("common"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(9));
            Assert.That(weightedCollection.Count(i => i == "common"), Is.EqualTo(90));
            Assert.That(weightedCollection.Count(), Is.EqualTo(100));
        }

        [Test]
        public void CreateWeightedCollectionOfUncommonAndRareAndVeryRare()
        {
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            var weightedCollection = selector.CreateWeighted(uncommon, rare: rare, veryRare: veryRare);
            Assert.That(weightedCollection, Is.Not.Null);
            Assert.That(weightedCollection, Is.Not.Empty);
            Assert.That(weightedCollection, Contains.Item("very rare"));
            Assert.That(weightedCollection, Contains.Item("rare"));
            Assert.That(weightedCollection, Contains.Item("uncommon"));
            Assert.That(weightedCollection.Count(i => i == "very rare"), Is.EqualTo(1));
            Assert.That(weightedCollection.Count(i => i == "rare"), Is.EqualTo(9));
            Assert.That(weightedCollection.Count(i => i == "uncommon"), Is.EqualTo(90));
            Assert.That(weightedCollection.Count(), Is.EqualTo(100));
        }
    }
}