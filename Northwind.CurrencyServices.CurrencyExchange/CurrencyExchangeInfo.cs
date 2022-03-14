using System.Collections.Generic;
using Newtonsoft.Json;

namespace Northwind.CurrencyServices.CurrencyExchange
{
    /// <summary>
    /// Class for json deserialization.
    /// </summary>
    internal class CurrencyExchangeInfo
    {
        /// <summary>
        /// Gets or sets a value indicating whether success.
        /// </summary>
        [JsonProperty("success")]
        public bool Success { get; set; }

        /// <summary>
        /// Gets or sets error.
        /// </summary>
        [JsonProperty("error")]
        public CurrencyExchangeError Error { get; set; }

        /// <summary>
        /// Gets or sets quotes.
        /// </summary>
        [JsonProperty("quotes")]
        public Dictionary<string, decimal> Quotes { get; set; }
    }
}
