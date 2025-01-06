using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.ConstantValues.TokenOptions;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models;
using DreamcoreHorrorGameApiServer.Models.Database;
using DreamcoreHorrorGameApiServer.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.Configuration;

namespace DreamcoreHorrorGameApiServer;

public class Program
{
    public static void Main(string[] args)
    {
        PrintTestAccessToken();
        PrintTestRefreshToken();

        try
        {
            WebApplicationBuilder builder = ConfigureWebApplicationBuilder(args);

            WebApplication app = builder.Build();

            app.UseHttpsRedirection();
            app.UseCors(CorsPolicyNames.Default);
            app.UseAuthentication();
            app.UseAuthorization();

            if (app.Environment.IsEnvironment(Environments.Development))
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.MapControllers();

            app.Run();
        }
        catch (SettingsPropertyNotFoundException ex)
        {
            var defaultForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Configuration property \"{ex.Message}\" was not found.");
            Console.ForegroundColor = defaultForegroundColor;

            Environment.Exit(-1);
        }
        catch (FormatException ex)
        {
            var defaultForegroundColor = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Configuration property \"{ex.Message}\" has a value of incorrect type.");
            Console.ForegroundColor = defaultForegroundColor;

            Environment.Exit(-1);
        }
    }

    private static void PrintTestAccessToken() => Console.WriteLine
    (
        $"ACCESS TOKEN:\n{ new TokenService().CreateAccessToken("test_login", AuthenticationRoles.FullAccessDeveloper) }\n"
    );

    private static void PrintTestRefreshToken() => Console.WriteLine
    (
        $"REFRESH TOKEN:\n{new TokenService().CreateRefreshToken("test_login", AuthenticationRoles.FullAccessDeveloper)}\n"
    );

    private static WebApplicationBuilder ConfigureWebApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string? loggingDirectory = builder.Configuration[ConfigurationProperties.LoggingDirectory];
        string? loggingFileNameTemplate = builder.Configuration[ConfigurationProperties.LoggingFileNameTemplate];
        string? maxLoggingFileSizeStr = builder.Configuration[ConfigurationProperties.MaxLoggingFileSize];

        if (string.IsNullOrEmpty(loggingDirectory))
            throw new SettingsPropertyNotFoundException(ConfigurationProperties.LoggingDirectory);

        if (string.IsNullOrEmpty(loggingFileNameTemplate))
            throw new SettingsPropertyNotFoundException(ConfigurationProperties.LoggingFileNameTemplate);

        if (string.IsNullOrEmpty(maxLoggingFileSizeStr))
            throw new SettingsPropertyNotFoundException(ConfigurationProperties.MaxLoggingFileSize);

        if (!long.TryParse(maxLoggingFileSizeStr, out long maxLoggingFileSize))
            throw new FormatException(ConfigurationProperties.MaxLoggingFileSize);

        string loggingDirectoryPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, loggingDirectory);

        if (!Directory.Exists(loggingDirectoryPath))
            Directory.CreateDirectory(loggingDirectoryPath);

        string loggingFilePathTemplate = Path.Combine(loggingDirectoryPath, loggingFileNameTemplate);

        builder.Logging
            .ClearProviders()
            .AddConsole()
            .AddFile(loggingFilePathTemplate, maxLoggingFileSize);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            var defaultJsonSerializerOptions = new JsonSerializerOptionsProvider().Default;
            options.ConfigureFrom(defaultJsonSerializerOptions);
        });

        builder.Services.AddSwaggerGen();
        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddSingleton<IJsonSerializerOptionsProvider, JsonSerializerOptionsProvider>();
        builder.Services.AddSingleton<ITokenService, TokenService>();
        builder.Services.AddSingleton<IPropertyPredicateValidator, PropertyPredicateValidator>();
        builder.Services.AddScoped<IRabbitMqProducer, RabbitMqProducer>();
        builder.Services.AddScoped<IHttpFetcher, HttpFetcher>();
        builder.Services.AddScoped<IPasswordHasher<Developer>, PasswordHasher<Developer>>();
        builder.Services.AddScoped<IPasswordHasher<Player>, PasswordHasher<Player>>();
        builder.Services.AddScoped<IPasswordHasher<Server>, PasswordHasher<Server>>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyNames.Default, policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
                policy.WithHeaders
                (
                    HeaderNames.Accept,
                    HeaderNames.Authorization,
                    HeaderNames.ContentLength,
                    HeaderNames.ContentType,
                    HeaderNames.Host,
                    HeaderNames.Origin,
                    HeaderNames.XXSSProtection,
                    RequestSenders.GameClient,
                    RequestSenders.GameServer,
                    RequestSenders.ApplicationForDevelopers
                );
            });
        });

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(AuthenticationSchemes.Access, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.FromMinutes(1),
                    ValidateIssuer = true,
                    ValidIssuer = AccessTokenOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = AccessTokenOptions.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = AccessTokenOptions.SecurityKey,
                    ValidateIssuerSigningKey = true
                };
            })
            .AddJwtBearer(AuthenticationSchemes.Refresh, options =>
            {
                options.RequireHttpsMetadata = true;
                options.SaveToken = true;
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ClockSkew = TimeSpan.FromMinutes(1),
                    ValidateIssuer = true,
                    ValidIssuer = RefreshTokenOptions.Issuer,
                    ValidateAudience = true,
                    ValidAudience = RefreshTokenOptions.Audience,
                    ValidateLifetime = true,
                    IssuerSigningKey = RefreshTokenOptions.SecurityKey,
                    ValidateIssuerSigningKey = true,
                };
            });

        string? defaultConnectionString = builder.Configuration.GetConnectionString(ConfigurationProperties.DefaultConnectionString);

        if (string.IsNullOrEmpty(defaultConnectionString))
            throw new SettingsPropertyNotFoundException($"ConnectionStrings: {ConfigurationProperties.DefaultConnectionString}");

        builder.Services.AddDbContext<DreamcoreHorrorGameContext>
        (
            optionsAction: options => options.UseNpgsql(defaultConnectionString),
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient
        );

        return builder;
    }
}
