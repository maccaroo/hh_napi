using hh_napi.Configurations;


var builder = WebApplication.CreateBuilder(args);
builder.ConfigureLogging();
builder.ConfigureDatabase();
builder.ConfigureAuthentication();
builder.ConfigureServices();

var app = builder.Build();
app.ConfigureMiddleware();
app.MapControllers();
app.Run();
