using System;
using System.Threading.Tasks;
using DependencyResolver;
using Interfaces;
using Microsoft.Extensions.DependencyInjection;

namespace ReportingApp
{
    /// <summary>
    /// Program class.
    /// </summary>
    public static class Program
    {
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

            Startup startup = new Startup();

            var service = startup.ServiceProvider.GetService<CurrentProductLocalPriceReport>();

            await service.PrintReport(args);
        }
    }
}
