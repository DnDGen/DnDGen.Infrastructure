using DnDGen.Core.Mappers.Collections;
using System.Collections.Generic;

namespace Also.Not.DnDGen.Core
{
    public class OtherCollectionsCaller
    {
        private readonly CollectionsMapper collectionsMapper;

        public OtherCollectionsCaller(CollectionsMapper collectionsMapper)
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
