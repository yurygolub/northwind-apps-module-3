using System.Collections.Generic;
using Newtonsoft.Json;

namespace Northwind.CurrencyServices.CurrencyExchange
{
    internal class CurrencyExchangeInfo
    {
        [JsonProperty("success")]
        public bool Success { get; set; }

        [JsonProperty("error")]
        public CurrencyExchangeError Error { get; set; }

        [JsonProperty("quotes")]
        public Dictionary<string, decimal> Quotes { get; set; }
    }
}
