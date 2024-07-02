using System.Reflection;

namespace DnDGen.Infrastructure.Tables
{
    internal class DomainAssemblyLoader : AssemblyLoader
    {
        public Assembly GetAssembly(string name)
        {
            return Assembly.Load(name);
        }
    }
}
