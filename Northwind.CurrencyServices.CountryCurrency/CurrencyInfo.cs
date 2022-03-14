using Newtonsoft.Json;

namespace Northwind.CurrencyServices.CountryCurrency
{
    /// <summary>
    /// Class for json deserialization.
    /// </summary>
    internal class CurrencyInfo
    {
        /// <summary>
        /// Gets or sets code.
        /// </summary>
        [JsonProperty("code")]
        public string Code { get; set; }

        /// <summary>
        /// Gets or sets symbol.
        /// </summary>
        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}
