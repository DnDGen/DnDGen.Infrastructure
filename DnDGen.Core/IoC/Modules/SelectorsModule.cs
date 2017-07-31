using DnDGen.Core.Selectors.Collections;
using DnDGen.Core.Selectors.Percentiles;
using Ninject.Modules;

namespace DnDGen.Core.IoC.Modules
{
    internal class SelectorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<IPercentileSelector>().To<PercentileSelector>().WhenInjectedInto<PercentileSelectorEventDecorator>();
            Bind<IPercentileSelector>().To<PercentileSelectorEventDecorator>();

            Bind<ICollectionsSelector>().To<CollectionsSelector>().WhenInjectedInto<CollectionsSelectorEventDecorator>();
            Bind<ICollectionsSelector>().To<CollectionsSelectorEventDecorator>();
        }
    }
}