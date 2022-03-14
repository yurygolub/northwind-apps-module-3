using System.Threading.Tasks;

namespace Interfaces.CurrencyServices
{
    /// <summary>
    /// Represents country currency service.
    /// </summary>
    public interface ICountryCurrencyService
    {
        /// <summary>
        /// Gets local currency by country name.
        /// </summary>
        /// <param name="countryName">Country name.</param>
        /// <returns>A <see cref="LocalCurrency"/> object.</returns>
        Task<LocalCurrency> GetLocalCurrencyByCountry(string countryName);
    }
}
