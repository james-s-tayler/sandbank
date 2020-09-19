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
            var assembly = Assembly.GetExecutingAssembly().GetName();

            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("Microsoft.EntityFrameworkCore.Database.Command", LogEventLevel.Information)
                .Enrich.FromLogContext()
                .Enrich.WithProperty("AssemblyName", $"{assembly.Name}")
                .Enrich.WithProperty("AssemblyVersion", $"{assembly.Version}")
                .Enrich.WithExceptionDetails() // enriches exceptions by adding ex.Data properties (just add like ex.Data.Add(key, value)) 
                .WriteTo.Console(new RenderedCompactJsonFormatter())
                .CreateLogger();

            try
            {
                var host = CreateWebHostBuilder(args).Build();

                using (var serviceScope = host.Services.GetService<IServiceScopeFactory>().CreateScope())
                {
                    await using var context = serviceScope.ServiceProvider.GetService<SandBankDbContext>();
                    context.Database.Migrate();
                    var seedDataService = serviceScope.ServiceProvider.GetService<ISeedTransactionDataService>();
                    await seedDataService.SeedData();
                }

                host.Run();
            }
            catch (Exception e)
            {
                Log.Error(e, e.Message);
            }
            finally
            {
                Log.CloseAndFlush();
            }
        }

        private static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>()
                .UseSerilog();
        }
    }
}
