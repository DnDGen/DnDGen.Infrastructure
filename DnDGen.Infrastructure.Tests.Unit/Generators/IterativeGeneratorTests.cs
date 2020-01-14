using DnDGen.EventGen;
using DnDGen.Infrastructure.Generators;
using Moq;
using NUnit.Framework;
using System;

namespace DnDGen.Infrastructure.Tests.Unit.Generators
{
    [TestFixture]
    public class IterativeGeneratorTests
    {
        private const int Limit = 42;

        private Generator generator;
        private int iterations;
        private Mock<GenEventQueue> mockEventQueue;

        [SetUp]
        public void Setup()
        {
            mockEventQueue = new Mock<GenEventQueue>();
            generator = new IterativeGenerator(mockEventQueue.Object);
            iterations = 0;
        }

        [Test]
        public void GeneratorDefaultsMaxAttempts()
        {
            Assert.That(generator.MaxAttempts, Is.EqualTo(1000));
        }

        [Test]
        public void GeneratorCanAlterMaxAttempts()
        {
            generator.MaxAttempts = Limit;
            Assert.That(generator.MaxAttempts, Is.EqualTo(Limit));
        }

        [Test]
        public void GenerateWithLambda()
        {
            var builtString = "built string";
            var randomString = generator.Generate(() => builtString, s => s.Contains("string"), () => string.Empty, s => string.Empty, string.Empty);
            Assert.That(randomString, Is.EqualTo(builtString));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Beginning iterative generation"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Completed iterative generation after 1 iterations"), Times.Once);
        }

        [Test]
        public void GenerateWithMethods()
        {
            var date = generator.Generate(Generate, IsValid, GenerateDefault, Failed, string.Empty);
            Assert.That(iterations, Is.EqualTo(1));
            Assert.That(date, Is.EqualTo(DateTime.Now).Within(1).Seconds);
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Beginning iterative generation"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Completed iterative generation after 1 iterations"), Times.Once);
        }

        private DateTime Generate()
        {
            iterations++;
            return DateTime.Now;
        }

        private bool IsValid(DateTime date)
        {
            return date.Year == DateTime.Now.Year;
        }

        private string Failed(DateTime date)
        {
            return date.ToString();
        }

        private DateTime GenerateDefault()
        {
            return new DateTime(1989, 10, 29);
        }

        [Test]
        public void GenerateNull()
        {
            var randomObject = generator.Generate(() => null, s => true, () => new object(), s => string.Empty, string.Empty);
            Assert.That(randomObject, Is.Null);
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(2));
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Beginning iterative generation"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Completed iterative generation after 1 iterations"), Times.Once);
        }

        [Test]
        public void RegenerateIfInvalid()
        {
            generator.MaxAttempts = Limit;

            var randomNumber = generator.Generate(() => iterations++, i => i > 0 && i % 2 == 0, () => -1, i => $"{i} is not valid", string.Empty);
            Assert.That(randomNumber, Is.EqualTo(2));
            Assert.That(iterations, Is.EqualTo(3));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(4));
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Beginning iterative generation"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "(Attempt 1) 0 is not valid"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "(Attempt 2) 1 is not valid"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Completed iterative generation after 3 iterations"), Times.Once);
        }

        [Test]
        public void ReturnDefault()
        {
            generator.MaxAttempts = Limit;

            var number = generator.Generate(() => iterations++, i => false, () => -1, i => $"{i} is not valid", "a thing and stuff");
            Assert.That(number, Is.EqualTo(-1));
            Assert.That(iterations, Is.EqualTo(Limit));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(Limit + 2));
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Beginning iterative generation"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Generating a thing and stuff by default"), Times.Once);

            for (var i = 0; i < generator.MaxAttempts; i++)
                mockEventQueue.Verify(q => q.Enqueue("Infrastructure", $"(Attempt {i + 1}) {i} is not valid"), Times.Once);
        }

        [Test]
        public void ReturnValidObjectAfterTooManyRetries()
        {
            generator.MaxAttempts = Limit;

            var randomString = generator.Generate(() => iterations++.ToString(), i => Convert.ToInt32(i) == Limit - 1, () => "nope", i => $"{i} is not valid", string.Empty);
            Assert.That(iterations, Is.EqualTo(Limit));
            Assert.That(randomString, Is.EqualTo($"{Limit - 1}"));
            mockEventQueue.Verify(q => q.Enqueue(It.IsAny<string>(), It.IsAny<string>()), Times.Exactly(Limit + 1));
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Beginning iterative generation"), Times.Once);
            mockEventQueue.Verify(q => q.Enqueue("Infrastructure", "Completed iterative generation after 42 iterations"), Times.Once);

            for (var i = 0; i < generator.MaxAttempts - 1; i++)
                mockEventQueue.Verify(q => q.Enqueue("Infrastructure", $"(Attempt {i + 1}) {i} is not valid"), Times.Once);
        }
    }
}
