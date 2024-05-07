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

        try
        {
            ConfiguredWebApplicationBuilder(args)
                .ConfiguredWebApplication()
                .Run();
        }
        catch (SettingsPropertyNotFoundException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Database connection string not found in the configuration.");
            Environment.Exit(-1);
        }
    }

    private static void PrintTestAccessToken() => Console.WriteLine
    (
        $"TOKEN:\n{ new TokenService().CreateAccessToken("test_login", AuthenticationRoles.FullAccessDeveloper) }\n"
    );

    private static WebApplicationBuilder ConfiguredWebApplicationBuilder(string[] args)
        => WebApplication.CreateBuilder(args)
            .WithControllersAndJsonOptions()
            .WithSwaggerGenerator()
            .WithEndpointsApiExplorer()
            .WithProjectDependencies()
            .WithCorsPolicies()
            .WithAuthentication()
            .WithDatabaseContext();
}

file static class WebApplicationBuilderExtensions
{
    public static WebApplication ConfiguredWebApplication(this WebApplicationBuilder builder)
    {
        WebApplication app = builder.Build();

        app.UseHttpsRedirection()
            .UseCors(CorsPolicyNames.Default)
            .UseAuthentication()
            .UseAuthorization();

        if (app.Environment.IsEnvironment(Environments.Development))
        {
            app.UseSwagger()
                .UseSwaggerUI();
        }

        app.MapControllers();

        return app;
    }

    public static WebApplicationBuilder WithControllersAndJsonOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            var defaultJsonSerializerOptions = new JsonSerializerOptionsProvider().Default;
            options.ConfigureFrom(defaultJsonSerializerOptions);
        });

        return builder;
    }

    public static WebApplicationBuilder WithSwaggerGenerator(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();
        return builder;
    }

    public static WebApplicationBuilder WithEndpointsApiExplorer(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        return builder;
    }

    public static WebApplicationBuilder WithProjectDependencies(this WebApplicationBuilder builder)
    {
        builder.Services
            .AddSingleton<IJsonSerializerOptionsProvider, JsonSerializerOptionsProvider>()
            .AddSingleton<ITokenService, TokenService>()
            .AddSingleton<IPropertyPredicateValidator, PropertyPredicateValidator>()
            .AddScoped<IHttpFetcher, HttpFetcher>()
            .AddScoped<IPasswordHasher<Developer>, PasswordHasher<Developer>>()
            .AddScoped<IPasswordHasher<Player>, PasswordHasher<Player>>()
            .AddScoped<IPasswordHasher<Server>, PasswordHasher<Server>>();

        return builder;
    }

    public static WebApplicationBuilder WithCorsPolicies(this WebApplicationBuilder builder)
    {
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
                    CorsHeaders.GameClient,
                    CorsHeaders.GameServer,
                    CorsHeaders.DeveloperWebApplication
                );
            });
        });

        return builder;
    }

    public static WebApplicationBuilder WithAuthentication(this WebApplicationBuilder builder)
    {
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

        return builder;
    }

    public static WebApplicationBuilder WithDatabaseContext(this WebApplicationBuilder builder)
    {
        string? defaultConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(defaultConnectionString))
            throw new SettingsPropertyNotFoundException();

        builder.Services.AddDbContext<DreamcoreHorrorGameContext>
        (
            optionsAction: options => options.UseNpgsql(defaultConnectionString),
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient
        );

        return builder;
    }
}
