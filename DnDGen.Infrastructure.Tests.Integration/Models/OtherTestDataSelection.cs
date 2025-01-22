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

        public static OtherTestDataSelection Map(string[] splitData)
        {
            if (splitData.Length != 2)
                throw new ArgumentException($"Data [{string.Join(",", splitData)}] invalid for TestDataSelection");

            var selection = new OtherTestDataSelection
            {
                Name = splitData[0],
                Age = Convert.ToInt32(splitData[1])
            };

            return selection;
        }

        public static string[] Map(OtherTestDataSelection selection)
        {
            return [selection.MyString, selection.MyNumber.ToString(), selection.MyBoolean.ToString(), selection.MyOtherString];
        }
    }
}
