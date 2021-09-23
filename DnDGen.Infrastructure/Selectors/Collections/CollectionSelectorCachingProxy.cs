using DnDGen.Infrastructure.Tables;
using System.Collections.Generic;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    internal class CollectionSelectorCachingProxy : ICollectionSelector
    {
        private readonly ICollectionSelector innerSelector;
        private readonly AssemblyLoader assemblyLoader;
        private readonly Dictionary<string, IEnumerable<string>> cachedExplodedCollections;
        private readonly Dictionary<string, IEnumerable<string>> cachedExplodedWithDuplicatesCollections;
        private readonly object myLock;

        public CollectionSelectorCachingProxy(ICollectionSelector innerSelector, AssemblyLoader assemblyLoader)
        {
            this.innerSelector = innerSelector;
            this.assemblyLoader = assemblyLoader;

            cachedExplodedCollections = new Dictionary<string, IEnumerable<string>>();
            cachedExplodedWithDuplicatesCollections = new Dictionary<string, IEnumerable<string>>();
            myLock = new object();
        }

        public IEnumerable<T> CreateWeighted<T>(IEnumerable<T> common = null, IEnumerable<T> uncommon = null, IEnumerable<T> rare = null, IEnumerable<T> veryRare = null)
        {
            return innerSelector.CreateWeighted(common, uncommon, rare, veryRare);
        }

        public IEnumerable<string> Explode(string tableName, string collectionName)
        {
            var assembly = assemblyLoader.GetRunningAssembly();
            var key = assembly.FullName + tableName + collectionName;

            lock (myLock)
            {
                if (!cachedExplodedCollections.ContainsKey(key))
                {
                    var explodedCollection = innerSelector.Explode(tableName, collectionName);
                    cachedExplodedCollections.Add(key, explodedCollection);
                }
            }

            return cachedExplodedCollections[key];
        }

        public IEnumerable<string> ExplodeAndPreserveDuplicates(string tableName, string collectionName)
        {
            var assembly = assemblyLoader.GetRunningAssembly();
            var key = assembly.FullName + tableName + collectionName;

            lock (myLock)
            {
                if (!cachedExplodedWithDuplicatesCollections.ContainsKey(key))
                {
                    var explodedCollection = innerSelector.ExplodeAndPreserveDuplicates(tableName, collectionName);
                    cachedExplodedWithDuplicatesCollections.Add(key, explodedCollection);
                }
            }

            return cachedExplodedWithDuplicatesCollections[key];
        }

        public string FindCollectionOf(string tableName, string entry, params string[] filteredCollectionNames)
        {
            return innerSelector.FindCollectionOf(tableName, entry, filteredCollectionNames);
        }

        public IEnumerable<string> Flatten(Dictionary<string, IEnumerable<string>> collections, IEnumerable<string> keys)
        {
            return innerSelector.Flatten(collections, keys);
        }

        public bool IsCollection(string tableName, string collectionName)
        {
            return innerSelector.IsCollection(tableName, collectionName);
        }

        public Dictionary<string, IEnumerable<string>> SelectAllFrom(string tableName)
        {
            return innerSelector.SelectAllFrom(tableName);
        }

        public IEnumerable<string> SelectFrom(string tableName, string collectionName)
        {
            return innerSelector.SelectFrom(tableName, collectionName);
        }

        public T SelectRandomFrom<T>(IEnumerable<T> collection)
        {
            return innerSelector.SelectRandomFrom(collection);
        }

        public string SelectRandomFrom(string tableName, string collectionName)
        {
            return innerSelector.SelectRandomFrom(tableName, collectionName);
        }

        public T SelectRandomFrom<T>(IEnumerable<T> common = null, IEnumerable<T> uncommon = null, IEnumerable<T> rare = null, IEnumerable<T> veryRare = null)
        {
            return innerSelector.SelectRandomFrom(common, uncommon, rare, veryRare);
        }
    }
}
