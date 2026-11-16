using System.Collections.Generic;
using System.Threading;

namespace DnDGen.Infrastructure.Mappers.Percentiles
{
    internal class PercentileMapperCachingProxy(PercentileMapper innerMapper) : PercentileMapper
    {
        private readonly Dictionary<string, Dictionary<int, string>> cachedTables = [];
        private readonly Lock myLock = new();

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