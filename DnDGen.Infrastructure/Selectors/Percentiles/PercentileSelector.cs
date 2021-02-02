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

        /// <summary>
        /// Return the value as True or False, depending on if the percentile roll is higher or lower then the threshold.
        /// A value less than or equal to the threshold is false.
        /// A value higher than the threshold is true.
        /// As an example, with threshold = .7, rolling a 70 produces False, while 71 produces True.
        /// </summary>
        /// <param name="threshold">The non-inclusive lower-bound percentage of success</param>
        /// <returns></returns>
        public bool SelectFrom(double threshold)
        {
            if (threshold >= 1)
                return false;
            else if (threshold <= 0)
                return true;

            return dice.Roll().Percentile().AsTrueOrFalse(threshold);
        }

        /// <summary>
        /// Return the value as True or False, depending on if the percentile roll is higher or lower then the threshold.
        /// A value less than or equal to the threshold is false.
        /// A value higher than the threshold is true.
        /// As an example, with threshold = 70, rolling a 69 produces False, while 70 produces True.
        /// </summary>
        /// <param name="threshold">The inclusive lower-bound roll value of success</param>
        /// <returns></returns>
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