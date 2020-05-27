using DnDGen.Infrastructure.Generators;
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

        [SetUp]
        public void Setup()
        {
            generator = new IterativeGenerator();
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
        }

        [Test]
        public void GenerateWithMethods()
        {
            var date = generator.Generate(Generate, IsValid, GenerateDefault, Failed, string.Empty);
            Assert.That(iterations, Is.EqualTo(1));
            Assert.That(date, Is.EqualTo(DateTime.Now).Within(1).Seconds);
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
        }

        [Test]
        public void RegenerateIfInvalid()
        {
            generator.MaxAttempts = Limit;

            var randomNumber = generator.Generate(() => iterations++, i => i > 0 && i % 2 == 0, () => -1, i => $"{i} is not valid", string.Empty);
            Assert.That(randomNumber, Is.EqualTo(2));
            Assert.That(iterations, Is.EqualTo(3));
        }

        [Test]
        public void ReturnDefault()
        {
            generator.MaxAttempts = Limit;

            var number = generator.Generate(() => iterations++, i => false, () => -1, i => $"{i} is not valid", "a thing and stuff");
            Assert.That(number, Is.EqualTo(-1));
            Assert.That(iterations, Is.EqualTo(Limit));
        }

        [Test]
        public void ReturnValidObjectAfterTooManyRetries()
        {
            generator.MaxAttempts = Limit;

            var randomString = generator.Generate(() => iterations++.ToString(), i => Convert.ToInt32(i) == Limit - 1, () => "nope", i => $"{i} is not valid", string.Empty);
            Assert.That(iterations, Is.EqualTo(Limit));
            Assert.That(randomString, Is.EqualTo($"{Limit - 1}"));
        }
    }
}
