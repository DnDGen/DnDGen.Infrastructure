using System.Collections.Generic;

namespace DnDGen.Infrastructure.Mappers.Collections
{
    public interface CollectionMapper
    {
        Dictionary<string, IEnumerable<string>> Map(string tableName);
    }
}