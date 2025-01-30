using DnDGen.Infrastructure.Selectors.Collections;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Integration.Selectors.Collections
{
    [TestFixture]
    public class CollectionTypeAndAmountSelectorTests : IntegrationTests
    {
        private ICollectionTypeAndAmountSelector collectionTypeAndAmountSelector;

        [SetUp]
        public void Setup()
        {
            collectionTypeAndAmountSelector = GetNewInstanceOf<ICollectionTypeAndAmountSelector>();
        }

        [Test]
        [Repeat(1000)]
        public void SelectFromTable_TreasureRates()
        {
            var selections = collectionTypeAndAmountSelector.SelectFrom(assemblyName, "TypeAndAmountCollectionTable", "Treasure Rates").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(5));
            Assert.That(selections[0].Type, Is.EqualTo("None"));
            Assert.That(selections[0].Roll, Is.EqualTo("0"));
            Assert.That(selections[0].Amount, Is.EqualTo(0));
            Assert.That(selections[0].AmountAsDouble, Is.EqualTo(0));
            Assert.That(selections[1].Type, Is.EqualTo("Bad"));
            Assert.That(selections[1].Roll, Is.EqualTo("0.5"));
            Assert.That(selections[1].Amount, Is.EqualTo(0));
            Assert.That(selections[1].AmountAsDouble, Is.EqualTo(0.5));
            Assert.That(selections[2].Type, Is.EqualTo("Ok"));
            Assert.That(selections[2].Roll, Is.EqualTo("1"));
            Assert.That(selections[2].Amount, Is.EqualTo(1));
            Assert.That(selections[2].AmountAsDouble, Is.EqualTo(1));
            Assert.That(selections[3].Type, Is.EqualTo("Great"));
            Assert.That(selections[3].Roll, Is.EqualTo("2"));
            Assert.That(selections[3].Amount, Is.EqualTo(2));
            Assert.That(selections[3].AmountAsDouble, Is.EqualTo(2));
            Assert.That(selections[4].Type, Is.EqualTo("Superb"));
            Assert.That(selections[4].Roll, Is.EqualTo("1d2+2"));
            Assert.That(selections[4].Amount, Is.InRange(3, 4));
            Assert.That(selections[4].AmountAsDouble, Is.InRange(3, 4));
        }

        [Test]
        public void SelectFromTable_Empty()
        {
            var selections = collectionTypeAndAmountSelector.SelectFrom(assemblyName, "TypeAndAmountCollectionTable", "This is an empty group").ToArray();
            Assert.That(selections, Is.Empty);
        }

        [Test]
        public void SelectFromTable_Lonely()
        {
            var selections = collectionTypeAndAmountSelector.SelectFrom(assemblyName, "TypeAndAmountCollectionTable", "Lonely").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(1));
            Assert.That(selections[0].Type, Is.EqualTo("One is the loneliest number"));
            Assert.That(selections[0].Roll, Is.EqualTo("1"));
            Assert.That(selections[0].Amount, Is.EqualTo(1));
            Assert.That(selections[0].AmountAsDouble, Is.EqualTo(1));
        }

        [Test]
        [Repeat(1000)]
        public void SelectFromTable_FunRolls()
        {
            var selections = collectionTypeAndAmountSelector.SelectFrom(assemblyName, "TypeAndAmountCollectionTable", "Fun Rolls").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(4));
            Assert.That(selections[0].Type, Is.EqualTo("Abilities"));
            Assert.That(selections[0].Roll, Is.EqualTo("3d6t1"));
            Assert.That(selections[0].Amount, Is.InRange(6, 18));
            Assert.That(selections[0].AmountAsDouble, Is.InRange(6, 18));
            Assert.That(selections[1].Type, Is.EqualTo("Big gem"));
            Assert.That(selections[1].Roll, Is.EqualTo("1d4*1000"));
            Assert.That(selections[1].Amount, Is.AnyOf([1000, 2000, 3000, 4000]));
            Assert.That(selections[1].AmountAsDouble, Is.AnyOf([1000, 2000, 3000, 4000]));
            Assert.That(selections[2].Type, Is.EqualTo("Uh oh negative!"));
            Assert.That(selections[2].Roll, Is.EqualTo("-4"));
            Assert.That(selections[2].Amount, Is.EqualTo(-4));
            Assert.That(selections[2].AmountAsDouble, Is.EqualTo(-4));
            Assert.That(selections[3].Type, Is.EqualTo("Nonsense"));
            Assert.That(selections[3].Roll, Is.EqualTo("1d2d3d4"));
            Assert.That(selections[3].Amount, Is.InRange(1, 24));
            Assert.That(selections[3].AmountAsDouble, Is.InRange(1, 24));
        }

        [Test]
        [Repeat(1000)]
        public void SelectAllFromTable()
        {
            var selectedCollections = collectionTypeAndAmountSelector.SelectAllFrom(assemblyName, "TypeAndAmountCollectionTable");
            Assert.That(selectedCollections, Has.Count.EqualTo(4)
                .And.ContainKey("Treasure Rates")
                .And.ContainKey("Lonely")
                .And.ContainKey("This is an empty group")
                .And.ContainKey("Fun Rolls"));

            Assert.That(selectedCollections["Treasure Rates"], Is.All.Not.Null);
            Assert.That(selectedCollections["Treasure Rates"].Count(), Is.EqualTo(5));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(0).Type, Is.EqualTo("None"));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(0).Roll, Is.EqualTo("0"));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(0).Amount, Is.EqualTo(0));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(0).AmountAsDouble, Is.EqualTo(0));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(1).Type, Is.EqualTo("Bad"));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(1).Roll, Is.EqualTo("0.5"));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(1).Amount, Is.EqualTo(0));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(1).AmountAsDouble, Is.EqualTo(0.5));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(2).Type, Is.EqualTo("Ok"));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(2).Roll, Is.EqualTo("1"));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(2).Amount, Is.EqualTo(1));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(2).AmountAsDouble, Is.EqualTo(1));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(3).Type, Is.EqualTo("Great"));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(3).Roll, Is.EqualTo("2"));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(3).Amount, Is.EqualTo(2));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(3).AmountAsDouble, Is.EqualTo(2));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(4).Type, Is.EqualTo("Superb"));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(4).Roll, Is.EqualTo("1d2+2"));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(4).Amount, Is.InRange(3, 4));
            Assert.That(selectedCollections["Treasure Rates"].ElementAt(4).AmountAsDouble, Is.InRange(3, 4));

            Assert.That(selectedCollections["This is an empty group"], Is.Empty);

            Assert.That(selectedCollections["Lonely"], Is.All.Not.Null);
            Assert.That(selectedCollections["Lonely"].Count(), Is.EqualTo(1));
            Assert.That(selectedCollections["Lonely"].ElementAt(0).Type, Is.EqualTo("One is the loneliest number"));
            Assert.That(selectedCollections["Lonely"].ElementAt(0).Roll, Is.EqualTo("1"));
            Assert.That(selectedCollections["Lonely"].ElementAt(0).Amount, Is.EqualTo(1));
            Assert.That(selectedCollections["Lonely"].ElementAt(0).AmountAsDouble, Is.EqualTo(1));

            Assert.That(selectedCollections["Fun Rolls"], Is.All.Not.Null);
            Assert.That(selectedCollections["Fun Rolls"].Count(), Is.EqualTo(4));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(0).Type, Is.EqualTo("Abilities"));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(0).Roll, Is.EqualTo("3d6t1"));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(0).Amount, Is.InRange(6, 18));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(0).AmountAsDouble, Is.InRange(6, 18));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(1).Type, Is.EqualTo("Big gem"));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(1).Roll, Is.EqualTo("1d4*1000"));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(1).Amount, Is.AnyOf([1000, 2000, 3000, 4000]));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(1).AmountAsDouble, Is.AnyOf([1000, 2000, 3000, 4000]));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(2).Type, Is.EqualTo("Uh oh negative!"));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(2).Roll, Is.EqualTo("-4"));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(2).Amount, Is.EqualTo(-4));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(2).AmountAsDouble, Is.EqualTo(-4));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(3).Type, Is.EqualTo("Nonsense"));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(3).Roll, Is.EqualTo("1d2d3d4"));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(3).Amount, Is.InRange(1, 24));
            Assert.That(selectedCollections["Fun Rolls"].ElementAt(3).AmountAsDouble, Is.InRange(1, 24));
        }

        [Test]
        public void TODO_IsCollection()
        {
            Assert.Fail("not yet written");
        }

        [Test]
        public void TODO_SelectOneFrom()
        {
            Assert.Fail("not yet written");
        }

        [Test]
        public void TODO_SelectRandomFrom()
        {
            Assert.Fail("not yet written");
        }
    }
}
