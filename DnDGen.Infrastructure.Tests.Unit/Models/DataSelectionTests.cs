using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Unit.Models
{
    [TestFixture]
    internal class DataSelectionTests
    {
        private FakeDataSelection selection;

        [SetUp]
        public void Setup()
        {
            selection = new FakeDataSelection();
        }

        [Test]
        public void Separator_HasDefault()
        {
            Assert.That(selection.Separator, Is.EqualTo('@'));
        }

        [Test]
        public void Separator_CanBeOverridden()
        {
            var otherSelection = new OtherFakeDataSelection();
            Assert.That(otherSelection.Separator, Is.EqualTo(','));
        }
    }
}
