using hh_napi.Mappings;
using hh_napi.Persistence.Repositories;
using hh_napi.Persistence.Repositories.Interfaces;
using hh_napi.Services;
using hh_napi.Services.Interfaces;

namespace hh_napi.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var services = builder.Services;
        
        // Controllers
        services.AddControllers();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDataPointRepository, DataPointRepository>();
        services.AddScoped<IDataSourceRepository, DataSourceRepository>();
        services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDataPointService, DataPointService>();
        services.AddScoped<IDataSourceService, DataSourceService>();

        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // OpenAPI (Swagger)
        services.AddOpenApi();
    }
}