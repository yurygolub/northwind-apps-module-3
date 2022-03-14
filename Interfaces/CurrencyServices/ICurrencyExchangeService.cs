using System.Threading.Tasks;

namespace Interfaces.CurrencyServices
{
    /// <summary>
    /// Represents currency exchange service.
    /// </summary>
    public interface ICurrencyExchangeService
    {
        /// <summary>
        /// Gets currency exchange rate.
        /// </summary>
        /// <param name="baseCurrency">Source currency.</param>
        /// <param name="exchangeCurrency">Exchange currency.</param>
        /// <returns>Currency exchange rate.</returns>
        Task<decimal> GetCurrencyExchangeRate(string baseCurrency, string exchangeCurrency);
    }
}
