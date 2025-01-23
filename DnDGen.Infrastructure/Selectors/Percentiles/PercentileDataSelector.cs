using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Selectors.Percentiles
{
    internal class PercentileDataSelector<T> : IPercentileDataSelector<T>
        where T : DataSelection<T>, new()
    {
        private readonly IPercentileSelector percentileSelector;

        public PercentileDataSelector(IPercentileSelector percentileSelector)
        {
            this.percentileSelector = percentileSelector;
        }

        public T SelectFrom(string assemblyName, string tableName)
        {
            var selection = percentileSelector.SelectFrom(assemblyName, tableName);
            var data = DataHelper.Parse<T>(selection);
            return data;
        }

        public IEnumerable<T> SelectAllFrom(string assemblyName, string tableName)
        {
            var selections = percentileSelector.SelectAllFrom(assemblyName, tableName);
            var data = selections.Select(DataHelper.Parse<T>);
            return data;
        }
    }
}
