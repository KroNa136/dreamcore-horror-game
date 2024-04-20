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
        string testToken = new TokenService()
            .CreateAccessToken("test_login", AuthenticationRoles.FullAccessDeveloper);

        Console.WriteLine($"TOKEN:\n{testToken}\n");

        WebApplicationBuilder builder = CreateWebApplicationBuilder(args);
        WebApplication app = builder.CreateWebApplication();

        app.Run();
    }

    private static WebApplicationBuilder CreateWebApplicationBuilder(string[] args)
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.AddControllersWithJsonOptions()
            .AddSwaggerGenerator()
            .AddEndpointsApiExplorer()
            .AddProjectDependencies()
            .AddCorsPolicies()
            .AddAuthentication();

        try
        {
            builder.AddDatabaseContext();
        }
        catch (SettingsPropertyNotFoundException)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Database connection string not found in the configuration.");
            Environment.Exit(-1);
        }

        return builder;
    }
}

file static class WebApplicationBuilderExtensions
{
    internal static WebApplicationBuilder AddControllersWithJsonOptions(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            var defaultJsonSerializerOptions = new JsonSerializerOptionsProvider().Default;
            options.ConfigureFrom(defaultJsonSerializerOptions);
        });

        return builder;
    }

    internal static WebApplicationBuilder AddSwaggerGenerator(this WebApplicationBuilder builder)
    {
        builder.Services.AddSwaggerGen();
        return builder;
    }

    internal static WebApplicationBuilder AddEndpointsApiExplorer(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        return builder;
    }

    internal static WebApplicationBuilder AddProjectDependencies(this WebApplicationBuilder builder)
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

    internal static WebApplicationBuilder AddCorsPolicies(this WebApplicationBuilder builder)
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

    internal static WebApplicationBuilder AddAuthentication(this WebApplicationBuilder builder)
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

    internal static WebApplicationBuilder AddDatabaseContext(this WebApplicationBuilder builder)
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

    internal static WebApplication CreateWebApplication(this WebApplicationBuilder builder)
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
}
