using DnDGen.Infrastructure.Another;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;

namespace DnDGen.Infrastructure.YetAnother
{
    public class YetAnotherGenerator
    {
        private readonly ICollectionSelector collectionSelector;
        private readonly IPercentileSelector percentileSelector;
        private readonly AnotherGenerator anotherGenerator;

        private const string assembly = "DnDGen.Infrastructure.YetAnother";

        public YetAnotherGenerator(ICollectionSelector collectionSelector, IPercentileSelector percentileSelector, AnotherGenerator anotherGenerator)
        {
            this.collectionSelector = collectionSelector;
            this.percentileSelector = percentileSelector;
            this.anotherGenerator = anotherGenerator;
        }

        public string Generate()
        {
            var collection1 = collectionSelector.SelectFrom(assembly, "RealCollectionTable", "Real Value 2");
            var collection2 = collectionSelector.SelectFrom(assembly, "DuplicateCollectionTable", "Definitely Not Value 2");
            var percentile = percentileSelector.SelectFrom(assembly, "RealPercentileTable");
            var another = anotherGenerator.Generate();

            return $"{collection1.First()} + {collection2.First()} + {percentile}: {another}";
        }
    }
}
