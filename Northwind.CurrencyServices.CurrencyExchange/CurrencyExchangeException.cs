using System;
using System.Runtime.Serialization;

namespace Northwind.CurrencyServices.CurrencyExchange
{
    /// <summary>
    /// Defines currency exchange exception.
    /// </summary>
    [Serializable]
    public class CurrencyExchangeException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyExchangeException"/> class.
        /// </summary>
        public CurrencyExchangeException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyExchangeException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        public CurrencyExchangeException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyExchangeException"/> class.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <param name="innerException">InnerException.</param>
        public CurrencyExchangeException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyExchangeException"/> class.
        /// </summary>
        /// <param name="serializationInfo">SerializationInfo.</param>
        /// <param name="streamingContext">StreamingContext.</param>
        protected CurrencyExchangeException(SerializationInfo serializationInfo, StreamingContext streamingContext)
            : base(serializationInfo, streamingContext)
        {
        }

        /// <summary>
        /// Gets or sets code.
        /// </summary>
        public string Code { get; set; }
    }
}
