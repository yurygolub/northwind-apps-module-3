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
            this.connection = connection ?? throw new ArgumentNullException(nameof(connection));
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetCurrentProducts()
        {
            var products = await this.GetProductsByCommandAsync("GetAllProducts");

            var query = products
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(query);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetMostExpensiveProductsReport(int count)
        {
            var sqlCommand = new SqlCommand("GetMostExpensiveProducts", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(sqlCommand, count, "@count", SqlDbType.Int, isNullable: false);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            var reader = await sqlCommand.ExecuteReaderAsync();

            var products = new List<Product>();
            while (await reader.ReadAsync())
            {
                products.Add(CreateProduct(reader));
            }

            var productPrices = products
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            sqlCommand.Dispose();

            return new ProductReport<ProductPrice>(productPrices);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetPriceLessThanProductsReport(decimal value)
        {
            var sqlCommand = new SqlCommand("GetPriceLessThanProducts", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(sqlCommand, value, "@value", SqlDbType.Int, isNullable: false);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            var reader = await sqlCommand.ExecuteReaderAsync();

            var products = new List<Product>();
            while (await reader.ReadAsync())
            {
                products.Add(CreateProduct(reader));
            }

            var productPrices = products
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            sqlCommand.Dispose();

            return new ProductReport<ProductPrice>(productPrices);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetPriceBetweenProductsReport(decimal first, decimal second)
        {
            var sqlCommand = new SqlCommand("GetPriceBetweenProducts", this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            SetParameter(sqlCommand, first, "@value1", SqlDbType.Int, isNullable: false);
            SetParameter(sqlCommand, second, "@value2", SqlDbType.Int, isNullable: false);

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

            var reader = await sqlCommand.ExecuteReaderAsync();

            var products = new List<Product>();
            while (await reader.ReadAsync())
            {
                products.Add(CreateProduct(reader));
            }

            var productPrices = products
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            sqlCommand.Dispose();

            return new ProductReport<ProductPrice>(productPrices);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetPriceAboveAverageProductsReport()
        {
            var products = await this.GetProductsByCommandAsync("GetPriceAboveAverageProducts");

            var query = products
                .OrderBy(p => p.ProductName)
                .Select(p => new ProductPrice() { Name = p.ProductName, Price = p.UnitPrice ?? 0 });

            return new ProductReport<ProductPrice>(query);
        }

        /// <inheritdoc/>
        public async Task<ProductReport<ProductPrice>> GetUnitsInStockDeficitProductsReport()
        {
            var products = await this.GetProductsByCommandAsync("GetUnitsInStockDeficitProducts");

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
            _ = countryCurrencyService ?? throw new ArgumentNullException(nameof(countryCurrencyService));
            _ = currencyExchangeService ?? throw new ArgumentNullException(nameof(currencyExchangeService));

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

        private static void SetParameter<T>(SqlCommand command, T property, string parameterName, SqlDbType dbType, int? size = null, bool isNullable = true)
        {
            if (size is null)
            {
                command.Parameters.Add(parameterName, dbType);
            }
            else
            {
                command.Parameters.Add(parameterName, dbType, (int)size);
            }

            command.Parameters[parameterName].IsNullable = isNullable;
            command.Parameters[parameterName].Value = property != null ? property : Convert.DBNull;
        }

        private async Task<IEnumerable<Product>> GetProductsByCommandAsync(string commandText)
        {
            _ = commandText ?? throw new ArgumentNullException(nameof(commandText));

            var sqlCommand = new SqlCommand(commandText, this.connection)
            {
                CommandType = CommandType.StoredProcedure,
            };

            if (this.connection.State != ConnectionState.Open)
            {
                await this.connection.OpenAsync();
            }

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
