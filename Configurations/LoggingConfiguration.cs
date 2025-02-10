using Serilog;

namespace hh_napi.Configurations;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
            .CreateLogger();

        builder.Host.UseSerilog();
    }
}