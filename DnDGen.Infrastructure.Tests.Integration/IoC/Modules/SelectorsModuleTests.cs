using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration.IoC.Modules
{
    [TestFixture]
    public class SelectorsModuleTests : IoCTests
    {
        [Test]
        public void PercentileSelectorIsNotInstantiatedAsSingleton()
        {
            AssertNotSingleton<IPercentileSelector>();
        }

        [Test]
        public void CollectionsSelectorIsInstantiatedAsSingleton()
        {
            AssertSingleton<ICollectionSelector>();
        }

        [Test]
        public void CollectionsSelectorIsDecorated()
        {
            AssertIsInstanceOf<ICollectionSelector, CollectionSelectorCachingProxy>();
        }
    }
}
