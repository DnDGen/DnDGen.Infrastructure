using System;

namespace DnDGen.Infrastructure.Generators
{
    [Obsolete("This is inefficient and should not be used")]
    internal class IterativeGenerator : Generator
    {
        public int MaxAttempts { get; set; }

        public IterativeGenerator()
        {
            MaxAttempts = 1000;
        }

        public T Generate<T>(Func<T> buildInstructions, Func<T, bool> isValid, Func<T> buildDefault, Func<T, string> failureDescription, string defaultDescription)
        {
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
                }
            }
            while (!objectIsValid && attempts++ < MaxAttempts);

            if (objectIsValid)
            {
                return builtObject;
            }

            builtObject = buildDefault();

            return builtObject;
        }
    }
}
