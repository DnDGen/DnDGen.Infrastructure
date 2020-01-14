using DnDGen.Infrastructure.Mappers.Collections;
using DnDGen.Infrastructure.Mappers.Percentiles;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration.IoC.Modules
{
    [TestFixture]
    public class MappersModuleTests : IoCTests
    {
        [Test]
        public void PercentileMapperIsInstantiatedAsSingleton()
        {
            AssertSingleton<PercentileMapper>();
        }

        [Test]
        public void PercentileMapperIsDecorated()
        {
            AssertIsInstanceOf<PercentileMapper, PercentileMapperCachingProxy>();
        }

        [Test]
        public void CollectionsMapperIsInstantiatedAsSingleton()
        {
            AssertSingleton<CollectionMapper>();
        }

        [Test]
        public void CollectionsMapperIsDecorated()
        {
            AssertIsInstanceOf<CollectionMapper, CollectionMapperCachingProxy>();
        }
    }
}
