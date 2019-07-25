using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
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
            
            /*var hostUrl = configuration["hosturl"];
            if (string.IsNullOrEmpty(hostUrl))
                hostUrl = "http://0.0.0.0:5000";*/
            
            CreateWebHostBuilder(args, configuration)
                .Build().Run();
            //.UseUrls(hostUrl)

        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args, IConfiguration configuration)
        {

            string allowedHosts = configuration["AllowedHosts"] ?? "127.0.0.1";
            if (allowedHosts == "*") allowedHosts = "0.0.0.0";
            string certificateFile = configuration["certificate:file"] ?? "sess-dev.p12";
            string certificatePassword = configuration["certificate:password"] ?? "sess-dev";


            return WebHost.CreateDefaultBuilder(args)
                .UseKestrel(options =>
                    {
                        options.Listen(IPAddress.Parse(allowedHosts), 5000);
                        options.Listen(IPAddress.Parse(allowedHosts), 5001, listenOptions =>
                        {
                            listenOptions.UseHttps(certificateFile, certificatePassword);
                        } );
                    }
                )
                .UseStartup<Startup>();
        }
    }
}