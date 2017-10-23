using System;
using System.Collections.Generic;
using System.Linq;

namespace BeFaster.Runner.Utils
{
    public struct Optional<T> where T : class
    {
        private readonly IEnumerable<T> values;

        public static Optional<T> Some(T value)
        {
            if (value == null)
            {
                throw new InvalidOperationException();
            }

            return new Optional<T>(new[] {value});
        }

        public static Optional<T> OfNullable(T value) => value == null ? None : Some(value);

        public static Optional<T> None => new Optional<T>(new T[0]);

        public bool HasValue => values != null && values.Any();

        public T OrElse(T other) => HasValue ? Value : other;

        public T OrElseThrow(Func<Exception> exceptionSupplier) => HasValue ? Value : throw exceptionSupplier();

        public T Value
        {
            get
            {
                if (!HasValue)
                {
                    throw new InvalidOperationException("Optional does not have a value");
                }

                return values.Single();
            }
        }
        
        private Optional(IEnumerable<T> values)
        {
            this.values = values;
        }
    }
}
