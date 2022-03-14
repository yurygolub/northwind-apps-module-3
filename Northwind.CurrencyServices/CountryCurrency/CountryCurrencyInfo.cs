using System;
using System.Collections.Generic;
using System.Text;
using Newtonsoft.Json;

namespace Northwind.CurrencyServices.CountryCurrency
{
    internal class CountryCurrencyInfo
    {
        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("currencies")]
        public IEnumerable<CurrencyInfo> Currencies { get; set; }
    }
}
