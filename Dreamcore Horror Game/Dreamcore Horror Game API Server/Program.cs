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
            var defaultJsonSerializerOptions = new JsonSerializerOptionsProvider().Default;
            options.ConfigureFrom(defaultJsonSerializerOptions);
        });

        builder.Services.AddEndpointsApiExplorer();

        builder.Services.AddCors(options =>
        {
            options.AddPolicy(CorsPolicyNames.Default, policy =>
            {
                policy.AllowAnyOrigin();
                policy.AllowAnyMethod();
                policy.WithHeaders(
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
