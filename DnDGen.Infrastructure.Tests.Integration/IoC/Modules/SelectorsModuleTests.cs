using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.Infrastructure.Tests.Integration.Models;
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
        public void CollectionDataSelectorIsInstantiated_TestDataSelection()
        {
            kernel.Bind<ICollectionDataSelector<TestDataSelection>>().To<CollectionDataSelector<TestDataSelection>>();
            AssertNotSingleton<ICollectionDataSelector<TestDataSelection>>();
        }

        [Test]
        public void CollectionDataSelectorIsInstantiated_OtherTestDataSelection()
        {
            kernel.Bind<ICollectionDataSelector<OtherTestDataSelection>>().To<CollectionDataSelector<OtherTestDataSelection>>();
            AssertNotSingleton<ICollectionDataSelector<OtherTestDataSelection>>();
        }
    }
}
