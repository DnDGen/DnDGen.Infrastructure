using DnDGen.Core.Generators;
using Ninject.Modules;

namespace DnDGen.Core.IoC.Modules
{
    internal class GeneratorsModule : NinjectModule
    {
        public override void Load()
        {
            Bind<Generator>().To<IterativeGenerator>();
        }
    }
}