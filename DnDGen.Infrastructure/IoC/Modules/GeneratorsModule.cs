using DnDGen.Infrastructure.Generators;
using DnDGen.Infrastructure.IoC.Providers;
using Ninject.Modules;

namespace DnDGen.Infrastructure.IoC.Modules
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