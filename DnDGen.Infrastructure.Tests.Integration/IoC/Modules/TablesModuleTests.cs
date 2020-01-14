using DnDGen.Infrastructure.Tables;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration.IoC.Modules
{
    [TestFixture]
    public class TablesModuleTests : IoCTests
    {
        [Test]
        public void StreamLoaderIsNotInstantiatedAsSingleton()
        {
            AssertNotSingleton<StreamLoader>();
        }

        [Test]
        public void AssemblyLoaderIsNotInstantiatedAsSingleton()
        {
            AssertNotSingleton<AssemblyLoader>();
        }
    }
}
