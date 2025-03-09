using System.Threading.RateLimiting;
// using Microsoft.AspNetCore.RateLimiting;

namespace hh_napi.Configurations;

public static class RateLimitingConfiguration
{
    public static void ConfigureRateLimiting(this WebApplicationBuilder builder)
    {
        var rateLimitSettings = builder.Configuration.GetSection("RateLimitSettings");
        
        // Add rate limiting services
        builder.Services.AddRateLimiter(options =>
        {
            // Configure default rate limiter options
            options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            
            // Add a comprehensive policy for login attempts that combines IP and endpoint-based limiting
            options.AddPolicy("login", httpContext =>
            {
                // Get the IP address
                var ip = httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                // Get the endpoint path
                var path = httpContext.Request.Path.ToString();
                
                // Create a partition key that combines IP and endpoint
                // This provides IP-based rate limiting per endpoint
                var partitionKey = $"{ip}:{path}";
                
                return RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: partitionKey,
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = int.Parse(rateLimitSettings["LoginEndpointPermitLimit"] ?? "10"),
                        Window = TimeSpan.FromMinutes(int.Parse(rateLimitSettings["LoginEndpointWindowMinutes"] ?? "15"))
                    });
            });
            
            // Global rate limiting for all endpoints
            options.GlobalLimiter = PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
                RateLimitPartition.GetFixedWindowLimiter(
                    partitionKey: httpContext.Connection.RemoteIpAddress?.ToString() ?? "unknown",
                    factory: partition => new FixedWindowRateLimiterOptions
                    {
                        AutoReplenishment = true,
                        PermitLimit = int.Parse(rateLimitSettings["GlobalPermitLimit"] ?? "100"),
                        Window = TimeSpan.FromMinutes(int.Parse(rateLimitSettings["GlobalWindowMinutes"] ?? "1"))
                    }));
        });
    }
}
