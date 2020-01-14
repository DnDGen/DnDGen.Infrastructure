using DnDGen.Infrastructure.IoC.Modules;
using DnDGen.Infrastructure.Tables;
using Ninject;

namespace DnDGen.Infrastructure.IoC
{
    public class InfrastructureModuleLoader
    {
        public void LoadModules(IKernel kernel)
        {
            kernel.Load<GeneratorsModule>();
            kernel.Load<SelectorsModule>();
            kernel.Load<MappersModule>();
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