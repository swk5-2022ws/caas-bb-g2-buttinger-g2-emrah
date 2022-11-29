
namespace CaaS.Util
{
    public class ExceptionUtil
    {
        public static ArgumentNullException ParameterNullException(string parameterName) =>
            new ArgumentNullException($"Parameter {parameterName} is null");

        public static ArgumentNullException NoSuchProductException() => new ArgumentNullException("No such product");

        public static ArgumentOutOfRangeException NoSuchIdException() => new ArgumentOutOfRangeException("No such id");
    }
}
