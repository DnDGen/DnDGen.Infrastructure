using System.Collections.Generic;

namespace DnDGen.Core.Mappers.Collections
{
    public interface CollectionMapper
    {
        Dictionary<string, IEnumerable<string>> Map(string tableName);
    }
}