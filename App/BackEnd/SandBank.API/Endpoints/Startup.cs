using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using Endpoints.Configuration;
using Endpoints.Data;
using Integration.AWS.SNS;
using Integration.OutboundTransactions;
﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Jwt;
using Core.MultiTenant;
using Database;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Services.Domain.Accounts;
using Services.System.NumberRange;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

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

        private IConfiguration Configuration { get; }
        private const string _localDevCorsPolicy = "localDevCorsPolicy";

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

                c.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Name = "Authorization",
                    In = "header",
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Type = "apiKey"
                });

                c.OperationFilter<SecurityRequirementsOperationFilter>();
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
            services.AddTransient<IAccountService, AccountService>();
            
            var jwtConfigSection = Configuration.GetSection(nameof(JwtTokenConfiguration));
            var jwtTokenConfiguration = jwtConfigSection.Get<JwtTokenConfiguration>();
            var secret = Encoding.UTF8.GetBytes(jwtTokenConfiguration.Secret);
            services.Configure<JwtTokenConfiguration>(jwtConfigSection);
            services.AddTransient<IJwtTokenService, JwtTokenService>();
            services.AddTransient<ISeedTransactionDataService, SeedTransactionDataService>();
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddScoped<ITenantProvider, TenantProvider>();

            services.AddAuthentication(x =>
                {
                    x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(secret),
                        ValidIssuer = jwtTokenConfiguration.Issuer,
                        ValidAudience = jwtTokenConfiguration.Audience,
                        ValidateIssuer = true,
                        ValidateAudience = true
                    };
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
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
            
            app.UseCors(_localDevCorsPolicy);
            app.UseAuthentication();
            app.UseMvc();
        }
    }

    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(Operation operation, OperationFilterContext context)
        {
            if (!context
                    .MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any())
            {
                operation.Security = new List<IDictionary<string, IEnumerable<string>>>
                {
                    new Dictionary<string, IEnumerable<string>>
                    {
                        {"Bearer", Array.Empty<string>()}
                    }
                };
            }
        }
    }
}
