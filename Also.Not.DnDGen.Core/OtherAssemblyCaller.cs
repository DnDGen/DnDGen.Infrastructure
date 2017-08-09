using Not.DnDGen.Core;
using System.Reflection;

namespace Also.Not.DnDGen.Core
{
    internal class OtherAssemblyCaller
    {
        private readonly AssemblyCaller caller;

        public OtherAssemblyCaller(AssemblyCaller caller)
        {
            this.caller = caller;
        }

        public Assembly Call()
        {
            return caller.Call();
        }
    }
}
