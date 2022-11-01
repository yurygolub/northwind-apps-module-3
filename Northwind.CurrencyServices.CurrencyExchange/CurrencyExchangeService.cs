using System;
using System.IO;
using System.Net.Http;
using System.Threading.Tasks;
using Interfaces.CurrencyServices;
using Newtonsoft.Json;

#pragma warning disable S4457 // Split method into two, one handling parameters check and the other handling the asynchronous code.

namespace Northwind.CurrencyServices.CurrencyExchange
{
    /// <summary>
    /// Represents currency exchange service.
    /// </summary>
    public class CurrencyExchangeService : ICurrencyExchangeService
    {
        private readonly string accessKey;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrencyExchangeService"/> class.
        /// </summary>
        /// <param name="accessKey">Access key for authentication in currencylayer API.</param>
        /// <exception cref="ArgumentException">Thrown if accessKey is is null or white space.</exception>
        public CurrencyExchangeService(string accessKey)
        {
            if (string.IsNullOrWhiteSpace(accessKey))
            {
                throw new ArgumentException("Access key is invalid.", nameof(accessKey));
            }

            this.accessKey = accessKey;
        }

        /// <summary>
        /// Gets currency exchange rate.
        /// </summary>
        /// <param name="baseCurrency">Source currency.</param>
        /// <param name="exchangeCurrency">Exchange currency.</param>
        /// <returns>Currency exchange rate.</returns>
        /// <exception cref="ArgumentNullException">Thrown if baseCurrency or exchangeCurrency is null.</exception>
        /// <exception cref="CurrencyExchangeException">Thrown if currencylayer don't give exchange rate.</exception>
        public async Task<decimal> GetCurrencyExchangeRate(string baseCurrency, string exchangeCurrency)
        {
            _ = baseCurrency ?? throw new ArgumentNullException(nameof(baseCurrency));
            _ = exchangeCurrency ?? throw new ArgumentNullException(nameof(exchangeCurrency));

            return await GetCurrencyExchangeRate();

            async Task<decimal> GetCurrencyExchangeRate()
            {
                using (HttpClient client = new HttpClient())
                {
                    Uri uri = new Uri($"http://api.currencylayer.com/live" +
                        $"?access_key={this.accessKey}" +
                        $"&currencies={exchangeCurrency}" +
                        $"&source={baseCurrency}" +
                        $"&format=1");

                    using (Stream response = await client.GetStreamAsync(uri))
                    {
                        using (TextReader textReader = new StreamReader(response))
                        {
                            using (JsonReader reader = new JsonTextReader(textReader))
                            {
                                var currencyExchangeInfo = new JsonSerializer()
                                    .Deserialize<CurrencyExchangeInfo>(reader);

                                if (currencyExchangeInfo.Success)
                                {
                                    return currencyExchangeInfo.Quotes[$"{baseCurrency}{exchangeCurrency}"];
                                }
                                else
                                {
                                    throw new CurrencyExchangeException(currencyExchangeInfo.Error.Info)
                                    {
                                        Code = currencyExchangeInfo.Error.Code,
                                    };
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
