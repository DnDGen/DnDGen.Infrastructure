using DnDGen.Infrastructure.Tables;
using System.Reflection;

namespace DnDGen.Infrastructure.Tests.Unit.Tables
{
    internal class TestAssemblyLoader : AssemblyLoader
    {
        private readonly AssemblyLoader innerLoader;

        public TestAssemblyLoader(AssemblyLoader innerLoader)
        {
            this.innerLoader = innerLoader;
        }

        public Assembly GetRunningAssembly()
        {
            return innerLoader.GetRunningAssembly();
        }
    }
}
