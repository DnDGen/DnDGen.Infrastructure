using System.Collections.Generic;

namespace DnDGen.Core.Selectors.Collections
{
    public interface ICollectionSelector
    {
        IEnumerable<string> SelectFrom(string tableName, string collectionName);
        Dictionary<string, IEnumerable<string>> SelectAllFrom(string tableName);
        string FindCollectionOf(string tableName, string entry, params string[] filteredCollectionNames);
        T SelectRandomFrom<T>(IEnumerable<T> collection);
        string SelectRandomFrom(string tableName, string collectionName);
        bool IsCollection(string tableName, string collectionName);
        IEnumerable<string> Explode(string tableName, string collectionName);
        IEnumerable<string> ExplodeAndPreserveDuplicates(string tableName, string collectionName);
        IEnumerable<string> Flatten(Dictionary<string, IEnumerable<string>> collections, IEnumerable<string> keys);
    }
}