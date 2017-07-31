using EventGen;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Core.Selectors.Collections
{
    internal class CollectionsSelectorEventDecorator : ICollectionsSelector
    {
        private readonly ICollectionsSelector innerSelector;
        private readonly GenEventQueue eventQueue;

        public CollectionsSelectorEventDecorator(ICollectionsSelector innerSelector, GenEventQueue eventQueue)
        {
            this.innerSelector = innerSelector;
            this.eventQueue = eventQueue;
        }

        public IEnumerable<string> SelectFrom(string tableName, string collectionName)
        {
            eventQueue.Enqueue("Core", $"Selecting {collectionName} from {tableName}");
            var collection = innerSelector.SelectFrom(tableName, collectionName);
            eventQueue.Enqueue("Core", $"Selected {collectionName} with {collection.Count()} entries");

            return collection;
        }

        public Dictionary<string, IEnumerable<string>> SelectAllFrom(string tableName)
        {
            eventQueue.Enqueue("Core", $"Selecting all from {tableName}");
            var collections = innerSelector.SelectAllFrom(tableName);
            eventQueue.Enqueue("Core", $"Selected {collections.Count()} collections from {tableName}");

            return collections;
        }

        public string SelectRandomFrom(string tableName, string collectionName)
        {
            eventQueue.Enqueue("Core", $"Selecting a random entry in {collectionName} from {tableName}");
            var entry = innerSelector.SelectRandomFrom(tableName, collectionName);
            eventQueue.Enqueue("Core", $"Selected {entry}");

            return entry;
        }

        public T SelectRandomFrom<T>(IEnumerable<T> collection)
        {
            eventQueue.Enqueue("Core", $"Selecting a random entry from {collection.Count()} entries");
            var entry = innerSelector.SelectRandomFrom(collection);
            eventQueue.Enqueue("Core", $"Selected {entry}");

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
            eventQueue.Enqueue("Core", $"Determining if {collectionName} is a collection in {tableName}");
            var isCollection = innerSelector.IsCollection(tableName, collectionName);

            if (isCollection)
                eventQueue.Enqueue("Core", $"{collectionName} is a collection in {tableName}");
            else
                eventQueue.Enqueue("Core", $"{collectionName} is not a collection in {tableName}");

            return isCollection;
        }

        public IEnumerable<string> Explode(string tableName, string collectionName)
        {
            eventQueue.Enqueue("Core", $"Exploding {collectionName} from {tableName}");
            var explodedCollection = innerSelector.Explode(tableName, collectionName);
            eventQueue.Enqueue("Core", $"Exploded {collectionName} into {explodedCollection.Count()} entries");

            return explodedCollection;
        }

        public IEnumerable<string> ExplodeInto(string tableName, string collectionName, string intoTableName)
        {
            eventQueue.Enqueue("Core", $"Exploding {collectionName} from {tableName} into {intoTableName}");
            var explodedCollection = innerSelector.ExplodeInto(tableName, collectionName, intoTableName);
            eventQueue.Enqueue("Core", $"Exploded {collectionName} into {explodedCollection.Count()} entries");

            return explodedCollection;
        }
    }
}
