using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.IoC.Providers;
using Ninject.Modules;

namespace DnDGen.Infrastructure.IoC.Modules
{
    internal class GeneratorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<JustInTimeFactory>().ToProvider<JustInTimeFactoryProvider>();
        }
    }
}