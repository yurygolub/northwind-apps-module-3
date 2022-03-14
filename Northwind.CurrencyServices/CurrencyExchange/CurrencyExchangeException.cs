using System;
using System.Runtime.Serialization;

namespace Northwind.CurrencyServices.CurrencyExchange
{
    [Serializable]
    public class CurrencyExchangeException : Exception
    {
        public CurrencyExchangeException()
        {
        }

        public CurrencyExchangeException(string message)
            : base(message)
        {
        }

        public CurrencyExchangeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        protected CurrencyExchangeException(SerializationInfo serializationInfo, StreamingContext streamingContext)
        {
        }

        public string Code { get; set; }
    }
}
