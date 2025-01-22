using DnDGen.Infrastructure.Factories;
using DnDGen.Infrastructure.IoC;
using DnDGen.RollGen;
using NUnit.Framework;
using System;

namespace DnDGen.Infrastructure.Tests.Integration.IoC
{
    [TestFixture]
    public class InfrastructureModuleLoaderTests : IoCTests
    {
        [Test]
        public void ModuleLoaderCanBeRunTwice()
        {
            //INFO: First time was in the IntegrationTest one-time setup
            var infrastructureLoader = new InfrastructureModuleLoader();
            infrastructureLoader.LoadModules(kernel);

            var justInTimeFactory = GetNewInstanceOf<JustInTimeFactory>();
            Assert.That(justInTimeFactory, Is.Not.Null);
        }

        [Test]
        public void ModuleLoaderLoadsRollGenDependency()
        {
            AssertNotSingleton<Dice>();
            AssertSingleton<Random>();
        }
    }
}
