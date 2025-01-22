using DnDGen.Infrastructure.Models;
using System;

namespace DnDGen.Infrastructure.Helpers
{
    public static class DataHelper
    {
        public static T Parse<T>(string rawData, Func<string[], T> map, char separator = '@')
        {
            var splitData = rawData.Split(separator);
            return map(splitData);
        }

        public static string Parse<T>(T data, Func<T, string[]> map, char separator = '@')
        {
            var splitData = map(data);
            return string.Join(separator, splitData);
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
