using System.Reflection;

namespace DnDGen.Core.Tables
{
    public interface AssemblyLoader
    {
        Assembly GetRunningAssembly();
    }
}
