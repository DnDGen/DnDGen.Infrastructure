using System.Collections.Generic;

namespace DnDGen.Infrastructure.Mappers.Collections
{
    internal class CollectionMapperCachingProxy : CollectionMapper
    {
        private readonly CollectionMapper innerMapper;
        private readonly Dictionary<string, Dictionary<string, IEnumerable<string>>> cachedTables;
        private readonly object myLock;

        public CollectionMapperCachingProxy(CollectionMapper innerMapper)
        {
            this.innerMapper = innerMapper;

            cachedTables = new Dictionary<string, Dictionary<string, IEnumerable<string>>>();
            myLock = new object();
        }

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