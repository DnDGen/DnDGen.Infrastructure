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
        public void SelectAllFromTable()
        {
            var selectedCollections = collectionDataSelector.SelectAllFrom(assemblyName, "DataCollectionTable");
            Assert.That(selectedCollections, Has.Count.EqualTo(3)
                .And.ContainKey("My Family")
                .And.ContainKey("Enemies")
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
            Assert.That(selectedCollections, Has.Count.EqualTo(3)
                .And.ContainKey("My Family")
                .And.ContainKey("Enemies")
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
            Assert.That(selectedCollections, Has.Count.EqualTo(3)
                .And.ContainKey("My Family")
                .And.ContainKey("Enemies")
                .And.ContainKey("Favorite Numbers"));

            Assert.That(selectedCollections["My Family"], Is.All.Not.Null);
            Assert.That(selectedCollections["My Family"].Count(), Is.EqualTo(3));
            Assert.That(selectedCollections["My Family"].ElementAt(0).Name, Is.EqualTo("Karl Speer"));
            Assert.That(selectedCollections["My Family"].ElementAt(0).Age, Is.EqualTo(35 + 9));
            Assert.That(selectedCollections["My Family"].ElementAt(1).Name, Is.EqualTo("Christine Gnieski"));
            Assert.That(selectedCollections["My Family"].ElementAt(1).Age, Is.EqualTo(33 + 10));
            Assert.That(selectedCollections["My Family"].ElementAt(2).Name, Is.EqualTo("Hugo Speer"));
            Assert.That(selectedCollections["My Family"].ElementAt(2).Age, Is.EqualTo(2 + 11));

            Assert.That(selectedCollections["Enemies"], Is.Empty);

            Assert.That(selectedCollections["Favorite Numbers"], Is.All.Not.Null);
            Assert.That(selectedCollections["Favorite Numbers"].Count(), Is.EqualTo(5));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(0).Name, Is.EqualTo("Random"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(0).Age, Is.EqualTo(9266 + 12));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(1).Name, Is.EqualTo("Beverly Hills"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(1).Age, Is.EqualTo(90210 + 13));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(2).Name, Is.EqualTo("The Answer"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(2).Age, Is.EqualTo(42 + 14));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(3).Name, Is.EqualTo("Highest Count"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(3).Age, Is.EqualTo(600 + 15));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(4).Name, Is.EqualTo("leetspeak"));
            Assert.That(selectedCollections["Favorite Numbers"].ElementAt(4).Age, Is.EqualTo(1337 + 16));
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
