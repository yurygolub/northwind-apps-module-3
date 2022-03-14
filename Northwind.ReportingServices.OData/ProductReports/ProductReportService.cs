using System;
using System.Collections.Generic;
using System.Data.Services.Client;
using System.Linq;
using System.Threading.Tasks;
using NorthwindModel;

#pragma warning disable CA2008 // Do not create tasks without passing a TaskScheduler

namespace Northwind.ReportingServices.OData.ProductReports
{
    /// <summary>
    /// Represents a service that produces product-related reports.
    /// </summary>
    public class ProductReportService
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

        /// <summary>
        /// Gets a product report with all current products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{T}"/>.</returns>
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

        /// <summary>
        /// Gets a product report with most expensive products.
        /// </summary>
        /// <param name="count">Items count.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
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

        /// <summary>
        /// Gets a product report with price less than value products.
        /// </summary>
        /// <param name="value">Value.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
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

        /// <summary>
        /// Gets a product report with price between first and second products.
        /// </summary>
        /// <param name="first">First value.</param>
        /// <param name="second">Second value.</param>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
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

        /// <summary>
        /// Gets a product report with price above average products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
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

        /// <summary>
        /// Gets a product report with units in stock less than units on order products.
        /// </summary>
        /// <returns>Returns <see cref="ProductReport{ProductPrice}"/>.</returns>
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
