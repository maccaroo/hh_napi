using Serilog;
using hh_napi.Middleware;

namespace hh_napi.Configurations;

public static class MiddlewareConfiguration
{
    public static void ConfigureMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlingMiddleware>();

        app.Use(async (context, next) =>
        {
            var logMessage = $"{context.Request.Method} {context.Request.Path} from {context.Connection.RemoteIpAddress}";
            Log.Information(logMessage);
            await next();
        });

        app.UseRouting();
        app.UseAuthentication();
        app.UseAuthorization();
        app.UseSerilogRequestLogging();
    }
}