using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace sess_api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            
            var configuration = new ConfigurationBuilder()
                .AddCommandLine(args)
                .Build();
            
            var hostUrl = configuration["hosturl"];
            if (string.IsNullOrEmpty(hostUrl))
                hostUrl = "http://0.0.0.0:5000";
            
            CreateWebHostBuilder(args)
                .UseUrls(hostUrl)
                .Build().Run();

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}