using DnDGen.Core.Selectors.Percentiles;
using DnDGen.Core.Tests;
using Ninject;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Integration.DnDGen.Core.Selectors.Percentiles
{
    [TestFixture]
    public class PercentileSelectorTests : IntegrationTests
    {
        [Inject]
        public IPercentileSelector PercentileSelector { get; set; }

        [Test]
        public void SelectFromTable()
        {
            var result = PercentileSelector.SelectFrom("StringPercentileTable");
            Assert.That(result, Is.EqualTo("one")
                .Or.EqualTo("two")
                .Or.EqualTo("three")
                .Or.Empty
                .Or.EqualTo("eleven to one hundred"));
        }

        [Test]
        public void SelectIntFromTable()
        {
            var result = PercentileSelector.SelectFrom<int>("IntPercentileTable");
            Assert.That(result, Is.EqualTo(1)
                .Or.EqualTo(2)
                .Or.EqualTo(3)
                .Or.EqualTo(0)
                .Or.EqualTo(-89));
        }

        [Test]
        public void SelectBooleanFromTable()
        {
            var result = PercentileSelector.SelectFrom<bool>("BooleanPercentileTable");
            Assert.That(result, Is.True.Or.False);
        }

        [Test]
        public void SelectAllFromTable()
        {
            var results = PercentileSelector.SelectAllFrom("StringPercentileTable");
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
            var results = PercentileSelector.SelectAllFrom<int>("IntPercentileTable");
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
            var results = PercentileSelector.SelectAllFrom<bool>("BooleanPercentileTable");
            Assert.That(results, Is.EquivalentTo(new[]
            {
                true,
                false
            }));
        }
    }
}