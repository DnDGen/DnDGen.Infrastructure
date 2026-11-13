using DnDGen.Infrastructure.Selectors.Collections;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Unit.Selectors.Collections
{
    public class RollConstantsTests
    {
        [TestCase(RollConstants.CommonThreshold, 1)]
        [TestCase(RollConstants.UncommonThreshold, 61)]
        [TestCase(RollConstants.RareThreshold, 91)]
        [TestCase(RollConstants.VeryRareThreshold, 100)]
        public void RollThreshold(int actual, int expected) => Assert.That(actual, Is.EqualTo(expected));

        [Test]
        public void CommonRoll() => Assert.That(RollConstants.CommonRoll, Is.EqualTo("1d100"));

        [Test]
        public void UncommonRoll() => Assert.That(RollConstants.UncommonRoll, Is.EqualTo("(1d20-1)*2+1d2+60"));

        [Test]
        public void RareRoll() => Assert.That(RollConstants.RareRoll, Is.EqualTo("1d10+90"));

        [Test]
        public void VeryRareRoll() => Assert.That(RollConstants.VeryRareRoll, Is.EqualTo("100"));
    }
}
