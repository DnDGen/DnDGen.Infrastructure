using DnDGen.Core.IoC;
using DnDGen.Core.Mappers.Collections;
using Ninject;
using NUnit.Framework;
using Tests.Integration.DnDGen.Core.Tables;

namespace Tests.Integration.DnDGen.Core.Mappers.Collections
{
    [TestFixture]
    public class CollectionsMapperTests : IntegrationTests
    {
        [Inject]
        public CollectionMapper CollectionsMapper { get; set; }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var coreModuleLoader = new CoreModuleLoader();
            coreModuleLoader.ReplaceAssemblyLoaderWith<NotCoreAssemblyLoader>(kernel);
        }

        [TestCase("Test Selector Value 1")]
        [TestCase("Test Selector Value 2", "Test Selector Subvalue 1", "Test Selector Subvalue 2")]
        public void MapTableViaMapperWithTestAssembly(string name, params string[] entries)
        {
            var collectionTable = CollectionsMapper.Map("TestSelectorCollectionTable");
            Assert.That(collectionTable[name], Is.EquivalentTo(entries));
        }
    }
}
