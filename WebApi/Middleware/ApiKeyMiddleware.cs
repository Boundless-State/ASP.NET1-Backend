using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;

public class ApiKeyMiddleware
{
    private readonly RequestDelegate _next;
    private readonly IConfiguration _configuration;

    public ApiKeyMiddleware(RequestDelegate next, IConfiguration configuration)
    {
        _next = next;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var path = context.Request.Path.Value;
        if (path.StartsWith("/swagger") ||
            path.StartsWith("/favicon.ico") ||
            path.EndsWith(".js") ||
            path.EndsWith(".css") ||
            path.EndsWith(".map") ||
            path.EndsWith(".json") ||
            path.EndsWith(".html"))
        {
            await _next(context);
            return;
        }

        if (!context.Request.Headers.TryGetValue("x-api-key", out var extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("API-nyckel saknas");
            return;
        }

        var configuredApiKey = _configuration.GetValue<string>("ApiKey");

        if (!configuredApiKey.Equals(extractedApiKey))
        {
            context.Response.StatusCode = 401;
            await context.Response.WriteAsync("Ogiltig API-nyckel");
            return;
        }

        await _next(context);
    }
}


