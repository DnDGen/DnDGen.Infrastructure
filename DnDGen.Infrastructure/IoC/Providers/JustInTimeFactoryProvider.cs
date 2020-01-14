using DnDGen.Infrastructure.Generators;
using Ninject.Activation;

namespace DnDGen.Infrastructure.IoC.Providers
{
    class JustInTimeFactoryProvider : Provider<JustInTimeFactory>
    {
        protected override JustInTimeFactory CreateInstance(IContext context)
        {
            var factory = new NinjectJustInTimeFactory(context.Kernel);

            return factory;
        }
    }
}
