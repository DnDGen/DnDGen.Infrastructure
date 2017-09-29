using DnDGen.Core.Tables;
using System.Reflection;

namespace DnDGen.Core.Tests.Unit.Tables
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
