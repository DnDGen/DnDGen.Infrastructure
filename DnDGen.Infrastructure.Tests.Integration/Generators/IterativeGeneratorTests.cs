using DnDGen.Infrastructure.Generators;
using NUnit.Framework;
using System;
using System.Diagnostics;

namespace DnDGen.Infrastructure.Tests.Integration.Generators
{
    [TestFixture]
    public class IterativeGeneratorTests : IntegrationTests
    {
        private Generator generator;
        private Stopwatch stopwatch;
        private Random random;

        [SetUp]
        public void Setup()
        {
            generator = GetNewInstanceOf<Generator>();
            stopwatch = new Stopwatch();
            random = GetNewInstanceOf<Random>();
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        [TestCase(1000)]
        public void GeneratorIsEfficient(int iterations)
        {
            var count = 1;
            stopwatch.Start();

            var result = generator.Generate(
                () => count++,
                c => c == iterations,
                () => 9266,
                c => $"{c} is not equal to {iterations}",
                "default 9266");

            stopwatch.Stop();

            Assert.That(result, Is.EqualTo(iterations));
            Assert.That(count, Is.EqualTo(iterations + 1));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(2));
        }

        [TestCase(1, 1)]
        [TestCase(1, 10)]
        [TestCase(1, 100)]
        [TestCase(1, 1000)]
        [TestCase(10, 1)]
        [TestCase(10, 10)]
        [TestCase(10, 100)]
        [TestCase(10, 1000)]
        [TestCase(100, 1)]
        [TestCase(100, 10)]
        [TestCase(100, 100)]
        [TestCase(100, 1000)]
        [TestCase(1000, 1)]
        [TestCase(1000, 10)]
        [TestCase(1000, 100)]
        [TestCase(1000, 1000)]
        public void GeneratorInceptionIsEfficient(int iterations, int subIterations)
        {
            var count = 1;
            stopwatch.Start();

            var result = generator.Generate(
                () => BuildInGenerator(count++, subIterations),
                c => c == iterations,
                () => 9266,
                c => $"{c} is not equal to {iterations}",
                "default 9266");

            stopwatch.Stop();

            Assert.That(result, Is.EqualTo(iterations));
            Assert.That(count, Is.EqualTo(iterations + 1));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(7));
        }

        private int BuildInGenerator(int count, int subIterations)
        {
            var subcount = 1;

            var result = generator.Generate(
                () => subcount++,
                c => c == subIterations,
                () => 90210,
                c => $"{c} is not equal to {subIterations}",
                "default 90210");

            return count;
        }

        [Test]
        public void GeneratorIsEfficientWithDefaults()
        {
            var count = 1;
            stopwatch.Start();

            var result = generator.Generate(
                () => count++,
                c => c == 9266,
                () => 90210,
                c => $"{c} is not equal to {9266}",
                "default 90210");

            stopwatch.Stop();

            Assert.That(result, Is.EqualTo(90210));
            Assert.That(count, Is.EqualTo(generator.MaxAttempts + 1));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(2));
        }

        [Test]
        public void GeneratorInceptionIsEfficientWithDefaults()
        {
            var count = 1;
            stopwatch.Start();

            var result = generator.Generate(
                () => BuildInGenerator(count++, 1337),
                c => c == 9266,
                () => 42,
                c => $"{c} is not equal to {9266}",
                "default 42");

            stopwatch.Stop();

            Assert.That(result, Is.EqualTo(42));
            Assert.That(count, Is.EqualTo(generator.MaxAttempts + 1));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(7));
        }

        [TestCase(1)]
        [TestCase(10)]
        [TestCase(100)]
        public void RareThingOccurs(int chanceDivisor)
        {
            var chance = 1d / chanceDivisor;

            stopwatch.Start();

            var result = generator.Generate(
                () => random.NextDouble(),
                c => c <= chance,
                () => 666,
                c => $"{c} is not less than {chance}",
                "FAILED TO GENERATE");

            stopwatch.Stop();

            Assert.That(result, Is.LessThan(chance));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(1));
        }

        [TestCase(1, 1)]
        [TestCase(10, 1)]
        [TestCase(100, 1)]
        [TestCase(1, 10)]
        [TestCase(10, 10)]
        [TestCase(100, 10)]
        [TestCase(1, 100)]
        [TestCase(10, 100)]
        [TestCase(100, 100)]
        public void RareThingWithGeneratorInceptionOccurs(int chanceDivisor, int subChanceDivisor)
        {
            var chance = 1d / chanceDivisor;

            stopwatch.Start();

            var result = generator.Generate(
                () => BuildRandomWithGenerator(subChanceDivisor),
                c => c <= chance,
                () => 666,
                c => $"{c} is not less than {chance}",
                "FAILED TO GENERATE");

            stopwatch.Stop();

            Assert.That(result, Is.LessThan(chance));

            Assert.That(stopwatch.Elapsed.TotalSeconds, Is.LessThan(1));
        }

        private double BuildRandomWithGenerator(int subChanceDivisor)
        {
            var subchance = 1d / subChanceDivisor;

            var result = generator.Generate(
                () => random.NextDouble(),
                c => c <= subchance,
                () => 666,
                c => $"sub {c} is not less than {subchance}",
                "FAILED TO GENERATE SUB");

            return random.NextDouble();
        }
    }
}
