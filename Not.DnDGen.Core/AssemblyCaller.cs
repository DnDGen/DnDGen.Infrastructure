using DnDGen.Core.Tables;
using System.Reflection;

namespace Not.DnDGen.Core
{
    internal class AssemblyCaller
    {
        private readonly AssemblyLoader loader;

        public AssemblyCaller(AssemblyLoader loader)
        {
            this.loader = loader;
        }

        public Assembly Call()
        {
            return loader.GetRunningAssembly();
        }
    }
}
