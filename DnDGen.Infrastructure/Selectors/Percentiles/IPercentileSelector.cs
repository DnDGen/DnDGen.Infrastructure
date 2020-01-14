using System.Collections.Generic;

namespace DnDGen.Infrastructure.Selectors.Percentiles
{
    public interface IPercentileSelector
    {
        string SelectFrom(string tableName);
        IEnumerable<string> SelectAllFrom(string tableName);
        T SelectFrom<T>(string tableName);
        IEnumerable<T> SelectAllFrom<T>(string tableName);
        bool SelectFrom(double chance);
        bool SelectFrom(int rollThreshold);
    }
}