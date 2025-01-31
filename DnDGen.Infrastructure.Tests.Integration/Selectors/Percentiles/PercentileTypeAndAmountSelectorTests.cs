using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Percentiles;
using DnDGen.RollGen;
using NUnit.Framework;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Integration.Selectors.Percentiles
{
    [TestFixture]
    public class PercentileTypeAndAmountSelectorTests : IntegrationTests
    {
        private IPercentileTypeAndAmountSelector percentileTypeAndAmountSelector;

        [SetUp]
        public void Setup()
        {
            percentileTypeAndAmountSelector = GetNewInstanceOf<IPercentileTypeAndAmountSelector>();
        }

        [Test]
        [Repeat(1000)]
        public void SelectFromTable()
        {
            var result = percentileTypeAndAmountSelector.SelectFrom(assemblyName, "TypeAndAmountPercentileTable");
            AssertTypeAndAmount(result);
        }

        private void AssertTypeAndAmount(TypeAndAmountDataSelection selection)
        {
            switch (selection.Type)
            {
                case "Low Treasure":
                    Assert.That(selection.Roll, Is.EqualTo("0.5"), selection.Type);
                    Assert.That(selection.Amount, Is.EqualTo(0), selection.Type);
                    Assert.That(selection.AmountAsDouble, Is.EqualTo(0.5), selection.Type);
                    break;
                case "High Treasure":
                    Assert.That(selection.Roll, Is.EqualTo("2"), selection.Type);
                    Assert.That(selection.Amount, Is.EqualTo(2), selection.Type);
                    Assert.That(selection.AmountAsDouble, Is.EqualTo(2), selection.Type);
                    break;
                case "":
                    Assert.That(selection.Roll, Is.EqualTo("0"), selection.Type);
                    Assert.That(selection.Amount, Is.EqualTo(0), selection.Type);
                    Assert.That(selection.AmountAsDouble, Is.EqualTo(0), selection.Type);
                    break;
                case "Big Gem":
                    Assert.That(selection.Roll, Is.EqualTo("1d4*1000"), selection.Type);
                    Assert.That(selection.Amount, Is.AnyOf([1000, 2000, 3000, 4000]), selection.Type);
                    Assert.That(selection.AmountAsDouble, Is.AnyOf([1000, 2000, 3000, 4000]), selection.Type);
                    break;
                case "Abilities":
                    Assert.That(selection.Roll, Is.EqualTo("3d6t1"), selection.Type);
                    Assert.That(selection.Amount, Is.InRange(6, 18), selection.Type);
                    Assert.That(selection.AmountAsDouble, Is.InRange(6, 18), selection.Type);
                    break;
                case "Lower Half":
                    var lowerRoll = RollHelper.GetRollWithMostEvenDistribution(1, 1_000_000, true, true);
                    Assert.That(selection.Roll, Is.EqualTo(lowerRoll), selection.Type);
                    Assert.That(selection.Amount, Is.InRange(1, 1_000_000), selection.Type);
                    Assert.That(selection.AmountAsDouble, Is.InRange(1, 1_000_000), selection.Type);
                    break;
                case "Upper Half":
                    var upperRoll = RollHelper.GetRollWithMostEvenDistribution(1, 1_000_000_000, true, true);
                    Assert.That(selection.Roll, Is.EqualTo(upperRoll), selection.Type);
                    Assert.That(selection.Amount, Is.InRange(1, 1_000_000_000), selection.Type);
                    Assert.That(selection.AmountAsDouble, Is.InRange(1, 1_000_000_000), selection.Type);
                    break;
                default:
                    Assert.Fail($"An unknown selection of [{selection.Type},{selection.Roll}] was returned");
                    break;
            }
        }

        [Test]
        [Repeat(1000)]
        public void SelectFromTable_Reroll()
        {
            var result1 = percentileTypeAndAmountSelector.SelectFrom(assemblyName, "BigTypeAndAmountPercentileTable");
            var result2 = percentileTypeAndAmountSelector.SelectFrom(assemblyName, "BigTypeAndAmountPercentileTable");
            var result3 = percentileTypeAndAmountSelector.SelectFrom(assemblyName, "BigTypeAndAmountPercentileTable");
            var result4 = percentileTypeAndAmountSelector.SelectFrom(assemblyName, "BigTypeAndAmountPercentileTable");
            AssertTypeAndAmount(result1);
            AssertTypeAndAmount(result2);
            AssertTypeAndAmount(result3);
            AssertTypeAndAmount(result4);

            var distinctValues = new[] { result1.Amount, result2.Amount, result3.Amount, result4.Amount }.Distinct().ToArray();
            Assert.That(distinctValues, Is.Unique.And.Length.AtLeast(3));
        }

        [Test]
        [Repeat(1000)]
        public void SelectAllFromTable()
        {
            var results = percentileTypeAndAmountSelector.SelectAllFrom(assemblyName, "TypeAndAmountPercentileTable").ToArray();
            Assert.That(results, Is.All.Not.Null);
            Assert.That(results.Length, Is.EqualTo(5));

            foreach (var selection in results)
                AssertTypeAndAmount(selection);
        }

        [Test]
        [Repeat(1000)]
        public void SelectAllFromTable_Reroll()
        {
            var results1 = percentileTypeAndAmountSelector.SelectAllFrom(assemblyName, "BigTypeAndAmountPercentileTable").ToArray();
            var results2 = percentileTypeAndAmountSelector.SelectAllFrom(assemblyName, "BigTypeAndAmountPercentileTable").ToArray();
            var results3 = percentileTypeAndAmountSelector.SelectAllFrom(assemblyName, "BigTypeAndAmountPercentileTable").ToArray();

            Assert.That(results1, Is.All.Not.Null);
            Assert.That(results1.Length, Is.EqualTo(2));
            Assert.That(results2, Is.All.Not.Null);
            Assert.That(results2.Length, Is.EqualTo(2));
            Assert.That(results3, Is.All.Not.Null);
            Assert.That(results3.Length, Is.EqualTo(2));

            foreach (var selection in results1)
                AssertTypeAndAmount(selection);
            foreach (var selection in results2)
                AssertTypeAndAmount(selection);
            foreach (var selection in results3)
                AssertTypeAndAmount(selection);

            var distinctValues = results1.Select(r => r.Amount)
                .Union(results2.Select(r => r.Amount))
                .Union(results3.Select(r => r.Amount))
                .Distinct()
                .ToArray();
            Assert.That(distinctValues, Is.Unique.And.Length.AtLeast(3));
        }
    }
}