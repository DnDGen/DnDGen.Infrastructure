using DnDGen.Infrastructure.Models;
using System;

namespace DnDGen.Infrastructure.Tests.Unit.Models
{
    internal class BadDataSelection : DataSelection<BadDataSelection>
    {
        public string Something { get; set; }
        public int Whatever { get; set; }

        public override Func<string[], BadDataSelection> MapTo => throw new NotImplementedException();

        public override Func<BadDataSelection, string[]> MapFrom => throw new NotImplementedException();

        public override int SectionCount => 2;
    }
}
