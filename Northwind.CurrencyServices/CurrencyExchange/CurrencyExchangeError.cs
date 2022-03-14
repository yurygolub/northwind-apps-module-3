using Newtonsoft.Json;

namespace Northwind.CurrencyServices.CurrencyExchange
{
    internal class CurrencyExchangeError
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("info")]
        public string Info { get; set; }
    }
}
