using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using Endpoints.Configuration;
using Endpoints.Data;
using Integration.AWS.SNS;
using Integration.OutboundTransactions;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Swashbuckle.AspNetCore.Swagger;

namespace Endpoints
{
    public class Startup
    {
        private IConfiguration _config { get; }
        private IHostingEnvironment _env { get; }

        public Startup(IHostingEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        public IConfiguration Configuration { get; }
        private static readonly string _localDevCorsPolicy = "localDevCorsPolicy";

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var connectionString = _config.GetConnectionString("DefaultConnection");

            services.AddDbContext<SandBankDbContext>(options =>
                options.UseNpgsql(connectionString));
            
            services.AddCors(options =>
            {
                options.AddPolicy(_localDevCorsPolicy,
                    builder =>
                    {
                        builder.WithOrigins("*")
                            .AllowAnyHeader()
                            .AllowAnyMethod();
                    });
            });
            
            services.AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddJsonOptions(options =>
            {
                options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            });
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "SandBank API", Version = "v1" });
            });

            services.AddTransient<INumberRangeService, NumberRangeService>();
            services.AddTransient<IOutboundTransactionProcessor, StubOutboundTransactionProcessor>();
            services.AddTransient(typeof(EventPublisher<>), typeof(EventPublisher<>));

            if (_env.IsDevelopment())
            {
                services.AddTransient(x => new LocalstackSNSClientFactory().CreateClient());
            }
            else
            {
                services.AddTransient(x => new DefaultSNSClientFactory().CreateClient());
            }
            
            var awsSqsOptions = new AWSOptions();
            awsSqsOptions.DefaultClientConfig.ProxyPort = 4576;
            awsSqsOptions.DefaultClientConfig.ProxyHost = "localstack";
            awsSqsOptions.DefaultClientConfig.ServiceURL = "http://localstack:4576";
            awsSqsOptions.DefaultClientConfig.RegionEndpoint = RegionEndpoint.USEast1;
            awsSqsOptions.DefaultClientConfig.UseHttp = true;
            services.AddAWSService<IAmazonSQS>(awsSqsOptions);
            
            services.AddLogging();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            UpdateDatabase(app);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            //app.UseHttpsRedirection();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SandBank API V1");
            });

            if (env.IsDevelopment())
            {
                app.UseCors(_localDevCorsPolicy);    
            }
            app.UseMvc();
        }
        
        private static void UpdateDatabase(IApplicationBuilder app)
        {
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                using (var context = serviceScope.ServiceProvider.GetService<SandBankDbContext>())
                {
                    context.Database.Migrate();
                }
            }
        }
    }
}
