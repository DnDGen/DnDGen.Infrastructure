using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Tests.Unit")]
[assembly: InternalsVisibleTo("DynamicProxyGenAssembly2")]
[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Tests.Integration")]
[assembly: InternalsVisibleTo("Not.DnDGen.Infrastructure")]
[assembly: InternalsVisibleTo("Also.Not.DnDGen.Infrastructure")]
namespace DnDGen.Infrastructure.Generators
{
    public interface JustInTimeFactory
    {
        T Build<T>();
        T Build<T>(string name);
    }
}
