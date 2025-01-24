using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.RollGen;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Unit.Selectors.Percentiles
{
    [TestFixture]
    internal class PercentileTypeAndAmountSelectorTests
    {
        private const string tableName = "table name";
        private const string assemblyName = "assembly name";

        private IPercentileTypeAndAmountSelector percentileTypeAndAmountSelector;
        private Mock<IPercentileDataSelector<TypeAndAmountDataSelection>> mockPercentileDataSelector;
        private Mock<Dice> mockDice;

        [SetUp]
        public void Setup()
        {
            mockPercentileDataSelector = new Mock<IPercentileDataSelector<TypeAndAmountDataSelection>>();
            mockDice = new Mock<Dice>();
            percentileTypeAndAmountSelector = new PercentileTypeAndAmountSelector(mockPercentileDataSelector.Object, mockDice.Object);
        }

        [Test]
        public void SelectFrom_ReturnsTypeAndAmountSelection()
        {
            var typeAndAmountSelection = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount" };
            mockPercentileDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName))
                .Returns(typeAndAmountSelection);

            mockDice.Setup(d => d.Roll("my amount").AsSum<double>()).Returns(9266);

            var result = percentileTypeAndAmountSelector.SelectFrom(assemblyName, tableName);
            Assert.That(result, Is.EqualTo(typeAndAmountSelection));
            Assert.That(result.Type, Is.EqualTo("my type"));
            Assert.That(result.Roll, Is.EqualTo("my amount"));
            Assert.That(result.Amount, Is.EqualTo(9266));
            Assert.That(result.AmountAsDouble, Is.EqualTo(9266));
        }

        [Test]
        public void SelectFrom_ReturnsTypeAndAmountSelection_Double()
        {
            var typeAndAmountSelection = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount" };
            mockPercentileDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName))
                .Returns(typeAndAmountSelection);

            mockDice.Setup(d => d.Roll("my amount").AsSum<double>()).Returns(90.210);

            var result = percentileTypeAndAmountSelector.SelectFrom(assemblyName, tableName);
            Assert.That(result, Is.EqualTo(typeAndAmountSelection));
            Assert.That(result.Type, Is.EqualTo("my type"));
            Assert.That(result.Roll, Is.EqualTo("my amount"));
            Assert.That(result.Amount, Is.EqualTo(90));
            Assert.That(result.AmountAsDouble, Is.EqualTo(90.210));
        }

        [Test]
        public void SelectFrom_RollsEveryCall()
        {
            var typeAndAmountSelection = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount" };
            mockPercentileDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName))
                .Returns(typeAndAmountSelection);

            mockDice
                .SetupSequence(d => d.Roll("my amount").AsSum<double>())
                .Returns(9266)
                .Returns(90.210);

            var result = percentileTypeAndAmountSelector.SelectFrom(assemblyName, tableName);
            Assert.That(result, Is.EqualTo(typeAndAmountSelection));
            Assert.That(result.Type, Is.EqualTo("my type"));
            Assert.That(result.Roll, Is.EqualTo("my amount"));
            Assert.That(result.Amount, Is.EqualTo(9266));
            Assert.That(result.AmountAsDouble, Is.EqualTo(9266));

            result = percentileTypeAndAmountSelector.SelectFrom(assemblyName, tableName);
            Assert.That(result, Is.EqualTo(typeAndAmountSelection));
            Assert.That(result.Type, Is.EqualTo("my type"));
            Assert.That(result.Roll, Is.EqualTo("my amount"));
            Assert.That(result.Amount, Is.EqualTo(90));
            Assert.That(result.AmountAsDouble, Is.EqualTo(90.210));
        }

        [Test]
        public void SelectAllFrom_ReturnsTypeAndAmountSelections()
        {
            var typeAndAmountSelections = new[]
            {
                new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount" },
                new TypeAndAmountDataSelection { Type = "my other type", Roll = "my other amount" },
            };
            mockPercentileDataSelector
                .Setup(s => s.SelectAllFrom(assemblyName, tableName))
                .Returns(typeAndAmountSelections);

            mockDice.Setup(d => d.Roll("my amount").AsSum<double>()).Returns(9266);
            mockDice.Setup(d => d.Roll("my other amount").AsSum<double>()).Returns(4.2);

            var results = percentileTypeAndAmountSelector.SelectAllFrom(assemblyName, tableName).ToList();
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.Not.Null);
            Assert.That(results[0].Type, Is.EqualTo("my type"));
            Assert.That(results[0].Roll, Is.EqualTo("my amount"));
            Assert.That(results[0].Amount, Is.EqualTo(9266));
            Assert.That(results[0].AmountAsDouble, Is.EqualTo(9266));
            Assert.That(results[1], Is.Not.Null);
            Assert.That(results[1].Type, Is.EqualTo("my other type"));
            Assert.That(results[1].Roll, Is.EqualTo("my other amount"));
            Assert.That(results[1].Amount, Is.EqualTo(4));
            Assert.That(results[1].AmountAsDouble, Is.EqualTo(4.2));
        }

        [Test]
        public void SelectAllFrom_RollsEveryCall()
        {
            var typeAndAmountSelections = new[]
            {
                new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount" },
                new TypeAndAmountDataSelection { Type = "my other type", Roll = "my other amount" },
            };
            mockPercentileDataSelector
                .Setup(s => s.SelectAllFrom(assemblyName, tableName))
                .Returns(typeAndAmountSelections);

            mockDice
                .SetupSequence(d => d.Roll("my amount").AsSum<double>())
                .Returns(9266)
                .Returns(90.210);
            mockDice
                .SetupSequence(d => d.Roll("my other amount").AsSum<double>())
                .Returns(4.2)
                .Returns(600);

            var results = percentileTypeAndAmountSelector.SelectAllFrom(assemblyName, tableName).ToList();
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.Not.Null);
            Assert.That(results[0].Type, Is.EqualTo("my type"));
            Assert.That(results[0].Roll, Is.EqualTo("my amount"));
            Assert.That(results[0].Amount, Is.EqualTo(9266));
            Assert.That(results[0].AmountAsDouble, Is.EqualTo(9266));
            Assert.That(results[1], Is.Not.Null);
            Assert.That(results[1].Type, Is.EqualTo("my other type"));
            Assert.That(results[1].Roll, Is.EqualTo("my other amount"));
            Assert.That(results[1].Amount, Is.EqualTo(4));
            Assert.That(results[1].AmountAsDouble, Is.EqualTo(4.2));

            results = percentileTypeAndAmountSelector.SelectAllFrom(assemblyName, tableName).ToList();
            Assert.That(results.Count, Is.EqualTo(2));
            Assert.That(results[0], Is.Not.Null);
            Assert.That(results[0].Type, Is.EqualTo("my type"));
            Assert.That(results[0].Roll, Is.EqualTo("my amount"));
            Assert.That(results[0].Amount, Is.EqualTo(90));
            Assert.That(results[0].AmountAsDouble, Is.EqualTo(90.210));
            Assert.That(results[1], Is.Not.Null);
            Assert.That(results[1].Type, Is.EqualTo("my other type"));
            Assert.That(results[1].Roll, Is.EqualTo("my other amount"));
            Assert.That(results[1].Amount, Is.EqualTo(600));
            Assert.That(results[1].AmountAsDouble, Is.EqualTo(600));
        }
    }
}
