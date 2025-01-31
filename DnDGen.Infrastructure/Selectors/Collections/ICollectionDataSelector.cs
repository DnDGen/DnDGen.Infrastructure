using DnDGen.Infrastructure.Models;
using System.Collections.Generic;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    public interface ICollectionDataSelector<T>
        where T : DataSelection<T>
    {
        IEnumerable<T> SelectFrom(string assemblyName, string tableName, string collectionName);
        T SelectOneFrom(string assemblyName, string tableName, string collectionName);
        Dictionary<string, IEnumerable<T>> SelectAllFrom(string assemblyName, string tableName);
        bool IsCollection(string assemblyName, string tableName, string collectionName);
        T SelectRandomFrom(string assemblyName, string tableName, string collectionName);
    }
}
