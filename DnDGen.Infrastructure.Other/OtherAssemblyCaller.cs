using DnDGen.Infrastructure.Tables;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Tests.Unit")]
[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Another")]
namespace DnDGen.Infrastructure.Other
{
    internal class OtherAssemblyCaller
    {
        private readonly AssemblyLoader loader;

        public OtherAssemblyCaller(AssemblyLoader loader)
        {
            this.loader = loader;
        }

        public Assembly Call()
        {
            return loader.GetRunningAssembly();
        }
    }
}
