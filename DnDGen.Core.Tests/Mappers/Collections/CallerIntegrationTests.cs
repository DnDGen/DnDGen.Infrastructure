using Also.Not.DnDGen.Core;
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
        [Inject]
        public OtherCollectionsCaller OtherCollectionsCaller { get; set; }

        [TestCase("Real Caller Value 1")]
        [TestCase("Real Caller Value 2", "Real Caller Subvalue 1", "Real Caller Subvalue 2")]
        public void MapTableViaCallerWithRealAssembly(string name, params string[] entries)
        {
            var collection = CollectionsCaller.Call("RealCallerCollectionTable", name);
            Assert.That(collection, Is.EquivalentTo(entries));
        }

        [TestCase("Real Caller Value 1")]
        [TestCase("Real Caller Value 2", "Real Caller Subvalue 1", "Real Caller Subvalue 2")]
        public void MapTableViaOtherCallerWithRealAssembly(string name, params string[] entries)
        {
            var collection = CollectionsCaller.Call("RealCallerCollectionTable", name);
            Assert.That(collection, Is.EquivalentTo(entries));
        }

        [TestCase("Not Value 1")]
        [TestCase("Not Value 2", "Not Subvalue 1", "Not Subvalue 2")]
        public void MapDuplicateTableViaCallerWithRealAssembly(string name, params string[] entries)
        {
            var collection = CollectionsCaller.Call("DuplicateCollectionTable", name);
            Assert.That(collection, Is.EquivalentTo(entries));
        }

        [TestCase("Also Not Value 1")]
        [TestCase("Also Not Value 2", "Also Not Subvalue 1", "Also Not Subvalue 2")]
        public void MapDuplicateTableViaOtherCallerWithRealAssembly(string name, params string[] entries)
        {
            var collection = OtherCollectionsCaller.Call("DuplicateCollectionTable", name);
            Assert.That(collection, Is.EquivalentTo(entries));
        }
    }
}
