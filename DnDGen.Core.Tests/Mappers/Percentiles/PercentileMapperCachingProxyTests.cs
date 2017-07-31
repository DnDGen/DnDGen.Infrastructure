using DnDGen.Core.Mappers.Percentiles;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.Core.Tests.Mappers.Percentiles
{
    [TestFixture]
    public class PercentileMapperCachingProxyTests
    {
        private PercentileMapper proxy;
        private Mock<PercentileMapper> mockInnerMapper;
        private Dictionary<int, string> table;

        [SetUp]
        public void Setup()
        {
            table = new Dictionary<int, string>();
            mockInnerMapper = new Mock<PercentileMapper>();
            mockInnerMapper.Setup(m => m.Map("table name")).Returns(table);

            proxy = new PercentileMapperCachingProxy(mockInnerMapper.Object);
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
    }
}