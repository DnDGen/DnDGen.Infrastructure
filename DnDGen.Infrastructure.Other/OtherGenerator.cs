using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Linq;

namespace DnDGen.Infrastructure.Other
{
    public class OtherGenerator
    {
        private readonly ICollectionSelector collectionSelector;
        private readonly IPercentileSelector percentileSelector;

        private const string assembly = "DnDGen.Infrastructure.Other";

        public OtherGenerator(ICollectionSelector collectionSelector, IPercentileSelector percentileSelector)
        {
            this.collectionSelector = collectionSelector;
            this.percentileSelector = percentileSelector;
        }

        public string Generate()
        {
            var collection1 = collectionSelector.SelectFrom(assembly, "RealSelectorCollectionTable", "Real Selector Value 2");
            var collection2 = collectionSelector.SelectFrom(assembly, "DuplicateCollectionTable", "Not Value 2");
            var percentile = percentileSelector.SelectFrom(assembly, "RealSelectorPercentileTable");

            return $"{collection1.First()} + {collection2.First()} + {percentile}";
        }
    }
}
