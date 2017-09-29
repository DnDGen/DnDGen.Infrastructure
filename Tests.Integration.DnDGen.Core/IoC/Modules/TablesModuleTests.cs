using DnDGen.Core.Tables;
using NUnit.Framework;

namespace Tests.Integration.DnDGen.Core.IoC.Modules
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
