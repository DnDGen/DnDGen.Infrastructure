using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Percentiles;
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
                    Assert.That(selection.Roll, Is.EqualTo("0.5"));
                    Assert.That(selection.Amount, Is.EqualTo(0));
                    Assert.That(selection.AmountAsDouble, Is.EqualTo(0.5));
                    break;
                case "High Treasure":
                    Assert.That(selection.Roll, Is.EqualTo("2"));
                    Assert.That(selection.Amount, Is.EqualTo(2));
                    Assert.That(selection.AmountAsDouble, Is.EqualTo(2));
                    break;
                case "":
                    Assert.That(selection.Roll, Is.EqualTo("0"));
                    Assert.That(selection.Amount, Is.EqualTo(0));
                    Assert.That(selection.AmountAsDouble, Is.EqualTo(0));
                    break;
                case "Big Gem":
                    Assert.That(selection.Roll, Is.EqualTo("1d4*1000"));
                    Assert.That(selection.Amount, Is.AnyOf([1000, 2000, 3000, 4000]));
                    Assert.That(selection.AmountAsDouble, Is.AnyOf([1000, 2000, 3000, 4000]));
                    break;
                case "Abilities":
                    Assert.That(selection.Roll, Is.EqualTo("3d6t1"));
                    Assert.That(selection.Amount, Is.InRange(6, 18));
                    Assert.That(selection.AmountAsDouble, Is.InRange(6, 18));
                    break;
                default:
                    Assert.Fail($"An unknown selection of [{selection.Type},{selection.Roll}] was returned");
                    break;
            }
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
    }
}