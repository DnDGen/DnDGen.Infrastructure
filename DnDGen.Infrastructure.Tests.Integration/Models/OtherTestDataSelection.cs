using DnDGen.Infrastructure.Models;
using System;

namespace DnDGen.Infrastructure.Tests.Integration.Models
{
    internal class OtherTestDataSelection : DataSelection<OtherTestDataSelection>
    {
        public string MyString { get; set; }
        public string MyOtherString { get; set; }
        public int MyNumber { get; set; }
        public bool MyBoolean { get; set; }

        public override Func<string[], OtherTestDataSelection> MapTo => Map;

        public override Func<OtherTestDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 4;

        public static OtherTestDataSelection Map(string[] splitData)
        {
            var selection = new OtherTestDataSelection
            {
                MyString = splitData[0],
                MyNumber = Convert.ToInt32(splitData[1]),
                MyBoolean = Convert.ToBoolean(splitData[2]),
                MyOtherString = splitData[3],
            };

            return selection;
        }

        public static string[] Map(OtherTestDataSelection selection)
        {
            return [selection.MyString, selection.MyNumber.ToString(), selection.MyBoolean.ToString(), selection.MyOtherString];
        }
    }
}
