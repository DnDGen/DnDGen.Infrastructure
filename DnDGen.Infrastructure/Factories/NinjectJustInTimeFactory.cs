using Ninject;

namespace DnDGen.Infrastructure.Factories
{
    internal class NinjectJustInTimeFactory(IKernel kernel) : JustInTimeFactory
    {
        public T Build<T>()
        {
            return kernel.Get<T>();
        }

        public T Build<T>(string name)
        {
            return kernel.Get<T>(name);
        }
    }
}
