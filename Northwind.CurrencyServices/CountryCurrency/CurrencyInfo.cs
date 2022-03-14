using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Northwind.CurrencyServices.CountryCurrency
{
    internal class CurrencyInfo
    {
        [JsonProperty("code")]
        public string Code { get; set; }

        [JsonProperty("symbol")]
        public string Symbol { get; set; }
    }
}
