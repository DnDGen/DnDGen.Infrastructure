using Ninject;
using Not.DnDGen.Core;
using NUnit.Framework;

namespace DnDGen.Core.Tests.Mappers.Collections
{
    [TestFixture]
    public class CallerIntegrationTests : IntegrationTests
    {
        [Inject]
        public CollectionsCaller CollectionsCaller { get; set; }

        [TestCase("Real Caller Value 1")]
        [TestCase("Real Caller Value 2", "Real Caller Subvalue 1", "Real Caller Subvalue 2")]
        public void MapTableViaCallerWithRealAssembly(string name, params string[] entries)
        {
            var collection = CollectionsCaller.Call("RealCallerCollectionTable", name);
            Assert.That(collection, Is.EquivalentTo(entries));
        }
    }
}
