using DnDGen.Infrastructure.IoC;
using Ninject;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration
{
    [TestFixture]
    public abstract class IntegrationTests
    {
        protected IKernel kernel;
        protected const string assemblyName = "DnDGen.Infrastructure.Tests.Integration";


        [OneTimeSetUp]
        public void IntegrationTestsFixtureSetup()
        {
            kernel = new StandardKernel();

            var infrastructureModuleLoader = new InfrastructureModuleLoader();
            infrastructureModuleLoader.LoadModules(kernel);
        }

        protected T GetNewInstanceOf<T>()
        {
            return kernel.Get<T>();
        }

        protected T GetNewInstanceOf<T>(string name)
        {
            return kernel.Get<T>(name);
        }
    }
}
