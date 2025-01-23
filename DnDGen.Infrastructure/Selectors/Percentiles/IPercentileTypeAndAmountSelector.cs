using DnDGen.Infrastructure.Models;
using System.Collections.Generic;

namespace DnDGen.Infrastructure.Selectors.Percentiles
{
    public interface IPercentileTypeAndAmountSelector
    {
        TypeAndAmountDataSelection SelectFrom(string assemblyName, string tableName);
        IEnumerable<TypeAndAmountDataSelection> SelectAllFrom(string assemblyName, string tableName);
    }
}
