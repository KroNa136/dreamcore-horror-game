using DreamcoreHorrorGameStatisticsServer.ConstantValues;
using DreamcoreHorrorGameStatisticsServer.Extensions;
using DreamcoreHorrorGameStatisticsServer.Models;
using DreamcoreHorrorGameStatisticsServer.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using System.Configuration;

namespace DreamcoreHorrorGameStatisticsServer;

public class Program
{
    public static void Main(string[] args)
    {
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

    private static WebApplicationBuilder ConfigureWebApplicationBuilder(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        string? loggingDirectory = builder.Configuration[ConfigurationPropertyNames.LoggingDirectory];
        string? loggingFileNameTemplate = builder.Configuration[ConfigurationPropertyNames.LoggingFileNameTemplate];
        string? maxLoggingFileSizeStr = builder.Configuration[ConfigurationPropertyNames.MaxLoggingFileSize];

        if (string.IsNullOrEmpty(loggingDirectory))
            throw new SettingsPropertyNotFoundException(ConfigurationPropertyNames.LoggingDirectory);

        if (string.IsNullOrEmpty(loggingFileNameTemplate))
            throw new SettingsPropertyNotFoundException(ConfigurationPropertyNames.LoggingFileNameTemplate);

        if (string.IsNullOrEmpty(maxLoggingFileSizeStr))
            throw new SettingsPropertyNotFoundException(ConfigurationPropertyNames.MaxLoggingFileSize);

        if (!long.TryParse(maxLoggingFileSizeStr, out long maxLoggingFileSize))
            throw new FormatException(ConfigurationPropertyNames.MaxLoggingFileSize);

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

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IJsonSerializerOptionsProvider, JsonSerializerOptionsProvider>();
        builder.Services.AddScoped<IRabbitMqConsumer, RabbitMqConsumer>();

        builder.Services.AddHostedService<RequestsBackgroundController>();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyNames.Default, policy =>
            {
                policy.WithOrigins
                (
                    "https://localhost"
                );
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
                    "api-server"
                );
            });
        });

        string? defaultConnectionString = builder.Configuration.GetConnectionString(ConfigurationPropertyNames.DefaultConnectionString);

        if (string.IsNullOrEmpty(defaultConnectionString))
            throw new SettingsPropertyNotFoundException($"ConnectionStrings: {ConfigurationPropertyNames.DefaultConnectionString}");

        builder.Services.AddDbContext<DreamcoreHorrorGameStatisticsContext>
        (
            optionsAction: options => options.UseNpgsql(defaultConnectionString),
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient
        );

        return builder;
    }
}
