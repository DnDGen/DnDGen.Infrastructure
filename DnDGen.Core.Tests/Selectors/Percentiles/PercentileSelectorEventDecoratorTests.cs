using DnDGen.Core.Selectors.Percentiles;
using EventGen;
using Moq;
using NUnit.Framework;

namespace DnDGen.Core.Tests.Selectors.Percentiles
{
    [TestFixture]
    public class PercentileSelectorEventDecoratorTests
    {
        private IPercentileSelector decorator;
        private Mock<IPercentileSelector> mockInnerSelector;
        private Mock<GenEventQueue> mockEventQueue;

        [SetUp]
        public void Setup()
        {
            mockInnerSelector = new Mock<IPercentileSelector>();
            mockEventQueue = new Mock<GenEventQueue>();
            decorator = new PercentileSelectorEventDecorator(mockInnerSelector.Object, mockEventQueue.Object);
        }

        [Test]
        public void ReturnInnerSelection()
        {
            mockInnerSelector.Setup(s => s.SelectFrom("table name")).Returns("random selection");
            var selection = decorator.SelectFrom("table name");
            Assert.That(selection, Is.EqualTo("random selection"));
        }

        [Test]
        public void LogEventsForSelection()
        {
            mockInnerSelector.Setup(s => s.SelectFrom("table name")).Returns("random selection");

            var selection = decorator.SelectFrom("table name");
            Assert.That(selection, Is.EqualTo("random selection"));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Rolling percentile in table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Selected random selection from table name"), Times.Once);
        }

        [Test]
        public void ReturnCastedInnerSelection()
        {
            mockInnerSelector.Setup(s => s.SelectFrom<int>("table name")).Returns(9266);
            var selection = decorator.SelectFrom<int>("table name");
            Assert.That(selection, Is.EqualTo(9266));
        }

        [Test]
        public void LogEventsForCastedSelection()
        {
            mockInnerSelector.Setup(s => s.SelectFrom<int>("table name")).Returns(9266);

            var selection = decorator.SelectFrom<int>("table name");
            Assert.That(selection, Is.EqualTo(9266));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Rolling percentile in table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Selected 9266 from table name"), Times.Once);
        }

        [Test]
        public void ReturnInnerAllSelection()
        {
            var percentiles = new[] { "thing 1", "thing 2" };
            mockInnerSelector.Setup(s => s.SelectAllFrom("table name")).Returns(percentiles);

            var selection = decorator.SelectAllFrom("table name");
            Assert.That(selection, Is.EqualTo(percentiles));
        }

        [Test]
        public void LogEventsForAllSelection()
        {
            var percentiles = new[] { "thing 1", "thing 2" };
            mockInnerSelector.Setup(s => s.SelectAllFrom("table name")).Returns(percentiles);

            var selection = decorator.SelectAllFrom("table name");
            Assert.That(selection, Is.EqualTo(percentiles));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Selecting all percentile results from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Selected 2 results from table name"), Times.Once);
        }

        [Test]
        public void ReturnCastedInnerAllSelection()
        {
            var percentiles = new[] { 9266, 90210 };
            mockInnerSelector.Setup(s => s.SelectAllFrom<int>("table name")).Returns(percentiles);

            var selection = decorator.SelectAllFrom<int>("table name");
            Assert.That(selection, Is.EqualTo(percentiles));
        }

        [Test]
        public void LogEventsForCastedAllSelection()
        {
            var percentiles = new[] { 9266, 90210 };
            mockInnerSelector.Setup(s => s.SelectAllFrom<int>("table name")).Returns(percentiles);

            var selection = decorator.SelectAllFrom<int>("table name");
            Assert.That(selection, Is.EqualTo(percentiles));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Selecting all percentile results from table name"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Core", $"Selected 2 results from table name"), Times.Once);
        }
    }
}
