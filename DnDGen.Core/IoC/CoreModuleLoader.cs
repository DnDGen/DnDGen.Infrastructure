using DnDGen.Core.IoC.Modules;
using Ninject;

namespace DnDGen.Core.IoC
{
    public class CoreModuleLoader
    {
        public void LoadModules(IKernel kernel)
        {
            kernel.Load<GeneratorsModule>();
            kernel.Load<SelectorsModule>();
            kernel.Load<MappersModule>();
            kernel.Load<TablesModule>();
        }
    }
}