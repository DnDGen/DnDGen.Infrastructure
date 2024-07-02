using DnDGen.Infrastructure.Another;
using DnDGen.Infrastructure.Other;
using DnDGen.Infrastructure.YetAnother;
using NUnit.Framework;

namespace DnDGen.Infrastructure.Tests.Integration
{
    internal class GeneratorTests : IntegrationTests
    {
        private OtherGenerator otherGenerator;
        private AnotherGenerator anotherGenerator;
        private YetAnotherGenerator yetAnotherGenerator;

        [SetUp]
        public void Setup()
        {
            otherGenerator = GetNewInstanceOf<OtherGenerator>();
            anotherGenerator = GetNewInstanceOf<AnotherGenerator>();
            yetAnotherGenerator = GetNewInstanceOf<YetAnotherGenerator>();
        }

        [Test]
        [Repeat(1000)]
        public void OtherGenerator_Generates()
        {
            var other = otherGenerator.Generate();
            Assert.That(other, Is.EqualTo("Real Selector Subvalue 1 + Not Subvalue 1 + one")
                .Or.EqualTo("Real Selector Subvalue 1 + Not Subvalue 1 + two")
                .Or.EqualTo("Real Selector Subvalue 1 + Not Subvalue 1 + three")
                .Or.EqualTo("Real Selector Subvalue 1 + Not Subvalue 1 + four to ten")
                .Or.EqualTo("Real Selector Subvalue 1 + Not Subvalue 1 + eleven to one hundred"));
        }

        [Test]
        [Repeat(1000)]
        public void AnotherGenerator_Generates()
        {
            var another = anotherGenerator.Generate();
            Assert.That(another, Does.StartWith("Real Caller Subvalue 1 + Also Not Subvalue 1 + one to ninety: ")
                .Or.StartWith("Real Caller Subvalue 1 + Also Not Subvalue 1 + ninety-one to ninety-seven: ")
                .Or.StartWith("Real Caller Subvalue 1 + Also Not Subvalue 1 + ninety-eight: ")
                .Or.StartWith("Real Caller Subvalue 1 + Also Not Subvalue 1 + ninety-nine: ")
                .Or.StartWith("Real Caller Subvalue 1 + Also Not Subvalue 1 + one hundred: "));
            Assert.That(another, Does.EndWith(": Real Selector Subvalue 1 + Not Subvalue 1 + one")
                .Or.EndWith(": Real Selector Subvalue 1 + Not Subvalue 1 + two")
                .Or.EndWith(": Real Selector Subvalue 1 + Not Subvalue 1 + three")
                .Or.EndWith(": Real Selector Subvalue 1 + Not Subvalue 1 + four to ten")
                .Or.EndWith(": Real Selector Subvalue 1 + Not Subvalue 1 + eleven to one hundred"));
        }

        [Test]
        [Repeat(1000)]
        public void YetAnotherGenerator_Generates()
        {
            var yetAnother = yetAnotherGenerator.Generate();
            Assert.That(yetAnother, Does.StartWith("Real Subvalue 1 + Definitely Not Subvalue 1 + one to thirty-nine: ")
                .Or.StartWith("Real Subvalue 1 + Definitely Not Subvalue 1 + just under fifty: ")
                .Or.StartWith("Real Subvalue 1 + Definitely Not Subvalue 1 + fifty: ")
                .Or.StartWith("Real Subvalue 1 + Definitely Not Subvalue 1 + just over fifty: ")
                .Or.StartWith("Real Subvalue 1 + Definitely Not Subvalue 1 + sixty-one to one hundred: "));
            Assert.That(yetAnother, Contains.Substring(": Real Caller Subvalue 1 + Also Not Subvalue 1 + one to ninety: ")
                .Or.Contains(": Real Caller Subvalue 1 + Also Not Subvalue 1 + ninety-one to ninety-seven: ")
                .Or.Contains(": Real Caller Subvalue 1 + Also Not Subvalue 1 + ninety-eight: ")
                .Or.Contains(": Real Caller Subvalue 1 + Also Not Subvalue 1 + ninety-nine: ")
                .Or.Contains(": Real Caller Subvalue 1 + Also Not Subvalue 1 + one hundred: "));
            Assert.That(yetAnother, Does.EndWith(": Real Selector Subvalue 1 + Not Subvalue 1 + one")
                .Or.EndWith(": Real Selector Subvalue 1 + Not Subvalue 1 + two")
                .Or.EndWith(": Real Selector Subvalue 1 + Not Subvalue 1 + three")
                .Or.EndWith(": Real Selector Subvalue 1 + Not Subvalue 1 + four to ten")
                .Or.EndWith(": Real Selector Subvalue 1 + Not Subvalue 1 + eleven to one hundred"));
        }
    }
}
