using System;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Serilog;

namespace BargainsForCouples
{
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args)
        {
            var commonAppData = Environment.ExpandEnvironmentVariables("%AllUsersProfile%");
            var host = Host.CreateDefaultBuilder(args)
                .ConfigureAppConfiguration((hostContext, configApp) =>
                {
                    var env = hostContext.HostingEnvironment;
                    //env.ApplicationName = Assembly.GetEntryAssembly()?.GetName().Name ?? throw new InvalidOperationException("Assembly name is null");
                    //env.EnvironmentName
                    //    = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Production";

                    var appData = Path.Combine(commonAppData, "BargainsForCouples", env.ApplicationName);
                    configApp
                        .SetBasePath(env.ContentRootPath)
                        .AddEnvironmentVariables()
                        .AddJsonFile("config/appsettings.json")
                        .AddJsonFile($"config/appsettings.{env.EnvironmentName}.json", true)
                        .AddJsonFile(Path.Combine(appData, "config/appsettings.json"), true)
                        .AddJsonFile("config/serilog.json")
                        .AddJsonFile($"config/serilog.{env.EnvironmentName}.json", true)
                        .AddJsonFile(Path.Combine(appData, "config/serilog.json"), true)
                        .AddEnvironmentVariables(env.ApplicationName + "_")
                        .AddCommandLine(args);
                }
                ).ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder
                        .UseStartup<Startup>().UseSerilog((hostingContext, loggerConfiguration) =>
                          loggerConfiguration
                            .ReadFrom.Configuration(hostingContext.Configuration)
                            .Enrich.FromLogContext()
                            .Enrich.WithThreadId())
                        .UseIISIntegration()
                        .UseUrls("http://*:5000");
                });
            return host;
        }
    }
}
