using System;
using Interfaces.ReportingServices;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NorthwindModel;
using ODataReportService = Northwind.ReportingServices.OData.ProductReports;
using SqlReportService = Northwind.ReportingServices.SqlService.ProductReports;

#pragma warning disable SA1600

namespace DependencyResolver
{
    public static class ServiceCollectionExtension
    {
        public static IServiceCollection AddODataServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddTransient<IProductReportService, ODataReportService.ProductReportService>(s =>
                    new ODataReportService.ProductReportService(
                        new NorthwindEntities(
                            new Uri(configuration["NorthwindServiceUrl"]))));
        }

        public static IServiceCollection AddSqlServices(this IServiceCollection services, IConfiguration configuration)
        {
            return services
                .AddTransient<IProductReportService, SqlReportService.ProductReportService>(s =>
                    new SqlReportService.ProductReportService(
                        new SqlConnection(configuration.GetConnectionString("SqlConnection"))));
        }
    }
}
