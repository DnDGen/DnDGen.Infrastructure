using DnDGen.Core.Tables;
using System.Collections.Generic;

namespace DnDGen.Core.Mappers.Percentiles
{
    internal class PercentileMapperCachingProxy : PercentileMapper
    {
        private readonly PercentileMapper innerMapper;
        private readonly AssemblyLoader assemblyLoader;
        private readonly Dictionary<string, Dictionary<int, string>> cachedTables;

        public PercentileMapperCachingProxy(PercentileMapper innerMapper, AssemblyLoader assemblyLoader)
        {
            this.innerMapper = innerMapper;
            this.assemblyLoader = assemblyLoader;

            cachedTables = new Dictionary<string, Dictionary<int, string>>();
        }

        public Dictionary<int, string> Map(string tableName)
        {
            var assembly = assemblyLoader.GetRunningAssembly();
            var key = assembly.FullName + tableName;

            if (!cachedTables.ContainsKey(key))
            {
                var mappedTable = innerMapper.Map(tableName);
                cachedTables.Add(key, mappedTable);
            }

            return cachedTables[key];
        }
    }
}