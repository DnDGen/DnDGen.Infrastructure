using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Models;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    internal class CollectionDataSelector<T> : ICollectionDataSelector<T>
        where T : DataSelection<T>, new()
    {
        private readonly ICollectionSelector collectionSelector;

        public CollectionDataSelector(ICollectionSelector collectionSelector)
        {
            this.collectionSelector = collectionSelector;
        }

        public IEnumerable<T> SelectFrom(string assemblyName, string tableName, string collectionName)
        {
            var selections = collectionSelector.SelectFrom(assemblyName, tableName, collectionName);
            var data = selections.Select(DataHelper.Parse<T>);
            return data;
        }

        public Dictionary<string, IEnumerable<T>> SelectAllFrom(string assemblyName, string tableName)
        {
            var selections = collectionSelector.SelectAllFrom(assemblyName, tableName);
            var data = selections.ToDictionary(s => s.Key, s => s.Value.Select(DataHelper.Parse<T>).ToArray().AsEnumerable());
            return data;
        }
    }
}