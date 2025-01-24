using System;

namespace DnDGen.Infrastructure.Models
{
    public class TypeAndAmountDataSelection : DataSelection<TypeAndAmountDataSelection>
    {
        public string Type { get; set; }
        public int Amount => Convert.ToInt32(AmountAsDouble);
        public double AmountAsDouble { get; set; }
        public string Roll { get; set; }

        public override Func<string[], TypeAndAmountDataSelection> MapTo => Map;
        public override Func<TypeAndAmountDataSelection, string[]> MapFrom => Map;

        public override int SectionCount => 2;

        public static TypeAndAmountDataSelection Map(string[] splitData)
        {
            var selection = new TypeAndAmountDataSelection
            {
                Type = splitData[0],
                Roll = splitData[1],
            };

            return selection;
        }

        public static string[] Map(TypeAndAmountDataSelection selection)
        {
            return [selection.Type, selection.Roll];
        }
    }
}
