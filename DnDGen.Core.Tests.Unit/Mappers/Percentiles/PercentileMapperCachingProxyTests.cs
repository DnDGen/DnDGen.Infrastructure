using DnDGen.Core.Mappers.Percentiles;
using DnDGen.Core.Tables;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Reflection;

namespace DnDGen.Core.Tests.Unit.Mappers.Percentiles
{
    [TestFixture]
    public class PercentileMapperCachingProxyTests
    {
        private PercentileMapper proxy;
        private Mock<PercentileMapper> mockInnerMapper;
        private Dictionary<int, string> table;
        private Mock<AssemblyLoader> mockAssemblyLoader;

        [SetUp]
        public void Setup()
        {
            mockInnerMapper = new Mock<PercentileMapper>();
            mockAssemblyLoader = new Mock<AssemblyLoader>();
            proxy = new PercentileMapperCachingProxy(mockInnerMapper.Object, mockAssemblyLoader.Object);

            table = new Dictionary<int, string>();
            table[9266] = "favorite number";
            table[90210] = "beverly hills";

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
        public void CacheTableWithAssembly()
        {
            var otherTable = new Dictionary<int, string>();
            otherTable[42] = "answer to life, the universe, and everything";
            otherTable[600] = "highest number I have counted";

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