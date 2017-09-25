using DnDGen.Core.Mappers.Collections;
using RollGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Core.Selectors.Collections
{
    internal class CollectionsSelector : ICollectionsSelector
    {
        private readonly CollectionsMapper mapper;
        private readonly Dice dice;

        public CollectionsSelector(CollectionsMapper mapper, Dice dice)
        {
            this.mapper = mapper;
            this.dice = dice;
        }

        public IEnumerable<string> SelectFrom(string tableName, string collectionName)
        {
            var table = SelectAllFrom(tableName);

            if (!IsCollection(tableName, collectionName))
                throw new ArgumentException($"{collectionName} is not a valid collection in the table {tableName}");

            return table[collectionName];
        }

        public Dictionary<string, IEnumerable<string>> SelectAllFrom(string tableName)
        {
            return mapper.Map(tableName);
        }

        public string SelectRandomFrom(string tableName, string collectionName)
        {
            var collection = SelectFrom(tableName, collectionName);
            return SelectRandomFrom(collection);
        }

        public T SelectRandomFrom<T>(IEnumerable<T> collection)
        {
            if (!collection.Any())
                throw new ArgumentException("Cannot select random from an empty collection");

            var count = collection.Count();
            var index = dice.Roll().d(count).AsSum() - 1;
            return collection.ElementAt(index);
        }

        public string FindCollectionOf(string tableName, string entry, params string[] filteredCollectionNames)
        {
            var allCollections = SelectAllFrom(tableName);

            if (!allCollections.Any(kvp => kvp.Value.Contains(entry)))
                throw new ArgumentException($"No collection in {tableName} contains {entry}");

            var filteredCollections = allCollections.Where(kvp => !filteredCollectionNames.Any() || filteredCollectionNames.Contains(kvp.Key));

            if (!filteredCollections.Any(kvp => kvp.Value.Contains(entry)))
                throw new ArgumentException($"No collection from the {filteredCollectionNames.Count()} filters in {tableName} contains {entry}");

            var collectionName = filteredCollections.First(kvp => kvp.Value.Contains(entry)).Key;

            return collectionName;
        }

        public bool IsCollection(string tableName, string collectionName)
        {
            var table = SelectAllFrom(tableName);
            return table.ContainsKey(collectionName);
        }

        public IEnumerable<string> Explode(string tableName, string collectionName)
        {
            var explodedCollection = SelectFrom(tableName, collectionName).ToList();
            var subCollectionNames = explodedCollection.Where(i => IsCollection(tableName, i) && i != collectionName)
                .ToArray(); //INFO: Doing immediate execution because looping below fails otherwise (modifying the source collection)

            foreach (var subCollectionName in subCollectionNames)
            {
                var explodedSubCollection = Explode(tableName, subCollectionName);
                explodedCollection.Remove(subCollectionName);
                explodedCollection.AddRange(explodedSubCollection);
            }

            return explodedCollection.Distinct();
        }

        public IEnumerable<string> ExplodeInto(string tableName, string collectionName, string intoTableName)
        {
            var explodedCollection = Explode(tableName, collectionName);
            explodedCollection = explodedCollection.SelectMany(g => SelectFrom(intoTableName, g)).Distinct();

            //INFO: Doing immediate execution to avoid assembly reference issues that may bubble up
            return explodedCollection.ToArray();
        }
    }
}