using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Interfaces.CurrencyServices;
using Newtonsoft.Json;

#pragma warning disable S4457 // Split method into two, one handling parameters check and the other handling the asynchronous code.

namespace Northwind.CurrencyServices.CountryCurrency
{
    /// <inheritdoc/>
    public class CountryCurrencyService : ICountryCurrencyService
    {
        /// <summary>
        /// Gets local currency by country name.
        /// </summary>
        /// <param name="countryName">Country name.</param>
        /// <returns>A <see cref="LocalCurrency"/> object.</returns>
        /// <exception cref="ArgumentNullException">Thrown if countryName is null.</exception>
        public async Task<LocalCurrency> GetLocalCurrencyByCountry(string countryName)
        {
            if (countryName is null)
            {
                throw new ArgumentNullException(nameof(countryName));
            }

            return await GetLocalCurrencyByCountry();

            async Task<LocalCurrency> GetLocalCurrencyByCountry()
            {
                Uri uri = new Uri($"https://restcountries.com/v2/name/{countryName}?fields=name,currencies");
                using (HttpClient client = new HttpClient())
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

                                return new LocalCurrency()
                                {
                                    CountryName = countryCurrencyInfo.Name,
                                    CurrencyCode = countryCurrencyInfo.Currencies.FirstOrDefault()?.Code,
                                    CurrencySymbol = countryCurrencyInfo.Currencies.FirstOrDefault()?.Symbol,
                                };
                            }
                        }
                    }
                }
            }
        }
    }
}
