﻿using System.Runtime.Serialization;

namespace ApparelShoppingAppAPI.Exceptions
{
    [Serializable]
    public class CategoryNotFoundException : Exception
    {
        public CategoryNotFoundException()
        {
        }

        public CategoryNotFoundException(string? message) : base(message)
        {
        }

        public CategoryNotFoundException(string? message, Exception? innerException) : base(message, innerException)
        {
        }

        protected CategoryNotFoundException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}