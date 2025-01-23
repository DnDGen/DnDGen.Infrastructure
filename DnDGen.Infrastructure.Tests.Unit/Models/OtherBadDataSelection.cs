using DnDGen.Infrastructure.Models;
using System;

namespace DnDGen.Infrastructure.Tests.Unit.Models
{
    internal class OtherBadDataSelection : DataSelection<OtherBadDataSelection>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public override Func<string[], OtherBadDataSelection> MapTo => throw new NotImplementedException();

        public override Func<OtherBadDataSelection, string[]> MapFrom => Map;

        public int ManualSectionCount { get; set; } = 666;
        public override int SectionCount => ManualSectionCount;

        public static OtherBadDataSelection Map(string[] splitData)
        {
            var selection = new OtherBadDataSelection
            {
                FirstName = splitData[0],
                LastName = splitData[1],
            };

            return selection;
        }

        public static string[] Map(OtherBadDataSelection selection)
        {
            return [selection.FirstName, selection.LastName];
        }
    }
}
