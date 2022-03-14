using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Extensions.Configuration;

namespace ReportingApp
{
    public class Startup
    {
        public Startup()
        {
            this.ConfigurationRoot = new ConfigurationBuilder()
                .AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), @"..\..\..\appsettings.json"))
                .Build();
        }

        public IConfigurationRoot ConfigurationRoot { get; }
    }
}
