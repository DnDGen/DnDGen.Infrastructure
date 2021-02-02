using DnDGen.Infrastructure.IoC.Modules;
using DnDGen.Infrastructure.Tables;
using DnDGen.RollGen.IoC;
using Ninject;
using System.Linq;

namespace DnDGen.Infrastructure.IoC
{
    public class InfrastructureModuleLoader
    {
        public void LoadModules(IKernel kernel)
        {
            //Dependencies
            var rollGenLoader = new RollGenModuleLoader();
            rollGenLoader.LoadModules(kernel);

            //Infrastructure
            var modules = kernel.GetModules();

            if (!modules.Any(m => m is GeneratorsModule))
                kernel.Load<GeneratorsModule>();

            if (!modules.Any(m => m is SelectorsModule))
                kernel.Load<SelectorsModule>();

            if (!modules.Any(m => m is MappersModule))
                kernel.Load<MappersModule>();

            if (!modules.Any(m => m is TablesModule))
                kernel.Load<TablesModule>();
        }

        public void ReplaceAssemblyLoaderWith<T>(IKernel kernel)
            where T : AssemblyLoader
        {
            var bindings = kernel.GetBindings(typeof(AssemblyLoader));
            foreach (var binding in bindings)
            {
                kernel.RemoveBinding(binding);
            }

            kernel.Bind<AssemblyLoader>().To<T>();
        }
    }
}