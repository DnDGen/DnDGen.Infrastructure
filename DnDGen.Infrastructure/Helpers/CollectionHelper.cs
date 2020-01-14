using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Helpers
{
    public static class CollectionHelper
    {
        public static IEnumerable<string> FlattenCollection(Dictionary<string, IEnumerable<string>> collections, IEnumerable<string> keys)
        {
            var flattenedCollection = collections.Where(kvp => keys.Contains(kvp.Key))
                .Select(kvp => kvp.Value)
                .SelectMany(v => v);

            flattenedCollection = flattenedCollection.Distinct();

            return flattenedCollection.ToArray();
        }
    }
}
