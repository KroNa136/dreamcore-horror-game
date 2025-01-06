using DreamcoreHorrorGameEmailServer.ConstantValues;
using DreamcoreHorrorGameEmailServer.Extensions;
using DreamcoreHorrorGameEmailServer.Services;
using Microsoft.Net.Http.Headers;
using System.Configuration;

namespace DreamcoreHorrorGameEmailServer;

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

        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddSingleton<IJsonSerializerOptionsProvider, JsonSerializerOptionsProvider>();
        builder.Services.AddSingleton<IRabbitMqConsumer, RabbitMqConsumer>();

        builder.Services.AddHostedService<EmailBackgroundService>();

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

        return builder;
    }
}
