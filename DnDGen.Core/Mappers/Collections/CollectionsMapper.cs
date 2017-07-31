using System.Collections.Generic;

namespace DnDGen.Core.Mappers.Collections
{
    public interface CollectionsMapper
    {
        Dictionary<string, IEnumerable<string>> Map(string tableName);
    }
}