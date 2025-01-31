using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Tests.Unit.Models;
using Moq;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Unit.Selectors.Collections
{
    [TestFixture]
    public class CollectionDataSelectorTests
    {
        private const string tableName = "table name";
        private const string assemblyName = "my assembly";

        private ICollectionDataSelector<FakeDataSelection> collectionDataSelector;
        private Mock<ICollectionSelector> mockCollectionSelector;

        [SetUp]
        public void Setup()
        {
            mockCollectionSelector = new Mock<ICollectionSelector>();
            collectionDataSelector = new CollectionDataSelector<FakeDataSelection>(mockCollectionSelector.Object);
        }

        [Test]
        public void SelectFrom_ReturnsEmpty()
        {
            mockCollectionSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([]);

            var result = collectionDataSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.Not.Null.And.Empty);
        }

        [Test]
        public void SelectFrom_ReturnsValueParsedAsDataSelection()
        {
            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            mockCollectionSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([data1]);

            var result = collectionDataSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
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
            mockCollectionSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([data1, data2]);

            var result = collectionDataSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
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

            var collectionDataSelector = new CollectionDataSelector<IncrementingDataSelection>(mockCollectionSelector.Object);
            var dummyData = new IncrementingDataSelection { Name = "Karl Speer", Age = 35 };

            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            var data2 = DataHelper.Parse(new FakeDataSelection { Name = "Hugo Speer", Age = 2 });
            mockCollectionSelector
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
        public void SelectOneFrom_ThrowsException_WhenCollectionEmpty()
        {
            mockCollectionSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([]);

            Assert.That(() => collectionDataSelector.SelectOneFrom(assemblyName, tableName, "my collection"),
                Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void SelectOneFrom_ReturnsValueParsedAsDataSelection()
        {
            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            mockCollectionSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([data1]);

            var result = collectionDataSelector.SelectOneFrom(assemblyName, tableName, "my collection");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Karl Speer"));
            Assert.That(result.Age, Is.EqualTo(35));
        }

        [Test]
        public void SelectOneFrom_ThrowsException_WhenCollectionHasMultipleValues()
        {
            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            var data2 = DataHelper.Parse(new FakeDataSelection { Name = "Hugo Speer", Age = 2 });
            mockCollectionSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([data1, data2]);

            Assert.That(() => collectionDataSelector.SelectOneFrom(assemblyName, tableName, "my collection"),
                Throws.InstanceOf<InvalidOperationException>());
        }

        [Test]
        public void SelectOneFrom_ParsesDataEveryCall()
        {
            IncrementingDataSelection.MapCount = 9266;

            var collectionDataSelector = new CollectionDataSelector<IncrementingDataSelection>(mockCollectionSelector.Object);
            var dummyData = new IncrementingDataSelection { Name = "Karl Speer", Age = 35 };

            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            mockCollectionSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([data1]);

            var result = collectionDataSelector.SelectOneFrom(assemblyName, tableName, "my collection");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Karl Speer"));
            Assert.That(result.Age, Is.EqualTo(35 + 9267));

            result = collectionDataSelector.SelectOneFrom(assemblyName, tableName, "my collection");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Karl Speer"));
            Assert.That(result.Age, Is.EqualTo(35 + 9268));
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
            mockCollectionSelector
                .Setup(s => s.SelectAllFrom(assemblyName, tableName))
                .Returns(table);

            var results = collectionDataSelector.SelectAllFrom(assemblyName, tableName);
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

            var collectionDataSelector = new CollectionDataSelector<IncrementingDataSelection>(mockCollectionSelector.Object);
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
            mockCollectionSelector
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

        [TestCase(true)]
        [TestCase(false)]
        public void IsCollection_ReturnsInnerResult(bool inner)
        {
            mockCollectionSelector
                .Setup(s => s.IsCollection(assemblyName, tableName, "my collection"))
                .Returns(inner);

            var result = collectionDataSelector.IsCollection(assemblyName, tableName, "my collection");
            Assert.That(result, Is.EqualTo(inner));
        }

        [Test]
        public void SelectRandomFrom_ReturnsParsedRandomValue()
        {
            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            mockCollectionSelector
                .Setup(s => s.SelectRandomFrom(assemblyName, tableName, "my collection"))
                .Returns(data1);

            var result = collectionDataSelector.SelectRandomFrom(assemblyName, tableName, "my collection");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Karl Speer"));
            Assert.That(result.Age, Is.EqualTo(35));
        }

        [Test]
        public void SelectRandomFrom_ParsesDataEveryCall()
        {
            IncrementingDataSelection.MapCount = 9266;

            var collectionDataSelector = new CollectionDataSelector<IncrementingDataSelection>(mockCollectionSelector.Object);
            var dummyData = new IncrementingDataSelection { Name = "Karl Speer", Age = 35 };

            var data1 = DataHelper.Parse(new FakeDataSelection { Name = "Karl Speer", Age = 35 });
            mockCollectionSelector
                .Setup(s => s.SelectRandomFrom(assemblyName, tableName, "my collection"))
                .Returns(data1);

            var result = collectionDataSelector.SelectRandomFrom(assemblyName, tableName, "my collection");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Karl Speer"));
            Assert.That(result.Age, Is.EqualTo(35 + 9267));

            result = collectionDataSelector.SelectRandomFrom(assemblyName, tableName, "my collection");
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Karl Speer"));
            Assert.That(result.Age, Is.EqualTo(35 + 9268));
        }
    }
}