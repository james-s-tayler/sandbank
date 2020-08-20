using System;
using System.Reflection;
using System.Threading.Tasks;
using Amazon;
using Amazon.CloudWatchLogs;
using Database;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Serilog;
using Serilog.Events;
using Serilog.Exceptions;
using Serilog.Formatting.Compact;
using Serilog.Sinks.AwsCloudWatch;
using Services.Domain.Accounts;

namespace Api
{
    public class Program
    {
        public static async Task Main(string[] args)
        {

            var options = new CloudWatchSinkOptions
            {
                // the name of the CloudWatch Log group for logging
                LogGroupName = "SandBankAPI",

                // the main formatter of the log event
                TextFormatter = new RenderedCompactJsonFormatter(),
  
                // other defaults defaults
                MinimumLogEventLevel = LogEventLevel.Debug,
                BatchSizeLimit = 10, //flush almost immediately (don't use in production)
                QueueSizeLimit = 100,
                Period = TimeSpan.FromSeconds(10),
                CreateLogGroup = true,
                LogStreamNameProvider = new DefaultLogStreamProvider(),
                RetryAttempts = 5
            };
            
            var assembly = Assembly.GetExecutingAssembly().GetName();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("AssemblyName", $"{assembly.Name}")
                .Enrich.WithProperty("AssemblyVersion", $"{assembly.Version}")
                .Enrich.WithExceptionDetails() // enriches exceptions by adding ex.Data properties (just add like ex.Data.Add(key, value)) 
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .WriteTo.AmazonCloudWatch(options, new AmazonCloudWatchLogsClient(RegionEndpoint.USEast1))
                //.WriteTo.File(new RenderedCompactJsonFormatter(), "/logs/log.json")
                .CreateLogger();
            
            var host = CreateWebHostBuilder(args).Build();

            using (var serviceScope = host.Services.GetService<IServiceScopeFactory>().CreateScope())
            using (var context = serviceScope.ServiceProvider.GetService<SandBankDbContext>())
            {
                context.Database.Migrate();
                var seedDataService = serviceScope.ServiceProvider.GetService<ISeedTransactionDataService>();
                await seedDataService.SeedData();
            }

            host.Run();
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
        }

    }
}
