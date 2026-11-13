using DnDGen.Infrastructure.Models;
using DnDGen.RollGen;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Selectors.Percentiles
{
    internal class PercentileTypeAndAmountSelector(IPercentileDataSelector<TypeAndAmountDataSelection> percentileDataSelector, Dice dice) : IPercentileTypeAndAmountSelector
    {
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
