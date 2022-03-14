using System.IO;
using Microsoft.Extensions.Configuration;

namespace ReportingApp
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
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\appsettings.json"))
                .Build();
        }

        /// <summary>
        /// Gets configurationRoot.
        /// </summary>
        public IConfigurationRoot ConfigurationRoot { get; }
    }
}
