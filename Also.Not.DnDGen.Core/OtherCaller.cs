using Not.DnDGen.Core;
using System.Reflection;

namespace Also.Not.DnDGen.Core
{
    internal class OtherCaller
    {
        private readonly Caller caller;

        public OtherCaller(Caller caller)
        {
            this.caller = caller;
        }

        public Assembly Call()
        {
            return caller.Call();
        }
    }
}
