using System.Reflection;

namespace DnDGen.Core.Tables
{
    internal interface AssemblyLoader
    {
        Assembly GetRunningAssembly();
    }
}
