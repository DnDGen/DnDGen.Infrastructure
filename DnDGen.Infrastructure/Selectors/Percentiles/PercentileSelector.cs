using DnDGen.Infrastructure.Mappers.Percentiles;
using DnDGen.RollGen;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Selectors.Percentiles
{
    internal class PercentileSelector : IPercentileSelector
    {
        private readonly PercentileMapper percentileMapper;
        private readonly Dice dice;

        public PercentileSelector(PercentileMapper percentileMapper, Dice dice)
        {
            this.percentileMapper = percentileMapper;
            this.dice = dice;
        }

        public string SelectFrom(string tableName)
        {
            return SelectFrom<string>(tableName);
        }

        public IEnumerable<string> SelectAllFrom(string tableName)
        {
            return SelectAllFrom<string>(tableName);
        }

        public T SelectFrom<T>(string tableName)
        {
            var table = percentileMapper.Map(tableName);
            var roll = dice.Roll().Percentile().AsSum();

            if (!table.ContainsKey(roll))
            {
                throw new ArgumentException($"{roll} is not a valid entry in the table {tableName}");
            }

            return GetValue<T>(table[roll]);
        }

        private T GetValue<T>(object source)
        {
            return (T)Convert.ChangeType(source, typeof(T));
        }

        public IEnumerable<T> SelectAllFrom<T>(string tableName)
        {
            var table = percentileMapper.Map(tableName);
            return table.Values.Select(v => GetValue<T>(v)).Distinct();
        }

        public bool SelectFrom(double threshold)
        {
            if (threshold >= 1)
                return false;
            else if (threshold <= 0)
                return true;

            return dice.Roll().Percentile().AsTrueOrFalse(threshold);
        }

        public bool SelectFrom(int rollThreshold)
        {
            if (rollThreshold > 100)
                return false;
            else if (rollThreshold <= 0)
                return true;

            return dice.Roll().Percentile().AsTrueOrFalse(rollThreshold);
        }
    }
}