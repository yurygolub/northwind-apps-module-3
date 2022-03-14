using System.Threading.Tasks;
using Interfaces.CurrencyServices;

namespace Interfaces.ReportingServices
{
    /// <summary>
    /// Represents a service that produces product-related reports.
    /// </summary>
    public interface IProductReportService
    {
        /// <summary>
        /// Gets a product report with all current products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetCurrentProducts();

        /// <summary>
        /// Gets a product report with most expensive products.
        /// </summary>
        /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count);

        /// <summary>
        /// Gets a product report with price less than value products.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetPriceLessThanProductsReport(decimal value);

        /// <summary>
        /// Gets a product report with price between first and second products.
        /// </summary>
        /// <param name="first">First value.</param>
        /// <param name="second">Second value.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetPriceBetweenProductsReport(decimal first, decimal second);

        /// <summary>
        /// Gets a product report with price above average products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetPriceAboveAverageProductsReport();

        /// <summary>
        /// Gets a product report with units in stock less than units on order products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
        Task<ProductReport<ProductPrice>> GetUnitsInStockDeficitProductsReport();

        /// <summary>
        /// Gets a product report with current products and local prices.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductLocalPrice}"/>.</returns>
        Task<ProductReport<ProductLocalPrice>> GetCurrentProductsWithLocalCurrencyReport(
            ICountryCurrencyService countryCurrencyService,
            ICurrencyExchangeService currencyExchangeService);
    }
}
