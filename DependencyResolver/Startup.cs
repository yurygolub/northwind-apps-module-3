using System;
using System.IO;
using Interfaces;
using Interfaces.CurrencyServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Northwind.CurrencyServices.CountryCurrency;
using Northwind.CurrencyServices.CurrencyExchange;

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
            this.Configuration = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"))
                .Build();

            ServiceCollection services = new ServiceCollection();
            this.ConfigureServices(services);
            this.ServiceProvider = services.BuildServiceProvider();
        }

        /// <summary>
        /// Gets configurationRoot.
        /// </summary>
        public IConfiguration Configuration { get; }

        /// <summary>
        /// Gets service provider.
        /// </summary>
        public IServiceProvider ServiceProvider { get; }

        /// <summary>
        /// Configures services.
        /// </summary>
        /// <param name="services">Services.</param>
        public void ConfigureServices(IServiceCollection services)
        {
            switch (this.Configuration["Mode"])
            {
                case "Sql":
                    services.AddSqlServices(this.Configuration);
                    break;

                case "OData":
                default:
                    services.AddODataServices(this.Configuration);
                    break;
            }

            services
                .AddTransient<ICurrencyExchangeService, CurrencyExchangeService>(s =>
                    new CurrencyExchangeService(this.Configuration["AccessKey"]))
                .AddTransient<ICountryCurrencyService, CountryCurrencyService>()
                .AddSingleton<CurrentProductLocalPriceReport>();
        }
    }
}
