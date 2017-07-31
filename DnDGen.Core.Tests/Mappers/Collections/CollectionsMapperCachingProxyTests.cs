using DnDGen.Core.Mappers.Collections;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;

namespace DnDGen.Core.Tests.Mappers.Collections
{
    [TestFixture]
    public class CollectionsMapperCachingProxyTests
    {
        private CollectionsMapper proxy;
        private Mock<CollectionsMapper> mockInnerMapper;
        private Dictionary<string, IEnumerable<string>> table;

        [SetUp]
        public void Setup()
        {
            table = new Dictionary<string, IEnumerable<string>>();
            mockInnerMapper = new Mock<CollectionsMapper>();
            mockInnerMapper.Setup(m => m.Map("table name")).Returns(table);

            proxy = new CollectionsMapperCachingProxy(mockInnerMapper.Object);
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