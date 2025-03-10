using FluentValidation;
using FluentValidation.AspNetCore;
using hh_napi.Mappings;
using hh_napi.Models.Validators;
using hh_napi.Persistence.Repositories;
using hh_napi.Persistence.Repositories.Interfaces;
using hh_napi.Services;
using hh_napi.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.OpenApi.Models;

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

        // Versioning
        services.AddApiVersioning(options =>
        {
            var apiVersionSettings = builder.Configuration.GetSection("ApiVersion");
            
            bool allowUnspecified = bool.TryParse(apiVersionSettings["AllowUnspecified"], out var unspecified) && unspecified;

            string? defaultVersionString = apiVersionSettings["DefaultVersion"];
            ApiVersion defaultApiVersion = new(1, 0);
            if (!string.IsNullOrEmpty(defaultVersionString) && ApiVersion.TryParse(defaultVersionString, out var parsedVersion))
            {
                defaultApiVersion = parsedVersion;
            }

            options.ReportApiVersions = true;
            options.AssumeDefaultVersionWhenUnspecified = allowUnspecified;
            options.DefaultApiVersion = defaultApiVersion;
            options.ApiVersionReader = new HeaderApiVersionReader("X-Api-Version");
        });

        services.AddVersionedApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        });

        // OpenAPI (Swagger)
        services.AddEndpointsApiExplorer();
        services.AddSwaggerGen(options =>
        {
            var provider = services.BuildServiceProvider().GetRequiredService<IApiVersionDescriptionProvider>();

            foreach (var description in provider.ApiVersionDescriptions)
            {
                options.SwaggerDoc(description.GroupName, new OpenApiInfo
                {
                    Title = "Home Historian API",
                    Version = description.ApiVersion.ToString(),
                    Description = "API for managing historical data points"
                });
            }

            // Add JWT Authentication support to Swagger
            options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
            {
                Description = "JWT Authorization header using the Bearer scheme. Example: \"Authorization: Bearer {token}\"",
                Name = "Authorization",
                In = ParameterLocation.Header,
                Type = SecuritySchemeType.ApiKey,
                Scheme = "Bearer"
            });

            options.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = "Bearer"
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });
    }
}
