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

        [TestCase("nunit.framework")]
        [TestCase("DnDGen.Infrastructure.Tests.Unit")]
        [TestCase("dndgen.infrastructure.tests.unit")]
        [TestCase("DNDGEN.INFRASTRUCTURE.TESTS.UNIT")]
        public void ReturnNamedAssembly(string assemblyName)
        {
            var assembly = assemblyLoader.GetAssembly(assemblyName);
            Assert.That(assembly.FullName, Does.StartWith($"{assemblyName}, "));
        }
    }
}