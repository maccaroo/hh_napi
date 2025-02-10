using hh_napi.Persistence;
using Microsoft.EntityFrameworkCore;

namespace hh_napi.Configurations;

public static class DatabaseConfiguration
{
    public static void ConfigureDatabase(this WebApplicationBuilder builder)
    {
        
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection string is missing from configuration.");
        }

        builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));
    }
}