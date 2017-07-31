using DnDGen.Core.Mappers.Collections;
using DnDGen.Core.Mappers.Percentiles;
using NUnit.Framework;

namespace DnDGen.Core.Tests.IoC.Modules
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
            AssertSingleton<CollectionsMapper>();
        }

        [Test]
        public void CollectionsMapperIsDecorated()
        {
            AssertIsInstanceOf<CollectionsMapper, CollectionsMapperCachingProxy>();
        }
    }
}
