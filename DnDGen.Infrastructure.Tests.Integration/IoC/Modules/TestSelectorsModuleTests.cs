using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.Infrastructure.Tests.Integration.Models;
using Ninject;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration.IoC.Modules
{
    [TestFixture]
    internal class TestSelectorsModuleTests : IoCTests
    {
        [OneTimeSetUp]
        public void SelectorsSetup()
        {
            kernel.Load<TestSelectorsModule>();
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
        public void CollectionDataSelectorIsInstantiated_IncrementingDataSelection()
        {
            AssertNotSingleton<ICollectionDataSelector<IncrementingDataSelection>>();
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
        public void PercentileDataSelectorIsInstantiated_IncrementingDataSelection()
        {
            AssertNotSingleton<IPercentileDataSelector<IncrementingDataSelection>>();
        }
    }
}
