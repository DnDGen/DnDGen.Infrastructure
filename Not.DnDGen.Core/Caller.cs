using DnDGen.Core.Tables;
using System.Reflection;

namespace Not.DnDGen.Core
{
    internal class Caller
    {
        private readonly AssemblyLoader loader;

        public Caller(AssemblyLoader loader)
        {
            this.loader = loader;
        }

        public Assembly Call()
        {
            return loader.GetRunningAssembly();
        }
    }
}
