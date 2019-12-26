using System;
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
using Serilog.Formatting.Compact;
using Serilog.Sinks.AwsCloudWatch;
using Services.Domain.Accounts;

namespace Endpoints
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
                MinimumLogEventLevel = LogEventLevel.Information,
                BatchSizeLimit = 10, //flush almost immediately (don't use in production)
                QueueSizeLimit = 100,
                Period = TimeSpan.FromSeconds(10),
                CreateLogGroup = true,
                LogStreamNameProvider = new DefaultLogStreamProvider(),
                RetryAttempts = 5
            };
            
            Log.Logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .WriteTo.AmazonCloudWatch(options, new AmazonCloudWatchLogsClient(RegionEndpoint.USEast1))
                //.WriteTo.File(new RenderedCompactJsonFormatter(), "/logs/log.json")
                .CreateLogger();

            try
            {
                Log.Information("Application Starting Up...");
                
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
            catch (Exception e)
            {
                Log.Fatal(e, "Application Startup Failed...");
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseSerilog()
                .UseStartup<Startup>();
    }
}