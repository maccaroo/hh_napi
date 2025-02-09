using hh_napi.Persistence;
using Microsoft.EntityFrameworkCore;
using hh_napi.Persistence.Repositories;
using hh_napi.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using hh_napi.Mappings;
using hh_napi.Services.Interfaces;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddAutoMapper(typeof(MappingProfile));

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

var jwtSettings = builder.Configuration.GetSection("JwtSettings");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        var key = jwtSettings["Key"];
        if (string.IsNullOrEmpty(key))
        {
            throw new InvalidOperationException("JWT key is missing from the configuration.");
        }

        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidateAudience = true,
            ValidAudience = jwtSettings["Audience"],
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddAuthorization();

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDataPointRepository, DataPointRepository>();
builder.Services.AddScoped<IDataSourceRepository, DataSourceRepository>();
builder.Services.AddScoped<IUserCredentialsRepository, UserCredentialsRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDataPointService, DataPointService>();
builder.Services.AddScoped<IDataSourceService, DataSourceService>();

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
