using DnDGen.Infrastructure.Models;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Unit.Models
{
    [TestFixture]
    internal class TypeAndAmountDataSelectionTests
    {
        private TypeAndAmountDataSelection selection;

        [SetUp]
        public void Setup()
        {
            selection = new TypeAndAmountDataSelection();
        }

        [Test]
        public void SectionCountIs2()
        {
            Assert.That(selection.SectionCount, Is.EqualTo(2));
        }

        [Test]
        public void Amount_FromInt()
        {
            selection.AmountAsDouble = 9266;
            Assert.That(selection.Amount, Is.EqualTo(9266));
        }

        [Test]
        public void Amount_FromDouble_RoundUp()
        {
            selection.AmountAsDouble = 92.66;
            Assert.That(selection.Amount, Is.EqualTo(93));
        }

        [Test]
        public void Amount_FromDouble_RoundDown()
        {
            selection.AmountAsDouble = 90.210;
            Assert.That(selection.Amount, Is.EqualTo(90));
        }

        [Test]
        public void Map_FromString_ReturnsSelection()
        {
            var selection = TypeAndAmountDataSelection.Map(["my type", "my amount"]);
            Assert.That(selection, Is.Not.Null);
            Assert.That(selection.Type, Is.EqualTo("my type"));
            Assert.That(selection.Roll, Is.EqualTo("my amount"));
            Assert.That(selection.Amount, Is.Zero);
        }

        [Test]
        public void Map_FromString_ReturnsSelection_Double()
        {
            var selection = TypeAndAmountDataSelection.Map(["my type", "my amount"]);
            Assert.That(selection, Is.Not.Null);
            Assert.That(selection.Type, Is.EqualTo("my type"));
            Assert.That(selection.Roll, Is.EqualTo("my amount"));
            Assert.That(selection.Amount, Is.Zero);
        }

        [Test]
        public void Map_FromSelection_ReturnsString()
        {
            var selection = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount", AmountAsDouble = 9266 };
            var rawData = TypeAndAmountDataSelection.Map(selection);
            Assert.That(rawData, Is.EqualTo(["my type", "my amount"]));
        }

        [Test]
        public void Map_FromSelection_ReturnsString_Double()
        {
            var selection = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount", AmountAsDouble = 90.210 };
            var rawData = TypeAndAmountDataSelection.Map(selection);
            Assert.That(rawData, Is.EqualTo(["my type", "my amount"]));
        }

        [Test]
        public void MapTo_ReturnsSelection()
        {
            var newSelection = selection.MapTo(["my type", "my amount"]);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.Type, Is.EqualTo("my type"));
            Assert.That(newSelection.Roll, Is.EqualTo("my amount"));
            Assert.That(newSelection.Amount, Is.Zero);
        }

        [Test]
        public void MapTo_ReturnsSelection_Double()
        {
            var doubleSelection = new TypeAndAmountDataSelection();
            var newSelection = doubleSelection.MapTo(["my type", "my amount"]);
            Assert.That(newSelection, Is.Not.Null);
            Assert.That(newSelection.Type, Is.EqualTo("my type"));
            Assert.That(newSelection.Roll, Is.EqualTo("my amount"));
            Assert.That(newSelection.Amount, Is.Zero);
        }

        [Test]
        public void MapFrom_ReturnsString()
        {
            var selection = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount", AmountAsDouble = 9266 };
            var rawData = selection.MapFrom(selection);
            Assert.That(rawData, Is.EqualTo(["my type", "my amount"]));
        }

        [Test]
        public void MapFrom_ReturnsString_Double()
        {
            var selection = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount", AmountAsDouble = 90.210 };
            var rawData = selection.MapFrom(selection);
            Assert.That(rawData, Is.EqualTo(["my type", "my amount"]));
        }
    }
}
