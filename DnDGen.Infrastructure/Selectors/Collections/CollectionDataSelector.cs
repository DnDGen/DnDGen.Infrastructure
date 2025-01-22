using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    internal class CollectionDataSelector<T> : ICollectionDataSelector<T>
        where T : DataSelection<T>, new()
    {
        private readonly ICollectionSelector collectionSelector;
        private readonly Func<string[], T> map;
        private readonly char separator;

        public CollectionDataSelector(ICollectionSelector collectionSelector)
        {
            this.collectionSelector = collectionSelector;

            var data = new T();
            map = data.MapTo;
            separator = data.Separator;
        }

        public IEnumerable<T> SelectFrom(string assemblyName, string tableName, string collectionName)
        {
            var selections = collectionSelector.SelectFrom(assemblyName, tableName, collectionName);
            var data = selections.Select(s => DataHelper.Parse(s, map, separator));
            return data;
        }

        public Dictionary<string, IEnumerable<T>> SelectAllFrom(string assemblyName, string tableName)
        {
            var selections = collectionSelector.SelectAllFrom(assemblyName, tableName);
            var data = selections.ToDictionary(s => s.Key, s => s.Value.Select(v => DataHelper.Parse(v, map, separator)));
            return data;
        }
    }
}