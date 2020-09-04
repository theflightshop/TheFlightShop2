using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NLog.Targets;
using NLog.Web;
using TheFlightShop.Logging;

namespace TheFlightShop
{
    public class Program
    {
        public static void Main(string[] args)
        {
            Target.Register<AmazonS3LogTarget>("AmazonS3");
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()//.UseUrls("http://localhost:5002/"); // for local debug
                .ConfigureLogging(logging => logging.ClearProviders())
                .UseNLog();
    }
}
