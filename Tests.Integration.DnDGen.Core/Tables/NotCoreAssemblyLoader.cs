using DnDGen.Core.Tables;
using Not.DnDGen.Core;
using System.Reflection;

namespace Tests.Integration.DnDGen.Core.Tables
{
    internal class NotCoreAssemblyLoader : AssemblyLoader
    {
        public Assembly GetRunningAssembly()
        {
            var type = typeof(CollectionsCaller);
            return type.Assembly;
        }
    }
}
