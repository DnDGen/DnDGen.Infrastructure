using DnDGen.Core.Tables;
using Ninject.Modules;

namespace DnDGen.Core.Tests.IoC.Modules
{
    internal class TestModule : NinjectModule
    {
        public override void Load()
        {
            Bind<AssemblyLoader>().To<TestAssemblyLoader>();
        }
    }
}
