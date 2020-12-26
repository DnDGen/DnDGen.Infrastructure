using DnDGen.Infrastructure.IoC;
using DnDGen.Infrastructure.Mappers.Collections;
using DnDGen.Infrastructure.Tests.Integration.Tables;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration.Mappers.Collections
{
    [TestFixture]
    public class CollectionsMapperTests : IntegrationTests
    {
        private CollectionMapper collectionsMapper;

        [SetUp]
        public void Setup()
        {
            collectionsMapper = GetNewInstanceOf<CollectionMapper>();
        }

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var coreModuleLoader = new InfrastructureModuleLoader();
            coreModuleLoader.ReplaceAssemblyLoaderWith<NotInfrastructureAssemblyLoader>(kernel);
        }

        [TestCase("Test Selector Value 1")]
        [TestCase("Test Selector Value 2", "Test Selector Subvalue 1", "Test Selector Subvalue 2")]
        public void MapTableViaMapperWithTestAssembly(string name, params string[] entries)
        {
            var collectionTable = collectionsMapper.Map("TestSelectorCollectionTable");
            Assert.That(collectionTable[name], Is.EquivalentTo(entries));
        }
    }
}
