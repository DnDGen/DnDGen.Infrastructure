using DnDGen.Infrastructure.Models;
using System;

namespace DnDGen.Infrastructure.Helpers
{
    public static class DataHelper
    {
        public static T Parse<T>(string rawData, Func<string[], T> map, char separator = '@')
            where T : DataSelection<T>, new()
        {
            var splitData = Parse(rawData, separator);
            var data = new T();

            if (splitData.Length != data.SectionCount)
                throw new ArgumentException($"Data [{rawData}] invalid for {typeof(T)}: Need {data.SectionCount} sections, got {splitData.Length}");

            return map(splitData);
        }

        public static string[] Parse(string rawData, char separator = '@') => rawData.Split(separator);
        public static string Parse(string[] splitData, char separator = '@') => string.Join(separator, splitData);

        public static string Parse<T>(T data, Func<T, string[]> map, char separator = '@')
            where T : DataSelection<T>
        {
            var splitData = map(data);
            var rawData = Parse(splitData, separator);

            if (splitData.Length != data.SectionCount)
                throw new ArgumentException($"Data [{rawData}] invalid for {typeof(T)}: Need {data.SectionCount} sections, got {splitData.Length}");

            return rawData;
        }

        public static T Parse<T>(string rawData)
            where T : DataSelection<T>, new()
        {
            var data = new T();
            return Parse(rawData, data.MapTo, data.Separator);
        }

        public static string Parse<T>(T data)
            where T : DataSelection<T>
        {
            return Parse(data, data.MapFrom, data.Separator);
        }
    }
}
