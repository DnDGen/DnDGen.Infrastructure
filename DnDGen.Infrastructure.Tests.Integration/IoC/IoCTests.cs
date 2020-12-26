using NUnit.Framework;
using System.Diagnostics;

namespace DnDGen.Infrastructure.Tests.Integration.IoC
{
    [TestFixture]
    public abstract class IoCTests : IntegrationTests
    {
        protected Stopwatch stopwatch;

        private const int TimeLimitInMilliseconds = 300;

        [SetUp]
        public void Setup()
        {
            stopwatch = new Stopwatch();
        }

        protected void AssertSingleton<T>()
        {
            var first = InjectAndAssertDuration<T>();
            var second = InjectAndAssertDuration<T>();
            Assert.That(first, Is.EqualTo(second));
        }

        private T InjectAndAssertDuration<T>()
        {
            stopwatch.Restart();

            var instance = GetNewInstanceOf<T>();
            Assert.That(stopwatch.Elapsed.TotalMilliseconds, Is.LessThan(TimeLimitInMilliseconds));

            return instance;
        }

        protected void AssertNotSingleton<T>()
        {
            var first = InjectAndAssertDuration<T>();
            var second = InjectAndAssertDuration<T>();
            Assert.That(first, Is.Not.EqualTo(second));
        }

        protected void AssertIsInstanceOf<I, T>()
            where T : I
        {
            var item = InjectAndAssertDuration<I>();
            Assert.That(item, Is.InstanceOf<T>());
        }
    }
}
