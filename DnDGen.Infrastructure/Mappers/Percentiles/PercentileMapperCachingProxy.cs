using System.Collections.Generic;

namespace DnDGen.Infrastructure.Mappers.Percentiles
{
    internal class PercentileMapperCachingProxy : PercentileMapper
    {
        private readonly PercentileMapper innerMapper;
        private readonly Dictionary<string, Dictionary<int, string>> cachedTables;
        private readonly object myLock;

        public PercentileMapperCachingProxy(PercentileMapper innerMapper)
        {
            this.innerMapper = innerMapper;

            cachedTables = new Dictionary<string, Dictionary<int, string>>();
            myLock = new object();
        }

        public Dictionary<int, string> Map(string assemblyName, string tableName)
        {
            var key = assemblyName + tableName;

            lock (myLock)
            {
                if (!cachedTables.ContainsKey(key))
                {
                    var mappedTable = innerMapper.Map(assemblyName, tableName);
                    cachedTables.Add(key, mappedTable);
                }
            }

            return cachedTables[key];
        }
    }
}