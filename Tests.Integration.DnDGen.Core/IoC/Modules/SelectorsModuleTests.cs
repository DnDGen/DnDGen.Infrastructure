using DnDGen.Core.Selectors.Collections;
using DnDGen.Core.Selectors.Percentiles;
using NUnit.Framework;

namespace Tests.Integration.DnDGen.Core.IoC.Modules
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
        public void CollectionsSelectorIsNotInstantiatedAsSingleton()
        {
            AssertNotSingleton<ICollectionSelector>();
        }

        [Test]
        public void CollectionsSelectorIsDecorated()
        {
            AssertIsInstanceOf<ICollectionSelector, CollectionSelectorEventDecorator>();
        }
    }
}
