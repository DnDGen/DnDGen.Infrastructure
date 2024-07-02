using DnDGen.Infrastructure.Tables;
using NUnit.Framework;
using System.IO;

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

        [TestCase("NUnit.Framework", "nunit.framework")]
        [TestCase("nunit.framework", "nunit.framework")]
        [TestCase("DnDGen.Infrastructure.Tests.Unit", "DnDGen.Infrastructure.Tests.Unit")]
        [TestCase("dndgen.infrastructure.tests.unit", "DnDGen.Infrastructure.Tests.Unit")]
        [TestCase("DNDGEN.INFRASTRUCTURE.TESTS.UNIT", "DnDGen.Infrastructure.Tests.Unit")]
        [TestCase("DnDGen.Infrastructure", "DnDGen.Infrastructure")]
        [TestCase("dndgen.infrastructure", "DnDGen.Infrastructure")]
        [TestCase("DnDGen.Infrastructure.Other", "DnDGen.Infrastructure.Other")]
        [TestCase("DnDGen.Infrastructure.Another", "DnDGen.Infrastructure.Another")]
        [TestCase("DnDGen.Infrastructure.YetAnother", "DnDGen.Infrastructure.YetAnother")]
        [TestCase("DnDGen.RollGen", "DnDGen.RollGen")]
        [TestCase("dndgen.rollgen", "DnDGen.RollGen")]
        public void ReturnNamedAssembly(string assemblyName, string expected)
        {
            var assembly = assemblyLoader.GetAssembly(assemblyName);
            Assert.That(assembly.FullName, Does.StartWith($"{expected}, "));
        }

        [Test]
        public void ThrowsError_IfAssemblyNotFound()
        {
            Assert.That(() => assemblyLoader.GetAssembly("dndgen.infrastructure.wrong"),
                Throws.InstanceOf<FileNotFoundException>());
        }
    }
}