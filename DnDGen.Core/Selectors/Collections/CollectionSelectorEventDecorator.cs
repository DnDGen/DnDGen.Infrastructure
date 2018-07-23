using EventGen;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Core.Selectors.Collections
{
    internal class CollectionSelectorEventDecorator : ICollectionSelector
    {
        private readonly ICollectionSelector innerSelector;
        private readonly GenEventQueue eventQueue;

        public CollectionSelectorEventDecorator(ICollectionSelector innerSelector, GenEventQueue eventQueue)
        {
            this.innerSelector = innerSelector;
            this.eventQueue = eventQueue;
        }

        public IEnumerable<string> SelectFrom(string tableName, string collectionName)
        {
            var collection = innerSelector.SelectFrom(tableName, collectionName);

            return collection;
        }

        public Dictionary<string, IEnumerable<string>> SelectAllFrom(string tableName)
        {
            eventQueue.Enqueue("Core", $"Selecting all collections from {tableName}");
            var collections = innerSelector.SelectAllFrom(tableName);
            eventQueue.Enqueue("Core", $"Selected {collections.Count()} collections from {tableName}");

            return collections;
        }

        public string SelectRandomFrom(string tableName, string collectionName)
        {
            var entry = innerSelector.SelectRandomFrom(tableName, collectionName);

            return entry;
        }

        public T SelectRandomFrom<T>(IEnumerable<T> collection)
        {
            var entry = innerSelector.SelectRandomFrom(collection);

            return entry;
        }

        public string FindCollectionOf(string tableName, string entry, params string[] filteredCollectionNames)
        {
            eventQueue.Enqueue("Core", $"Finding collection to which {entry} belongs from {tableName}");
            var collectionName = innerSelector.FindCollectionOf(tableName, entry, filteredCollectionNames);
            eventQueue.Enqueue("Core", $"{entry} belongs to {collectionName}");

            return collectionName;
        }

        public bool IsCollection(string tableName, string collectionName)
        {
            var isCollection = innerSelector.IsCollection(tableName, collectionName);

            return isCollection;
        }

        public IEnumerable<string> Explode(string tableName, string collectionName)
        {
            eventQueue.Enqueue("Core", $"Exploding {collectionName} from {tableName}");
            var explodedCollection = innerSelector.Explode(tableName, collectionName);
            eventQueue.Enqueue("Core", $"Exploded {collectionName} into {explodedCollection.Count()} entries");

            return explodedCollection;
        }

        public IEnumerable<string> Flatten(Dictionary<string, IEnumerable<string>> collections, IEnumerable<string> keys)
        {
            eventQueue.Enqueue("Core", $"Flattening {collections.Count} collections with {keys.Count()} keys");
            var flattenedCollection = innerSelector.Flatten(collections, keys);
            eventQueue.Enqueue("Core", $"Flattened {collections.Count} collections into {flattenedCollection.Count()} entries");

            return flattenedCollection;
        }

        public IEnumerable<string> ExplodeAndPreserveDuplicates(string tableName, string collectionName)
        {
            eventQueue.Enqueue("Core", $"Exploding {collectionName} from {tableName}");
            var explodedCollection = innerSelector.ExplodeAndPreserveDuplicates(tableName, collectionName);
            eventQueue.Enqueue("Core", $"Exploded {collectionName} into {explodedCollection.Count()} entries");

            return explodedCollection;
        }

        public IEnumerable<T> CreateWeighted<T>(IEnumerable<T> common = null, IEnumerable<T> uncommon = null, IEnumerable<T> rare = null, IEnumerable<T> veryRare = null)
        {
            var weightedCollection = innerSelector.CreateWeighted(common, uncommon, rare, veryRare);

            return weightedCollection;
        }

        public T SelectRandomFrom<T>(IEnumerable<T> common = null, IEnumerable<T> uncommon = null, IEnumerable<T> rare = null, IEnumerable<T> veryRare = null)
        {
            var selected = innerSelector.SelectRandomFrom(common, uncommon, rare, veryRare);

            return selected;
        }
    }
}
