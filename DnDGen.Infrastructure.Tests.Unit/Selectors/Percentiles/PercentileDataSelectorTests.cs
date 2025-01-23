using DnDGen.Infrastructure.Helpers;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.Infrastructure.Tests.Unit.Models;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Unit.Selectors.Percentiles
{
    [TestFixture]
    internal class PercentileDataSelectorTests
    {
        private const string tableName = "table name";
        private const string assemblyName = "assembly name";

        private IPercentileDataSelector<FakeDataSelection> percentileDataSelector;
        private Mock<IPercentileSelector> mockPercentileSelector;
        private FakeDataSelection dummyData;

        [SetUp]
        public void Setup()
        {
            mockPercentileSelector = new Mock<IPercentileSelector>();
            percentileDataSelector = new PercentileDataSelector<FakeDataSelection>(mockPercentileSelector.Object);
            dummyData = new FakeDataSelection { Name = "Karl Speer", Age = 35 };
        }

        [Test]
        public void SelectFrom_ReturnsValueParsedAsDataSelection()
        {
            var data = DataHelper.Parse(dummyData);
            mockPercentileSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName))
                .Returns(data);

            var result = percentileDataSelector.SelectFrom(assemblyName, tableName);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo(dummyData.Name));
            Assert.That(result.Age, Is.EqualTo(dummyData.Age));
        }

        [Test]
        public void SelectFrom_ParsesDataEveryCall()
        {
            IncrementingDataSelection.MapCount = 9266;

            var percentileDataSelector = new PercentileDataSelector<IncrementingDataSelection>(mockPercentileSelector.Object);
            var dummyData = new IncrementingDataSelection { Name = "Karl Speer", Age = 35 };

            var data = DataHelper.Parse(dummyData);
            mockPercentileSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName))
                .Returns(data);

            var result = percentileDataSelector.SelectFrom(assemblyName, tableName);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Karl Speer"));
            Assert.That(result.Age, Is.EqualTo(35 + 9267));

            result = percentileDataSelector.SelectFrom(assemblyName, tableName);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Name, Is.EqualTo("Karl Speer"));
            Assert.That(result.Age, Is.EqualTo(35 + 9268));
        }

        [Test]
        public void SelectAllFrom_ReturnsValuesParsedAsDataSelections()
        {
            var data1 = DataHelper.Parse(dummyData);
            var data2 = DataHelper.Parse(new FakeDataSelection { Name = "Hugo Speer", Age = 2 });
            mockPercentileSelector
                .Setup(s => s.SelectAllFrom(assemblyName, tableName))
                .Returns([data1, data2]);

            var results = percentileDataSelector.SelectAllFrom(assemblyName, tableName).ToList();
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.Not.Null);
            Assert.That(results[0].Name, Is.EqualTo("Karl Speer"));
            Assert.That(results[0].Age, Is.EqualTo(35));
            Assert.That(results[1], Is.Not.Null);
            Assert.That(results[1].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(results[1].Age, Is.EqualTo(2));
        }

        [Test]
        public void SelectAllFrom_ParsesDataEveryCall()
        {
            IncrementingDataSelection.MapCount = 9266;

            var percentileDataSelector = new PercentileDataSelector<IncrementingDataSelection>(mockPercentileSelector.Object);
            var data1 = DataHelper.Parse(new IncrementingDataSelection { Name = "Karl Speer", Age = 35 });
            var data2 = DataHelper.Parse(new IncrementingDataSelection { Name = "Hugo Speer", Age = 2 });
            mockPercentileSelector
                .Setup(s => s.SelectAllFrom(assemblyName, tableName))
                .Returns([data1, data2]);

            var results = percentileDataSelector.SelectAllFrom(assemblyName, tableName).ToList();
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.Not.Null);
            Assert.That(results[0].Name, Is.EqualTo("Karl Speer"));
            Assert.That(results[0].Age, Is.EqualTo(35 + 9267));
            Assert.That(results[1], Is.Not.Null);
            Assert.That(results[1].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(results[1].Age, Is.EqualTo(2 + 9268));

            results = percentileDataSelector.SelectAllFrom(assemblyName, tableName).ToList();
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.Not.Null);
            Assert.That(results[0].Name, Is.EqualTo("Karl Speer"));
            Assert.That(results[0].Age, Is.EqualTo(35 + 9269));
            Assert.That(results[1], Is.Not.Null);
            Assert.That(results[1].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(results[1].Age, Is.EqualTo(2 + 9270));
        }
    }
}
