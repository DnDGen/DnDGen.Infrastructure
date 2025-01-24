using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
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
        public void SelectFrom_ReturnsTypeAndAmountSelection()
        {
            var selection = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount" };
            mockCollectionDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([selection]);

            mockDice.Setup(d => d.Roll("my amount").AsSum<double>()).Returns(9266);

            var result = collectionTypeAndAmountSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.EqualTo([selection]));
            Assert.That(result[0].Type, Is.EqualTo("my type"));
            Assert.That(result[0].Roll, Is.EqualTo("my amount"));
            Assert.That(result[0].Amount, Is.EqualTo(9266));
            Assert.That(result[0].AmountAsDouble, Is.EqualTo(9266));
        }

        [Test]
        public void SelectFrom_ReturnsTypeAndAmountSelection_Double()
        {
            var selection = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount" };
            mockCollectionDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([selection]);

            mockDice.Setup(d => d.Roll("my amount").AsSum<double>()).Returns(90.210);

            var result = collectionTypeAndAmountSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.EqualTo([selection]));
            Assert.That(result[0].Type, Is.EqualTo("my type"));
            Assert.That(result[0].Roll, Is.EqualTo("my amount"));
            Assert.That(result[0].Amount, Is.EqualTo(90));
            Assert.That(result[0].AmountAsDouble, Is.EqualTo(90.210));
        }

        [Test]
        public void SelectFrom_ReturnsTypeAndAmountSelections()
        {
            var selection1 = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount" };
            var selection2 = new TypeAndAmountDataSelection { Type = "my other type", Roll = "my other amount" };
            mockCollectionDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([selection1, selection2]);

            mockDice.Setup(d => d.Roll("my amount").AsSum<double>()).Returns(9266);
            mockDice.Setup(d => d.Roll("my other amount").AsSum<double>()).Returns(4.2);

            var result = collectionTypeAndAmountSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.EqualTo([selection1, selection2]));
            Assert.That(result[0].Type, Is.EqualTo("my type"));
            Assert.That(result[0].Roll, Is.EqualTo("my amount"));
            Assert.That(result[0].Amount, Is.EqualTo(9266));
            Assert.That(result[0].AmountAsDouble, Is.EqualTo(9266));
            Assert.That(result[1].Type, Is.EqualTo("my other type"));
            Assert.That(result[1].Roll, Is.EqualTo("my other amount"));
            Assert.That(result[1].Amount, Is.EqualTo(4));
            Assert.That(result[1].AmountAsDouble, Is.EqualTo(4.2));
        }

        [Test]
        public void SelectFrom_RollsEveryCall()
        {
            var selection1 = new TypeAndAmountDataSelection { Type = "my type", Roll = "my amount" };
            var selection2 = new TypeAndAmountDataSelection { Type = "my other type", Roll = "my other amount" };
            mockCollectionDataSelector
                .Setup(s => s.SelectFrom(assemblyName, tableName, "my collection"))
                .Returns([selection1, selection2]);

            mockDice.SetupSequence(d => d.Roll("my amount").AsSum<double>()).Returns(9266).Returns(90.210);
            mockDice.SetupSequence(d => d.Roll("my other amount").AsSum<double>()).Returns(4.2).Returns(600);

            var result = collectionTypeAndAmountSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.EqualTo([selection1, selection2]));
            Assert.That(result[0].Type, Is.EqualTo("my type"));
            Assert.That(result[0].Roll, Is.EqualTo("my amount"));
            Assert.That(result[0].Amount, Is.EqualTo(9266));
            Assert.That(result[0].AmountAsDouble, Is.EqualTo(9266));
            Assert.That(result[1].Type, Is.EqualTo("my other type"));
            Assert.That(result[1].Roll, Is.EqualTo("my other amount"));
            Assert.That(result[1].Amount, Is.EqualTo(4));
            Assert.That(result[1].AmountAsDouble, Is.EqualTo(4.2));

            result = collectionTypeAndAmountSelector.SelectFrom(assemblyName, tableName, "my collection").ToArray();
            Assert.That(result, Is.EqualTo([selection1, selection2]));
            Assert.That(result[0].Type, Is.EqualTo("my type"));
            Assert.That(result[0].Roll, Is.EqualTo("my amount"));
            Assert.That(result[0].Amount, Is.EqualTo(90));
            Assert.That(result[0].AmountAsDouble, Is.EqualTo(90.210));
            Assert.That(result[1].Type, Is.EqualTo("my other type"));
            Assert.That(result[1].Roll, Is.EqualTo("my other amount"));
            Assert.That(result[1].Amount, Is.EqualTo(600));
            Assert.That(result[1].AmountAsDouble, Is.EqualTo(600));
        }

        [Test]
        public void SelectAllFrom_ReturnsTypeAndAmountSelections()
        {
            var selection1 = new TypeAndAmountDataSelection { Type = "type 1", Roll = "amount 1" };
            var selection2 = new TypeAndAmountDataSelection { Type = "type 2", Roll = "amount 2" };
            var selection3 = new TypeAndAmountDataSelection { Type = "type 3", Roll = "amount 3" };
            var selection4 = new TypeAndAmountDataSelection { Type = "type 4", Roll = "amount 4" };
            var table = new Dictionary<string, IEnumerable<TypeAndAmountDataSelection>>
            {
                ["big"] = [selection1, selection2, selection3],
                ["empty"] = [],
                ["small"] = [selection4],
            };
            mockCollectionDataSelector
                .Setup(s => s.SelectAllFrom(assemblyName, tableName))
                .Returns(table);

            mockDice.Setup(d => d.Roll("amount 1").AsSum<double>()).Returns(9266);
            mockDice.Setup(d => d.Roll("amount 2").AsSum<double>()).Returns(4.2);
            mockDice.Setup(d => d.Roll("amount 3").AsSum<double>()).Returns(13.37);
            mockDice.Setup(d => d.Roll("amount 4").AsSum<double>()).Returns(96);

            var results = collectionTypeAndAmountSelector.SelectAllFrom(assemblyName, tableName);
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results.Keys, Is.EqualTo(table.Keys));
            Assert.That(results["big"], Is.All.Not.Null);
            Assert.That(results["big"].Count(), Is.EqualTo(3));
            Assert.That(results["big"].ElementAt(0).Type, Is.EqualTo("type 1"));
            Assert.That(results["big"].ElementAt(0).Roll, Is.EqualTo("amount 1"));
            Assert.That(results["big"].ElementAt(0).Amount, Is.EqualTo(9266));
            Assert.That(results["big"].ElementAt(0).AmountAsDouble, Is.EqualTo(9266));
            Assert.That(results["big"].ElementAt(1).Type, Is.EqualTo("type 2"));
            Assert.That(results["big"].ElementAt(1).Roll, Is.EqualTo("amount 2"));
            Assert.That(results["big"].ElementAt(1).Amount, Is.EqualTo(4));
            Assert.That(results["big"].ElementAt(1).AmountAsDouble, Is.EqualTo(4.2));
            Assert.That(results["big"].ElementAt(2).Type, Is.EqualTo("type 3"));
            Assert.That(results["big"].ElementAt(2).Roll, Is.EqualTo("amount 3"));
            Assert.That(results["big"].ElementAt(2).Amount, Is.EqualTo(13));
            Assert.That(results["big"].ElementAt(2).AmountAsDouble, Is.EqualTo(13.37));
            Assert.That(results["empty"], Is.Empty);
            Assert.That(results["small"], Is.All.Not.Null);
            Assert.That(results["small"].Count(), Is.EqualTo(1));
            Assert.That(results["small"].ElementAt(0).Type, Is.EqualTo("type 4"));
            Assert.That(results["small"].ElementAt(0).Roll, Is.EqualTo("amount 4"));
            Assert.That(results["small"].ElementAt(0).Amount, Is.EqualTo(96));
            Assert.That(results["small"].ElementAt(0).AmountAsDouble, Is.EqualTo(96));
        }

        [Test]
        public void SelectAllFrom_RollsEveryCall()
        {
            var selection1 = new TypeAndAmountDataSelection { Type = "type 1", Roll = "amount 1" };
            var selection2 = new TypeAndAmountDataSelection { Type = "type 2", Roll = "amount 2" };
            var selection3 = new TypeAndAmountDataSelection { Type = "type 3", Roll = "amount 3" };
            var selection4 = new TypeAndAmountDataSelection { Type = "type 4", Roll = "amount 4" };
            var table = new Dictionary<string, IEnumerable<TypeAndAmountDataSelection>>
            {
                ["big"] = [selection1, selection2, selection3],
                ["empty"] = [],
                ["small"] = [selection4],
            };
            mockCollectionDataSelector
                .Setup(s => s.SelectAllFrom(assemblyName, tableName))
                .Returns(table);

            mockDice.SetupSequence(d => d.Roll("amount 1").AsSum<double>()).Returns(9266).Returns(90.210);
            mockDice.SetupSequence(d => d.Roll("amount 2").AsSum<double>()).Returns(4.2).Returns(600);
            mockDice.SetupSequence(d => d.Roll("amount 3").AsSum<double>()).Returns(13.37).Returns(1336);
            mockDice.SetupSequence(d => d.Roll("amount 4").AsSum<double>()).Returns(96).Returns(78.3);

            var results = collectionTypeAndAmountSelector.SelectAllFrom(assemblyName, tableName);
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results.Keys, Is.EqualTo(table.Keys));
            Assert.That(results["big"], Is.All.Not.Null);
            Assert.That(results["big"].Count(), Is.EqualTo(3));
            Assert.That(results["big"].ElementAt(0).Type, Is.EqualTo("type 1"));
            Assert.That(results["big"].ElementAt(0).Roll, Is.EqualTo("amount 1"));
            Assert.That(results["big"].ElementAt(0).Amount, Is.EqualTo(9266));
            Assert.That(results["big"].ElementAt(0).AmountAsDouble, Is.EqualTo(9266));
            Assert.That(results["big"].ElementAt(1).Type, Is.EqualTo("type 2"));
            Assert.That(results["big"].ElementAt(1).Roll, Is.EqualTo("amount 2"));
            Assert.That(results["big"].ElementAt(1).Amount, Is.EqualTo(4));
            Assert.That(results["big"].ElementAt(1).AmountAsDouble, Is.EqualTo(4.2));
            Assert.That(results["big"].ElementAt(2).Type, Is.EqualTo("type 3"));
            Assert.That(results["big"].ElementAt(2).Roll, Is.EqualTo("amount 3"));
            Assert.That(results["big"].ElementAt(2).Amount, Is.EqualTo(13));
            Assert.That(results["big"].ElementAt(2).AmountAsDouble, Is.EqualTo(13.37));
            Assert.That(results["empty"], Is.Empty);
            Assert.That(results["small"], Is.All.Not.Null);
            Assert.That(results["small"].Count(), Is.EqualTo(1));
            Assert.That(results["small"].ElementAt(0).Type, Is.EqualTo("type 4"));
            Assert.That(results["small"].ElementAt(0).Roll, Is.EqualTo("amount 4"));
            Assert.That(results["small"].ElementAt(0).Amount, Is.EqualTo(96));
            Assert.That(results["small"].ElementAt(0).AmountAsDouble, Is.EqualTo(96));

            results = collectionTypeAndAmountSelector.SelectAllFrom(assemblyName, tableName);
            Assert.That(results.Count, Is.EqualTo(3));
            Assert.That(results.Keys, Is.EqualTo(table.Keys));
            Assert.That(results["big"], Is.All.Not.Null);
            Assert.That(results["big"].Count(), Is.EqualTo(3));
            Assert.That(results["big"].ElementAt(0).Type, Is.EqualTo("type 1"));
            Assert.That(results["big"].ElementAt(0).Roll, Is.EqualTo("amount 1"));
            Assert.That(results["big"].ElementAt(0).Amount, Is.EqualTo(90));
            Assert.That(results["big"].ElementAt(0).AmountAsDouble, Is.EqualTo(90.21));
            Assert.That(results["big"].ElementAt(1).Type, Is.EqualTo("type 2"));
            Assert.That(results["big"].ElementAt(1).Roll, Is.EqualTo("amount 2"));
            Assert.That(results["big"].ElementAt(1).Amount, Is.EqualTo(600));
            Assert.That(results["big"].ElementAt(1).AmountAsDouble, Is.EqualTo(600));
            Assert.That(results["big"].ElementAt(2).Type, Is.EqualTo("type 3"));
            Assert.That(results["big"].ElementAt(2).Roll, Is.EqualTo("amount 3"));
            Assert.That(results["big"].ElementAt(2).Amount, Is.EqualTo(1336));
            Assert.That(results["big"].ElementAt(2).AmountAsDouble, Is.EqualTo(1336));
            Assert.That(results["empty"], Is.Empty);
            Assert.That(results["small"], Is.All.Not.Null);
            Assert.That(results["small"].Count(), Is.EqualTo(1));
            Assert.That(results["small"].ElementAt(0).Type, Is.EqualTo("type 4"));
            Assert.That(results["small"].ElementAt(0).Roll, Is.EqualTo("amount 4"));
            Assert.That(results["small"].ElementAt(0).Amount, Is.EqualTo(78));
            Assert.That(results["small"].ElementAt(0).AmountAsDouble, Is.EqualTo(78.3));
        }
    }
}