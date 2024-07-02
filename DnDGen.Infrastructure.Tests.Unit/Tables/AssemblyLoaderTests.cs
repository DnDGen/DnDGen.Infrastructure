using DnDGen.Infrastructure.Tables;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Unit.Tables
{
    [TestFixture]
    public class AssemblyLoaderTests
    {
        private AssemblyLoader assemblyLoader;

        [SetUp]
        public void Setup()
        {
            assemblyLoader = new DomainAssemblyLoader();
        }

        [TestCase("nunit.framework", "nunit.framework")]
        [TestCase("DnDGen.Infrastructure.Tests.Unit", "DnDGen.Infrastructure.Tests.Unit")]
        [TestCase("dndgen.infrastructure.tests.unit", "DnDGen.Infrastructure.Tests.Unit")]
        [TestCase("DNDGEN.INFRASTRUCTURE.TESTS.UNIT", "DnDGen.Infrastructure.Tests.Unit")]
        [TestCase("DnDGen.Infrastructure", "DnDGen.Infrastructure")]
        public void ReturnNamedAssembly(string assemblyName, string expected)
        {
            var assembly = assemblyLoader.GetAssembly(assemblyName);
            Assert.That(assembly.FullName, Does.StartWith($"{expected}, "));
        }
    }
}