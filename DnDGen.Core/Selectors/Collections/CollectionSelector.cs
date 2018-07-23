using DnDGen.Core.Helpers;
using DnDGen.Core.Mappers.Collections;
using EventGen;
using RollGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Core.Selectors.Collections
{
    internal class CollectionSelector : ICollectionSelector
    {
        private readonly CollectionMapper mapper;
        private readonly Dice dice;
        //INFO: We are injecting the event queue directly in order to log recursive events in Explode
        private readonly GenEventQueue eventQueue;

        public CollectionSelector(CollectionMapper mapper, Dice dice, GenEventQueue eventQueue)
        {
            this.mapper = mapper;
            this.dice = dice;
            this.eventQueue = eventQueue;
        }

        public IEnumerable<string> SelectFrom(string tableName, string collectionName)
        {
            if (!IsCollection(tableName, collectionName))
                throw new ArgumentException($"{collectionName} is not a valid collection in the table {tableName}");

            var table = SelectAllFrom(tableName);
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
            var explodedCollection = ExplodeAndPreserveDuplicates(tableName, collectionName);

            return explodedCollection.Distinct();
        }

        public IEnumerable<string> Flatten(Dictionary<string, IEnumerable<string>> collections, IEnumerable<string> keys)
        {
            return CollectionHelper.FlattenCollection(collections, keys);
        }

        public IEnumerable<string> ExplodeAndPreserveDuplicates(string tableName, string collectionName)
        {
            eventQueue.Enqueue("Core", $"Recursively retrieving {collectionName} from {tableName}");

            var explodedCollection = SelectFrom(tableName, collectionName).ToList();
            var subCollectionNames = explodedCollection
                .Where(i => IsCollection(tableName, i) && i != collectionName)
                .ToArray(); //INFO: Doing immediate execution because looping below fails otherwise (modifying the source collection)

            foreach (var subCollectionName in subCollectionNames)
            {
                var explodedSubCollection = ExplodeAndPreserveDuplicates(tableName, subCollectionName);
                explodedCollection.Remove(subCollectionName);
                explodedCollection.AddRange(explodedSubCollection);
            }

            return explodedCollection;
        }

        public IEnumerable<string> CreateWeighted(IEnumerable<string> common = null, IEnumerable<string> uncommon = null, IEnumerable<string> rare = null, IEnumerable<string> veryRare = null)
        {
            common = common ?? Enumerable.Empty<String>();
            uncommon = uncommon ?? Enumerable.Empty<String>();
            rare = rare ?? Enumerable.Empty<String>();
            veryRare = veryRare ?? Enumerable.Empty<String>();

            var weightedCollection = new List<string>(veryRare);

            var rareMultiplier = GetRareMultiplier(rare, veryRare);
            var rareWeighted = Duplicate(rare, rareMultiplier);
            weightedCollection.AddRange(rareWeighted);

            var uncommonMultiplier = GetUncommonMultiplier(common, uncommon, rare, veryRare);
            var uncommonWeighted = Duplicate(uncommon, uncommonMultiplier);
            weightedCollection.AddRange(uncommonWeighted);

            var commonMultiplier = GetCommonMultiplier(common, uncommon, rare, veryRare);
            var commonWeighted = Duplicate(common, commonMultiplier);
            weightedCollection.AddRange(commonWeighted);

            return weightedCollection;
        }

        private int GetRareMultiplier(IEnumerable<string> rare, IEnumerable<string> veryRare)
        {
            var againstVeryRare = 1d;

            if (rare.Any())
                againstVeryRare = 9 * veryRare.Count() / (double)rare.Count();

            var multipliers = new[] { againstVeryRare, 1 };
            var multiplier = multipliers.Max();

            return RoundMultiplier(multiplier);
        }

        private int GetUncommonMultiplier(IEnumerable<string> common, IEnumerable<string> uncommon, IEnumerable<string> rare, IEnumerable<string> veryRare)
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

            var multipliers = new[] { againstVeryRare, againstRareAndVeryRare, againstRare, 1 };
            var multiplier = multipliers.Max();

            return RoundMultiplier(multiplier);
        }

        private int GetCommonMultiplier(IEnumerable<string> common, IEnumerable<string> uncommon, IEnumerable<string> rare, IEnumerable<string> veryRare)
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

            var multipliers = new[] { againstUncommon, againstRare, againstVeryRare, 1 };
            var multiplier = multipliers.Max();

            return RoundMultiplier(multiplier);
        }

        private int RoundMultiplier(double raw)
        {
            var rounded = Math.Round(raw, 3);
            var ceiling = Math.Ceiling(rounded);

            return Convert.ToInt32(ceiling);
        }

        private IEnumerable<string> Duplicate(IEnumerable<string> source, int quantity)
        {
            return Enumerable.Repeat(source, quantity).SelectMany(a => a);
        }

        public string SelectRandomFrom(IEnumerable<string> common = null, IEnumerable<string> uncommon = null, IEnumerable<string> rare = null, IEnumerable<string> veryRare = null)
        {
            var weighted = CreateWeighted(common, uncommon, rare, veryRare);
            var selected = SelectRandomFrom(weighted);

            return selected;
        }
    }
}