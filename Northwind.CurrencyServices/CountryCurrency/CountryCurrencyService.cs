using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Northwind.CurrencyServices.CountryCurrency
{
    public class CountryCurrencyService
    {
        public async Task<LocalCurrency> GetLocalCurrencyByCountry(string countryName)
        {
            if (countryName is null)
            {
                throw new ArgumentNullException(nameof(countryName));
            }

            LocalCurrency localCurrency = null;
            Uri uri = new Uri($"https://restcountries.com/v2/name/{countryName}?fields=name,currencies");
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    using (Stream response = await client.GetStreamAsync(uri))
                    {
                        using (TextReader textReader = new StreamReader(response))
                        {
                            using (JsonReader reader = new JsonTextReader(textReader))
                            {
                                var countryCurrencyInfo = new JsonSerializer()
                                    .Deserialize<IEnumerable<CountryCurrencyInfo>>(reader)
                                    .FirstOrDefault();

                                localCurrency = new LocalCurrency()
                                {
                                    CountryName = countryCurrencyInfo.Name,
                                    CurrencyCode = countryCurrencyInfo.Currencies.FirstOrDefault()?.Code,
                                    CurrencySymbol = countryCurrencyInfo.Currencies.FirstOrDefault()?.Symbol,
                                };
                            }
                        }
                    }
                }
                catch (HttpRequestException e)
                {
                    Console.WriteLine(e.Message);
                    throw e;
                }
            }

            return localCurrency;
        }
    }
}
