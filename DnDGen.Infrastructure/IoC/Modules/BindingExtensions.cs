using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Selectors.Percentiles;
using Ninject;
using System.Linq;

namespace DnDGen.Infrastructure.IoC.Modules
{
    public static class BindingExtensions
    {
        public static IKernel BindDataSelection<T>(this IKernel kernel)
            where T : DataSelection<T>, new()
        {
            var bindings = kernel.GetBindings(typeof(ICollectionDataSelector<T>));
            if (bindings.Any())
                return kernel;

            kernel.Bind<ICollectionDataSelector<T>>().To<CollectionDataSelector<T>>();
            kernel.Bind<IPercentileDataSelector<T>>().To<PercentileDataSelector<T>>();

            return kernel;
        }
    }
}
