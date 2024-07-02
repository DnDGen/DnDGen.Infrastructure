using System.Reflection;

namespace DnDGen.Infrastructure.Tables
{
    public interface AssemblyLoader
    {
        Assembly GetAssembly(string name);
    }
}
