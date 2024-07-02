using DnDGen.Infrastructure.Mappers.Collections;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Tests.Unit")]
namespace DnDGen.Infrastructure.YetAnother
{
    public class YetAnotherCollectionsCaller
    {
        private readonly CollectionMapper collectionsMapper;

        public YetAnotherCollectionsCaller(CollectionMapper collectionsMapper)
        {
            this.collectionsMapper = collectionsMapper;
        }

        public IEnumerable<string> Call(string tableName, string name)
        {
            var table = collectionsMapper.Map("DnDGen.Infrastructure.YetAnother", tableName);
            return table[name];
        }
    }
}
