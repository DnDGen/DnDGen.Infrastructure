using DnDGen.Infrastructure.Mappers.Collections;
using System.Collections.Generic;

namespace DnDGen.Infrastructure.Another
{
    public class AnotherCollectionsCaller
    {
        private readonly CollectionMapper collectionsMapper;

        public AnotherCollectionsCaller(CollectionMapper collectionsMapper)
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
