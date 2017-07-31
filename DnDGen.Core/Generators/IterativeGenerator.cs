using EventGen;
using System;

namespace DnDGen.Core.Generators
{
    internal class IterativeGenerator : Generator
    {
        public int MaxAttempts { get; set; }

        private readonly GenEventQueue eventQueue;

        public IterativeGenerator(GenEventQueue eventQueue)
        {
            this.eventQueue = eventQueue;
            MaxAttempts = 1000;
        }

        public T Generate<T>(Func<T> buildInstructions, Func<T, bool> isValid, Func<T> buildDefault, Func<T, string> failureDescription, string defaultDescription)
        {
            T builtObject;
            var attempts = 1;

            do
            {
                builtObject = buildInstructions();

                if (!isValid(builtObject))
                {
                    var message = failureDescription(builtObject);
                    eventQueue.Enqueue("Core", message);
                }
            }
            while (!isValid(builtObject) && attempts++ < MaxAttempts);

            if (isValid(builtObject))
                return builtObject;

            builtObject = buildDefault();
            eventQueue.Enqueue("Core", $"Generating {defaultDescription} by default");

            return builtObject;
        }
    }
}
