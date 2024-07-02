using DnDGen.Infrastructure.Other;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using System.Linq;

namespace DnDGen.Infrastructure.Another
{
    public class AnotherGenerator
    {
        private readonly ICollectionSelector collectionSelector;
        private readonly IPercentileSelector percentileSelector;
        private readonly OtherGenerator otherGenerator;

        private const string assembly = "DnDGen.Infrastructure.Another";

        public AnotherGenerator(ICollectionSelector collectionSelector, IPercentileSelector percentileSelector, OtherGenerator otherGenerator)
        {
            this.collectionSelector = collectionSelector;
            this.percentileSelector = percentileSelector;
            this.otherGenerator = otherGenerator;
        }

        public string Generate()
        {
            var collection1 = collectionSelector.SelectFrom(assembly, "RealCallerCollectionTable", "Real Caller Value 2");
            var collection2 = collectionSelector.SelectFrom(assembly, "DuplicateCollectionTable", "Also Not Value 2");
            var percentile = percentileSelector.SelectFrom(assembly, "RealCallerPercentileTable");
            var other = otherGenerator.Generate();

            return $"{collection1.First()} + {collection2.First()} + {percentile}: {other}";
        }
    }
}
