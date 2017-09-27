using DnDGen.Core.Selectors.Collections;
using DnDGen.Core.Selectors.Percentiles;
using Ninject.Modules;

namespace DnDGen.Core.IoC.Modules
{
    internal class SelectorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPercentileSelector>().To<PercentileSelector>();

            Bind<ICollectionSelector>().To<CollectionSelector>().WhenInjectedInto<CollectionSelectorEventDecorator>();
            Bind<ICollectionSelector>().To<CollectionSelectorEventDecorator>();
        }
    }
}