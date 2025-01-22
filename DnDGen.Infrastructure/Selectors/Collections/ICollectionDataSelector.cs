using DnDGen.Infrastructure.Models;
using System.Collections.Generic;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    public interface ICollectionDataSelector<T>
        where T : DataSelection<T>
    {
        IEnumerable<T> SelectFrom(string assemblyName, string tableName, string collectionName);
        Dictionary<string, IEnumerable<T>> SelectAllFrom(string assemblyName, string tableName);
    }
}
