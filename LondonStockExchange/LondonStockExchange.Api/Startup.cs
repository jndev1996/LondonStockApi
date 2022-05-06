

using Hellang.Middleware.ProblemDetails;
using LondonStockExchange.Api.Middleware;
using LondonStockExchange.Api.Models;
using LondonStockExchange.Service.Services;
using LondonStockExchange.Sql.Configuration;
using LondonStockExchange.Sql.Repositories;
using Microsoft.OpenApi.Models;

namespace LondonStockExchange.Api
{
    public class Startup
    {
        private IConfiguration Configuration { get; }
        
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }
        
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddProblemDetails();
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.AddSecurityDefinition("ApiKey", new OpenApiSecurityScheme()
                {
                    In = ParameterLocation.Header,
                    Name = "ApiKey", //header with api key
                    Type = SecuritySchemeType.ApiKey,
                    Scheme = "ApiKeyScheme"
                });
                
                var key = new OpenApiSecurityScheme()
                {
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "ApiKey"
                    },
                    In = ParameterLocation.Header
                };
                var requirement = new OpenApiSecurityRequirement
                {
                    { key, new List<string>() }
                };
                c.AddSecurityRequirement(requirement);
            });

            services.AddTransient<IStockValueRepository, StockValueRepository>();
            services.AddTransient<ITradeRepository, TradeRepository>();
            services.AddTransient<IStockValueService, StockValueService>();
            services.AddTransient<ITradeService, TradeService>();

            services.Configure<SqlDatabaseConfiguration>(Configuration.GetSection("SqlDatabaseConfiguration"));
            services.Configure<ApiKeyConfiguration>(Configuration.GetSection("ApiKeyConfiguration"));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, ILogger<Startup> logger)
        {
            app.UseSwagger();
            app.UseSwaggerUI(options =>
            {
                options.SwaggerEndpoint("swagger/v1/swagger.json", "v1");
                options.RoutePrefix = string.Empty;
            });
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseMiddleware<ApiKeyMiddleware>();
            app.UseProblemDetails();
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }
    }
}

