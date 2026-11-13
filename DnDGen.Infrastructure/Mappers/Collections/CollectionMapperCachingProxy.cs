using System.Collections.Generic;
using System.Threading;

namespace DnDGen.Infrastructure.Mappers.Collections
{
    internal class CollectionMapperCachingProxy(CollectionMapper innerMapper) : CollectionMapper
    {
        private readonly Dictionary<string, Dictionary<string, IEnumerable<string>>> cachedTables = [];
        private readonly Lock myLock = new();

        public Dictionary<string, IEnumerable<string>> Map(string assemblyName, string tableName)
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