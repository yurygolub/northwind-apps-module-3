using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;
using Northwind.ReportingServices.OData.ProductReports;

namespace ReportingApp
{
    /// <summary>
    /// Program class.
    /// </summary>
    public sealed class Program
    {
        private const string NorthwindServiceUrl = "https://services.odata.org/V3/Northwind/Northwind.svc";

        /// <summary>
        /// A program entry point.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            if (args is null)
            {
                throw new ArgumentNullException(nameof(args));
            }

            IConfigurationRoot configurationRoot = new Startup().ConfigurationRoot;

            var currentProductLocalPriceReport = new CurrentProductLocalPriceReport(
                new ProductReportService(new Uri(NorthwindServiceUrl)),
                new CurrencyExchangeService(configurationRoot["AccessKey"]),
                new CountryCurrencyService());

            await currentProductLocalPriceReport.PrintReport(args);
        }
    }
}
