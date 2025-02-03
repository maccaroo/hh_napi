using hh_napi.Persistence;
using Microsoft.EntityFrameworkCore;
using hh_napi.Persistence.Repositories;
using hh_napi.Services;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<AppDbContext>(options => options.UseNpgsql(connectionString));

builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IDataPointRepository, DataPointRepository>();
builder.Services.AddScoped<IDataSourceRepository, DataSourceRepository>();

builder.Services.AddScoped<IUserService, UserService>();
builder.Services.AddScoped<IDataPointService, DataPointService>();
builder.Services.AddScoped<IDataSourceService, DataSourceService>();

builder.Services.AddControllers();

// Add services to the container.
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();
