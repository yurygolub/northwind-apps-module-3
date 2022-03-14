﻿using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Northwind.ReportingServices.OData.ProductReports;

namespace ReportingApp
{
    /// <summary>
    /// Program class.
    /// </summary>
    public sealed class Program
    {
        private const string NorthwindServiceUrl = "https://services.odata.org/V3/Northwind/Northwind.svc";
        private const string CurrentProductsReport = "current-products";
        private const string MostExpensiveProductsReport = "most-expensive-products";
        private const string LessThanProductsReport = "price-less-then-products";
        private const string BetweenProductsReport = "price-between-products";
        private const string PriceAboveProductsReport = "price-above-average-products";
        private const string UnitsInStockDeficitProductsReport = "units-in-stock-deficit";
        private const string CurrentProductsLocalReport = "current-products-local-prices";

        private static IConfigurationRoot configurationRoot;

        /// <summary>
        /// A program entry point.
        /// </summary>
        /// <param name="args">Program arguments.</param>
        /// <returns>A <see cref="Task"/> representing the asynchronous operation.</returns>
        public static async Task Main(string[] args)
        {
            configurationRoot = new Startup().ConfigurationRoot;

            if (args == null || args.Length < 1)
            {
                ShowHelp();
                return;
            }

            string reportName = args[0];

            switch (reportName)
            {
                case CurrentProductsReport:
                    await ShowCurrentProducts();
                    return;

                case MostExpensiveProductsReport:
                    if (args.Length > 1 && int.TryParse(args[1], out int count))
                    {
                        await ShowMostExpensiveProducts(count);
                    }

                    return;

                case LessThanProductsReport:
                    if (args.Length > 1 && decimal.TryParse(args[1], out decimal value))
                    {
                        await ShowPriceLessThanProducts(value);
                    }

                    return;

                case BetweenProductsReport:
                    if (args.Length > 2 && decimal.TryParse(args[1], out decimal first) && decimal.TryParse(args[2], out decimal second))
                    {
                        await ShowPriceBetweenProducts(first, second);
                    }

                    return;

                case PriceAboveProductsReport:
                    await ShowPriceAboveAverageProducts();
                    return;

                case UnitsInStockDeficitProductsReport:
                    await ShowUnitsInStockDeficitProducts();
                    return;

                case CurrentProductsLocalReport:
                    await ShowCurrentProductsLocalPrices();
                    return;

                default:
                    ShowHelp();
                    return;
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
        }

        private static async Task ShowCurrentProducts()
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl), configurationRoot["AccessKey"]);
            var report = await service.GetCurrentProducts();
            PrintProductReport("current products:", report);
        }

        private static async Task ShowMostExpensiveProducts(int count)
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl), configurationRoot["AccessKey"]);
            var report = await service.GetMostExpensiveProductsReport(count);
            PrintProductReport($"{count} most expensive products:", report);
        }

        private static void PrintProductReport(string header, ProductReport<ProductPrice> productReport)
        {
            Console.WriteLine($"Report - {header}");
            foreach (var reportLine in productReport.Products)
            {
                Console.WriteLine("{0}, {1}", reportLine.Name, reportLine.Price);
            }
        }

        private static async Task ShowPriceLessThanProducts(decimal value)
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl), configurationRoot["AccessKey"]);
            var report = await service.GetPriceLessThanProductsReport(value);
            PrintProductReport($"price less than {value} products:", report);
        }

        private static async Task ShowPriceBetweenProducts(decimal first, decimal second)
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl), configurationRoot["AccessKey"]);
            var report = await service.GetPriceBetweenProductsReport(first, second);
            PrintProductReport($"products with price between {first} and {second}:", report);
        }

        private static async Task ShowPriceAboveAverageProducts()
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl), configurationRoot["AccessKey"]);
            var report = await service.GetPriceAboveAverageProductsReport();
            PrintProductReport("products with price above average:", report);
        }

        private static async Task ShowUnitsInStockDeficitProducts()
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl), configurationRoot["AccessKey"]);
            var report = await service.GetUnitsInStockDeficitProductsReport();
            PrintProductReport($"units in stock deficit products:", report);
        }

        private static async Task ShowCurrentProductsLocalPrices()
        {
            var service = new ProductReportService(new Uri(NorthwindServiceUrl), configurationRoot["AccessKey"]);
            ProductReport<ProductLocalPrice> report = await service.GetCurrentProductsWithLocalCurrencyReport();
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
    }
}
