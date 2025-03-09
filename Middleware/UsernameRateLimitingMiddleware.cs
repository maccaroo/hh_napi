using System.Text.Json;
using hh_napi.Models;
using hh_napi.Services;

namespace hh_napi.Middleware;

public class UsernameRateLimitingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<UsernameRateLimitingMiddleware> _logger;
    private readonly IConfiguration _configuration;

    public UsernameRateLimitingMiddleware(
        RequestDelegate next,
        ILogger<UsernameRateLimitingMiddleware> logger,
        IConfiguration configuration)
    {
        _next = next;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task InvokeAsync(HttpContext context, ILoginAttemptService loginAttemptService)
    {
        // Only apply to login endpoint
        if (!IsLoginEndpoint(context))
        {
            await _next(context);
            return;
        }

        // Try to extract username from request body
        string? username = await ExtractUsernameFromRequestAsync(context);
        
        if (!string.IsNullOrEmpty(username))
        {
            // Check if the username is locked out
            if (await loginAttemptService.IsUserLockedOutAsync(username))
            {
                _logger.LogWarning("Username rate limit triggered for {Username}", username);
                
                context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
                context.Response.ContentType = "application/json";
                
                var response = new
                {
                    Message = "Too many login attempts for this account. Please try again later."
                };
                
                await context.Response.WriteAsync(JsonSerializer.Serialize(response));
                return;
            }
        }

        // Continue with the request
        await _next(context);
    }

    private bool IsLoginEndpoint(HttpContext context)
    {
        return context.Request.Method == "POST" && 
               context.Request.Path.StartsWithSegments("/api/auth/login", StringComparison.OrdinalIgnoreCase);
    }

    private async Task<string?> ExtractUsernameFromRequestAsync(HttpContext context)
    {
        try
        {
            // Ensure we can read the body
            context.Request.EnableBuffering();
            
            // Read the request body
            using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
            var body = await reader.ReadToEndAsync();
            
            // Reset the position to allow reading again in the controller
            context.Request.Body.Position = 0;
            
            // Parse the JSON to get the username
            if (!string.IsNullOrEmpty(body))
            {
                var loginRequest = JsonSerializer.Deserialize<LoginRequest>(body, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });
                
                return loginRequest?.Username;
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error extracting username from request body");
        }
        
        return null;
    }
}

// Extension method to add the middleware to the request pipeline
public static class UsernameRateLimitingMiddlewareExtensions
{
    public static IApplicationBuilder UseUsernameRateLimiting(this IApplicationBuilder builder)
    {
        return builder.UseMiddleware<UsernameRateLimitingMiddleware>();
    }
}
