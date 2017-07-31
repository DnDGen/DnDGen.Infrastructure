using System.Collections.Generic;

namespace DnDGen.Core.Mappers.Collections
{
    internal class CollectionsMapperCachingProxy : CollectionsMapper
    {
        private CollectionsMapper innerMapper;
        private Dictionary<string, Dictionary<string, IEnumerable<string>>> cachedTables;

        public CollectionsMapperCachingProxy(CollectionsMapper innerMapper)
        {
            this.innerMapper = innerMapper;
            cachedTables = new Dictionary<string, Dictionary<string, IEnumerable<string>>>();
        }

        public Dictionary<string, IEnumerable<string>> Map(string tableName)
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