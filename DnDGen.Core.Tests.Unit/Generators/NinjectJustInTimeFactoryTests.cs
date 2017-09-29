using DnDGen.Core.Generators;
using DnDGen.Core.Selectors.Percentiles;
using Moq;
using Ninject;
using NUnit.Framework;

namespace DnDGen.Core.Tests.Unit.Generators
{
    [TestFixture]
    public class NinjectJustInTimeFactoryTests
    {
        private JustInTimeFactory justInTimeFactory;
        private IKernel kernel;

        [SetUp]
        public void Setup()
        {
            kernel = new StandardKernel();
            justInTimeFactory = new NinjectJustInTimeFactory(kernel);
        }

        [Test]
        public void FactoryBuilds()
        {
            var mockGenerator = new Mock<IPercentileSelector>();
            kernel.Bind<IPercentileSelector>().ToMethod(c => mockGenerator.Object);

            var generator = justInTimeFactory.Build<IPercentileSelector>();
            Assert.That(generator, Is.Not.Null);
            Assert.That(generator, Is.EqualTo(mockGenerator.Object));
        }

        [Test]
        public void FactoryDoesNotBuildIfNameGivenWhenNoNameBound()
        {
            var mockGenerator = new Mock<IPercentileSelector>();
            kernel.Bind<IPercentileSelector>().ToMethod(c => mockGenerator.Object);

            Assert.That(() => justInTimeFactory.Build<IPercentileSelector>("name"), Throws.Exception);
        }

        [Test]
        public void FactoryBuildsWithName()
        {
            var mockGenerator = new Mock<IPercentileSelector>();
            kernel.Bind<IPercentileSelector>().ToMethod(c => mockGenerator.Object).Named("name");

            var generator = justInTimeFactory.Build<IPercentileSelector>("name");
            Assert.That(generator, Is.Not.Null);
            Assert.That(generator, Is.EqualTo(mockGenerator.Object));
        }

        [Test]
        public void FactoryBuildsWithCorrectName()
        {
            var mockGenerator = new Mock<IPercentileSelector>();
            var otherMockGenerator = new Mock<IPercentileSelector>();

            kernel.Bind<IPercentileSelector>().ToMethod(c => mockGenerator.Object).Named("name");
            kernel.Bind<IPercentileSelector>().ToMethod(c => otherMockGenerator.Object).Named("other name");

            var generator = justInTimeFactory.Build<IPercentileSelector>("name");
            Assert.That(generator, Is.Not.Null);
            Assert.That(generator, Is.EqualTo(mockGenerator.Object));
        }

        [Test]
        public void FactoryBuildsWithNoNameIfOnlyOneNameBound()
        {
            var mockGenerator = new Mock<IPercentileSelector>();
            kernel.Bind<IPercentileSelector>().ToMethod(c => mockGenerator.Object).Named("name");

            var generator = justInTimeFactory.Build<IPercentileSelector>("name");
            Assert.That(generator, Is.Not.Null);
            Assert.That(generator, Is.EqualTo(mockGenerator.Object));
        }

        [Test]
        public void FactoryDoesNotBuildIfNameNotGivenWhenNameBound()
        {
            var mockGenerator = new Mock<IPercentileSelector>();
            var otherMockGenerator = new Mock<IPercentileSelector>();

            kernel.Bind<IPercentileSelector>().ToMethod(c => mockGenerator.Object).Named("name");
            kernel.Bind<IPercentileSelector>().ToMethod(c => otherMockGenerator.Object).Named("other name");

            Assert.That(() => justInTimeFactory.Build<IPercentileSelector>(), Throws.Exception);
        }

        [Test]
        public void FactoryDoesNotBuildIfWrongNameNotGivenWhenNameBound()
        {
            var mockGenerator = new Mock<IPercentileSelector>();
            kernel.Bind<IPercentileSelector>().ToMethod(c => mockGenerator.Object).Named("name");

            Assert.That(() => justInTimeFactory.Build<IPercentileSelector>("other name"), Throws.Exception);
        }
    }
}
