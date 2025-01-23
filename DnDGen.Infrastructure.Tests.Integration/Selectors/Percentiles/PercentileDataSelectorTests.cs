using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.Infrastructure.Tests.Integration.IoC.Modules;
using DnDGen.Infrastructure.Tests.Integration.Models;
using Ninject;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Integration.Selectors.Percentiles
{
    [TestFixture]
    public class PercentileDataSelectorTests : IntegrationTests
    {
        private IPercentileDataSelector<TestDataSelection> percentileSelector;

        [OneTimeSetUp]
        public void SelectorsSetup()
        {
            kernel.Load<TestSelectorsModule>();
        }

        [SetUp]
        public void Setup()
        {
            percentileSelector = GetNewInstanceOf<IPercentileDataSelector<TestDataSelection>>();
        }

        [Test]
        [Repeat(1000)]
        public void SelectFromTable()
        {
            var result = percentileSelector.SelectFrom(assemblyName, "DataPercentileTable");
            if (result.Name == "Karl Speer")
                Assert.That(result.Age, Is.EqualTo(35));
            else if (result.Name == "Christine Gnieski")
                Assert.That(result.Age, Is.EqualTo(33));
            else if (result.Name == "Hugo Speer")
                Assert.That(result.Age, Is.EqualTo(2));
            else if (result.Name == "Busy Bee")
                Assert.That(result.Age, Is.EqualTo(0));
            else if (result.Name == string.Empty)
                Assert.That(result.Age, Is.Zero);
            else
                Assert.Fail($"An unknown result of [{result.Name},{result.Age}] was returned");
        }

        [Test]
        [Repeat(1000)]
        public void SelectFromTable_Reparse()
        {
            IncrementingDataSelection.MapCount = 0;
            var percentileSelector = GetNewInstanceOf<IPercentileDataSelector<IncrementingDataSelection>>();

            var result = percentileSelector.SelectFrom(assemblyName, "DataPercentileTable");
            if (result.Name == "Karl Speer")
                Assert.That(result.Age, Is.EqualTo(35 + 1));
            else if (result.Name == "Christine Gnieski")
                Assert.That(result.Age, Is.EqualTo(33 + 1));
            else if (result.Name == "Hugo Speer")
                Assert.That(result.Age, Is.EqualTo(2 + 1));
            else if (result.Name == "Busy Bee")
                Assert.That(result.Age, Is.EqualTo(0 + 1));
            else if (result.Name == string.Empty)
                Assert.That(result.Age, Is.EqualTo(0 + 1));
            else
                Assert.Fail($"An unknown result of [{result.Name},{result.Age}] was returned");

            result = percentileSelector.SelectFrom(assemblyName, "DataPercentileTable");
            if (result.Name == "Karl Speer")
                Assert.That(result.Age, Is.EqualTo(35 + 2));
            else if (result.Name == "Christine Gnieski")
                Assert.That(result.Age, Is.EqualTo(33 + 2));
            else if (result.Name == "Hugo Speer")
                Assert.That(result.Age, Is.EqualTo(2 + 2));
            else if (result.Name == "Busy Bee")
                Assert.That(result.Age, Is.EqualTo(0 + 2));
            else if (result.Name == string.Empty)
                Assert.That(result.Age, Is.EqualTo(0 + 2));
            else
                Assert.Fail($"An unknown result of [{result.Name},{result.Age}] was returned");
        }

        [Test]
        public void SelectAllFromTable()
        {
            var results = percentileSelector.SelectAllFrom(assemblyName, "DataPercentileTable").ToArray();
            Assert.That(results, Is.All.Not.Null);
            Assert.That(results.Length, Is.EqualTo(5));
            Assert.That(results[0].Name, Is.EqualTo("Busy Bee"));
            Assert.That(results[0].Age, Is.EqualTo(0));
            Assert.That(results[1].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(results[1].Age, Is.EqualTo(2));
            Assert.That(results[2].Name, Is.Empty);
            Assert.That(results[2].Age, Is.EqualTo(0));
            Assert.That(results[3].Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(results[3].Age, Is.EqualTo(33));
            Assert.That(results[4].Name, Is.EqualTo("Karl Speer"));
            Assert.That(results[4].Age, Is.EqualTo(35));
        }

        [Test]
        public void SelectAllFromTable_Reparse()
        {
            IncrementingDataSelection.MapCount = 0;
            var percentileSelector = GetNewInstanceOf<IPercentileDataSelector<IncrementingDataSelection>>();

            var results = percentileSelector.SelectAllFrom(assemblyName, "DataPercentileTable").ToArray();
            Assert.That(results, Is.All.Not.Null);
            Assert.That(results.Length, Is.EqualTo(5));
            Assert.That(results[0].Name, Is.EqualTo("Busy Bee"));
            Assert.That(results[0].Age, Is.EqualTo(0 + 1));
            Assert.That(results[1].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(results[1].Age, Is.EqualTo(2 + 2));
            Assert.That(results[2].Name, Is.Empty);
            Assert.That(results[2].Age, Is.EqualTo(0 + 3));
            Assert.That(results[3].Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(results[3].Age, Is.EqualTo(33 + 4));
            Assert.That(results[4].Name, Is.EqualTo("Karl Speer"));
            Assert.That(results[4].Age, Is.EqualTo(35 + 5));

            results = percentileSelector.SelectAllFrom(assemblyName, "DataPercentileTable").ToArray();
            Assert.That(results, Is.All.Not.Null);
            Assert.That(results.Length, Is.EqualTo(5));
            Assert.That(results[0].Name, Is.EqualTo("Busy Bee"));
            Assert.That(results[0].Age, Is.EqualTo(0 + 6));
            Assert.That(results[1].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(results[1].Age, Is.EqualTo(2 + 7));
            Assert.That(results[2].Name, Is.Empty);
            Assert.That(results[2].Age, Is.EqualTo(0 + 8));
            Assert.That(results[3].Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(results[3].Age, Is.EqualTo(33 + 9));
            Assert.That(results[4].Name, Is.EqualTo("Karl Speer"));
            Assert.That(results[4].Age, Is.EqualTo(35 + 10));
        }
    }
}