using System;
using System.IO;
using Interfaces;
using Interfaces.CurrencyServices;
using Interfaces.ReportingServices;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;
using NorthwindModel;
using ODataReportService = Northwind.ReportingServices.OData.ProductReports;
using SqlReportService = Northwind.ReportingServices.SqlService.ProductReports;

namespace DependencyResolver
{
    /// <summary>
    /// Startup class.
    /// </summary>
    public class Startup
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        public Startup()
        {
            this.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"))
                .Build();
        }

        /// <summary>
        /// Gets configurationRoot.
        /// </summary>
        public IConfigurationRoot ConfigurationRoot { get; }

        /// <summary>
        /// Gets service provider.
        /// </summary>
        public IServiceProvider ServiceProvider { get; private set; }

        /// <summary>
        /// Creates service provider.
        /// </summary>
        public void CreateServiceProvider()
        {
            this.ServiceProvider = new ServiceCollection()
                .AddTransient<IProductReportService, ODataReportService.ProductReportService>(s =>
                    new ODataReportService.ProductReportService(
                        new NorthwindEntities(
                            new Uri(this.ConfigurationRoot["NorthwindServiceUrl"]))))
                .AddTransient<IProductReportService, SqlReportService.ProductReportService>(s =>
                    new SqlReportService.ProductReportService(
                        new SqlConnection(this.ConfigurationRoot.GetConnectionString("SqlConnection"))))
                .AddTransient<ICurrencyExchangeService, CurrencyExchangeService>(s =>
                    new CurrencyExchangeService(this.ConfigurationRoot["AccessKey"]))
                .AddTransient<ICountryCurrencyService, CountryCurrencyService>()
                .AddSingleton<CurrentProductLocalPriceReport>()
                .BuildServiceProvider();
        }
    }
}
