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

        public Stream LoadFor(string filename)
        {
            if (!filename.EndsWith(".xml"))
            {
                var message = string.Format("\"{0}\" is not a valid XML file", filename);
                throw new ArgumentException(message);
            }

            var assembly = assemblyLoader.GetRunningAssembly();
            var resources = assembly.GetManifestResourceNames();
            var fileNames = resources.Select(r => GetFileName(r));

            if (!fileNames.Contains(filename))
                throw new FileNotFoundException($"{filename} does not exist in {assembly.FullName}");

            var streamSource = resources.Single(r => r.EndsWith("." + filename));
            return assembly.GetManifestResourceStream(streamSource);
        }

        private string GetFileName(string resource)
        {
            var segments = resource.Split('.');
            var lastIndex = segments.Length - 1;
            var secondToLastIndex = lastIndex - 1;

            var fileName = segments[secondToLastIndex];
            var extension = segments[lastIndex];

            return $"{fileName}.{extension}";
        }
    }
}