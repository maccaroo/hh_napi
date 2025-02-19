using FluentValidation;
using FluentValidation.AspNetCore;
using hh_napi.Mappings;
using hh_napi.Models.Validators;
using hh_napi.Persistence.Repositories;
using hh_napi.Persistence.Repositories.Interfaces;
using hh_napi.Services;
using hh_napi.Services.Interfaces;

namespace hh_napi.Configurations;

public static class ServiceConfiguration
{
    public static void ConfigureServices(this WebApplicationBuilder builder)
    {
        var angularClientSettings = builder.Configuration.GetSection("AngularClient");
        var url = angularClientSettings["Url"];
        if (string.IsNullOrEmpty(url))
        {
            throw new InvalidOperationException("Angular client URL is missing from the configuration.");
        }

        var services = builder.Services;

        // Settings
        services.Configure<ValidationSettings>(builder.Configuration.GetSection("Validation"));

        services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularApp", builder => builder
                .WithOrigins(url)
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowCredentials()
            );
        });

        // Controllers
        services.AddControllers();

        // FluentValidation
        services.AddFluentValidationAutoValidation()
                .AddFluentValidationClientsideAdapters();
        services.AddValidatorsFromAssemblyContaining<CreateUserRequestValidator>();

        // Repositories
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDataPointRepository, DataPointRepository>();
        services.AddScoped<IDataSourceRepository, DataSourceRepository>();
        services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();

        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

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