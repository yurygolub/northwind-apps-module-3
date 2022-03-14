using Newtonsoft.Json;

#pragma warning disable CA1812

namespace Northwind.CurrencyServices.CurrencyExchange
{
    /// <summary>
    /// Class for json deserialization.
    /// </summary>
    internal class CurrencyExchangeError
    {
        /// <summary>
        /// Gets or sets code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets info.
        /// </summary>
        [JsonProperty("info")]
        public string Info { get; set; }
    }
}
