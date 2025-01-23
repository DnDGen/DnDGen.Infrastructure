using DnDGen.Infrastructure.Models;
using System;

namespace DnDGen.Infrastructure.Tests.Integration.Models
{
    internal class TestDataSelection : DataSelection<TestDataSelection>
    {
        public string Name { get; set; }
        public int Age { get; set; }

        public override Func<string[], TestDataSelection> MapTo => Map;

        public override Func<TestDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 2;

        public static TestDataSelection Map(string[] splitData)
        {
            var selection = new TestDataSelection
            {
                Name = splitData[0],
                Age = Convert.ToInt32(splitData[1])
            };

            return selection;
        }

        public static string[] Map(TestDataSelection selection)
        {
            return [selection.Name, selection.Age.ToString()];
        }
    }
}
