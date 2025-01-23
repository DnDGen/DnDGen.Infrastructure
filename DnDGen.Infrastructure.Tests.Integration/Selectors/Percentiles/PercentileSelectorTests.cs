using DnDGen.Infrastructure.Selectors.Percentiles;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration.Selectors.Percentiles
{
    [TestFixture]
    public class PercentileSelectorTests : IntegrationTests
    {
        private IPercentileSelector percentileSelector;

        [SetUp]
        public void Setup()
        {
            percentileSelector = GetNewInstanceOf<IPercentileSelector>();
        }

        [Test]
        [Repeat(1000)]
        public void SelectFromTable()
        {
            var result = percentileSelector.SelectFrom(assemblyName, "StringPercentileTable");
            Assert.That(result, Is.EqualTo("one")
                .Or.EqualTo("two")
                .Or.EqualTo("three")
                .Or.Empty
                .Or.EqualTo("eleven to one hundred"));
        }

        [Test]
        [Repeat(1000)]
        public void SelectIntFromTable()
        {
            var result = percentileSelector.SelectFrom<int>(assemblyName, "IntPercentileTable");
            Assert.That(result, Is.EqualTo(1)
                .Or.EqualTo(2)
                .Or.EqualTo(3)
                .Or.EqualTo(0)
                .Or.EqualTo(-89));
        }

        [Test]
        [Repeat(1000)]
        public void SelectBooleanFromTable()
        {
            var result = percentileSelector.SelectFrom<bool>(assemblyName, "BooleanPercentileTable");
            Assert.That(result, Is.True.Or.False);
        }

        [Test]
        public void SelectAllFromTable()
        {
            var results = percentileSelector.SelectAllFrom(assemblyName, "StringPercentileTable");
            Assert.That(results, Is.EquivalentTo(new[]
            {
                "one",
                "two",
                "three",
                string.Empty,
                "eleven to one hundred"
            }));
        }

        [Test]
        public void SelectAllIntFromTable()
        {
            var results = percentileSelector.SelectAllFrom<int>(assemblyName, "IntPercentileTable");
            Assert.That(results, Is.EquivalentTo(new[]
            {
                1,
                2,
                3,
                0,
                -89
            }));
        }

        [Test]
        public void SelectAllBooleanFromTable()
        {
            var results = percentileSelector.SelectAllFrom<bool>(assemblyName, "BooleanPercentileTable");
            Assert.That(results, Is.EquivalentTo(new[]
            {
                true,
                false
            }));
        }
    }
}