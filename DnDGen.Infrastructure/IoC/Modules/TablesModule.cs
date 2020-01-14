using DnDGen.Infrastructure.Tables;
using Ninject.Modules;

namespace DnDGen.Infrastructure.IoC.Modules
{
    internal class TablesModule : NinjectModule
    {
        public override void Load()
        {
            Bind<StreamLoader>().To<EmbeddedResourceStreamLoader>();
            Bind<AssemblyLoader>().To<DomainAssemblyLoader>();
        }
    }
}