using Ninject.Modules;
using System;

namespace DnDGen.Infrastructure.Tests.Integration.IoC.Modules
{
    internal class TestSelectorsModule : NinjectModule
    {
        public override void Load()
        {
            throw new NotImplementedException("TODO: some fake data selectors, to validate that binding makes sense");
        }
    }
}