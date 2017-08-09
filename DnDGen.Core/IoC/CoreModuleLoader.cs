using DnDGen.Core.IoC.Modules;
using DnDGen.Core.Tables;
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