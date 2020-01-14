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
            AssertNotSingleton<Generator>();
        }

        [Test]
        public void JustInTimeFactoryIsNotInstantiatedAsSingleton()
        {
            AssertNotSingleton<JustInTimeFactory>();
        }
    }
}
