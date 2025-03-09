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
        services.AddScoped<IRefreshTokenRepository, RefreshTokenRepository>();

        // UnitOfWork
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        // Services
        services.AddScoped<IUserService, UserService>();
        services.AddScoped<IDataPointService, DataPointService>();
        services.AddScoped<IDataSourceService, DataSourceService>();
        services.AddScoped<ITokenService, TokenService>();
        services.AddSingleton<ILoginAttemptService, LoginAttemptService>();

        // AutoMapper
        services.AddAutoMapper(typeof(MappingProfile));

        // OpenAPI (Swagger)
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
            {
                Title = "Home Historian API",
                Version = "v1",
                Description = "API for managing historical data points"
            });

            // Add JWT Authentication support to Swagger
            c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.Models.OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = Microsoft.OpenApi.Models.ParameterLocation.Header,
                Type = Microsoft.OpenApi.Models.SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement
            {
                {
                    new Microsoft.OpenApi.Models.OpenApiSecurityScheme
                    {
                        Reference = new Microsoft.OpenApi.Models.OpenApiReference
                        {
                            Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
