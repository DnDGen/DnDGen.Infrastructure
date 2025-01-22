using System;

namespace DnDGen.Infrastructure.Models
{
    public abstract class DataSelection<T>
    {
        public abstract Func<string[], T> MapTo { get; }
        public abstract Func<T, string[]> MapFrom { get; }
        public virtual char Separator => '@';
    }
}
