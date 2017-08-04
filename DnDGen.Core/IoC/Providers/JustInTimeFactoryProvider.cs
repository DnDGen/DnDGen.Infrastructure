using DnDGen.Core.Generators;
using Ninject.Activation;

namespace DnDGen.Core.IoC.Providers
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
