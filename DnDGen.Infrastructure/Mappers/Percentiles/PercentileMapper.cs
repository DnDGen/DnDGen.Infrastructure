using System.Collections.Generic;

namespace DnDGen.Infrastructure.Mappers.Percentiles
{
    public interface PercentileMapper
    {
        Dictionary<int, string> Map(string tableName);
    }
}