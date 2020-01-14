using DnDGen.EventGen;
using System;

namespace DnDGen.Infrastructure.Generators
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
            eventQueue.Enqueue("Infrastructure", $"Beginning iterative generation");

            T builtObject;
            var attempts = 1;
            var objectIsValid = false;

            do
            {
                builtObject = buildInstructions();
                objectIsValid = isValid(builtObject);

                if (!objectIsValid)
                {
                    var message = failureDescription(builtObject);
                    eventQueue.Enqueue("Infrastructure", $"(Attempt {attempts}) {message}");
                }
            }
            while (!objectIsValid && attempts++ < MaxAttempts);

            if (objectIsValid)
            {
                eventQueue.Enqueue("Infrastructure", $"Completed iterative generation after {attempts} iterations");
                return builtObject;
            }

            eventQueue.Enqueue("Infrastructure", $"Generating {defaultDescription} by default");
            builtObject = buildDefault();

            return builtObject;
        }
    }
}
