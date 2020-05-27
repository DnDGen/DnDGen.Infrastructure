using DnDGen.Infrastructure.Mappers.Collections;
using DnDGen.Infrastructure.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;

namespace DnDGen.Infrastructure.Tests.Unit.Mappers.Collections
{
    [TestFixture]
    public class CollectionsMapperCachingProxyTests
    {
        private CollectionMapper proxy;
        private Mock<CollectionMapper> mockInnerMapper;
        private Dictionary<string, IEnumerable<string>> table;
        private Mock<AssemblyLoader> mockAssemblyLoader;

        [SetUp]
        public void Setup()
        {
            mockInnerMapper = new Mock<CollectionMapper>();
            mockAssemblyLoader = new Mock<AssemblyLoader>();
            proxy = new CollectionMapperCachingProxy(mockInnerMapper.Object, mockAssemblyLoader.Object);

            table = new Dictionary<string, IEnumerable<string>>();
            table["name"] = new[] { "entry 1", "entry 2" };
            table["other name"] = new[] { "entry 3", "entry 4" };

            mockInnerMapper.Setup(m => m.Map("table name")).Returns(table);
            mockAssemblyLoader.Setup(l => l.GetRunningAssembly()).Returns(Assembly.GetExecutingAssembly());
        }

        [Test]
        public void ReturnTableFromInnerMapper()
        {
            var result = proxy.Map("table name");
            Assert.That(result, Is.EqualTo(table));
        }

        [Test]
        public void CacheTable()
        {
            proxy.Map("table name");
            var result = proxy.Map("table name");

            Assert.That(result, Is.EqualTo(table));
            mockInnerMapper.Verify(p => p.Map(It.IsAny<string>()), Times.Once);
        }

        [Test]
        public async Task CacheTableIsThreadsafe()
        {
            Assert.Fail("not yet written");
        }

        [Test]
        public void CacheTableWithAssembly()
        {
            var otherTable = new Dictionary<string, IEnumerable<string>>();
            otherTable["other name"] = new[] { "other entry 1", "other entry 2" };
            otherTable["other other name"] = new[] { "other entry 3", "other entry 4" };

            mockInnerMapper.SetupSequence(m => m.Map("table name"))
                .Returns(table)
                .Returns(otherTable)
                .Returns(table)
                .Returns(otherTable);

            mockAssemblyLoader.SetupSequence(l => l.GetRunningAssembly())
                .Returns(Assembly.GetExecutingAssembly())
                .Returns(typeof(int).Assembly)
                .Returns(Assembly.GetExecutingAssembly())
                .Returns(typeof(int).Assembly);

            proxy.Map("table name");
            proxy.Map("table name");

            var firstResult = proxy.Map("table name");
            var secondResult = proxy.Map("table name");

            Assert.That(firstResult, Is.EqualTo(table));
            Assert.That(secondResult, Is.EqualTo(otherTable));

            mockInnerMapper.Verify(p => p.Map(It.IsAny<string>()), Times.Exactly(2));
        }
    }
}