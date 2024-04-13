using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.ConstantValues.TokenOptions;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.Models.Database;
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
        Console.WriteLine($"TOKEN:\n{TokenService.CreateAccessToken("test", AuthenticationRoles.FullAccessDeveloper)}\n");

        WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

        builder.Services.AddControllers().AddJsonOptions(options =>
        {
            var sharedOptions = JsonSerializerOptionsProvider.Shared;

            options.JsonSerializerOptions.AllowTrailingCommas = sharedOptions.AllowTrailingCommas;

            if (sharedOptions.Converters is not null && sharedOptions.Converters.IsNotEmpty())
                foreach (var converter in sharedOptions.Converters)
                    options.JsonSerializerOptions.Converters.Add(converter);

            options.JsonSerializerOptions.DefaultBufferSize = sharedOptions.DefaultBufferSize;
            options.JsonSerializerOptions.DefaultIgnoreCondition = sharedOptions.DefaultIgnoreCondition;

            if (sharedOptions.DictionaryKeyPolicy is not null)
                options.JsonSerializerOptions.DictionaryKeyPolicy = sharedOptions.DictionaryKeyPolicy;

            if (sharedOptions.Encoder is not null)
                options.JsonSerializerOptions.Encoder = sharedOptions.Encoder;

            options.JsonSerializerOptions.IgnoreReadOnlyFields = sharedOptions.IgnoreReadOnlyFields;
            options.JsonSerializerOptions.IgnoreReadOnlyProperties = sharedOptions.IgnoreReadOnlyProperties;
            options.JsonSerializerOptions.IncludeFields = sharedOptions.IncludeFields;
            options.JsonSerializerOptions.MaxDepth = sharedOptions.MaxDepth;
            options.JsonSerializerOptions.NumberHandling = sharedOptions.NumberHandling;
            options.JsonSerializerOptions.PropertyNameCaseInsensitive = sharedOptions.PropertyNameCaseInsensitive;

            if (sharedOptions.PropertyNamingPolicy is not null)
                options.JsonSerializerOptions.PropertyNamingPolicy = sharedOptions.PropertyNamingPolicy;

            options.JsonSerializerOptions.ReadCommentHandling = sharedOptions.ReadCommentHandling;

            if (sharedOptions.ReferenceHandler is not null)
                options.JsonSerializerOptions.ReferenceHandler = sharedOptions.ReferenceHandler;

            if (sharedOptions.TypeInfoResolver is not null)
                options.JsonSerializerOptions.TypeInfoResolver = sharedOptions.TypeInfoResolver;

            options.JsonSerializerOptions.UnknownTypeHandling = sharedOptions.UnknownTypeHandling;
            options.JsonSerializerOptions.WriteIndented = sharedOptions.WriteIndented;
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

        builder.Services.AddTransient<IPasswordHasher<Developer>, PasswordHasher<Developer>>();
        builder.Services.AddTransient<IPasswordHasher<Player>, PasswordHasher<Player>>();
        builder.Services.AddTransient<IPasswordHasher<Server>, PasswordHasher<Server>>();

        WebApplication app = builder.Build();

        app.UseHttpsRedirection();

        app.UseCors(CorsPolicyNames.Default);

        app.UseAuthentication();
        app.UseAuthorization();

        app.MapControllers();

        app.Run();
    }
}
