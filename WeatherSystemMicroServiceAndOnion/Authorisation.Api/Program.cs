using Authorisation.Application.Interfaces;
using Authorisation.Domain.Interfaces;
using Authorisation.Infrastructure.Data;
using Authorisation.Infrastructure.Repository;
using Authorisation.Application.Service;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.Sinks.MSSqlServer;
using System.Text;


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
                        retainedFileCountLimit: 2)

                    //.WriteTo.MSSqlServer(
                    //    connectionString: _connectionString,
                    //    sinkOptions: new MSSqlServerSinkOptions
                    //    {
                    //        TableName = "AuthorisationServiceLogs",
                    //        AutoCreateSqlTable = true
                    //    })

                    .CreateLogger();

builder.Host.UseSerilog();

builder.Services.AddScoped<AuthorisationService>();
builder.Services.AddScoped<ILocalAuthServices>(s => s.GetRequiredService<AuthorisationService>());
builder.Services.AddScoped<ITokenService>(s => s.GetRequiredService<AuthorisationService>());
builder.Services.AddScoped<IExternalAuthService>(s=>s.GetRequiredService<AuthorisationService>());

builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddDbContext<AppDbContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("SqlConnection")));


//JWT Configuration
var jwtsettings = builder.Configuration.GetSection("JWTSetting");
var key = jwtsettings["SecretKey"] ?? throw new InvalidOperationException("Secret Key Section is Missing");

builder.Services.AddAuthentication(option =>
{
    option.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    option.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(option =>
    {
        option.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateIssuerSigningKey = true,
            ValidateLifetime = true,
            ValidIssuer = jwtsettings["Issuer"],
            ValidAudience = jwtsettings["Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key))

        };
    })
    .AddCookie()
    .AddGoogle(option =>
    {
        option.ClientId = builder.Configuration["Authentication:Google:ClientId"]!;
        option.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"]!;
        option.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;
        option.AdditionalAuthorizationParameters.Add("prompt", "select_account");
    })
    .AddFacebook(option =>
    {
        option.AppId = builder.Configuration["Authentication:Facebook:AppId"]!;
        option.AppSecret = builder.Configuration["Authentication:Facebook:AppSecret"]!;
        option.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

        option.Fields.Add("email");
        option.Fields.Add("name");
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
    Log.Information("Authentication Starting...");
    app.Run();
}
catch (Exception ex)
{
    Log.Fatal(ex, "Authentication failed to start");
}
finally
{
    Log.CloseAndFlush();
}
