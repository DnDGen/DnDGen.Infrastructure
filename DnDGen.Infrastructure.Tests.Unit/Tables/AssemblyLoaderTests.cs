using DnDGen.Infrastructure.Another;
using DnDGen.Infrastructure.Other;
using DnDGen.Infrastructure.Tables;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Unit.Tables
{
    [TestFixture]
    public class AssemblyLoaderTests
    {
        private OtherAssemblyCaller caller;
        private AssemblyLoader assemblyLoader;

        [SetUp]
        public void Setup()
        {
            assemblyLoader = new DomainAssemblyLoader();
            caller = new OtherAssemblyCaller(assemblyLoader);
        }

        [Test]
        public void GetsNonDndGenInfrastructureAssembly()
        {
            var assembly = caller.Call();
            Assert.That(assembly.FullName, Does.StartWith("DnDGen.Infrastructure.Other, "));
        }

        [Test]
        public void GetsNonDndGenInfrastructureAssemblyThroughMultipleInfrastructureLayers()
        {
            var extraLoader = new TestAssemblyLoader(assemblyLoader);
            caller = new OtherAssemblyCaller(extraLoader);

            var assembly = caller.Call();
            Assert.That(assembly.FullName, Does.StartWith("DnDGen.Infrastructure.Other, "));
        }

        [Test]
        public void GetsFirstNonDndGenInfrastructureAssembly()
        {
            var otherCaller = new AnotherAssemblyCaller(caller);
            var assembly = caller.Call();
            Assert.That(assembly.FullName, Does.StartWith("DnDGen.Infrastructure.Other, "));
        }

        [Test]
        public void ReturnAssemblyEvenIfDirectlyCalled()
        {
            var assembly = assemblyLoader.GetRunningAssembly();
            Assert.That(assembly.FullName, Does.StartWith("nunit.framework, "));
        }
    }
}