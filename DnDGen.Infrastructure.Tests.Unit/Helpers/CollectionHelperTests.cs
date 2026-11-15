using DnDGen.Infrastructure.Helpers;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Unit.Helpers
{
    [TestFixture]
    public class CollectionHelperTests
    {
        [Test]
        public void FlattenCollection()
        {
            var otherCollections = new Dictionary<string, IEnumerable<string>>
            {
                ["first"] = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()],
                ["sub 1"] = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()],
                ["sub 2"] = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()],
                ["fourth"] = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()],
                ["third"] = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()]
            };

            var keys = new[]
            {
                "first",
                "second",
                "sub 1",
                "sub 2",
                "third",
            };

            var flattenedCollection = CollectionHelper.FlattenCollection(otherCollections, keys);
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
            var otherCollections = new Dictionary<string, IEnumerable<string>>
            {
                ["first"] = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()],
                ["sub 1"] = [Guid.NewGuid().ToString(), "repeat"],
                ["sub 2"] = [Guid.NewGuid().ToString(), "repeat"],
                ["fourth"] = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()],
                ["third"] = [Guid.NewGuid().ToString(), Guid.NewGuid().ToString()]
            };

            var keys = new[]
            {
                "first",
                "second",
                "sub 1",
                "sub 2",
                "third",
            };

            var flattenedCollection = CollectionHelper.FlattenCollection(otherCollections, keys);
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
    }
}
