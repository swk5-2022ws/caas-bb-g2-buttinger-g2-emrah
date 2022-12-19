
using System.Data;

namespace CaaS.Util
{
    public class ExceptionUtil
    {
        public static ArgumentNullException ParameterNullException(string parameterName) =>
            new ArgumentNullException($"Parameter {parameterName} is null");

        public static ArgumentNullException NoSuchProductException() => new ArgumentNullException("No such product");

        public static ArgumentOutOfRangeException NoSuchIdException(string? name = null) => new ArgumentOutOfRangeException($"No such id {(!string.IsNullOrEmpty(name) ? $"for {name}" : "")}");
        public static ArgumentException ReferenceException(string? name = null) => new ArgumentException($"A reference already exists for {(!string.IsNullOrEmpty(name) ? $"for {name}" : "")}");
    }
}
