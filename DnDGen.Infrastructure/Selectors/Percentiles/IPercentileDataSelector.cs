using DnDGen.Infrastructure.Models;
using System.Collections.Generic;

namespace DnDGen.Infrastructure.Selectors.Percentiles
{
    public interface IPercentileDataSelector<T>
        where T : DataSelection<T>
    {
        T SelectFrom(string assemblyName, string tableName);
        IEnumerable<T> SelectAllFrom(string assemblyName, string tableName);
    }
}
