using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.Infrastructure.Tests.Integration.Models;
using Ninject;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration.IoC.Modules
{
    [TestFixture]
    public class SelectorsModuleTests : IoCTests
    {
        [OneTimeSetUp]
        public void SelectorsSetup()
        {
            kernel.Load<TestSelectorsModule>();
        }

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
            AssertNotSingleton<ICollectionDataSelector<TestDataSelection>>();
        }

        [Test]
        public void CollectionDataSelectorIsInstantiated_OtherTestDataSelection()
        {
            AssertNotSingleton<ICollectionDataSelector<OtherTestDataSelection>>();
        }

        [Test]
        public void PercentileDataSelectorIsInstantiated_TestDataSelection()
        {
            AssertNotSingleton<IPercentileDataSelector<TestDataSelection>>();
        }

        [Test]
        public void PercentileDataSelectorIsInstantiated_OtherTestDataSelection()
        {
            AssertNotSingleton<IPercentileDataSelector<OtherTestDataSelection>>();
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
