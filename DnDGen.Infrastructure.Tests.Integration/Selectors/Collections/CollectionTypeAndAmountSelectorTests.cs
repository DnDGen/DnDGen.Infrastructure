using DnDGen.Infrastructure.Models;
using DnDGen.Infrastructure.Selectors.Collections;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Integration.Selectors.Collections
{
    [TestFixture]
    public class CollectionTypeAndAmountSelectorTests : IntegrationTests
    {
        private ICollectionTypeAndAmountSelector collectionTypeAndAmountSelector;

        private const int IterationsForReroll = 20;

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
        [Repeat(1000)]
        public void SelectFromTable_TreasureRates_Reroll()
        {
            var allSelections = GetRerolls("Treasure Rates");
            Assert.That(allSelections, Has.Count.EqualTo(5)
                .And.ContainKey("None")
                .And.ContainKey("Bad")
                .And.ContainKey("Ok")
                .And.ContainKey("Great")
                .And.ContainKey("Superb"));
            AssertAllSame(allSelections["None"], 0);
            AssertAllSame(allSelections["Bad"], 0.5);
            AssertAllSame(allSelections["Ok"], 1);
            AssertAllSame(allSelections["Great"], 2);
            AssertDistinct(allSelections["Superb"], 3, 4);
        }

        private Dictionary<string, List<TypeAndAmountDataSelection>> GetRerolls(string name)
        {
            var allSelections = new Dictionary<string, List<TypeAndAmountDataSelection>>();
            while (NeedToReroll(allSelections))
            {
                var selections = collectionTypeAndAmountSelector.SelectFrom(assemblyName, "TypeAndAmountCollectionTable", name).ToArray();

                foreach (var selection in selections)
                {
                    if (!allSelections.ContainsKey(selection.Type))
                        allSelections[selection.Type] = [];

                    allSelections[selection.Type].Add(selection);
                }
            }

            return allSelections;
        }

        private bool NeedToReroll<T>(Dictionary<string, List<T>> selections) => selections == null
            || selections.Count == 0
            || selections.Any(kvp => kvp.Value.Count < IterationsForReroll);
        private bool NeedToReroll<T, U>(Dictionary<string, Dictionary<T, U>> selections) => selections == null
            || selections.Count == 0
            || selections.Any(kvp => kvp.Value.Count < IterationsForReroll);

        private void AssertAllSame(IEnumerable<TypeAndAmountDataSelection> selections, double expected)
        {
            var amounts = selections.Select(s => s.AmountAsDouble);
            var message = $"[{string.Join(',', amounts)}]";
            Assert.That(selections.Count(), Is.AtLeast(2), message);
            Assert.That(selections.Select(s => s.AmountAsDouble), Is.All.EqualTo(expected), message);
        }

        private void AssertDistinct(IEnumerable<TypeAndAmountDataSelection> selections, int min, int max)
        {
            var amounts = selections.Select(s => s.AmountAsDouble);
            var message = $"[{string.Join(',', amounts)}]";
            Assert.That(selections.Count(), Is.AtLeast(2), message);

            var rolls = amounts.Distinct();
            Assert.That(rolls, Is.All.InRange(min, max));
            Assert.That(rolls.Count(), Is.AtLeast(2), message);
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
        public void SelectFromTable_Lonely_Reroll()
        {
            var allSelections = GetRerolls("Lonely");
            Assert.That(allSelections, Has.Count.EqualTo(1)
                .And.ContainKey("One is the loneliest number"));
            AssertAllSame(allSelections["One is the loneliest number"], 1);
        }

        [Test]
        [Repeat(1000)]
        public void SelectFromTable_Plural()
        {
            var selections = collectionTypeAndAmountSelector.SelectFrom(assemblyName, "TypeAndAmountCollectionTable", "Plural").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(1));
            Assert.That(selections[0].Type, Is.EqualTo("More than one"));
            Assert.That(selections[0].Roll, Is.EqualTo("1+1d100"));
            Assert.That(selections[0].Amount, Is.InRange(2, 101));
            Assert.That(selections[0].AmountAsDouble, Is.InRange(2, 101));
        }

        [Test]
        [Repeat(1000)]
        public void SelectFromTable_Plural_Reroll()
        {
            var allSelections = GetRerolls("Plural");
            Assert.That(allSelections, Has.Count.EqualTo(1)
                .And.ContainKey("More than one"));
            AssertDistinct(allSelections["More than one"], 2, 101);
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
        public void SelectFromTable_FunRolls_Reroll()
        {
            var allSelections = GetRerolls("Fun Rolls");
            Assert.That(allSelections, Has.Count.EqualTo(4)
                .And.ContainKey("Abilities")
                .And.ContainKey("Uh oh negative!")
                .And.ContainKey("Big gem")
                .And.ContainKey("Nonsense"));
            AssertAllSame(allSelections["Uh oh negative!"], -4);
            AssertDistinct(allSelections["Abilities"], 6, 18);
            AssertDistinct(allSelections["Big gem"], 1000, 4000);
            AssertDistinct(allSelections["Nonsense"], 1, 24);
        }

        [Test]
        [Repeat(1000)]
        public void SelectAllFromTable()
        {
            var selectedCollections = collectionTypeAndAmountSelector.SelectAllFrom(assemblyName, "TypeAndAmountCollectionTable");
            Assert.That(selectedCollections, Has.Count.EqualTo(5)
                .And.ContainKey("Treasure Rates")
                .And.ContainKey("Lonely")
                .And.ContainKey("Plural")
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

            Assert.That(selectedCollections["Plural"], Is.All.Not.Null);
            Assert.That(selectedCollections["Plural"].Count(), Is.EqualTo(1));
            Assert.That(selectedCollections["Plural"].ElementAt(0).Type, Is.EqualTo("More than one"));
            Assert.That(selectedCollections["Plural"].ElementAt(0).Roll, Is.EqualTo("1+1d100"));
            Assert.That(selectedCollections["Plural"].ElementAt(0).Amount, Is.InRange(2, 101));
            Assert.That(selectedCollections["Plural"].ElementAt(0).AmountAsDouble, Is.InRange(2, 101));

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
        [Repeat(1000)]
        public void SelectAllFromTable_Reroll()
        {
            var allSelections = GetAllRerolls();
            Assert.That(allSelections, Has.Count.EqualTo(5)
                .And.ContainKey("Treasure Rates")
                .And.ContainKey("Lonely")
                .And.ContainKey("Plural")
                .And.ContainKey("This is an empty group")
                .And.ContainKey("Fun Rolls"));

            Assert.That(allSelections["Treasure Rates"], Has.Count.EqualTo(5)
                .And.ContainKey("None")
                .And.ContainKey("Bad")
                .And.ContainKey("Ok")
                .And.ContainKey("Great")
                .And.ContainKey("Superb"));
            AssertAllSame(allSelections["Treasure Rates"]["None"], 0);
            AssertAllSame(allSelections["Treasure Rates"]["Bad"], 0.5);
            AssertAllSame(allSelections["Treasure Rates"]["Ok"], 1);
            AssertAllSame(allSelections["Treasure Rates"]["Great"], 2);
            AssertDistinct(allSelections["Treasure Rates"]["Superb"], 3, 4);

            Assert.That(allSelections["Lonely"], Has.Count.EqualTo(1)
                .And.ContainKey("One is the loneliest number"));
            AssertAllSame(allSelections["Lonely"]["One is the loneliest number"], 1);

            Assert.That(allSelections["Plural"], Has.Count.EqualTo(1)
                .And.ContainKey("More than one"));
            AssertDistinct(allSelections["Plural"]["More than one"], 2, 101);

            Assert.That(allSelections["This is an empty group"], Is.Empty);

            Assert.That(allSelections["Fun Rolls"], Has.Count.EqualTo(4)
                .And.ContainKey("Abilities")
                .And.ContainKey("Uh oh negative!")
                .And.ContainKey("Big gem")
                .And.ContainKey("Nonsense"));
            AssertAllSame(allSelections["Fun Rolls"]["Uh oh negative!"], -4);
            AssertDistinct(allSelections["Fun Rolls"]["Abilities"], 6, 18);
            AssertDistinct(allSelections["Fun Rolls"]["Big gem"], 1000, 4000);
            AssertDistinct(allSelections["Fun Rolls"]["Nonsense"], 1, 24);
        }

        private Dictionary<string, Dictionary<string, List<TypeAndAmountDataSelection>>> GetAllRerolls()
        {
            var allSelections = new Dictionary<string, Dictionary<string, List<TypeAndAmountDataSelection>>>();
            while (NeedToReroll(allSelections) && NeedToReroll(allSelections.FirstOrDefault().Value))
            {
                var collections = collectionTypeAndAmountSelector.SelectAllFrom(assemblyName, "TypeAndAmountCollectionTable");

                foreach (var collection in collections)
                {
                    if (!allSelections.ContainsKey(collection.Key))
                        allSelections[collection.Key] = [];

                    foreach (var selection in collection.Value)
                    {
                        if (!allSelections[collection.Key].ContainsKey(selection.Type))
                            allSelections[collection.Key][selection.Type] = [];

                        allSelections[collection.Key][selection.Type].Add(selection);
                    }
                }
            }

            return allSelections;
        }

        [TestCase("Treasure Rates", true)]
        [TestCase("treasure rates", false)]
        [TestCase("TreasureRates", false)]
        [TestCase("Treasure Rates ", false)]
        [TestCase("None", false)]
        [TestCase("Fun Rolls", true)]
        [TestCase("Abilities", false)]
        [TestCase("This is an empty group", true)]
        [TestCase("Lonely", true)]
        [TestCase("One is the loneliest number", false)]
        [TestCase("Plural", true)]
        [TestCase("More than one", false)]
        [TestCase("whatever", false)]
        [TestCase("", false)]
        public void IsCollection(string name, bool expected)
        {
            var isCollection = collectionTypeAndAmountSelector.IsCollection(assemblyName, "TypeAndAmountCollectionTable", name);
            Assert.That(isCollection, Is.EqualTo(expected));
        }

        [Test]
        public void SelectOneFrom_Lonely()
        {
            var selection = collectionTypeAndAmountSelector.SelectOneFrom(assemblyName, "TypeAndAmountCollectionTable", "Lonely");
            Assert.That(selection, Is.Not.Null);
            Assert.That(selection.Type, Is.EqualTo("One is the loneliest number"));
            Assert.That(selection.Roll, Is.EqualTo("1"));
            Assert.That(selection.Amount, Is.EqualTo(1));
            Assert.That(selection.AmountAsDouble, Is.EqualTo(1));
        }

        [Test]
        [Repeat(1000)]
        public void SelectOneFrom_Plural()
        {
            var selection = collectionTypeAndAmountSelector.SelectOneFrom(assemblyName, "TypeAndAmountCollectionTable", "Plural");
            Assert.That(selection, Is.Not.Null);
            Assert.That(selection.Type, Is.EqualTo("More than one"));
            Assert.That(selection.Roll, Is.EqualTo("1+1d100"));
            Assert.That(selection.Amount, Is.InRange(2, 101));
            Assert.That(selection.AmountAsDouble, Is.InRange(2, 101));
        }

        [Test]
        [Repeat(1000)]
        public void SelectOneFrom_Reparse()
        {
            var allSelections = GetReroll("Plural");
            Assert.That(allSelections, Has.Count.EqualTo(1)
                .And.ContainKey("More than one"));
            AssertDistinct(allSelections["More than one"], 2, 101);
        }

        private Dictionary<string, List<TypeAndAmountDataSelection>> GetReroll(string name)
        {
            var allSelections = new Dictionary<string, List<TypeAndAmountDataSelection>>();
            while (NeedToReroll(allSelections))
            {
                var selection = collectionTypeAndAmountSelector.SelectOneFrom(assemblyName, "TypeAndAmountCollectionTable", name);

                if (!allSelections.ContainsKey(selection.Type))
                    allSelections[selection.Type] = [];

                allSelections[selection.Type].Add(selection);
            }

            return allSelections;
        }

        [Test]
        [Repeat(1000)]
        public void SelectRandomFrom()
        {
            var selection = collectionTypeAndAmountSelector.SelectRandomFrom(assemblyName, "TypeAndAmountCollectionTable", "Fun Rolls");
            if (selection.Type == "Abilities")
            {
                Assert.That(selection.Roll, Is.EqualTo("3d6t1"));
                Assert.That(selection.Amount, Is.InRange(6, 18));
                Assert.That(selection.AmountAsDouble, Is.InRange(6, 18));
            }
            else if (selection.Type == "Big gem")
            {
                Assert.That(selection.Roll, Is.EqualTo("1d4*1000"));
                Assert.That(selection.Amount, Is.AnyOf([1000, 2000, 3000, 4000]));
                Assert.That(selection.AmountAsDouble, Is.AnyOf([1000, 2000, 3000, 4000]));
            }
            else if (selection.Type == "Uh oh negative!")
            {
                Assert.That(selection.Roll, Is.EqualTo("-4"));
                Assert.That(selection.Amount, Is.EqualTo(-4));
                Assert.That(selection.AmountAsDouble, Is.EqualTo(-4));
            }
            else if (selection.Type == "Nonsense")
            {
                Assert.That(selection.Roll, Is.EqualTo("1d2d3d4"));
                Assert.That(selection.Amount, Is.InRange(1, 24));
                Assert.That(selection.AmountAsDouble, Is.InRange(1, 24));
            }
            else
            {
                Assert.Fail($"An unknown selection of [{selection.Type},{selection.Roll}] was returned");
            }
        }

        [Test]
        [Repeat(1000)]
        public void SelectRandomFrom_Reroll()
        {
            var allSelections = GetRandomReroll("Fun Rolls");
            Assert.That(allSelections, Has.Count.EqualTo(4)
                .And.ContainKey("Abilities")
                .And.ContainKey("Nonsense")
                .And.ContainKey("Uh oh negative!")
                .And.ContainKey("Big gem"));
            AssertAllSame(allSelections["Uh oh negative!"], -4);
            AssertDistinct(allSelections["Abilities"], 6, 18);
            AssertDistinct(allSelections["Big gem"], 1000, 4000);
            AssertDistinct(allSelections["Nonsense"], 1, 24);
        }

        private Dictionary<string, List<TypeAndAmountDataSelection>> GetRandomReroll(string name)
        {
            var allSelections = new Dictionary<string, List<TypeAndAmountDataSelection>>();
            while (NeedToReroll(allSelections))
            {
                var selection = collectionTypeAndAmountSelector.SelectRandomFrom(assemblyName, "TypeAndAmountCollectionTable", name);

                if (!allSelections.ContainsKey(selection.Type))
                    allSelections[selection.Type] = [];

                allSelections[selection.Type].Add(selection);
            }

            return allSelections;
        }
    }
}
