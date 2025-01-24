using DnDGen.Infrastructure.Models;
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
        public void CollectionSelectorIsInstantiatedAsSingleton()
        {
            AssertSingleton<ICollectionSelector>();
        }

        [Test]
        public void CollectionSelectorIsDecorated()
        {
            AssertIsInstanceOf<ICollectionSelector, CollectionSelectorCachingProxy>();
        }

        [Test]
        public void CollectionDataSelectorIsInstantiated_TypeAndAmountDataSelection()
        {
            AssertNotSingleton<ICollectionDataSelector<TypeAndAmountDataSelection>>();
        }

        [Test]
        public void PercentileDataSelectorIsInstantiated_TypeAndAmountDataSelection()
        {
            AssertNotSingleton<IPercentileDataSelector<TypeAndAmountDataSelection>>();
        }

        [Test]
        public void CollectionTypeAndAmountSelectorIsInstantiated()
        {
            AssertNotSingleton<ICollectionTypeAndAmountSelector>();
        }

        [Test]
        public void PercentileTypeAndAmountSelectorIsInstantiated()
        {
            AssertNotSingleton<IPercentileTypeAndAmountSelector>();
        }
    }
}
