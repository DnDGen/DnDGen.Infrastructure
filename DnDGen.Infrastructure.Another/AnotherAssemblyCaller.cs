using DnDGen.Infrastructure.Other;
using System.Reflection;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("DnDGen.Infrastructure.Tests.Unit")]
namespace DnDGen.Infrastructure.Another
{
    internal class AnotherAssemblyCaller
    {
        private readonly OtherAssemblyCaller caller;

        public AnotherAssemblyCaller(OtherAssemblyCaller caller)
        {
            this.caller = caller;
        }

        public Assembly Call()
        {
            return caller.Call();
        }
    }
}
