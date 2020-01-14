using System;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Tests.Unit")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Tests.Integration")]
[assembly: InternalsVisibleTo("Not.DnDGen.Infrastructure")]
[assembly: InternalsVisibleTo("Also.Not.DnDGen.Infrastructure")]
namespace DnDGen.Infrastructure.Generators
{
    public interface Generator
    {
        int MaxAttempts { get; set; }

        T Generate<T>(Func<T> buildInstructions, Func<T, bool> isValid, Func<T> buildDefault, Func<T, string> failureDescription, string defaultDescription);
    }
}
