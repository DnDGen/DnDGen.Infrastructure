using System.Collections.Generic;

namespace DnDGen.Core.Mappers.Percentiles
{
    public interface PercentileMapper
    {
        Dictionary<int, string> Map(string tableName);
    }
}