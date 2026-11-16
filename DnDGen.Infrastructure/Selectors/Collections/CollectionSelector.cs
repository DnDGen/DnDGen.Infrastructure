using DnDGen.Infrastructure.Mappers.Collections;
using DnDGen.RollGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    internal class CollectionSelector(CollectionMapper mapper, Dice dice) : ICollectionSelector
    {
        public IEnumerable<string> SelectFrom(string assemblyName, string tableName, string collectionName)
        {
            if (!IsCollection(assemblyName, tableName, collectionName))
                throw new ArgumentException($"{collectionName} is not a valid collection in the table {tableName}");

            var table = SelectAllFrom(assemblyName, tableName);
            return table[collectionName];
        }

        public Dictionary<string, IEnumerable<string>> SelectAllFrom(string assemblyName, string tableName)
        {
            return mapper.Map(assemblyName, tableName);
        }

        public string SelectRandomFrom(string assemblyName, string tableName, string collectionName)
        {
            var collection = SelectFrom(assemblyName, tableName, collectionName);
            return SelectRandomFrom(collection);
        }

        public T SelectRandomFrom<T>(IEnumerable<T> collection)
        {
            if (!collection.Any())
                throw new ArgumentException("Cannot select random from an empty collection");

            var array = collection.ToArray();
            var index = dice.Roll().d(array.Length).AsSum() - 1;
            return array[index];
        }

        public string FindCollectionOf(string assemblyName, string tableName, string entry, params string[] filteredCollectionNames)
        {
            var allCollections = SelectAllFrom(assemblyName, tableName);

            if (!allCollections.Any(kvp => kvp.Value.Contains(entry)))
                throw new ArgumentException($"No collection in {tableName} contains {entry}");

            var filteredCollections = allCollections.Where(kvp => filteredCollectionNames.Length == 0 || filteredCollectionNames.Contains(kvp.Key));

            if (!filteredCollections.Any(kvp => kvp.Value.Contains(entry)))
                throw new ArgumentException($"No collection from the {filteredCollectionNames.Length} filters in {tableName} contains {entry}");

            var collectionName = filteredCollections.First(kvp => kvp.Value.Contains(entry)).Key;

            return collectionName;
        }

        public bool IsCollection(string assemblyName, string tableName, string collectionName)
        {
            var table = SelectAllFrom(assemblyName, tableName);
            return table.ContainsKey(collectionName);
        }

        public T SelectRandomFrom<T>(IEnumerable<T> common = null, IEnumerable<T> uncommon = null, IEnumerable<T> rare = null, IEnumerable<T> veryRare = null)
        {
            common ??= [];
            uncommon ??= [];
            rare ??= [];
            veryRare ??= [];

            var roll = GetRandomRoll(common, uncommon, rare);
            var value = dice.Roll(roll).AsSum();

            if (value >= RollConstants.VeryRareThreshold && veryRare.Any())
                return SelectRandomFrom(veryRare);

            if (value >= RollConstants.RareThreshold && rare.Any())
                return SelectRandomFrom(rare);

            if (value >= RollConstants.UncommonThreshold && uncommon.Any())
                return SelectRandomFrom(uncommon);

            return SelectRandomFrom(common);
        }

        private static string GetRandomRoll<T>(IEnumerable<T> common, IEnumerable<T> uncommon, IEnumerable<T> rare)
        {
            if (common.Any())
                return RollConstants.CommonRoll;

            if (uncommon.Any())
                return RollConstants.UncommonRoll;

            if (rare.Any())
                return RollConstants.RareRoll;

            return RollConstants.VeryRareRoll;
        }
    }
}