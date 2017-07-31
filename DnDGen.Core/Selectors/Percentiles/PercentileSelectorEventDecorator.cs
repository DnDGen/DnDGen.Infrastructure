using EventGen;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Core.Selectors.Percentiles
{
    internal class PercentileSelectorEventDecorator : IPercentileSelector
    {
        private readonly IPercentileSelector innerSelector;
        private readonly GenEventQueue eventQueue;

        public PercentileSelectorEventDecorator(IPercentileSelector innerSelector, GenEventQueue eventQueue)
        {
            this.innerSelector = innerSelector;
            this.eventQueue = eventQueue;
        }

        public IEnumerable<string> SelectAllFrom(string tableName)
        {
            eventQueue.Enqueue("Core", $"Selecting all percentile results from {tableName}");
            var percentileResults = innerSelector.SelectAllFrom(tableName);
            eventQueue.Enqueue("Core", $"Selected {percentileResults.Count()} results from {tableName}");

            return percentileResults;
        }

        public IEnumerable<T> SelectAllFrom<T>(string tableName)
        {
            eventQueue.Enqueue("Core", $"Selecting all percentile results from {tableName}");
            var percentileResults = innerSelector.SelectAllFrom<T>(tableName);
            eventQueue.Enqueue("Core", $"Selected {percentileResults.Count()} results from {tableName}");

            return percentileResults;
        }

        public string SelectFrom(string tableName)
        {
            eventQueue.Enqueue("Core", $"Rolling percentile in {tableName}");
            var result = innerSelector.SelectFrom(tableName);
            eventQueue.Enqueue("Core", $"Selected {result} from {tableName}");

            return result;
        }

        public T SelectFrom<T>(string tableName)
        {
            eventQueue.Enqueue("Core", $"Rolling percentile in {tableName}");
            var result = innerSelector.SelectFrom<T>(tableName);
            eventQueue.Enqueue("Core", $"Selected {result} from {tableName}");

            return result;
        }
    }
}
