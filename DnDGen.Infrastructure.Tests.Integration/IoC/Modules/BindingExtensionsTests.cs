using DnDGen.Infrastructure.IoC.Modules;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.Infrastructure.Tests.Integration.Models;
using Ninject;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration.IoC.Modules
{
    [TestFixture]
    internal class BindingExtensionsTests : IoCTests
    {
        [Test]
        public void BindDataSelection_AddsPercentileDataSelector()
        {
            kernel.BindDataSelection<TestDataSelection>();
            var selector = kernel.Get<IPercentileDataSelector<TestDataSelection>>();
            Assert.That(selector, Is.Not.Null.And.InstanceOf<PercentileDataSelector<TestDataSelection>>());
        }

        [Test]
        public void BindDataSelection_AddsCollectionDataSelector()
        {
            kernel.BindDataSelection<TestDataSelection>();
            var selector = kernel.Get<ICollectionDataSelector<TestDataSelection>>();
            Assert.That(selector, Is.Not.Null.And.InstanceOf<CollectionDataSelector<TestDataSelection>>());
        }

        [Test]
        public void BindDataSelection_AddsAllDataSelectors()
        {
            kernel.BindDataSelection<TestDataSelection>();
            var percentile = kernel.Get<IPercentileDataSelector<TestDataSelection>>();
            var collection = kernel.Get<ICollectionDataSelector<TestDataSelection>>();
            Assert.That(percentile, Is.Not.Null.And.InstanceOf<PercentileDataSelector<TestDataSelection>>());
            Assert.That(collection, Is.Not.Null.And.InstanceOf<CollectionDataSelector<TestDataSelection>>());
        }

        [Test]
        public void BindDataSelection_HandlesRedundantCalls()
        {
            kernel.BindDataSelection<TestDataSelection>();
            kernel.BindDataSelection<TestDataSelection>();

            var percentile = kernel.Get<IPercentileDataSelector<TestDataSelection>>();
            var collection = kernel.Get<ICollectionDataSelector<TestDataSelection>>();
            Assert.That(percentile, Is.Not.Null.And.InstanceOf<PercentileDataSelector<TestDataSelection>>());
            Assert.That(collection, Is.Not.Null.And.InstanceOf<CollectionDataSelector<TestDataSelection>>());
        }

        [Test]
        public void BindDataSelection_AddsAllDataSelectors_MultipleDataSelections()
        {
            kernel.BindDataSelection<TestDataSelection>();
            kernel.BindDataSelection<OtherTestDataSelection>();

            var percentile = kernel.Get<IPercentileDataSelector<TestDataSelection>>();
            var collection = kernel.Get<ICollectionDataSelector<TestDataSelection>>();
            var otherPercentile = kernel.Get<IPercentileDataSelector<OtherTestDataSelection>>();
            var otherCollection = kernel.Get<ICollectionDataSelector<OtherTestDataSelection>>();

            Assert.That(percentile, Is.Not.Null.And.InstanceOf<PercentileDataSelector<TestDataSelection>>());
            Assert.That(otherPercentile, Is.Not.Null.And.InstanceOf<PercentileDataSelector<OtherTestDataSelection>>());
            Assert.That(percentile, Is.Not.SameAs(otherPercentile));

            Assert.That(collection, Is.Not.Null.And.InstanceOf<CollectionDataSelector<TestDataSelection>>());
            Assert.That(otherCollection, Is.Not.Null.And.InstanceOf<CollectionDataSelector<OtherTestDataSelection>>());
            Assert.That(collection, Is.Not.SameAs(otherCollection));
        }
    }
}
