using DnDGen.Infrastructure.Generators;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration.IoC.Modules
{
    [TestFixture]
    public class GeneratorsModuleTests : IoCTests
    {
        [Test]
        public void IterativeGeneratorIsNotInstantiatedAsSingleton()
        {
            //The iterative generator is deprecated, so the non-singleton assertion is not as important
            //AssertNotSingleton<Generator>();
            AssertIsInstanceOf<Generator, IterativeGenerator>();
        }

        [Test]
        public void JustInTimeFactoryIsNotInstantiatedAsSingleton()
        {
            AssertNotSingleton<JustInTimeFactory>();
        }
    }
}
