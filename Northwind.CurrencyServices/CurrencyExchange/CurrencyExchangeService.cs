using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace Northwind.CurrencyServices.CurrencyExchange
{
    public class CurrencyExchangeService
    {
        private readonly string accessKey;

        public CurrencyExchangeService(string accesskey)
        {
            if (string.IsNullOrWhiteSpace(accesskey))
            {
                throw new ArgumentException("Access key is invalid.", nameof(accesskey));
            }

            this.accessKey = accesskey;
        }

        public async Task<decimal> GetCurrencyExchangeRate(string baseCurrency, string exchangeCurrency)
        {
            if (baseCurrency is null)
            {
                throw new ArgumentNullException(nameof(baseCurrency));
            }

            if (exchangeCurrency is null)
            {
                throw new ArgumentNullException(nameof(exchangeCurrency));
            }

            Uri uri = new Uri($"http://api.currencylayer.com/live" +
                $"?access_key={this.accessKey}" +
                $"&currencies={exchangeCurrency}" +
                $"&source={baseCurrency}" +
                $"&format=1");

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
                catch (HttpRequestException e)
                {
                    Console.WriteLine(e.Message);
                    throw e;
                }
            }
        }
    }
}
