using System.Runtime.Serialization;

namespace ApparelShoppingAppAPI.Exceptions
{
    [Serializable]
    internal class ReviewNotFoundException : Exception
    {
        public ReviewNotFoundException()
        {
        }

        public ReviewNotFoundException(string? message) : base(message)
        {
        }

        public ReviewNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected ReviewNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}