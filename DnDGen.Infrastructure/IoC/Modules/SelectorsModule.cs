using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Ninject.Modules;

namespace DnDGen.Infrastructure.IoC.Modules
{
    internal class SelectorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPercentileSelector>().To<PercentileSelector>();

            Bind<ICollectionSelector>().To<CollectionSelector>().WhenInjectedInto<CollectionSelectorCachingProxy>();
            Bind<ICollectionSelector>().To<CollectionSelectorCachingProxy>().InSingletonScope();
        }
    }
}