using DnDGen.Core.Selectors.Collections;
using DnDGen.Core.Selectors.Percentiles;
using NUnit.Framework;

namespace DnDGen.Core.Tests.IoC.Modules
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
        public void PercentileSelectorIsDecorated()
        {
            AssertIsInstanceOf<IPercentileSelector, PercentileSelectorEventDecorator>();
        }

        [Test]
        public void CollectionsSelectorIsNotInstantiatedAsSingleton()
        {
            AssertNotSingleton<ICollectionsSelector>();
        }

        [Test]
        public void CollectionsSelectorIsDecorated()
        {
            AssertIsInstanceOf<ICollectionsSelector, CollectionsSelectorEventDecorator>();
        }
    }
}
