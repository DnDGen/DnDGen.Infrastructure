using DnDGen.Core.Tables;
using Not.DnDGen.Core;
using System.Reflection;

namespace DnDGen.Core.Tests.Tables
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
