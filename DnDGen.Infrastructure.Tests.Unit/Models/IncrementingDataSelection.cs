using DnDGen.Infrastructure.Models;
using System;

namespace DnDGen.Infrastructure.Tests.Unit.Models
{
    internal class IncrementingDataSelection : DataSelection<IncrementingDataSelection>
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override Func<string[], IncrementingDataSelection> MapTo => Map;

        public override Func<IncrementingDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 2;

        public static int MapCount { get; set; } = 0;

        public static IncrementingDataSelection Map(string[] splitData)
        {
            MapCount++;
            var selection = new IncrementingDataSelection
            {
                Name = splitData[0],
                Age = Convert.ToInt32(splitData[1]) + MapCount
            };

            return selection;
        }

        public static string[] Map(IncrementingDataSelection selection)
        {
            return [selection.Name, selection.Age.ToString()];
        }
    }
}
