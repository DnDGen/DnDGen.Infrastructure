using DnDGen.Infrastructure.Mappers.Collections;
using DnDGen.Infrastructure.Selectors.Collections;
using DnDGen.RollGen;
using Moq;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DnDGen.Infrastructure.Tests.Unit.Selectors.Collections
{
    [TestFixture]
    public class CollectionSelectorTests
    {
        private const string TableName = "table name";
        private const string AssemblyName = "my assembly";

        private ICollectionSelector selector;
        private Mock<CollectionMapper> mockMapper;
        private Mock<Dice> mockDice;
        private Dictionary<string, IEnumerable<string>> allCollections;

        [SetUp]
        public void Setup()
        {
            mockMapper = new Mock<CollectionMapper>();
            mockDice = new Mock<Dice>();
            selector = new CollectionSelector(mockMapper.Object, mockDice.Object);
            allCollections = [];

            mockMapper.Setup(m => m.Map(AssemblyName, TableName)).Returns(allCollections);
            mockDice.Setup(d => d.Roll("100").AsSum<int>()).Returns(100);
            mockDice.Setup(d => d.Roll(1).d(1).AsSum<int>()).Returns(1);


        }

        [Test]
        public void SelectFrom_SelectCollection()
        {
            allCollections["entry"] = Enumerable.Empty<string>();
            var collection = selector.SelectFrom(AssemblyName, TableName, "entry");
            Assert.That(collection, Is.EqualTo(allCollections["entry"]));
        }

        [Test]
        public void SelectAllFrom_SelectAllCollections()
        {
            var collections = selector.SelectAllFrom(AssemblyName, TableName);
            Assert.That(collections, Is.EqualTo(allCollections));
        }

        [Test]
        public void SelectFrom_IfEntryNotPresentInTable_ThrowException()
        {
            Assert.That(() => selector.SelectFrom(AssemblyName, TableName, "entry"),
                Throws.Exception.With.Message.EqualTo("entry is not a valid collection in the table table name"));
        }

        [Test]
        public void SelectRandomFrom_SelectRandomItemFromCollection()
        {
            var collection = new[] { "item 1", "item 2", "item 3" };
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(2);

            var item = selector.SelectRandomFrom(collection);
            Assert.That(item, Is.EqualTo("item 2"));
        }

        [Test]
        public void SelectRandomFrom_SelectRandomItemFromTable()
        {
            allCollections["entry"] = new[] { "item 1", "item 2", "item 3" };
            mockDice.Setup(d => d.Roll(1).d(3).AsSum<int>()).Returns(2);

            var item = selector.SelectRandomFrom(AssemblyName, TableName, "entry");
            Assert.That(item, Is.EqualTo("item 2"));
        }

        [Test]
        public void SelectRandomFrom_CannotSelectRandomFromEmptyCollection()
        {
            var collection = Enumerable.Empty<string>();
            Assert.That(() => selector.SelectRandomFrom(collection), Throws.ArgumentException.With.Message.EqualTo("Cannot select random from an empty collection"));
        }

        [Test]
        public void SelectRandomFrom_CannotSelectRandomFromEmptyTable()
        {
            allCollections["entry"] = [];
            Assert.That(() => selector.SelectRandomFrom(AssemblyName, TableName, "entry"),
                Throws.ArgumentException.With.Message.EqualTo("Cannot select random from an empty collection"));
        }

        [Test]
        public void SelectRandomFrom_CannotSelectRandomFromInvalidEntry()
        {
            Assert.That(() => selector.SelectRandomFrom(AssemblyName, TableName, "entry"),
                Throws.Exception.With.Message.EqualTo("entry is not a valid collection in the table table name"));
        }

        [Test]
        public void SelectRandomFrom_SelectRandomFromNonStringCollection()
        {
            var collection = new[] { 9266, 90210, 42, 600, 1337 };

            mockDice.Setup(d => d.Roll(1).d(5).AsSum<int>()).Returns(2);

            var entry = selector.SelectRandomFrom(collection);
            Assert.That(entry, Is.EqualTo(90210));
        }

        [Test]
        public void FindCollectionOf_FindCollectionContainingEntry()
        {
            allCollections["entry"] = ["first", "fourth"];
            allCollections["other entry"] = ["third", "fourth"];
            allCollections["wrong entry"] = ["fifth", "second"];

            var collectionName = selector.FindCollectionOf(AssemblyName, TableName, "fourth");
            Assert.That(collectionName, Is.EqualTo("entry"));
        }

        [Test]
        public void FindCollectionOf_DoNotFindCollectionContainingEntry()
        {
            allCollections["entry"] = new[] { "first", "fourth" };
            allCollections["other entry"] = new[] { "third", "fourth" };
            allCollections["wrong entry"] = new[] { "fifth", "second" };

            Assert.That(() => selector.FindCollectionOf(AssemblyName, TableName, "sixth"),
                Throws.ArgumentException.With.Message.EqualTo("No collection in table name contains sixth"));
        }

        [Test]
        public void FindCollectionOf_FindCollectionContainingEntryWithFilteredCollectionNames()
        {
            allCollections["entry"] = new[] { "first", "second" };
            allCollections["other entry"] = new[] { "third", "fourth" };
            allCollections["wrong entry"] = new[] { "fifth", "fourth" };

            var group = selector.FindCollectionOf(AssemblyName, TableName, "fourth", "entry", "other entry");
            Assert.That(group, Is.EqualTo("other entry"));
        }

        [Test]
        public void FindCollectionOf_FindCollectionContainingEntryThrowsExceptionIfNotInFilteredCollectionNames()
        {
            allCollections["entry"] = new[] { "first", "second" };
            allCollections["other entry"] = new[] { "third", "fifth" };
            allCollections["wrong entry"] = new[] { "third", "fourth" };

            Assert.That(() => selector.FindCollectionOf(AssemblyName, TableName, "fourth", "entry", "other entry"),
                Throws.ArgumentException.With.Message.EqualTo("No collection from the 2 filters in table name contains fourth"));
        }

        [Test]
        public void IsCollection_IsCollection()
        {
            allCollections["entry"] = new[] { "first", "second" };
            var isCollection = selector.IsCollection(AssemblyName, TableName, "entry");
            Assert.That(isCollection, Is.True);
        }

        [Test]
        public void IsCollection_IsNotEntry()
        {
            allCollections["entry"] = new[] { "first", "second" };
            var isCollection = selector.IsCollection(AssemblyName, TableName, "other entry");
            Assert.That(isCollection, Is.False);
        }

        [Test]
        public void IsCollection_EntryIsNotCollection()
        {
            allCollections["entry"] = new[] { "first", "second" };
            var isCollection = selector.IsCollection(AssemblyName, TableName, "first");
            Assert.That(isCollection, Is.False);
        }

        [Test]
        public void IsCollection_EntryIsCollection()
        {
            allCollections["entry"] = ["first", "second"];
            allCollections["first"] = ["first", "third"];

            var isCollection = selector.IsCollection(AssemblyName, TableName, "first");
            Assert.That(isCollection, Is.True);
        }

        [Test]
        public void SelectRandomFrom_SelectRandomFromEmptyWeightedCollectionWithDefault()
        {
            Assert.That(() => selector.SelectRandomFrom<string>(), Throws.ArgumentException.With.Message.EqualTo("Cannot select random from an empty collection"));
        }

        [Test]
        public void SelectRandomFrom_SelectRandomFromEmptyWeightedCollection()
        {
            var common = new List<string>();
            var uncommon = new List<string>();
            var rare = new List<string>();
            var veryRare = new List<string>();

            Assert.That(() => selector.SelectRandomFrom(common, uncommon, rare, veryRare),
                Throws.ArgumentException.With.Message.EqualTo("Cannot select random from an empty collection"));
        }

        [TestCase("very rare", 100, 100)]
        [TestCase("rare", 91, 99)]
        [TestCase("uncommon", 61, 90)]
        [TestCase("common", 1, 60)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollection(string result, int lower, int upper)
        {
            var common = new[] { "common" };
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, uncommon, rare, veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("very rare", 100, 100)]
        [TestCase("rare", 91, 99)]
        [TestCase("uncommon", 61, 90)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithNullCommon(string result, int lower, int upper)
        {
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            for (var i = 61; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("(1d20-1)*2+1d2+60").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(uncommon: uncommon, rare: rare, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("very rare", 100, 100)]
        [TestCase("rare", 91, 99)]
        [TestCase("common", 1, 90)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithNullUncommon(string result, int lower, int upper)
        {
            var common = new[] { "common" };
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, rare: rare, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("very rare", 100, 100)]
        [TestCase("uncommon", 61, 99)]
        [TestCase("common", 1, 60)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithNullRare(string result, int lower, int upper)
        {
            var common = new[] { "common" };
            var uncommon = new[] { "uncommon" };
            var veryRare = new[] { "very rare" };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, uncommon, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("rare", 91, 100)]
        [TestCase("uncommon", 61, 90)]
        [TestCase("common", 1, 60)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithNullVeryRare(string result, int lower, int upper)
        {
            var common = new[] { "common" };
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, uncommon, rare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("very rare", 100, 100)]
        [TestCase("rare", 91, 99)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithNullCommonAndUncommon(string result, int lower, int upper)
        {
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            for (var i = 91; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d10+90").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(rare: rare, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("very rare", 100, 100)]
        [TestCase("uncommon", 61, 99)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithNullCommonAndRare(string result, int lower, int upper)
        {
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            for (var i = 61; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("(1d20-1)*2+1d2+60").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(uncommon: uncommon, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("rare", 91, 100)]
        [TestCase("uncommon", 61, 90)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithNullCommonAndVeryRare(string result, int lower, int upper)
        {
            var uncommon = new[] { "uncommon" };
            var rare = new[] { "rare" };
            var veryRare = new[] { "very rare" };

            for (var i = 61; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("(1d20-1)*2+1d2+60").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(uncommon: uncommon, rare: rare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("very rare", 100, 100)]
        [TestCase("common", 1, 99)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithNullUncommonAndRare(string result, int lower, int upper)
        {
            var common = new[] { "common" };
            var veryRare = new[] { "very rare" };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("rare", 91, 100)]
        [TestCase("common", 1, 90)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithNullUncommonAndVeryRare(string result, int lower, int upper)
        {
            var common = new[] { "common" };
            var rare = new[] { "rare" };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, rare: rare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("uncommon", 61, 100)]
        [TestCase("common", 1, 60)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithNullRareAndVeryRare(string result, int lower, int upper)
        {
            var common = new[] { "common" };
            var uncommon = new[] { "uncommon" };
            var veryRare = new[] { "very rare" };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, uncommon);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("very rare", 100, 100)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithOnlyVeryRare(string result, int lower, int upper)
        {
            var veryRare = new[] { "very rare" };

            for (var i = 100; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("rare", 91, 100)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithOnlyRare(string result, int lower, int upper)
        {
            var rare = new[] { "rare" };

            for (var i = 91; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d10+90").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(rare: rare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("uncommon", 61, 100)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithOnlyUncommon(string result, int lower, int upper)
        {
            var uncommon = new[] { "uncommon" };

            for (var i = 61; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("(1d20-1)*2+1d2+60").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(uncommon: uncommon);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase("common", 1, 100)]
        public void SelectRandomFrom_SelectRandomFromWeightedCollectionWithOnlyCommon(string result, int lower, int upper)
        {
            var common = new[] { "common" };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common: common);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [Test]
        public void SelectRandomFrom_SelectRandomNonStringFromEmptyWeightedCollectionWithDefault()
        {
            Assert.That(() => selector.SelectRandomFrom<int>(), Throws.ArgumentException.With.Message.EqualTo("Cannot select random from an empty collection"));
        }

        [Test]
        public void SelectRandomFrom_SelectRandomNonStringFromEmptyWeightedCollection()
        {
            var common = new List<int>();
            var uncommon = new List<int>();
            var rare = new List<int>();
            var veryRare = new List<int>();

            Assert.That(() => selector.SelectRandomFrom(common, uncommon, rare, veryRare),
                Throws.ArgumentException.With.Message.EqualTo("Cannot select random from an empty collection"));
        }

        [TestCase(9266, 100, 100)]
        [TestCase(90210, 91, 99)]
        [TestCase(42, 61, 90)]
        [TestCase(600, 1, 60)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollection(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, uncommon, rare, veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Roll {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Roll {i}");
                }
            }
        }

        [TestCase(9266, 100, 100)]
        [TestCase(90210, 91, 99)]
        [TestCase(42, 61, 90)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithNullCommon(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 61; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("(1d20-1)*2+1d2+60").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(uncommon: uncommon, rare: rare, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(9266, 100, 100)]
        [TestCase(90210, 91, 99)]
        [TestCase(600, 1, 90)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithNullUncommon(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, rare: rare, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(9266, 100, 100)]
        [TestCase(42, 61, 99)]
        [TestCase(600, 1, 60)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithNullRare(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, uncommon, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(90210, 91, 100)]
        [TestCase(42, 61, 90)]
        [TestCase(600, 1, 60)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithNullVeryRare(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common, uncommon, rare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(9266, 100, 100)]
        [TestCase(90210, 91, 99)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithNullCommonAndUncommon(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 91; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d10+90").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(rare: rare, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(9266, 100, 100)]
        [TestCase(42, 61, 99)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithNullCommonAndRare(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 61; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("(1d20-1)*2+1d2+60").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(uncommon: uncommon, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(90210, 91, 100)]
        [TestCase(42, 61, 90)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithNullCommonAndVeryRare(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 61; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("(1d20-1)*2+1d2+60").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(uncommon: uncommon, rare: rare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(9266, 100, 100)]
        [TestCase(600, 1, 99)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithNullUncommonAndRare(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common: common, veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(90210, 91, 100)]
        [TestCase(600, 1, 90)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithNullUncommonAndVeryRare(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common: common, rare: rare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(42, 61, 100)]
        [TestCase(600, 1, 60)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithNullRareAndVeryRare(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common: common, uncommon: uncommon);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(600, 1, 100)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithOnlyCommon(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 1; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(common: common);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(42, 61, 100)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithOnlyUncommon(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 61; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("(1d20-1)*2+1d2+60").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(uncommon: uncommon);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(90210, 91, 100)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithOnlyRare(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 91; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("1d10+90").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(rare: rare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }

        [TestCase(9266, 100, 100)]
        public void SelectRandomFrom_SelectRandomNonStringFromWeightedCollectionWithOnlyVeryRare(int result, int lower, int upper)
        {
            var common = new[] { 600 };
            var uncommon = new[] { 42 };
            var rare = new[] { 90210 };
            var veryRare = new[] { 9266 };

            for (var i = 100; i <= 100; i++)
            {
                mockDice.Setup(d => d.Roll("100").AsSum<int>()).Returns(i);

                var random = selector.SelectRandomFrom(veryRare: veryRare);

                if (lower <= i && i <= upper)
                {
                    Assert.That(random, Is.EqualTo(result), $"Index {i}");
                }
                else
                {
                    Assert.That(random, Is.Not.EqualTo(result), $"Index {i}");
                }
            }
        }
    }
}