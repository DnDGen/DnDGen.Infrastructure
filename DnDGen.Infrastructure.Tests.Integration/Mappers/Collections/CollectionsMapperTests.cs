using DnDGen.Infrastructure.Mappers.Collections;
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

        [TestCase("Test Selector Value 1")]
        [TestCase("Test Selector Value 2", "Test Selector Subvalue 1", "Test Selector Subvalue 2")]
        public void MapTableViaMapperWithTestAssembly(string name, params string[] entries)
        {
            var collectionTable = collectionsMapper.Map("DnDGen.Infrastructure.Other", "TestSelectorCollectionTable");
            Assert.That(collectionTable[name], Is.EquivalentTo(entries));
        }
    }
}
