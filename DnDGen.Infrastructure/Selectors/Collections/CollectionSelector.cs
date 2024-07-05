using DnDGen.Infrastructure.Helpers;
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

        public CollectionSelector(CollectionMapper mapper, Dice dice)
        {
            this.mapper = mapper;
            this.dice = dice;
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

        public IEnumerable<string> Explode(string assemblyName, string tableName, string collectionName) => ExplodeRecursive(assemblyName, tableName, collectionName, false);

        public IEnumerable<string> Flatten(Dictionary<string, IEnumerable<string>> collections, IEnumerable<string> keys)
        {
            return CollectionHelper.FlattenCollection(collections, keys);
        }

        public IEnumerable<string> ExplodeAndPreserveDuplicates(string assemblyName, string tableName, string collectionName)
            => ExplodeRecursive(assemblyName, tableName, collectionName, true);

        private IEnumerable<string> ExplodeRecursive(string assemblyName, string tableName, string collectionName, bool preserveDuplicates)
        {
            var rootCollection = SelectFrom(assemblyName, tableName, collectionName);
            var explodedCollection = new List<string>();
            var explodedUniqueCollection = new HashSet<string>();

            foreach (var entry in rootCollection)
            {
                if (IsCollection(assemblyName, tableName, entry) && entry != collectionName)
                {
                    var subCollection = ExplodeRecursive(assemblyName, tableName, entry, preserveDuplicates);

                    explodedCollection.AddRange(subCollection);
                    explodedUniqueCollection.UnionWith(subCollection);
                }
                else
                {
                    explodedCollection.Add(entry);
                    explodedUniqueCollection.Add(entry);
                }
            }

            if (preserveDuplicates)
                return explodedCollection;

            return explodedUniqueCollection;
        }

        public IEnumerable<T> CreateWeighted<T>(IEnumerable<T> common = null, IEnumerable<T> uncommon = null, IEnumerable<T> rare = null, IEnumerable<T> veryRare = null)
        {
            common ??= Enumerable.Empty<T>();
            uncommon ??= Enumerable.Empty<T>();
            rare ??= Enumerable.Empty<T>();
            veryRare ??= Enumerable.Empty<T>();

            var weightedCollection = veryRare;

            var rareMultiplier = GetRareMultiplier(rare, veryRare);
            var rareWeighted = Duplicate(rare, rareMultiplier);
            weightedCollection = weightedCollection.Concat(rareWeighted);

            var uncommonMultiplier = GetUncommonMultiplier(common, uncommon, rare, veryRare);
            var uncommonWeighted = Duplicate(uncommon, uncommonMultiplier);
            weightedCollection = weightedCollection.Concat(uncommonWeighted);

            var commonMultiplier = GetCommonMultiplier(common, uncommon, rare, veryRare);
            var commonWeighted = Duplicate(common, commonMultiplier);
            weightedCollection = weightedCollection.Concat(commonWeighted);

            return weightedCollection;
        }

        private int GetRareMultiplier<T>(IEnumerable<T> rare, IEnumerable<T> veryRare)
        {
            var againstVeryRare = 1d;

            if (rare.Any())
                againstVeryRare = 9 * veryRare.Count() / (double)rare.Count();

            var multiplier = Math.Max(againstVeryRare, 1);

            return RoundMultiplier(multiplier);
        }

        private int GetUncommonMultiplier<T>(IEnumerable<T> common, IEnumerable<T> uncommon, IEnumerable<T> rare, IEnumerable<T> veryRare)
        {
            var veryRareAmount = veryRare.Count();

            var rareMultiplier = GetRareMultiplier(rare, veryRare);
            var rareAmount = rareMultiplier * rare.Count();

            var uncommonCount = uncommon.Count();
            var commonDivisor = common.Any() ? 3 : 1;

            var againstRareAndVeryRare = 1d;
            var againstRare = 1d;
            var againstVeryRare = 1d;

            if (uncommonCount > 0)
            {
                againstRareAndVeryRare = 3d * (rareAmount + veryRareAmount) / uncommonCount;
                againstRare = (9d * (rareAmount + veryRareAmount)) / commonDivisor / uncommonCount;
                againstVeryRare = (99d * veryRareAmount - rareAmount) / commonDivisor / uncommonCount;
            }

            var multiplier = Math.Max(Math.Max(againstVeryRare, againstRareAndVeryRare), Math.Max(againstRare, 1));

            return RoundMultiplier(multiplier);
        }

        private int GetCommonMultiplier<T>(IEnumerable<T> common, IEnumerable<T> uncommon, IEnumerable<T> rare, IEnumerable<T> veryRare)
        {
            var veryRareAmount = veryRare.Count();

            var rareMultiplier = GetRareMultiplier(rare, veryRare);
            var rareAmount = rareMultiplier * rare.Count();

            var uncommonMultiplier = GetUncommonMultiplier(common, uncommon, rare, veryRare);
            var uncommonAmount = uncommonMultiplier * uncommon.Count();

            var commonCount = common.Count();

            var againstUncommon = 1d;
            var againstRare = 1d;
            var againstVeryRare = 1d;

            if (commonCount > 0)
            {
                againstUncommon = 2d * uncommonAmount / commonCount;
                againstRare = (9d * (rareAmount + veryRareAmount) - uncommonAmount) / commonCount;
                againstVeryRare = (99d * veryRareAmount - rareAmount - uncommonAmount) / commonCount;
            }

            var multiplier = Math.Max(Math.Max(againstUncommon, againstRare), Math.Max(againstVeryRare, 1));

            return RoundMultiplier(multiplier);
        }

        private int RoundMultiplier(double raw)
        {
            var rounded = Math.Round(raw, 3);
            var ceiling = Math.Ceiling(rounded);

            return Convert.ToInt32(ceiling);
        }

        private IEnumerable<T> Duplicate<T>(IEnumerable<T> source, int quantity)
        {
            return Enumerable.Repeat(source, quantity).SelectMany(a => a);
        }

        public T SelectRandomFrom<T>(IEnumerable<T> common = null, IEnumerable<T> uncommon = null, IEnumerable<T> rare = null, IEnumerable<T> veryRare = null)
        {
            if (common?.Any() != true)
            {
                var weighted = CreateWeighted(common, uncommon, rare, veryRare);
                return SelectRandomFrom(weighted);
            }

            var roll = dice.Roll().Percentile().AsSum();
            if (roll == 100 && veryRare?.Any() == true)
                return SelectRandomFrom(veryRare);

            if (roll > 90 && rare?.Any() == true)
                return SelectRandomFrom(rare);

            if (roll > 60 && uncommon?.Any() == true)
                return SelectRandomFrom(uncommon);

            return SelectRandomFrom(common);
        }
    }
}