using System.Collections.Generic;

namespace DnDGen.Core.Mappers.Percentiles
{
    internal class PercentileMapperCachingProxy : PercentileMapper
    {
        private PercentileMapper innerMapper;
        private Dictionary<string, Dictionary<int, string>> cachedTables;

        public PercentileMapperCachingProxy(PercentileMapper innerMapper)
        {
            this.innerMapper = innerMapper;
            cachedTables = new Dictionary<string, Dictionary<int, string>>();
        }

        public Dictionary<int, string> Map(string tableName)
        {
            if (!cachedTables.ContainsKey(tableName))
            {
                var mappedTable = innerMapper.Map(tableName);
                cachedTables.Add(tableName, mappedTable);
            }

            return cachedTables[tableName];
        }
    }
}