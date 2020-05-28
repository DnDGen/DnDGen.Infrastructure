using DnDGen.Infrastructure.Tables;
using System.Collections.Generic;

namespace DnDGen.Infrastructure.Mappers.Collections
{
    internal class CollectionMapperCachingProxy : CollectionMapper
    {
        private readonly CollectionMapper innerMapper;
        private readonly Dictionary<string, Dictionary<string, IEnumerable<string>>> cachedTables;
        private readonly AssemblyLoader assemblyLoader;
        private readonly object myLock;

        public CollectionMapperCachingProxy(CollectionMapper innerMapper, AssemblyLoader assemblyLoader)
        {
            this.innerMapper = innerMapper;
            this.assemblyLoader = assemblyLoader;

            cachedTables = new Dictionary<string, Dictionary<string, IEnumerable<string>>>();
            myLock = new object();
        }

        public Dictionary<string, IEnumerable<string>> Map(string tableName)
        {
            var assembly = assemblyLoader.GetRunningAssembly();
            var key = assembly.FullName + tableName;

            lock (myLock)
            {
                if (!cachedTables.ContainsKey(key))
                {
                    var mappedTable = innerMapper.Map(tableName);
                    cachedTables.Add(key, mappedTable);
                }
            }

            return cachedTables[key];
        }
    }
}