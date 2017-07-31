using System.Reflection;

namespace DnDGen.Core.Tables
{
    //INFO: No implementations will exist in this project.  Projects using DnDGen.Core will have to implement this interface
    public interface AssemblyLoader
    {
        Assembly GetRunningAssembly();
    }
}
