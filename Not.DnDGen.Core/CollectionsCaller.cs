using DnDGen.Core.Mappers.Collections;
using System.Collections.Generic;

namespace Not.DnDGen.Core
{
    public class CollectionsCaller
    {
        private readonly CollectionsMapper collectionsMapper;

        public CollectionsCaller(CollectionsMapper collectionsMapper)
        {
            this.collectionsMapper = collectionsMapper;
        }

        public IEnumerable<string> Call(string tableName, string name)
        {
            var table = collectionsMapper.Map(tableName);
            return table[name];
        }
    }
}
