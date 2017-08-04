using DnDGen.Core.Generators;
using NUnit.Framework;

namespace DnDGen.Core.Tests.IoC.Modules
{
    [TestFixture]
    public class GeneratorsModuleTests : IoCTests
    {
        [Test]
        public void IterativeGeneratorIsNotInstantiatedAsSingleton()
        {
            AssertNotSingleton<Generator>();
        }

        [Test]
        public void JustInTimeFactoryIsNotInstantiatedAsSingleton()
        {
            AssertNotSingleton<JustInTimeFactory>();
        }
    }
}
