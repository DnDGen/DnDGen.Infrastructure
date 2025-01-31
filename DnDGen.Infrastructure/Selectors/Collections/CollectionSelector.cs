using DnDGen.Infrastructure.Mappers.Collections;
using DnDGen.RollGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    internal class CollectionSelector : ICollectionSelector
    {
        private readonly CollectionMapper mapper;
        private readonly Dice dice;
        private readonly string commonRoll;
        private readonly string uncommonRoll;
        private readonly string rareRoll;
        private readonly string veryRareRoll;

        private const int commonThreshold = 1;
        private const int uncommonThreshold = 61;
        private const int rareThreshold = 91;
        private const int veryRareThreshold = 100;

        public CollectionSelector(CollectionMapper mapper, Dice dice)
        {
            this.mapper = mapper;
            this.dice = dice;

            commonRoll = RollHelper.GetRollWithMostEvenDistribution(commonThreshold, 100, true);
            uncommonRoll = RollHelper.GetRollWithMostEvenDistribution(uncommonThreshold, 100, true);
            rareRoll = RollHelper.GetRollWithMostEvenDistribution(rareThreshold, 100, true);
            veryRareRoll = RollHelper.GetRollWithMostEvenDistribution(veryRareThreshold, 100, true);
        }

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

            var filteredCollections = allCollections.Where(kvp => !filteredCollectionNames.Any() || filteredCollectionNames.Contains(kvp.Key));

            if (!filteredCollections.Any(kvp => kvp.Value.Contains(entry)))
                throw new ArgumentException($"No collection from the {filteredCollectionNames.Count()} filters in {tableName} contains {entry}");

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

            var roll = GetRandomRoll(common, uncommon, rare, veryRare);
            var value = dice.Roll(roll).AsSum();

            if (value >= veryRareThreshold && veryRare.Any())
                return SelectRandomFrom(veryRare);

            if (value >= rareThreshold && rare.Any())
                return SelectRandomFrom(rare);

            if (value >= uncommonThreshold && uncommon.Any())
                return SelectRandomFrom(uncommon);

            return SelectRandomFrom(common);
        }

        private string GetRandomRoll<T>(IEnumerable<T> common, IEnumerable<T> uncommon, IEnumerable<T> rare, IEnumerable<T> veryRare)
        {
            if (common.Any())
                return commonRoll;

            if (uncommon.Any())
                return uncommonRoll;

            if (rare?.Any() == true)
                return rareRoll;

            return veryRareRoll;
        }
    }
}