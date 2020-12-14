using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Helpo.Common
{
    public static class Guard
    {
        public static void NotNull(object value, string argumentName)
        {
            if (argumentName is null)
                throw new ArgumentNullException(nameof(argumentName));

            if (value is null)
                throw new ArgumentNullException(argumentName);
        }

        public static void NotNullOrWhiteSpace(string value, string argumentName)
        {
            NotNull(value, argumentName);

            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException($"Value is empty or whitespace.", argumentName);
        }

        public static void NotZeroOrNegative(int value, string argumentName)
        {
            if (value <= 0)
                throw new ArgumentException($"Value can't be less or equal to zero, but is {value}.", argumentName);
        }
    }
}
