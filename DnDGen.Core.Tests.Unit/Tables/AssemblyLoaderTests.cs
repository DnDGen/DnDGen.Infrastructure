using Also.Not.DnDGen.Core;
using DnDGen.Core.Tables;
using Not.DnDGen.Core;
using NUnit.Framework;

namespace DnDGen.Core.Tests.Unit.Tables
{
    [TestFixture]
    public class AssemblyLoaderTests
    {
        private AssemblyCaller caller;
        private AssemblyLoader assemblyLoader;

        [SetUp]
        public void Setup()
        {
            assemblyLoader = new DomainAssemblyLoader();
            caller = new AssemblyCaller(assemblyLoader);
        }

        [Test]
        public void GetsNonDndGenCoreAssembly()
        {
            var assembly = caller.Call();
            Assert.That(assembly.FullName, Does.StartWith("Not.DnDGen.Core"));
        }

        [Test]
        public void GetsNonDndGenCoreAssemblyThroughMultipleCoreLayers()
        {
            var extraLoader = new TestAssemblyLoader(assemblyLoader);
            caller = new AssemblyCaller(extraLoader);

            var assembly = caller.Call();
            Assert.That(assembly.FullName, Does.StartWith("Not.DnDGen.Core"));
        }

        [Test]
        public void GetsFirstNonDndGenCoreAssembly()
        {
            var otherCaller = new OtherAssemblyCaller(caller);
            var assembly = caller.Call();
            Assert.That(assembly.FullName, Does.StartWith("Not.DnDGen.Core"));
        }

        [Test]
        public void ReturnAssemblyEvenIfDirectlyCalled()
        {
            var assembly = assemblyLoader.GetRunningAssembly();
            Assert.That(assembly.FullName, Does.StartWith("mscorlib"));
        }
    }
}