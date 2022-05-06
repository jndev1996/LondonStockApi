using LondonStockExchange.Api.Models;
using Microsoft.Extensions.Options;

namespace LondonStockExchange.Api.Middleware
{
    public class ApiKeyMiddleware
    {
        private readonly RequestDelegate _next;
        private const string APIKEYNAME = "ApiKey";
        private readonly ApiKeyConfiguration _apiKeyConfiguration;
        
        public ApiKeyMiddleware(RequestDelegate next, IOptions<ApiKeyConfiguration> apiKeyConfiguration)
        {
            _next = next;
            _apiKeyConfiguration = apiKeyConfiguration.Value;
        }
        public async Task InvokeAsync(HttpContext context)
        {
            if (context.Request.Path.Value == "/swagger/v1/swagger.json")
            {
                await _next(context);
            }
            
            if (!context.Request.Headers.TryGetValue(APIKEYNAME, out var extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Api Key was not provided. (Using ApiKeyMiddleware) ");
                return;
            }
            
            if (!_apiKeyConfiguration.Key.Equals(extractedApiKey))
            {
                context.Response.StatusCode = 401;
                await context.Response.WriteAsync("Unauthorized client. (Using ApiKeyMiddleware)");
                return;
            }
            
            await _next(context);
        }
    }
}