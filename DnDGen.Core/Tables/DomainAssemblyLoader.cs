using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace DnDGen.Core.Tables
{
    internal class DomainAssemblyLoader : AssemblyLoader
    {
        public Assembly GetRunningAssembly()
        {
            var stacktrace = new StackTrace();
            var frames = stacktrace.GetFrames();
            var assembly = frames
                .Select(f => f.GetMethod())
                .Select(m => m.ReflectedType.Assembly)
                .First(a => !a.FullName.StartsWith("DnDGen.Core"));

            return assembly;
        }
    }
}
