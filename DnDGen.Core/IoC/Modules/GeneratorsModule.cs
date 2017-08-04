using DnDGen.Core.Generators;
using DnDGen.Core.IoC.Providers;
using Ninject.Modules;

namespace DnDGen.Core.IoC.Modules
{
    internal class GeneratorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Generator>().To<IterativeGenerator>();
            Bind<JustInTimeFactory>().ToProvider<JustInTimeFactoryProvider>();
        }
    }
}