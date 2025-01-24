using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Tests.Unit.Models;
using DnDGen.RollGen;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Unit.Selectors.Collections
{
    [TestFixture]
    public class CollectionTypeAndAmountSelectorTests
    {
        private const string tableName = "table name";
        private const string assemblyName = "my assembly";

        private ICollectionTypeAndAmountSelector collectionTypeAndAmountSelector;
        private Mock<ICollectionDataSelector<TypeAndAmountDataSelection>> mockCollectionDataSelector;
        private Mock<Dice> mockDice;

        [SetUp]
        public void Setup()
        {
            mockCollectionDataSelector = new Mock<ICollectionDataSelector<TypeAndAmountDataSelection>>();
            mockDice = new Mock<Dice>();
            collectionTypeAndAmountSelector = new CollectionTypeAndAmountSelector(mockCollectionDataSelector.Object, mockDice.Object);
        }

        [Test]
        public void SelectFrom_ReturnsEmpty()
        {
            mockCollectionDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([]);

            var result = collectionTypeAndAmountSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.Not.Null.And.Empty);
        }

        [Test]
        public void SelectFrom_ReturnsValueParsedAsDataSelection()
        {
            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            mockCollectionDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([data1]);

            var result = collectionTypeAndAmountSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(1));
            Assert.That(result[0].Name, Is.EqualTo("Karl Speer"));
            Assert.That(result[0].Age, Is.EqualTo(35));
        }

        [Test]
        public void SelectFrom_ReturnsValuesParsedAsDataSelection()
        {
            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            var data2 = DataHelper.Parse(new FakeDataSelection { Name = "Hugo Speer", Age = 2 });
            mockCollectionDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([data1, data2]);

            var result = collectionTypeAndAmountSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Karl Speer"));
            Assert.That(result[0].Age, Is.EqualTo(35));
            Assert.That(result[1].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(result[1].Age, Is.EqualTo(2));
        }

        [Test]
        public void SelectFrom_ParsesDataEveryCall()
        {
            IncrementingDataSelection.MapCount = 9266;

            var collectionDataSelector = new CollectionDataSelector<IncrementingDataSelection>(mockCollectionDataSelector.Object);
            var dummyData = new IncrementingDataSelection { Name = "Karl Speer", Age = 35 };

            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            var data2 = DataHelper.Parse(new FakeDataSelection { Name = "Hugo Speer", Age = 2 });
            mockCollectionDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([data1, data2]);

            var result = collectionDataSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Karl Speer"));
            Assert.That(result[0].Age, Is.EqualTo(35 + 9267));
            Assert.That(result[1].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(result[1].Age, Is.EqualTo(2 + 9268));

            result = collectionDataSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Length, Is.EqualTo(2));
            Assert.That(result[0].Name, Is.EqualTo("Karl Speer"));
            Assert.That(result[0].Age, Is.EqualTo(35 + 9269));
            Assert.That(result[1].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(result[1].Age, Is.EqualTo(2 + 9270));
        }

        [Test]
        public void SelectAllFrom_ReturnsValuesParsedAsDataSelections()
        {
            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            var data2 = DataHelper.Parse(new FakeDataSelection { Name = "Christine Gnieski", Age = 33 });
            var data3 = DataHelper.Parse(new FakeDataSelection { Name = "Hugo Speer", Age = 2 });
            var data4 = DataHelper.Parse(new FakeDataSelection { Name = "Busy Bee", Age = 0 });
            var table = new Dictionary<string, IEnumerable<string>>
            {
                ["My Family"] = [data1, data2, data3],
                ["Enemies"] = [],
                ["Upcoming"] = [data4],
            };
            mockCollectionDataSelector
                .Setup(s => s.SelectAllFrom(assemblyName, tableName))
                .Returns(table);

            var results = collectionTypeAndAmountSelector.SelectAllFrom(assemblyName, tableName);
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results.Keys, Is.EqualTo(table.Keys));
            Assert.That(results["My Family"], Is.All.Not.Null);
            Assert.That(results["My Family"].Count(), Is.EqualTo(3));
            Assert.That(results["My Family"].ElementAt(0).Name, Is.EqualTo("Karl Speer"));
            Assert.That(results["My Family"].ElementAt(0).Age, Is.EqualTo(35));
            Assert.That(results["My Family"].ElementAt(1).Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(results["My Family"].ElementAt(1).Age, Is.EqualTo(33));
            Assert.That(results["My Family"].ElementAt(2).Name, Is.EqualTo("Hugo Speer"));
            Assert.That(results["My Family"].ElementAt(2).Age, Is.EqualTo(2));
            Assert.That(results["Enemies"], Is.Empty);
            Assert.That(results["Upcoming"], Is.All.Not.Null);
            Assert.That(results["Upcoming"].Count(), Is.EqualTo(1));
            Assert.That(results["Upcoming"].ElementAt(0).Name, Is.EqualTo("Busy Bee"));
            Assert.That(results["Upcoming"].ElementAt(0).Age, Is.EqualTo(0));
        }

        [Test]
        public void SelectAllFrom_ParsesDataEveryCall()
        {
            IncrementingDataSelection.MapCount = 9266;

            var collectionDataSelector = new CollectionDataSelector<IncrementingDataSelection>(mockCollectionDataSelector.Object);
            var data1 = DataHelper.Parse(new IncrementingDataSelection { Name = "Karl Speer", Age = 35 });
            var data2 = DataHelper.Parse(new IncrementingDataSelection { Name = "Christine Gnieski", Age = 33 });
            var data3 = DataHelper.Parse(new IncrementingDataSelection { Name = "Hugo Speer", Age = 2 });
            var data4 = DataHelper.Parse(new IncrementingDataSelection { Name = "Busy Bee", Age = 0 });
            var table = new Dictionary<string, IEnumerable<string>>
            {
                ["My Family"] = [data1, data2, data3],
                ["Enemies"] = [],
                ["Upcoming"] = [data4],
            };
            mockCollectionDataSelector
                .Setup(s => s.SelectAllFrom(assemblyName, tableName))
                .Returns(table);

            var results = collectionDataSelector.SelectAllFrom(assemblyName, tableName);
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results.Keys, Is.EqualTo(table.Keys));
            Assert.That(results["My Family"], Is.All.Not.Null);
            Assert.That(results["My Family"].Count(), Is.EqualTo(3));
            Assert.That(results["My Family"].ElementAt(0).Name, Is.EqualTo("Karl Speer"));
            Assert.That(results["My Family"].ElementAt(0).Age, Is.EqualTo(35 + 9267));
            Assert.That(results["My Family"].ElementAt(1).Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(results["My Family"].ElementAt(1).Age, Is.EqualTo(33 + 9268));
            Assert.That(results["My Family"].ElementAt(2).Name, Is.EqualTo("Hugo Speer"));
            Assert.That(results["My Family"].ElementAt(2).Age, Is.EqualTo(2 + 9269));
            Assert.That(results["Enemies"], Is.Empty);
            Assert.That(results["Upcoming"], Is.All.Not.Null);
            Assert.That(results["Upcoming"].Count(), Is.EqualTo(1));
            Assert.That(results["Upcoming"].ElementAt(0).Name, Is.EqualTo("Busy Bee"));
            Assert.That(results["Upcoming"].ElementAt(0).Age, Is.EqualTo(0 + 9270));

            results = collectionDataSelector.SelectAllFrom(assemblyName, tableName);
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results.Keys, Is.EqualTo(table.Keys));
            Assert.That(results["My Family"], Is.All.Not.Null);
            Assert.That(results["My Family"].Count(), Is.EqualTo(3));
            Assert.That(results["My Family"].ElementAt(0).Name, Is.EqualTo("Karl Speer"));
            Assert.That(results["My Family"].ElementAt(0).Age, Is.EqualTo(35 + 9271));
            Assert.That(results["My Family"].ElementAt(1).Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(results["My Family"].ElementAt(1).Age, Is.EqualTo(33 + 9272));
            Assert.That(results["My Family"].ElementAt(2).Name, Is.EqualTo("Hugo Speer"));
            Assert.That(results["My Family"].ElementAt(2).Age, Is.EqualTo(2 + 9273));
            Assert.That(results["Enemies"], Is.Empty);
            Assert.That(results["Upcoming"], Is.All.Not.Null);
            Assert.That(results["Upcoming"].Count(), Is.EqualTo(1));
            Assert.That(results["Upcoming"].ElementAt(0).Name, Is.EqualTo("Busy Bee"));
            Assert.That(results["Upcoming"].ElementAt(0).Age, Is.EqualTo(0 + 9274));
        }
    }
}