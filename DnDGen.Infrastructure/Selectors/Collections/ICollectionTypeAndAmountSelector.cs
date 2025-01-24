using DnDGen.Infrastructure.Models;
using System.Collections.Generic;

namespace DnDGen.Infrastructure.Selectors.Collections
{
    public interface ICollectionTypeAndAmountSelector
    {
        IEnumerable<TypeAndAmountDataSelection> SelectFrom(string assemblyName, string tableName, string collectionName);
        Dictionary<string, IEnumerable<TypeAndAmountDataSelection>> SelectAllFrom(string assemblyName, string tableName);
    }
}
