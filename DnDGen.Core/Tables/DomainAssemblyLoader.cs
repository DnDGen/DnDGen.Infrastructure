using System.Diagnostics;
using System.Reflection;

namespace DnDGen.Core.Tables
{
    internal class DomainAssemblyLoader : AssemblyLoader
    {
        public Assembly GetRunningAssembly()
        {
            var stacktrace = new StackTrace();
            var frames = stacktrace.GetFrames();
            var frameIndex = 0;
            var assemblyName = "DnDGen.Core";
            var assembly = Assembly.GetExecutingAssembly();

            while (assemblyName.StartsWith("DnDGen.Core") && frameIndex++ < frames.Length)
            {
                var frame = frames[frameIndex];
                var method = frame.GetMethod();

                assemblyName = method.ReflectedType.AssemblyQualifiedName;
                assembly = method.ReflectedType.Assembly;
            }

            return assembly;
        }
    }
}
