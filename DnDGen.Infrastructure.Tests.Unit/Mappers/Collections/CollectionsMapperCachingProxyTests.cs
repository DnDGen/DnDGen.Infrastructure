using DnDGen.Infrastructure.Mappers.Collections;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DnDGen.Infrastructure.Tests.Unit.Mappers.Collections
{
    [TestFixture]
    public class CollectionsMapperCachingProxyTests
    {
        private CollectionMapper proxy;
        private Mock<CollectionMapper> mockInnerMapper;
        private Dictionary<string, IEnumerable<string>> table;

        [SetUp]
        public void Setup()
        {
            mockInnerMapper = new Mock<CollectionMapper>();
            proxy = new CollectionMapperCachingProxy(mockInnerMapper.Object);

            table = new Dictionary<string, IEnumerable<string>>();
            table["name"] = new[] { "entry 1", "entry 2" };
            table["other name"] = new[] { "entry 3", "entry 4" };

            mockInnerMapper.Setup(m => m.Map("my assembly", "table name")).Returns(table);
        }

        [Test]
        public void ReturnTableFromInnerMapper()
        {
            var result = proxy.Map("my assembly", "table name");
            Assert.That(result, Is.EqualTo(table));
        }

        [Test]
        public void CacheTable()
        {
            proxy.Map("my assembly", "table name");
            var result = proxy.Map("my assembly", "table name");

            Assert.That(result, Is.EqualTo(table));
            mockInnerMapper.Verify(p => p.Map(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void DoNotCacheDifferentTable()
        {
            var otherTable = new Dictionary<string, IEnumerable<string>>();
            otherTable["other name"] = new[] { "other entry 1", "other entry 2" };
            otherTable["other other name"] = new[] { "other entry 3", "other entry 4" };

            mockInnerMapper.SetupSequence(m => m.Map("my assembly", "other table name")).Returns(otherTable);

            var result = proxy.Map("my assembly", "table name");
            var otherResult = proxy.Map("my assembly", "other table name");

            Assert.That(result, Is.EqualTo(table));
            Assert.That(otherResult, Is.EqualTo(otherTable));
            mockInnerMapper.Verify(p => p.Map(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockInnerMapper.Verify(p => p.Map("my assembly", "table name"), Times.Once);
            mockInnerMapper.Verify(p => p.Map("my assembly", "other table name"), Times.Once);
        }

        [Test]
        public async Task CacheTableIsThreadsafe()
        {
            var tasks = new List<Task<Dictionary<string, IEnumerable<string>>>>();
            while (tasks.Count < 10)
            {
                var task = Task.Run(() => proxy.Map("my assembly", "table name"));
                tasks.Add(task);
            }

            await Task.WhenAll(tasks);

            var results = tasks.Select(t => t.Result);
            foreach (var result in results)
            {
                Assert.That(result, Is.EqualTo(table));
            }

            mockInnerMapper.Verify(p => p.Map(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Test]
        public void CacheTableWithAssembly()
        {
            var otherTable = new Dictionary<string, IEnumerable<string>>();
            otherTable["other name"] = new[] { "other entry 1", "other entry 2" };
            otherTable["other other name"] = new[] { "other entry 3", "other entry 4" };

            mockInnerMapper.SetupSequence(m => m.Map("my assembly", "table name")).Returns(table);
            mockInnerMapper.SetupSequence(m => m.Map("my other assembly", "table name")).Returns(otherTable);

            proxy.Map("my assembly", "table name");
            proxy.Map("my other assembly", "table name");

            var firstResult = proxy.Map("my assembly", "table name");
            var secondResult = proxy.Map("my other assembly", "table name");

            Assert.That(firstResult, Is.EqualTo(table));
            Assert.That(secondResult, Is.EqualTo(otherTable));

            mockInnerMapper.Verify(p => p.Map(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockInnerMapper.Verify(p => p.Map("my assembly", It.IsAny<string>()), Times.Once);
            mockInnerMapper.Verify(p => p.Map("my other assembly", It.IsAny<string>()), Times.Once);
        }
    }
}