using System.Collections.Generic;

namespace DnDGen.Infrastructure.Selectors.Percentiles
{
    public interface IPercentileSelector
    {
        string SelectFrom(string tableName);
        IEnumerable<string> SelectAllFrom(string tableName);
        T SelectFrom<T>(string tableName);
        IEnumerable<T> SelectAllFrom<T>(string tableName);
        /// <summary>
        /// Return the value as True or False, depending on if the percentile roll is higher or lower then the threshold.
        /// A value less than or equal to the threshold is false.
        /// A value higher than the threshold is true.
        /// As an example, with threshold = .7, rolling a 70 produces False, while 71 produces True.
        /// </summary>
        /// <param name="threshold">The non-inclusive lower-bound percentage of success</param>
        /// <returns></returns>
        bool SelectFrom(double threshold);
        /// <summary>
        /// Return the value as True or False, depending on if the percentile roll is higher or lower then the threshold.
        /// A value less than or equal to the threshold is false.
        /// A value higher than the threshold is true.
        /// As an example, with threshold = 70, rolling a 69 produces False, while 70 produces True.
        /// </summary>
        /// <param name="threshold">The inclusive lower-bound roll value of success</param>
        /// <returns></returns>
        bool SelectFrom(int rollThreshold);
    }
}