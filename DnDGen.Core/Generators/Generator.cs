using System;

namespace DnDGen.Core.Generators
{
    public interface Generator
    {
        int MaxAttempts { get; set; }

        T Generate<T>(Func<T> buildInstructions, Func<T, bool> isValid, Func<T> buildDefault, Func<T, string> failureDescription, string defaultDescription);
    }
}
