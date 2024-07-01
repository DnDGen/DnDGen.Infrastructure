using DnDGen.Infrastructure.Mappers.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Tests.Unit")]
[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Another")]
namespace DnDGen.Infrastructure.Other
{
    public class OtherCollectionsCaller
    {
        private readonly CollectionMapper collectionsMapper;

        public OtherCollectionsCaller(CollectionMapper collectionsMapper)
        {
            this.collectionsMapper = collectionsMapper;
        }

        public IEnumerable<string> Call(string tableName, string name)
        {
            var table = collectionsMapper.Map("DnDGen.Infrastructure.Other", tableName);
            return table[name];
        }
    }
}
