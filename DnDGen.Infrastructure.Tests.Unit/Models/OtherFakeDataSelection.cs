using DnDGen.Infrastructure.Models;
using System;

namespace DnDGen.Infrastructure.Tests.Unit.Models
{
    internal class OtherFakeDataSelection : DataSelection<OtherFakeDataSelection>
    {
        public string MyString { get; set; }
        public int MyInt { get; set; }
        public bool MyBoolean { get; set; }

        public override Func<string[], OtherFakeDataSelection> MapTo => Map;

        public override Func<OtherFakeDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 3;
        public override char Separator => ',';

        public static OtherFakeDataSelection Map(string[] splitData)
        {
            var selection = new OtherFakeDataSelection
            {
                MyString = splitData[0],
                MyInt = Convert.ToInt32(splitData[1]),
                MyBoolean = Convert.ToBoolean(splitData[2]),
            };

            return selection;
        }

        public static string[] Map(OtherFakeDataSelection selection)
        {
            return [selection.MyString, selection.MyInt.ToString(), selection.MyBoolean.ToString()];
        }
    }
}
