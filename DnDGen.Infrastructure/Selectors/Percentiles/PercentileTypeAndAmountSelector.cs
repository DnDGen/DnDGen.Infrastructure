using DnDGen.Infrastructure.Models;
using DnDGen.RollGen;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Selectors.Percentiles
{
    internal class PercentileTypeAndAmountSelector : IPercentileTypeAndAmountSelector
    {
        private readonly IPercentileDataSelector<TypeAndAmountDataSelection> percentileDataSelector;
        private readonly Dice dice;

        public PercentileTypeAndAmountSelector(IPercentileDataSelector<TypeAndAmountDataSelection> percentileDataSelector, Dice dice)
        {
            this.percentileDataSelector = percentileDataSelector;
            this.dice = dice;
        }

        public TypeAndAmountDataSelection SelectFrom(string assemblyName, string tableName)
        {
            var selection = percentileDataSelector.SelectFrom(assemblyName, tableName);
            selection.AmountAsDouble = dice.Roll(selection.Roll).AsSum<double>();
            return selection;
        }

        public IEnumerable<TypeAndAmountDataSelection> SelectAllFrom(string assemblyName, string tableName)
        {
            var selections = percentileDataSelector.SelectAllFrom(assemblyName, tableName).ToArray();

            foreach (var selection in selections)
                selection.AmountAsDouble = dice.Roll(selection.Roll).AsSum<double>();

            return selections;
        }
    }
}
