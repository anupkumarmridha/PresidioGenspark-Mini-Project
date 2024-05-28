using System.Runtime.Serialization;

namespace ApparelShoppingAppAPI.Exceptions
{
    [Serializable]
    public class SellerNotFoundException : Exception
    {
        public SellerNotFoundException()
        {
        }

        public SellerNotFoundException(string? message) : base(message)
        {
        }

        public SellerNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected SellerNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}