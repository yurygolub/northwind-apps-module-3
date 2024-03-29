﻿using System;
using System.Threading.Tasks;
using Interfaces.CurrencyServices;
using Interfaces.ReportingServices;

namespace Interfaces
{
    /// <summary>
    /// Class that is responsible for printing the report.
    /// </summary>
    public class CurrentProductLocalPriceReport
    {
        private const string CurrentProductsReport = "current-products";
        private const string MostExpensiveProductsReport = "most-expensive-products";
        private const string LessThanProductsReport = "price-less-then-products";
        private const string BetweenProductsReport = "price-between-products";
        private const string PriceAboveProductsReport = "price-above-average-products";
        private const string UnitsInStockDeficitProductsReport = "units-in-stock-deficit";
        private const string CurrentProductsLocalReport = "current-products-local-prices";

        private readonly IProductReportService productReportService;
        private readonly ICurrencyExchangeService currencyExchangeService;
        private readonly ICountryCurrencyService countryCurrencyService;

        /// <summary>
        /// Initializes a new instance of the <see cref="CurrentProductLocalPriceReport"/> class.
        /// </summary>
        /// <param name="productReportService">ProductReportService.</param>
        /// <param name="currencyExchangeService">CurrencyExchangeService.</param>
        /// <param name="countryCurrencyService">CountryCurrencyService.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if productReportService or currencyExchangeService or countryCurrencyService is null.
        /// </exception>
        public CurrentProductLocalPriceReport(
            IProductReportService productReportService,
            ICurrencyExchangeService currencyExchangeService,
            ICountryCurrencyService countryCurrencyService)
        {
            this.productReportService = productReportService ?? throw new ArgumentNullException(nameof(productReportService));
            this.currencyExchangeService = currencyExchangeService ?? throw new ArgumentNullException(nameof(currencyExchangeService));
            this.countryCurrencyService = countryCurrencyService ?? throw new ArgumentNullException(nameof(countryCurrencyService));
        }

        /// <summary>
        /// Prints report.
        /// </summary>
        /// <param name="args">Args.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public async Task PrintReport(string[] args)
        {
            if (args == null || args.Length < 1)
            {
                ShowHelp();
                return;
            }

            string reportName = args[0];

            switch (reportName)
            {
                case CurrentProductsReport:
                    await this.ShowCurrentProducts();
                    return;

                case MostExpensiveProductsReport:
                    if (args.Length > 1 && int.TryParse(args[1], out int count))
                    {
                        await this.ShowMostExpensiveProducts(count);
                    }

                    return;

                case LessThanProductsReport:
                    if (args.Length > 1 && decimal.TryParse(args[1], out decimal value))
                    {
                        await this.ShowPriceLessThanProducts(value);
                    }

                    return;

                case BetweenProductsReport:
                    if (args.Length > 2 && decimal.TryParse(args[1], out decimal first) && decimal.TryParse(args[2], out decimal second))
                    {
                        await this.ShowPriceBetweenProducts(first, second);
                    }

                    return;

                case PriceAboveProductsReport:
                    await this.ShowPriceAboveAverageProducts();
                    return;

                case UnitsInStockDeficitProductsReport:
                    await this.ShowUnitsInStockDeficitProducts();
                    return;

                case CurrentProductsLocalReport:
                    await this.ShowCurrentProductsLocalPrices();
                    return;

                default:
                    ShowHelp();
                    return;
            }
        }

        private async Task ShowCurrentProducts()
        {
            var report = await this.productReportService.GetCurrentProducts();
            PrintProductReport("current products:", report);
        }

        private async Task ShowMostExpensiveProducts(int count)
        {
            var report = await this.productReportService.GetMostExpensiveProductsReport(count);
            PrintProductReport($"{count} most expensive products:", report);
        }

        private async Task ShowPriceLessThanProducts(decimal value)
        {
            var report = await this.productReportService.GetPriceLessThanProductsReport(value);
            PrintProductReport($"price less than {value} products:", report);
        }

        private async Task ShowPriceBetweenProducts(decimal first, decimal second)
        {
            var report = await this.productReportService.GetPriceBetweenProductsReport(first, second);
            PrintProductReport($"products with price between {first} and {second}:", report);
        }

        private async Task ShowPriceAboveAverageProducts()
        {
            var report = await this.productReportService.GetPriceAboveAverageProductsReport();
            PrintProductReport("products with price above average:", report);
        }

        private async Task ShowUnitsInStockDeficitProducts()
        {
            var report = await this.productReportService.GetUnitsInStockDeficitProductsReport();
            PrintProductReport($"units in stock deficit products:", report);
        }

        private static void PrintProductReport(string header, ProductReport<ProductPrice> productReport)
        {
            Console.WriteLine($"Report - {header}");
            foreach (var reportLine in productReport.Products)
            {
                Console.WriteLine("{0}, {1}", reportLine.Name, reportLine.Price);
            }
        }

        private async Task ShowCurrentProductsLocalPrices()
        {
            ProductReport<ProductLocalPrice> report = await this.productReportService.GetCurrentProductsWithLocalCurrencyReport(
                this.countryCurrencyService, this.currencyExchangeService);
            PrintProductLocalPriceReport("current products with local price:", report);
        }

        private static void PrintProductLocalPriceReport(string header, ProductReport<ProductLocalPrice> productReport)
        {
            Console.WriteLine($"Report - {header}");
            foreach (var reportLine in productReport.Products)
            {
                Console.WriteLine($"{reportLine.Name}, {reportLine.Price}$, " +
                    $"{reportLine.Country}, {reportLine.LocalPrice}{reportLine.CurrencySymbol}");
            }
        }

        private static void ShowHelp()
        {
            Console.WriteLine("Usage:");
            Console.WriteLine("\tReportingApp.exe <report> <report-argument1> <report-argument2> ...");
            Console.WriteLine();
            Console.WriteLine("Reports:");
            Console.WriteLine($"\t{CurrentProductsReport}\t\tShows current products.");
            Console.WriteLine($"\t{MostExpensiveProductsReport}\t\tShows specified number of the most expensive products.");
            Console.WriteLine($"\t{LessThanProductsReport}\t\tShows products with price less than specified number.");
            Console.WriteLine($"\t{BetweenProductsReport}\t\tShows products with price in specified range.");
            Console.WriteLine($"\t{PriceAboveProductsReport}\t\tShows products with price above average.");
            Console.WriteLine($"\t{UnitsInStockDeficitProductsReport}\t\tShows products with units in stock less than units on order.");
            Console.WriteLine($"\t{CurrentProductsLocalReport}\t\tShows current products and their local prices.");
        }
    }
}
