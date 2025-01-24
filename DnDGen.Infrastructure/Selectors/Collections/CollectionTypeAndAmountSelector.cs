using DnDGen.Infrastructure.Models;
using DnDGen.RollGen;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    internal class CollectionTypeAndAmountSelector : ICollectionTypeAndAmountSelector
    {
        private readonly ICollectionDataSelector<TypeAndAmountDataSelection> collectionDataSelector;
        private readonly Dice dice;

        public CollectionTypeAndAmountSelector(ICollectionDataSelector<TypeAndAmountDataSelection> collectionDataSelector, Dice dice)
        {
            this.collectionDataSelector = collectionDataSelector;
            this.dice = dice;
        }

        public IEnumerable<TypeAndAmountDataSelection> SelectFrom(string assemblyName, string tableName, string collectionName)
        {
            var selections = collectionDataSelector.SelectFrom(assemblyName, tableName, collectionName).ToArray();

            foreach (var selection in selections)
                selection.AmountAsDouble = dice.Roll(selection.Roll).AsSum<double>();

            return selections;
        }

        public Dictionary<string, IEnumerable<TypeAndAmountDataSelection>> SelectAllFrom(string assemblyName, string tableName)
        {
            var selections = collectionDataSelector.SelectAllFrom(assemblyName, tableName);

            foreach (var selection in selections.Values.SelectMany(v => v))
                selection.AmountAsDouble = dice.Roll(selection.Roll).AsSum<double>();

            return selections;
        }
    }
}