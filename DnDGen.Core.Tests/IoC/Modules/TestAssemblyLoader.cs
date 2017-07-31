using DnDGen.Core.Tables;
using System.Reflection;

namespace DnDGen.Core.Tests.IoC.Modules
{
    internal class TestAssemblyLoader : AssemblyLoader
    {
        public Assembly GetRunningAssembly()
        {
            return Assembly.GetExecutingAssembly();
        }
    }
}