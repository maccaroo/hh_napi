using Serilog;

namespace hh_napi.Configurations;

public static class LoggingConfiguration
{
    public static void ConfigureLogging(this WebApplicationBuilder builder)
    {
        var configuration = builder.Configuration;

        Serilog.Debugging.SelfLog.Enable(Console.Error);

        Console.WriteLine("⚠️ Logging Configuration is being applied...");

        Log.Logger = new LoggerConfiguration()
            .ReadFrom.Configuration(configuration)
                .WriteTo.Console()
                .WriteTo.Debug()
            .CreateLogger();

        Console.WriteLine("✅ Serilog should now be configured.");

        builder.Host.UseSerilog();
    }
}