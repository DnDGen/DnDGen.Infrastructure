using System;
using System.IO;
using System.Linq;

namespace DnDGen.Infrastructure.Tables
{
    internal class EmbeddedResourceStreamLoader : StreamLoader
    {
        private readonly AssemblyLoader assemblyLoader;

        public EmbeddedResourceStreamLoader(AssemblyLoader assemblyLoader)
        {
            this.assemblyLoader = assemblyLoader;
        }

        public Stream LoadFor(string assemblyName, string filename)
        {
            if (!filename.EndsWith(".xml"))
            {
                throw new ArgumentException($"{filename} is not a valid XML file");
            }

            var assembly = assemblyLoader.GetAssembly(assemblyName);
            var resources = assembly.GetManifestResourceNames();
            var streamSources = resources.Where(r => r.EndsWith($".{filename}"));

            if (!streamSources.Any())
                throw new FileNotFoundException($"{filename} does not exist in {assembly.FullName}");

            var streamSource = streamSources.Single();
            return assembly.GetManifestResourceStream(streamSource);
        }
    }
}