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

namespace DreamcoreHorrorGameApiServer;

public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine($"TOKEN:\n{new TokenService().CreateAccessToken("test", AuthenticationRoles.Player)}\n");

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            var defaultOptions = new JsonSerializerOptionsProvider().Default;

            options.JsonSerializerOptions.AllowTrailingCommas = defaultOptions.AllowTrailingCommas;

            if (defaultOptions.Converters is not null && defaultOptions.Converters.IsNotEmpty())
                foreach (var converter in defaultOptions.Converters)
                    options.JsonSerializerOptions.Converters.Add(converter);

            options.JsonSerializerOptions.DefaultBufferSize = defaultOptions.DefaultBufferSize;
            options.JsonSerializerOptions.DefaultIgnoreCondition = defaultOptions.DefaultIgnoreCondition;

            if (defaultOptions.DictionaryKeyPolicy is not null)
                options.JsonSerializerOptions.DictionaryKeyPolicy = defaultOptions.DictionaryKeyPolicy;

            if (defaultOptions.Encoder is not null)
                options.JsonSerializerOptions.Encoder = defaultOptions.Encoder;

            options.JsonSerializerOptions.IgnoreReadOnlyFields = defaultOptions.IgnoreReadOnlyFields;
            options.JsonSerializerOptions.IgnoreReadOnlyProperties = defaultOptions.IgnoreReadOnlyProperties;
            options.JsonSerializerOptions.IncludeFields = defaultOptions.IncludeFields;
            options.JsonSerializerOptions.MaxDepth = defaultOptions.MaxDepth;
            options.JsonSerializerOptions.NumberHandling = defaultOptions.NumberHandling;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = defaultOptions.PropertyNameCaseInsensitive;

            if (defaultOptions.PropertyNamingPolicy is not null)
                options.JsonSerializerOptions.PropertyNamingPolicy = defaultOptions.PropertyNamingPolicy;

            options.JsonSerializerOptions.ReadCommentHandling = defaultOptions.ReadCommentHandling;

            if (defaultOptions.ReferenceHandler is not null)
                options.JsonSerializerOptions.ReferenceHandler = defaultOptions.ReferenceHandler;

            if (defaultOptions.TypeInfoResolver is not null)
                options.JsonSerializerOptions.TypeInfoResolver = defaultOptions.TypeInfoResolver;

            options.JsonSerializerOptions.UnknownTypeHandling = defaultOptions.UnknownTypeHandling;
            options.JsonSerializerOptions.WriteIndented = defaultOptions.WriteIndented;
        });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyNames.Default, policy =>
            {
                policy.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .WithHeaders(
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

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(AuthenticationSchemes.Access, options =>
            {
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

        string? dbConnectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        if (dbConnectionString is not null)
            builder.Services.AddDbContext<DreamcoreHorrorGameContext>(options => options.UseNpgsql(dbConnectionString));

        builder.Services.AddSingleton<IJsonSerializerOptionsProvider, JsonSerializerOptionsProvider>();
        builder.Services.AddSingleton<ITokenService, TokenService>();

        builder.Services.AddScoped<IHttpFetcher, HttpFetcher>();

        builder.Services.AddScoped<IPasswordHasher<Developer>, PasswordHasher<Developer>>();
        builder.Services.AddScoped<IPasswordHasher<Player>, PasswordHasher<Player>>();
        builder.Services.AddScoped<IPasswordHasher<Server>, PasswordHasher<Server>>();

        WebApplication app = builder.Build();

        app.UseHttpsRedirection();

        app.UseCors(CorsPolicyNames.Default);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
