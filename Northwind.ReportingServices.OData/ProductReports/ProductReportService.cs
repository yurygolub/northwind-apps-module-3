using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CurrencyServices;
using Interfaces.ReportingServices;
using NorthwindModel;

#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler

namespace Northwind.ReportingServices.OData.ProductReports
{
    /// <inheritdoc/>
    public class ProductReportService : IProductReportService
    {
        private readonly NorthwindEntities entities;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReportService"/> class.
        /// </summary>
        /// <param name="northwindServiceUri">An URL to Northwind OData service.</param>
        public ProductReportService(Uri northwindServiceUri)
        {
            if (northwindServiceUri is null)
            {
                throw new ArgumentNullException(nameof(northwindServiceUri));
            }

            this.entities = new NorthwindEntities(northwindServiceUri);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetCurrentProducts()
        {
            var query = (DataServiceQuery<ProductPrice>)(
                from p in this.entities.Products
                where !p.Discontinued
                orderby p.ProductName
                select new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>
                .Factory
                .FromAsync(query.BeginExecute(null, query), ar => query.EndExecute(ar))
                .ContinueWith(x => this.ContinuePage(x.Result));

            return new ProductReport<ProductPrice>(result);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count)
        {
            var query = (DataServiceQuery<ProductPrice>)this.entities.Products.
                Where(p => p.UnitPrice != null).
                OrderByDescending(p => p.UnitPrice.Value).
                Take(count)
                .Select(p =>
                    new ProductPrice
                    {
                        Name = p.ProductName,
                        Price = p.UnitPrice ?? 0,
                    });

            var result = await Task<IEnumerable<ProductPrice>>.Factory.FromAsync(query.BeginExecute(null, null), (ar) =>
            {
                return query.EndExecute(ar);
            });

            List<ProductPrice> productPrices = new List<ProductPrice>();
            foreach (var product in result)
            {
                productPrices.Add(product);
            }

            return new ProductReport<ProductPrice>(productPrices);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetPriceLessThanProductsReport(decimal value)
        {
            var query = (DataServiceQuery<ProductPrice>)(
                from p in this.entities.Products
                where p.UnitPrice < value
                orderby p.ProductName
                select new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>
                .Factory
                .FromAsync(query.BeginExecute(null, query), ar => query.EndExecute(ar))
                .ContinueWith(x => this.ContinuePage(x.Result));

            return new ProductReport<ProductPrice>(result);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetPriceBetweenProductsReport(decimal first, decimal second)
        {
            var query = (DataServiceQuery<ProductPrice>)(
                from p in this.entities.Products
                where p.UnitPrice > first && p.UnitPrice < second
                orderby p.ProductName
                select new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>
                .Factory
                .FromAsync(query.BeginExecute(null, query), ar => query.EndExecute(ar))
                .ContinueWith(x => this.ContinuePage(x.Result));

            return new ProductReport<ProductPrice>(result);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetPriceAboveAverageProductsReport()
        {
            decimal average = this.entities.Products.Average(product => product.UnitPrice ?? 0);
            var query = (DataServiceQuery<ProductPrice>)(
                from p in this.entities.Products
                where p.UnitPrice > average
                orderby p.ProductName
                select new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>
                .Factory
                .FromAsync(query.BeginExecute(null, query), ar => query.EndExecute(ar))
                .ContinueWith(x => this.ContinuePage(x.Result));

            return new ProductReport<ProductPrice>(result);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetUnitsInStockDeficitProductsReport()
        {
            var query = (DataServiceQuery<ProductPrice>)(
                from p in this.entities.Products
                where p.UnitsInStock < p.UnitsOnOrder
                orderby p.ProductName
                select new ProductPrice
                {
                    Name = p.ProductName,
                    Price = p.UnitPrice ?? 0,
                });

            var result = await Task<IEnumerable<ProductPrice>>
                .Factory
                .FromAsync(query.BeginExecute(null, query), ar => query.EndExecute(ar))
                .ContinueWith(x => this.ContinuePage(x.Result));

            return new ProductReport<ProductPrice>(result);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductLocalPrice>> GetCurrentProductsWithLocalCurrencyReport(
            ICountryCurrencyService countryCurrencyService,
            ICurrencyExchangeService currencyExchangeService)
        {
            if (countryCurrencyService is null)
            {
                throw new ArgumentNullException(nameof(countryCurrencyService));
            }

            if (currencyExchangeService is null)
            {
                throw new ArgumentNullException(nameof(currencyExchangeService));
            }

            var productReportTask = this.GetCurrentProducts();

            LocalCurrency localCurrency = await countryCurrencyService.GetLocalCurrencyByCountry(RegionInfo.CurrentRegion.EnglishName);

            decimal rate = await currencyExchangeService.GetCurrencyExchangeRate("USD", localCurrency.CurrencyCode);

            List<ProductLocalPrice> productLocalPrices = new List<ProductLocalPrice>();
            foreach (var product in productReportTask.Result.Products)
            {
                productLocalPrices.Add(new ProductLocalPrice()
                {
                    Country = localCurrency.CountryName,
                    CurrencySymbol = localCurrency.CurrencySymbol,
                    LocalPrice = product.Price * rate,
                    Name = product.Name,
                    Price = product.Price,
                });
            }

            return new ProductReport<ProductLocalPrice>(productLocalPrices);
        }

        private IEnumerable<ProductPrice> ContinuePage(IEnumerable<ProductPrice> response)
        {
            foreach (var element in response)
            {
                yield return element;
            }

            if ((response as QueryOperationResponse)?.GetContinuation() is DataServiceQueryContinuation<ProductPrice> continuation)
            {
                var innerTask = Task<IEnumerable<ProductPrice>>
                    .Factory
                    .FromAsync(this.entities.BeginExecute(continuation, null, null), this.entities.EndExecute<ProductPrice>)
                    .ContinueWith(t => this.ContinuePage(t.Result));

                foreach (var productPrice in innerTask.Result)
                {
                    yield return productPrice;
                }
            }
        }
    }
}
