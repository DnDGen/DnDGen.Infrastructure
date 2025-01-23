using DnDGen.Infrastructure.IoC.Modules;
using DnDGen.Infrastructure.Tests.Integration.Models;
using Ninject.Modules;

namespace DnDGen.Infrastructure.Tests.Integration.IoC.Modules
{
    internal class TestSelectorsModule : NinjectModule
    {
        public override void Load()
        {
            Kernel.BindDataSelection<TestDataSelection>();
            Kernel.BindDataSelection<OtherTestDataSelection>();
            Kernel.BindDataSelection<IncrementingDataSelection>();
        }
    }
}