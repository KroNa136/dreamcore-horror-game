using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DreamcoreHorrorGameApiServer.ConstantValues.TokenOptions;

public static class RefreshTokenOptions
{
    public const string Issuer = "RefreshDreamcoreHorrorGameAPIServer";

    public const string Audience = "RefreshDreamcoreHorrorGameUser";

    private const string Key = "project_eaglex_9bk24v6qx9rv9tr8N4qSU";

    public const int LifetimeMinutes = 60 * 24 * 30;

    public static SymmetricSecurityKey SecurityKey => new(Encoding.ASCII.GetBytes(Key));
}
