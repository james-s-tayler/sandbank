using System;
using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.SQS;
using Integration.AWS.SNS;
using Integration.OutboundTransactions;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using Amazon.DynamoDBv2;
using Core.Jwt;
using Core.MultiTenant;
using Database;
using Integration.AWS.CloudWatch;
using Integration.AWS.DynamoDB;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Services.Domain.Accounts;
using Services.System.NumberRange;
using Swashbuckle.AspNetCore.SwaggerGen;
using Serilog;
using Serilog.Context;

namespace Api
{
    public class Startup
    {
        private IConfiguration _config { get; }
        private IWebHostEnvironment _env { get; }
        
        private const string _localDevCorsPolicy = "localDevCorsPolicy";

        public Startup(IWebHostEnvironment env, IConfiguration config)
        {
            _env = env;
            _config = config;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var awsOptions = _config.GetAWSOptions();
            if (awsOptions.Region == null)
                throw new ArgumentNullException(nameof(awsOptions.Region));
            
            Console.WriteLine($"Configured to use AWS.Region: {awsOptions.Region}");
            
            var dbConfigSection = _config.GetSection(nameof(DatabaseConnection));
            var dbConfig = dbConfigSection.Get<DatabaseConnection>();
            Console.WriteLine(dbConfig.GetConnectionString());
            
            services.AddDbContext<SandBankDbContext>(options =>
                options.UseNpgsql(dbConfig.GetConnectionString()));
            
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

            services.AddControllers()
                .AddApplicationPart(typeof(Startup).Assembly)
                //System.Text.Json is slightly faster, but has many unacceptable breaking changes as of now
                //Performance-wise Utf8json blows everything out of the water, but I haven't tested it for compatibility yet
                .AddNewtonsoftJson();
            

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SandBank API", Version = "v1" });
                c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme.",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Scheme = "bearer",
                    Type = SecuritySchemeType.Http,
                    BearerFormat = "Bearer {token}"
                });
                c.OperationFilter<SecurityRequirementsOperationFilter>();
            });

            services.AddTransient<INumberRangeService, NumberRangeService>();
            services.AddTransient<IOutboundTransactionProcessor, StubOutboundTransactionProcessor>();
            services.AddTransient(typeof(EventPublisher<>), typeof(EventPublisher<>));

            if (_env.IsDevelopment())
            {
                //services.AddTransient(x => new LocalstackSNSClientFactory().CreateClient());
                //services.AddTransient(x => new DefaultCloudWatchClientFactory().CreateClient());
                //services.AddTransient(x => new LocalstackCloudWatchLogsClientFactory().CreateClient());
                //services.AddTransient(x => new DefaultDynamoDbClientFactory().CreateClient());
            }
            else
            {
                //services.AddTransient(x => new DefaultSNSClientFactory().CreateClient());
                //services.AddTransient(x => new DefaultCloudWatchClientFactory().CreateClient());
                //services.AddTransient(x => new DefaultCloudWatchLogsClientFactory().CreateClient());
                
                //temporary - this should actually be run as part of localstack and we should use the localstack dynamo container for Development + Test
                //just getting the proof of concept working
                /*if (_env.IsEnvironment("Test"))
                {
                    services.AddTransient<IAmazonDynamoDB>(x =>
                    {
                        var clientConfig = new AmazonDynamoDBConfig
                        {
                            ServiceURL = "http://localhost:8000",
                        };
                        return new AmazonDynamoDBClient(clientConfig);
                    });
                }
                else
                {
                    services.AddTransient(x => new DefaultDynamoDbClientFactory().CreateClient());    
                }*/
            }
            
            /*var awsSqsOptions = new AWSOptions();
            awsSqsOptions.DefaultClientConfig.ProxyPort = 4576;
            awsSqsOptions.DefaultClientConfig.ProxyHost = "localstack";
            awsSqsOptions.DefaultClientConfig.ServiceURL = "http://localstack:4576";
            awsSqsOptions.DefaultClientConfig.RegionEndpoint = RegionEndpoint.USEast1;
            awsSqsOptions.DefaultClientConfig.UseHttp = true;
            services.AddAWSService<IAmazonSQS>(awsSqsOptions);*/
            
            //services.AddLogging();
            services.AddTransient<IAccountService, AccountService>();
            
            var jwtConfigSection = _config.GetSection(nameof(JwtTokenConfiguration));
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
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "SandBank API V1");
                });
            }
            else
            {
                app.UseHttpsRedirection();
                app.UseHsts();
            }
            
            app.UseSerilogRequestLogging(options =>
            {
                // Attach additional properties to the request completion event
                options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
                {
                    var userId = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                    diagnosticContext.Set("UserId", userId);
                };
            });
            app.UseRouting();
            app.UseCors(_localDevCorsPolicy);
            app.UseAuthentication();
            app.UseAuthorization();
            
            //add UserId to stuff logged outside the middleware
            app.Use(async (httpContext, next) =>
            {
                var userId = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "anonymous";
                LogContext.PushProperty("UserId", userId);    
                
                await next.Invoke();
            });
            
            app.UseEndpoints(endpoints => {
                endpoints.MapControllers().RequireAuthorization();
            });
        }
    }

    public class SecurityRequirementsOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (!context
                    .MethodInfo
                    .GetCustomAttributes(true)
                    .OfType<AllowAnonymousAttribute>()
                    .Any())
            {
                operation.Security = new List<OpenApiSecurityRequirement>
                {
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference { Type = ReferenceType.SecurityScheme, Id = "Bearer" }
                            },
                            new List<string>()
                        }
                    }
                };
            }
        }
    }
}
