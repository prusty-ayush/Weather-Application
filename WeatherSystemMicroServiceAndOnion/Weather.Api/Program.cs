using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using MongoDB.Driver.Core.Configuration;
using Serilog;
using System.Text;
using Weather.Application.Interfaces;
using Weather.Domain.Interfaces;
using Weather.Infrastructure.Data;
using Weather.Infrastructure.Repository;
using Weather.Infrastructure.Services;

var builder = WebApplication.CreateBuilder(args);

//var _connectionString = builder.Configuration.GetConnectionString("SqlConnection");

//if (string.IsNullOrEmpty(_connectionString))
//    throw new Exception("SqlConnection missing in appsettings");


Log.Logger = new LoggerConfiguration()
                    .ReadFrom.Configuration(builder.Configuration)
                    .Enrich.FromLogContext()
                    .WriteTo.Console()
                    .WriteTo.File(
                        path: "Logs/log-.txt",
                        rollingInterval: RollingInterval.Day,
                        retainedFileCountLimit:2
                        )
                    //.WriteTo.MSSqlServer(
                    //    connectionString: _connectionString,
                    //    sinkOptions: new Serilog.Sinks.MSSqlServer.MSSqlServerSinkOptions
                    //    {
                    //        TableName = "WeatherServiceLogs",
                    //        AutoCreateSqlTable = true
                    //    })
                    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddHttpClient<IWeatherService, WeatherService>();
builder.Services.AddSingleton<MongoDbContext>();
builder.Services.AddScoped<IWeatherDataRepository, WeatherDataRepository>();

builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration["Redis:ConnectionString"];
    options.InstanceName = "WeatherApp:";
});

var jwtSettings = builder.Configuration.GetSection("JWTSetting");
var key = jwtSettings["SecretKey"] ?? throw new InvalidOperationException("Secret Key missing");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtSettings["Issuer"],
            ValidAudience = jwtSettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.WithOrigins("http://localhost:4200")
              .AllowAnyHeader()
              .AllowAnyMethod()
              .AllowCredentials();
    });
});

builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();

var app = builder.Build();
app.UseHttpsRedirection();
app.UseRouting();
app.UseSerilogRequestLogging();
app.UseCors("AllowAll");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();

try
{
    Log.Information("Weather Api starting ...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Weather Api failed to start");
}
finally
{
    Log.CloseAndFlush();
}