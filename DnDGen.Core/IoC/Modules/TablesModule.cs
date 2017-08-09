using DnDGen.Core.Tables;
using Ninject.Modules;

namespace DnDGen.Core.IoC.Modules
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