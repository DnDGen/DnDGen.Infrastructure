using DnDGen.Infrastructure.Mappers.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Tests.Unit")]
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
            var table = collectionsMapper.Map("DnDGen.Infrastructure.Another", tableName);
            return table[name];
        }
    }
}
