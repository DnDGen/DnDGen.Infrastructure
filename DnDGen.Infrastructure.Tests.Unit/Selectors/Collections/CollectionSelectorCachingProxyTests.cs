using DnDGen.Infrastructure.Selectors.Collections;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDGen.Infrastructure.Tests.Unit.Selectors.Collections
{
    [TestFixture]
    public class CollectionSelectorCachingProxyTests
    {
        private ICollectionSelector proxy;
        private Mock<ICollectionSelector> mockInnerSelector;

        [SetUp]
        public void Setup()
        {
            mockInnerSelector = new Mock<ICollectionSelector>();
            proxy = new CollectionSelectorCachingProxy(mockInnerSelector.Object);
        }

        [Test]
        public void Explode_ReturnsFromInnerSelector()
        {
            mockInnerSelector
                .Setup(s => s.Explode("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2" });

            var result = proxy.Explode("my assembly", "table name", "my entry");
            Assert.That(result, Is.EqualTo(new[] { "entry 1", "entry 2" }));
        }

        [Test]
        public void Explode_CacheResults()
        {
            mockInnerSelector
                .Setup(s => s.Explode("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2" });

            proxy.Explode("my assembly", "table name", "my entry");
            var result = proxy.Explode("my assembly", "table name", "my entry");

            Assert.That(result, Is.EqualTo(new[] { "entry 1", "entry 2" }));

            mockInnerSelector.Verify(p => p.Explode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Explode_DoNotCacheDifferentResults()
        {
            mockInnerSelector
                .Setup(s => s.Explode("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2" });
            mockInnerSelector
                .Setup(s => s.Explode("my assembly", "table name", "my other entry"))
                .Returns(new[] { "entry 3", "entry 4" });

            var result = proxy.Explode("my assembly", "table name", "my entry");
            var otherResult = proxy.Explode("my assembly", "table name", "my other entry");

            Assert.That(result, Is.EqualTo(new[] { "entry 1", "entry 2" }));
            Assert.That(otherResult, Is.EqualTo(new[] { "entry 3", "entry 4" }));

            mockInnerSelector.Verify(p => p.Explode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockInnerSelector.Verify(p => p.Explode("my assembly", "table name", "my entry"), Times.Once);
            mockInnerSelector.Verify(p => p.Explode("my assembly", "table name", "my other entry"), Times.Once);
        }

        [Test]
        public void Explode_CacheStaticResults()
        {
            var i = 0;
            var exploded = Enumerable.Range(0, 2).Select(n => $"entry {i++}");
            mockInnerSelector
                .Setup(s => s.Explode("my assembly", "table name", "my entry"))
                .Returns(exploded);

            proxy.Explode("my assembly", "table name", "my entry");
            var result = proxy.Explode("my assembly", "table name", "my entry");

            Assert.That(result, Is.EqualTo(new[] { "entry 0", "entry 1" }));
            Assert.That(i, Is.EqualTo(2));

            mockInnerSelector.Verify(p => p.Explode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task Explode_CacheResultsIsThreadsafe()
        {
            mockInnerSelector
                .Setup(s => s.Explode("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2" });

            var tasks = new List<Task<IEnumerable<string>>>();
            while (tasks.Count < 10)
            {
                var task = Task.Run(() => proxy.Explode("my assembly", "table name", "my entry"));
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            var results = tasks.Select(t => t.Result);
            foreach (var result in results)
            {
                Assert.That(result, Is.EqualTo(new[] { "entry 1", "entry 2" }));
            }

            mockInnerSelector.Verify(p => p.Explode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Explode_CacheResultsWithAssembly()
        {
            mockInnerSelector.Setup(s => s.Explode("my assembly", "table name", "my entry")).Returns(new[] { "entry 1", "entry 2" });
            mockInnerSelector.Setup(s => s.Explode("my other assembly", "table name", "my entry")).Returns(new[] { "entry 3", "entry 4" });

            proxy.Explode("my assembly", "table name", "my entry");
            proxy.Explode("my other assembly", "table name", "my entry");

            var firstResult = proxy.Explode("my assembly", "table name", "my entry");
            var secondResult = proxy.Explode("my other assembly", "table name", "my entry");

            Assert.That(firstResult, Is.EqualTo(new[] { "entry 1", "entry 2" }));
            Assert.That(secondResult, Is.EqualTo(new[] { "entry 3", "entry 4" }));

            mockInnerSelector.Verify(p => p.Explode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockInnerSelector.Verify(p => p.Explode("my assembly", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockInnerSelector.Verify(p => p.Explode("my other assembly", It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void Explode_CacheResults_WithoutDuplicates()
        {
            mockInnerSelector
                .Setup(s => s.Explode("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2" });
            mockInnerSelector
                .Setup(s => s.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2", "entry 2" });

            proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry");
            proxy.Explode("my assembly", "table name", "my entry");
            var result = proxy.Explode("my assembly", "table name", "my entry");

            Assert.That(result, Is.EqualTo(new[] { "entry 1", "entry 2" }));

            mockInnerSelector.Verify(p => p.Explode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ExplodeAndPreserveDuplicates_ReturnsFromInnerSelector()
        {
            mockInnerSelector
                .Setup(s => s.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2", "entry 2" });

            var result = proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry");
            Assert.That(result, Is.EqualTo(new[] { "entry 1", "entry 2", "entry 2" }));
        }

        [Test]
        public void ExplodeAndPreserveDuplicates_CacheResults()
        {
            mockInnerSelector
                .Setup(s => s.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2", "entry 2" });

            proxy.Explode("my assembly", "table name", "my entry");
            var result = proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry");

            Assert.That(result, Is.EqualTo(new[] { "entry 1", "entry 2", "entry 2" }));

            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ExplodeAndPreserveDuplicates_DoNotCacheDifferentResults()
        {
            mockInnerSelector
                .Setup(s => s.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2", "entry 2" });
            mockInnerSelector
                .Setup(s => s.ExplodeAndPreserveDuplicates("my assembly", "table name", "my other entry"))
                .Returns(new[] { "entry 3", "entry 3", "entry 4" });

            proxy.Explode("my assembly", "table name", "my entry");
            proxy.Explode("my assembly", "table name", "my other entry");

            var result = proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry");
            var otherResult = proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my other entry");

            Assert.That(result, Is.EqualTo(new[] { "entry 1", "entry 2", "entry 2" }));
            Assert.That(otherResult, Is.EqualTo(new[] { "entry 3", "entry 3", "entry 4" }));

            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry"), Times.Once);
            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates("my assembly", "table name", "my other entry"), Times.Once);
        }

        [Test]
        public async Task ExplodeAndPreserveDuplicates_CacheResultsIsThreadsafe()
        {
            mockInnerSelector
                .Setup(s => s.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2", "entry 2" });

            var tasks = new List<Task<IEnumerable<string>>>();
            while (tasks.Count < 10)
            {
                var task = Task.Run(() => proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry"));
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            var results = tasks.Select(t => t.Result);
            foreach (var result in results)
            {
                Assert.That(result, Is.EqualTo(new[] { "entry 1", "entry 2", "entry 2" }));
            }

            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ExplodeAndPreserveDuplicates_CacheResultsWithAssembly()
        {
            mockInnerSelector.Setup(s => s.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry")).Returns(new[] { "entry 1", "entry 2", "entry 2" });
            mockInnerSelector.Setup(s => s.ExplodeAndPreserveDuplicates("my other assembly", "table name", "my entry")).Returns(new[] { "entry 3", "entry 4", "entry 4" });

            proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry");
            proxy.ExplodeAndPreserveDuplicates("my other assembly", "table name", "my entry");

            var firstResult = proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry");
            var secondResult = proxy.ExplodeAndPreserveDuplicates("my other assembly", "table name", "my entry");

            Assert.That(firstResult, Is.EqualTo(new[] { "entry 1", "entry 2", "entry 2" }));
            Assert.That(secondResult, Is.EqualTo(new[] { "entry 3", "entry 4", "entry 4" }));

            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry"), Times.Once);
            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates("my other assembly", "table name", "my entry"), Times.Once);
        }

        [Test]
        public void ExplodeAndPreserveDuplicates_CacheResults_WithDuplicates()
        {
            mockInnerSelector
                .Setup(s => s.Explode("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2" });
            mockInnerSelector
                .Setup(s => s.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry"))
                .Returns(new[] { "entry 1", "entry 2", "entry 2" });

            proxy.Explode("my assembly", "table name", "my entry");
            proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry");
            var result = proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry");

            Assert.That(result, Is.EqualTo(new[] { "entry 1", "entry 2", "entry 2" }));

            mockInnerSelector.Verify(p => p.Explode(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void ExplodeAndPreserveDuplicates_CacheStaticResults()
        {
            var i = 0;
            var exploded = Enumerable.Range(0, 2).SelectMany(n => new[] { $"entry {i}", $"entry {i++}" });
            mockInnerSelector
                .Setup(s => s.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry"))
                .Returns(exploded);

            proxy.Explode("my assembly", "table name", "my entry");
            var result = proxy.ExplodeAndPreserveDuplicates("my assembly", "table name", "my entry");

            Assert.That(result, Is.EqualTo(new[] { "entry 0", "entry 0", "entry 1", "entry 1" }));
            Assert.That(i, Is.EqualTo(2));

            mockInnerSelector.Verify(p => p.ExplodeAndPreserveDuplicates(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CreateWeighted_ReturnsInnerResult()
        {
            var common = new[] { "common 1", "common 2" };
            var uncommon = new[] { "uncommon 1", "uncommon 2" };
            var rare = new[] { "rare 1", "rare 2" };
            var veryRare = new[] { "very rare 1", "very rare 2" };

            mockInnerSelector
                .Setup(s => s.CreateWeighted(common, uncommon, rare, veryRare))
                .Returns(new[] { "my result", "my other result" });

            var result = proxy.CreateWeighted(common, uncommon, rare, veryRare);
            Assert.That(result, Is.EqualTo(new[] { "my result", "my other result" }));
        }

        [Test]
        public void FindCollectionOf_ReturnsInnerResult()
        {
            mockInnerSelector
                .Setup(s => s.FindCollectionOf("my assembly", "my table", "my entry", "collection 1", "collection 2"))
                .Returns("my collection");

            var result = proxy.FindCollectionOf("my assembly", "my table", "my entry", "collection 1", "collection 2");
            Assert.That(result, Is.EqualTo("my collection"));
        }

        [Test]
        public void Flatten_ReturnsInnerResult()
        {
            var collections = new Dictionary<string, IEnumerable<string>>();
            collections["my entry"] = new[] { "entry 1", "entry 2" };
            collections["my other entry"] = new[] { "entry 3", "entry 4" };

            var keys = new[] { "key 1", "key 2" };

            mockInnerSelector
                .Setup(s => s.Flatten(collections, keys))
                .Returns(new[] { "my result", "my other result" });

            var result = proxy.Flatten(collections, keys);
            Assert.That(result, Is.EqualTo(new[] { "my result", "my other result" }));
        }

        [Test]
        public void SelectAllFrom_ReturnsInnerResult()
        {
            var collections = new Dictionary<string, IEnumerable<string>>();
            collections["my entry"] = new[] { "entry 1", "entry 2" };
            collections["my other entry"] = new[] { "entry 3", "entry 4" };

            mockInnerSelector
                .Setup(s => s.SelectAllFrom("my assembly", "my table"))
                .Returns(collections);

            var result = proxy.SelectAllFrom("my assembly", "my table");
            Assert.That(result, Is.EqualTo(collections));
        }

        [Test]
        public void SelectFrom_ReturnsInnerResult()
        {
            mockInnerSelector
                .Setup(s => s.SelectFrom("my assembly", "my table", "my collection"))
                .Returns(new[] { "my result", "my other result" });

            var result = proxy.SelectFrom("my assembly", "my table", "my collection");
            Assert.That(result, Is.EqualTo(new[] { "my result", "my other result" }));
        }

        [Test]
        public void SelectRandomFrom_Weighted_ReturnsInnerResult()
        {
            var common = new[] { "common 1", "common 2" };
            var uncommon = new[] { "uncommon 1", "uncommon 2" };
            var rare = new[] { "rare 1", "rare 2" };
            var veryRare = new[] { "very rare 1", "very rare 2" };

            mockInnerSelector
                .Setup(s => s.SelectRandomFrom(common, uncommon, rare, veryRare))
                .Returns("my result");

            var result = proxy.SelectRandomFrom(common, uncommon, rare, veryRare);
            Assert.That(result, Is.EqualTo("my result"));
        }

        [Test]
        public void SelectRandomFrom_ReturnsInnerResult()
        {
            var common = new[] { "common 1", "common 2" };
            var uncommon = new[] { "uncommon 1", "uncommon 2" };
            var rare = new[] { "rare 1", "rare 2" };
            var veryRare = new[] { "very rare 1", "very rare 2" };

            mockInnerSelector
                .Setup(s => s.SelectRandomFrom("my assembly", "my table", "my collection"))
                .Returns("my result");

            var result = proxy.SelectRandomFrom("my assembly", "my table", "my collection");
            Assert.That(result, Is.EqualTo("my result"));
        }

        [Test]
        public void SelectRandomFrom_Collection_ReturnsInnerResult()
        {
            var collection = new[] { "item 1", "item 2" };

            mockInnerSelector
                .Setup(s => s.SelectRandomFrom(collection))
                .Returns("my result");

            var result = proxy.SelectRandomFrom(collection);
            Assert.That(result, Is.EqualTo("my result"));
        }

        [TestCase(true)]
        [TestCase(false)]
        public void IsCollection_ReturnsInnerResult(bool isCollection)
        {
            mockInnerSelector
                .Setup(s => s.IsCollection("my assembly", "my table", "my collection"))
                .Returns(isCollection);

            var result = proxy.IsCollection("my assembly", "my table", "my collection");
            Assert.That(result, Is.EqualTo(isCollection));
        }
    }
}
