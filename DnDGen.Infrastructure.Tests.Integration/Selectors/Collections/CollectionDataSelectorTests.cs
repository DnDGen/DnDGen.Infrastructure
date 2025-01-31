using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.Infrastructure.Tests.Integration.IoC.Modules;
using DnDGen.Infrastructure.Tests.Integration.Models;
using Ninject;
using NUnit.Framework;
using System;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Integration.Selectors.Collections
{
    [TestFixture]
    public class CollectionDataSelectorTests : IntegrationTests
    {
        private ICollectionDataSelector<TestDataSelection> collectionDataSelector;

        [OneTimeSetUp]
        public void SelectorsSetup()
        {
            kernel.Load<TestSelectorsModule>();
        }

        [SetUp]
        public void Setup()
        {
            collectionDataSelector = GetNewInstanceOf<ICollectionDataSelector<TestDataSelection>>();
        }

        [Test]
        public void SelectFromTable_Family()
        {
            var selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "My Family").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(3));
            Assert.That(selections[0].Name, Is.EqualTo("Karl Speer"));
            Assert.That(selections[0].Age, Is.EqualTo(35));
            Assert.That(selections[1].Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(selections[1].Age, Is.EqualTo(33));
            Assert.That(selections[2].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(selections[2].Age, Is.EqualTo(2));
        }

        [Test]
        public void SelectFromTable_Enemies()
        {
            var selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "Enemies").ToArray();
            Assert.That(selections, Is.Empty);
        }

        [Test]
        public void SelectFromTable_FavoriteNumbers()
        {
            var selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "Favorite Numbers").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(5));
            Assert.That(selections[0].Name, Is.EqualTo("Random"));
            Assert.That(selections[0].Age, Is.EqualTo(9266));
            Assert.That(selections[1].Name, Is.EqualTo("Beverly Hills"));
            Assert.That(selections[1].Age, Is.EqualTo(90210));
            Assert.That(selections[2].Name, Is.EqualTo("The Answer"));
            Assert.That(selections[2].Age, Is.EqualTo(42));
            Assert.That(selections[3].Name, Is.EqualTo("Highest Count"));
            Assert.That(selections[3].Age, Is.EqualTo(600));
            Assert.That(selections[4].Name, Is.EqualTo("leetspeak"));
            Assert.That(selections[4].Age, Is.EqualTo(1337));
        }

        [Test]
        public void SelectFromTable_Singular()
        {
            var selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "Singular").ToArray();
            Assert.That(selections.Length, Is.EqualTo(1));
            Assert.That(selections[0].Name, Is.EqualTo("The Loneliest Number"));
            Assert.That(selections[0].Age, Is.EqualTo(1));
        }

        [Test]
        public void SelectFromTable_Family_Reparse()
        {
            IncrementingDataSelection.MapCount = 0;
            var collectionDataSelector = GetNewInstanceOf<ICollectionDataSelector<IncrementingDataSelection>>();

            var selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "My Family").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(3));
            Assert.That(selections[0].Name, Is.EqualTo("Karl Speer"));
            Assert.That(selections[0].Age, Is.EqualTo(35 + 1));
            Assert.That(selections[1].Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(selections[1].Age, Is.EqualTo(33 + 2));
            Assert.That(selections[2].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(selections[2].Age, Is.EqualTo(2 + 3));

            selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "My Family").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(3));
            Assert.That(selections[0].Name, Is.EqualTo("Karl Speer"));
            Assert.That(selections[0].Age, Is.EqualTo(35 + 4));
            Assert.That(selections[1].Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(selections[1].Age, Is.EqualTo(33 + 5));
            Assert.That(selections[2].Name, Is.EqualTo("Hugo Speer"));
            Assert.That(selections[2].Age, Is.EqualTo(2 + 6));
        }

        [Test]
        public void SelectFromTable_Enemies_Reparse()
        {
            IncrementingDataSelection.MapCount = 0;
            var collectionDataSelector = GetNewInstanceOf<ICollectionDataSelector<IncrementingDataSelection>>();

            var selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "Enemies").ToArray();
            Assert.That(selections, Is.Empty);

            selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "Enemies").ToArray();
            Assert.That(selections, Is.Empty);
        }

        [Test]
        public void SelectFromTable_FavoriteNumbers_Reparse()
        {
            IncrementingDataSelection.MapCount = 0;
            var collectionDataSelector = GetNewInstanceOf<ICollectionDataSelector<IncrementingDataSelection>>();

            var selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "Favorite Numbers").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(5));
            Assert.That(selections[0].Name, Is.EqualTo("Random"));
            Assert.That(selections[0].Age, Is.EqualTo(9266 + 1));
            Assert.That(selections[1].Name, Is.EqualTo("Beverly Hills"));
            Assert.That(selections[1].Age, Is.EqualTo(90210 + 2));
            Assert.That(selections[2].Name, Is.EqualTo("The Answer"));
            Assert.That(selections[2].Age, Is.EqualTo(42 + 3));
            Assert.That(selections[3].Name, Is.EqualTo("Highest Count"));
            Assert.That(selections[3].Age, Is.EqualTo(600 + 4));
            Assert.That(selections[4].Name, Is.EqualTo("leetspeak"));
            Assert.That(selections[4].Age, Is.EqualTo(1337 + 5));

            selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "Favorite Numbers").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(5));
            Assert.That(selections[0].Name, Is.EqualTo("Random"));
            Assert.That(selections[0].Age, Is.EqualTo(9266 + 6));
            Assert.That(selections[1].Name, Is.EqualTo("Beverly Hills"));
            Assert.That(selections[1].Age, Is.EqualTo(90210 + 7));
            Assert.That(selections[2].Name, Is.EqualTo("The Answer"));
            Assert.That(selections[2].Age, Is.EqualTo(42 + 8));
            Assert.That(selections[3].Name, Is.EqualTo("Highest Count"));
            Assert.That(selections[3].Age, Is.EqualTo(600 + 9));
            Assert.That(selections[4].Name, Is.EqualTo("leetspeak"));
            Assert.That(selections[4].Age, Is.EqualTo(1337 + 10));
        }

        [Test]
        public void SelectFromTable_Singular_Reparse()
        {
            IncrementingDataSelection.MapCount = 0;
            var collectionDataSelector = GetNewInstanceOf<ICollectionDataSelector<IncrementingDataSelection>>();

            var selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "Singular").ToArray();
            Assert.That(selections.Length, Is.EqualTo(1));
            Assert.That(selections[0].Name, Is.EqualTo("The Loneliest Number"));
            Assert.That(selections[0].Age, Is.EqualTo(1 + 1));

            selections = collectionDataSelector.SelectFrom(assemblyName, "DataCollectionTable", "Singular").ToArray();
            Assert.That(selections, Is.All.Not.Null);
            Assert.That(selections.Length, Is.EqualTo(1));
            Assert.That(selections[0].Name, Is.EqualTo("The Loneliest Number"));
            Assert.That(selections[0].Age, Is.EqualTo(1 + 2));
        }

        [Test]
        public void SelectAllFromTable()
        {
            var selectedCollections = collectionDataSelector.SelectAllFrom(assemblyName, "DataCollectionTable");
            Assert.That(selectedCollections, Has.Count.EqualTo(4)
                .And.ContainKey("My Family")
                .And.ContainKey("Enemies")
                .And.ContainKey("Singular")
                .And.ContainKey("Favorite Numbers"));

            Assert.That(selectedCollections["My Family"], Is.All.Not.Null);
            Assert.That(selectedCollections["My Family"].Count(), Is.EqualTo(3));
            Assert.That(selectedCollections["My Family"].ElementAt(0).Name, Is.EqualTo("Karl Speer"));
            Assert.That(selectedCollections["My Family"].ElementAt(0).Age, Is.EqualTo(35));
            Assert.That(selectedCollections["My Family"].ElementAt(1).Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(selectedCollections["My Family"].ElementAt(1).Age, Is.EqualTo(33));
            Assert.That(selectedCollections["My Family"].ElementAt(2).Name, Is.EqualTo("Hugo Speer"));
            Assert.That(selectedCollections["My Family"].ElementAt(2).Age, Is.EqualTo(2));

            Assert.That(selectedCollections["Enemies"], Is.Empty);

            Assert.That(selectedCollections["Singular"], Is.All.Not.Null);
            Assert.That(selectedCollections["Singular"].Count(), Is.EqualTo(1));
            Assert.That(selectedCollections["Singular"].ElementAt(0).Name, Is.EqualTo("The Loneliest Number"));
            Assert.That(selectedCollections["Singular"].ElementAt(0).Age, Is.EqualTo(1));

            Assert.That(selectedCollections["Favorite Numbers"], Is.All.Not.Null);
            Assert.That(selectedCollections["Favorite Numbers"].Count(), Is.EqualTo(5));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(0).Name, Is.EqualTo("Random"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(0).Age, Is.EqualTo(9266));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(1).Name, Is.EqualTo("Beverly Hills"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(1).Age, Is.EqualTo(90210));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(2).Name, Is.EqualTo("The Answer"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(2).Age, Is.EqualTo(42));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(3).Name, Is.EqualTo("Highest Count"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(3).Age, Is.EqualTo(600));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(4).Name, Is.EqualTo("leetspeak"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(4).Age, Is.EqualTo(1337));
        }

        [Test]
        public void SelectAllFromTable_Reparse()
        {
            IncrementingDataSelection.MapCount = 0;
            var collectionDataSelector = GetNewInstanceOf<ICollectionDataSelector<IncrementingDataSelection>>();

            var selectedCollections = collectionDataSelector.SelectAllFrom(assemblyName, "DataCollectionTable");
            Assert.That(selectedCollections, Has.Count.EqualTo(4)
                .And.ContainKey("My Family")
                .And.ContainKey("Enemies")
                .And.ContainKey("Singular")
                .And.ContainKey("Favorite Numbers"));

            Assert.That(selectedCollections["My Family"], Is.All.Not.Null);
            Assert.That(selectedCollections["My Family"].Count(), Is.EqualTo(3));
            Assert.That(selectedCollections["My Family"].ElementAt(0).Name, Is.EqualTo("Karl Speer"));
            Assert.That(selectedCollections["My Family"].ElementAt(0).Age, Is.EqualTo(35 + 1));
            Assert.That(selectedCollections["My Family"].ElementAt(1).Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(selectedCollections["My Family"].ElementAt(1).Age, Is.EqualTo(33 + 2));
            Assert.That(selectedCollections["My Family"].ElementAt(2).Name, Is.EqualTo("Hugo Speer"));
            Assert.That(selectedCollections["My Family"].ElementAt(2).Age, Is.EqualTo(2 + 3));

            Assert.That(selectedCollections["Enemies"], Is.Empty);

            Assert.That(selectedCollections["Singular"], Is.All.Not.Null);
            Assert.That(selectedCollections["Singular"].Count(), Is.EqualTo(1));
            Assert.That(selectedCollections["Singular"].ElementAt(0).Name, Is.EqualTo("The Loneliest Number"));
            Assert.That(selectedCollections["Singular"].ElementAt(0).Age, Is.EqualTo(1 + 9));

            Assert.That(selectedCollections["Favorite Numbers"], Is.All.Not.Null);
            Assert.That(selectedCollections["Favorite Numbers"].Count(), Is.EqualTo(5));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(0).Name, Is.EqualTo("Random"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(0).Age, Is.EqualTo(9266 + 4));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(1).Name, Is.EqualTo("Beverly Hills"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(1).Age, Is.EqualTo(90210 + 5));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(2).Name, Is.EqualTo("The Answer"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(2).Age, Is.EqualTo(42 + 6));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(3).Name, Is.EqualTo("Highest Count"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(3).Age, Is.EqualTo(600 + 7));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(4).Name, Is.EqualTo("leetspeak"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(4).Age, Is.EqualTo(1337 + 8));

            selectedCollections = collectionDataSelector.SelectAllFrom(assemblyName, "DataCollectionTable");
            Assert.That(selectedCollections, Has.Count.EqualTo(4)
                .And.ContainKey("My Family")
                .And.ContainKey("Enemies")
                .And.ContainKey("Singular")
                .And.ContainKey("Favorite Numbers"));

            Assert.That(selectedCollections["My Family"], Is.All.Not.Null);
            Assert.That(selectedCollections["My Family"].Count(), Is.EqualTo(3));
            Assert.That(selectedCollections["My Family"].ElementAt(0).Name, Is.EqualTo("Karl Speer"));
            Assert.That(selectedCollections["My Family"].ElementAt(0).Age, Is.EqualTo(35 + 10));
            Assert.That(selectedCollections["My Family"].ElementAt(1).Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(selectedCollections["My Family"].ElementAt(1).Age, Is.EqualTo(33 + 11));
            Assert.That(selectedCollections["My Family"].ElementAt(2).Name, Is.EqualTo("Hugo Speer"));
            Assert.That(selectedCollections["My Family"].ElementAt(2).Age, Is.EqualTo(2 + 12));

            Assert.That(selectedCollections["Enemies"], Is.Empty);

            Assert.That(selectedCollections["Singular"], Is.All.Not.Null);
            Assert.That(selectedCollections["Singular"].Count(), Is.EqualTo(1));
            Assert.That(selectedCollections["Singular"].ElementAt(0).Name, Is.EqualTo("The Loneliest Number"));
            Assert.That(selectedCollections["Singular"].ElementAt(0).Age, Is.EqualTo(1 + 18));

            Assert.That(selectedCollections["Favorite Numbers"], Is.All.Not.Null);
            Assert.That(selectedCollections["Favorite Numbers"].Count(), Is.EqualTo(5));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(0).Name, Is.EqualTo("Random"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(0).Age, Is.EqualTo(9266 + 13));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(1).Name, Is.EqualTo("Beverly Hills"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(1).Age, Is.EqualTo(90210 + 14));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(2).Name, Is.EqualTo("The Answer"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(2).Age, Is.EqualTo(42 + 15));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(3).Name, Is.EqualTo("Highest Count"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(3).Age, Is.EqualTo(600 + 16));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(4).Name, Is.EqualTo("leetspeak"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(4).Age, Is.EqualTo(1337 + 17));
        }

        [TestCase("My Family", true)]
        [TestCase("my family", false)]
        [TestCase("MyFamily", false)]
        [TestCase("My Family ", false)]
        [TestCase("Enemies", true)]
        [TestCase("Singular", true)]
        [TestCase("Karl Speer", false)]
        [TestCase("Favorite Numbers", true)]
        [TestCase("whatever", false)]
        [TestCase("", false)]
        public void IsCollection(string name, bool expected)
        {
            var isCollection = collectionDataSelector.IsCollection(assemblyName, "DataCollectionTable", name);
            Assert.That(isCollection, Is.EqualTo(expected));
        }

        [Test]
        public void SelectOneFrom()
        {
            var selection = collectionDataSelector.SelectOneFrom(assemblyName, "DataCollectionTable", "Singular");
            Assert.That(selection, Is.Not.Null);
            Assert.That(selection.Name, Is.EqualTo("The Loneliest Number"));
            Assert.That(selection.Age, Is.EqualTo(1));
        }

        [Test]
        public void SelectOneFrom_Reparse()
        {
            IncrementingDataSelection.MapCount = 0;
            var collectionDataSelector = GetNewInstanceOf<ICollectionDataSelector<IncrementingDataSelection>>();

            var selection = collectionDataSelector.SelectOneFrom(assemblyName, "DataCollectionTable", "Singular");
            Assert.That(selection, Is.Not.Null);
            Assert.That(selection.Name, Is.EqualTo("The Loneliest Number"));
            Assert.That(selection.Age, Is.EqualTo(1 + 1));

            selection = collectionDataSelector.SelectOneFrom(assemblyName, "DataCollectionTable", "Singular");
            Assert.That(selection, Is.Not.Null);
            Assert.That(selection.Name, Is.EqualTo("The Loneliest Number"));
            Assert.That(selection.Age, Is.EqualTo(1 + 2));
        }

        [Test]
        [Repeat(1000)]
        public void SelectRandomFrom()
        {
            var selection = collectionDataSelector.SelectRandomFrom(assemblyName, "DataCollectionTable", "My Family");
            if (selection.Name == "Karl Speer")
                Assert.That(selection.Age, Is.EqualTo(35));
            else if (selection.Name == "Christine Gnieski")
                Assert.That(selection.Age, Is.EqualTo(33));
            else if (selection.Name == "Hugo Speer")
                Assert.That(selection.Age, Is.EqualTo(2));
            else
                Assert.Fail($"An unknown result of [{selection.Name},{selection.Age}] was returned");
        }

        [Test]
        [Repeat(1000)]
        public void SelectRandomFrom_Reparse()
        {
            IncrementingDataSelection.MapCount = 0;
            var collectionDataSelector = GetNewInstanceOf<ICollectionDataSelector<IncrementingDataSelection>>();

            var selection = collectionDataSelector.SelectRandomFrom(assemblyName, "DataCollectionTable", "My Family");
            if (selection.Name == "Karl Speer")
                Assert.That(selection.Age, Is.EqualTo(35 + 1));
            else if (selection.Name == "Christine Gnieski")
                Assert.That(selection.Age, Is.EqualTo(33 + 1));
            else if (selection.Name == "Hugo Speer")
                Assert.That(selection.Age, Is.EqualTo(2 + 1));
            else
                Assert.Fail($"An unknown result of [{selection.Name},{selection.Age}] was returned");

            selection = collectionDataSelector.SelectRandomFrom(assemblyName, "DataCollectionTable", "My Family");
            if (selection.Name == "Karl Speer")
                Assert.That(selection.Age, Is.EqualTo(35 + 2));
            else if (selection.Name == "Christine Gnieski")
                Assert.That(selection.Age, Is.EqualTo(33 + 2));
            else if (selection.Name == "Hugo Speer")
                Assert.That(selection.Age, Is.EqualTo(2 + 2));
            else
                Assert.Fail($"An unknown result of [{selection.Name},{selection.Age}] was returned");
        }
    }
}
