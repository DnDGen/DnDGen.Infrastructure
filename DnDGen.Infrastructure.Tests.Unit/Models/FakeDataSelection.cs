using DnDGen.Infrastructure.Models;
using System;

namespace DnDGen.Infrastructure.Tests.Unit.Models
{
    internal class FakeDataSelection : DataSelection<FakeDataSelection>
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override Func<string[], FakeDataSelection> MapTo => Map;

        public override Func<FakeDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 2;

        public static FakeDataSelection Map(string[] splitData)
        {
            var selection = new FakeDataSelection
            {
                Name = splitData[0],
                Age = Convert.ToInt32(splitData[1])
            };

            return selection;
        }

        public static string[] Map(FakeDataSelection selection)
        {
            return [selection.Name, selection.Age.ToString()];
        }
    }
}
