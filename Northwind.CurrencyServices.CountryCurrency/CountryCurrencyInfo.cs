using System.Collections.Generic;
using Newtonsoft.Json;

#pragma warning disable CA1812

namespace Northwind.CurrencyServices.CountryCurrency
{
    /// <summary>
    /// Class for json deserialization.
    /// </summary>
    internal class CountryCurrencyInfo
    {
        /// <summary>
        /// Gets or sets name.
        /// </summary>
        [JsonProperty("name")]
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets currencies.
        /// </summary>
        [JsonProperty("currencies")]
        public IEnumerable<CurrencyInfo> Currencies { get; set; }
    }
}
