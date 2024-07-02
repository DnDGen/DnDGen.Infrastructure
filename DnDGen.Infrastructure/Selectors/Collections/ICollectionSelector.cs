using System.Collections.Generic;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    public interface ICollectionSelector
    {
        IEnumerable<string> SelectFrom(string assemblyName, string tableName, string collectionName);
        Dictionary<string, IEnumerable<string>> SelectAllFrom(string assemblyName, string tableName);
        string FindCollectionOf(string assemblyName, string tableName, string entry, params string[] filteredCollectionNames);
        T SelectRandomFrom<T>(IEnumerable<T> collection);
        string SelectRandomFrom(string assemblyName, string tableName, string collectionName);
        T SelectRandomFrom<T>(IEnumerable<T> common = null, IEnumerable<T> uncommon = null, IEnumerable<T> rare = null, IEnumerable<T> veryRare = null);
        bool IsCollection(string assemblyName, string tableName, string collectionName);
        IEnumerable<string> Explode(string assemblyName, string tableName, string collectionName);
        IEnumerable<string> ExplodeAndPreserveDuplicates(string assemblyName, string tableName, string collectionName);
        IEnumerable<string> Flatten(Dictionary<string, IEnumerable<string>> collections, IEnumerable<string> keys);
        IEnumerable<T> CreateWeighted<T>(IEnumerable<T> common = null, IEnumerable<T> uncommon = null, IEnumerable<T> rare = null, IEnumerable<T> veryRare = null);
    }
}