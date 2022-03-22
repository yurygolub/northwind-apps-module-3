using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Interfaces.CurrencyServices;
using Interfaces.ReportingServices;
using Microsoft.Data.SqlClient;

#pragma warning disable S4457 // Split method into two, one handling parameters check and the other handling the asynchronous code.
#pragma warning disable CA2100 // Review SQL queries for security vulnerabilities

namespace Northwind.ReportingServices.SqlService.ProductReports
{
    /// <inheritdoc/>
    public class ProductReportService : IProductReportService
    {
        private readonly SqlConnection connection;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProductReportService"/> class.
        /// </summary>
        /// <param name="connection">Sql connection object.</param>
        public ProductReportService(SqlConnection connection)
        {
            if (connection is null)
            {
                throw new ArgumentNullException(nameof(connection));
            }

            this.connection = connection;
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetCurrentProducts()
        {
            var products = await this.GetProductsByCommand("GetAllProducts");

            var query = products
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(query);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count)
        {
            var products = await this.GetProductsByCommand("GetMostExpensiveProducts");

            var query = products
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(query);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetPriceLessThanProductsReport(decimal value)
        {
            var products = await this.GetProductsByCommand("GetPriceLessThanProducts");

            var query = products
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(query);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetPriceBetweenProductsReport(decimal first, decimal second)
        {
            var products = await this.GetProductsByCommand("GetPriceBetweenProducts");

            var query = products
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(query);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetPriceAboveAverageProductsReport()
        {
            var products = await this.GetProductsByCommand("GetPriceAboveAverageProducts");

            var query = products
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(query);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetUnitsInStockDeficitProductsReport()
        {
            var products = await this.GetProductsByCommand("GetUnitsInStockDeficitProducts");

            var query = products
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(query);
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

            return await GetCurrentProductsWithLocalCurrencyReport();

            async Task<ProductReport<ProductLocalPrice>> GetCurrentProductsWithLocalCurrencyReport()
            {
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
        }

        private static Product CreateProduct(SqlDataReader reader)
        {
            return new Product
            {
                ProductID = (int)reader["ProductID"],
                ProductName = (string)reader["ProductName"],
                SupplierID = GetValueStruct<int>("SupplierID"),
                CategoryID = GetValueStruct<int>("CategoryID"),
                QuantityPerUnit = GetValueClass<string>("QuantityPerUnit"),
                UnitPrice = GetValueStruct<decimal>("UnitPrice"),
                UnitsInStock = GetValueStruct<short>("UnitsInStock"),
                UnitsOnOrder = GetValueStruct<short>("UnitsOnOrder"),
                ReorderLevel = GetValueStruct<short>("ReorderLevel"),
                Discontinued = (bool)reader["Discontinued"],
            };

            T GetValueClass<T>(string text)
                where T : class
                => reader[text] == DBNull.Value ? null : (T)reader[text];

            T? GetValueStruct<T>(string text)
                where T : struct
                => reader[text] == DBNull.Value ? null : (T?)reader[text];
        }

        private async Task<IEnumerable<Product>> GetProductsByCommand(string commandText)
        {
            if (commandText is null)
            {
                throw new ArgumentNullException(nameof(commandText));
            }

            var sqlCommand = new SqlCommand(commandText, this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            await this.connection.OpenAsync();

            var reader = await sqlCommand.ExecuteReaderAsync();

            var products = new List<Product>();
            while (await reader.ReadAsync())
            {
                Product product = CreateProduct(reader);
                products.Add(product);
            }

            sqlCommand.Dispose();

            return products;
        }
    }
}
