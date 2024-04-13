using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DreamcoreHorrorGameApiServer.ConstantValues.TokenOptions;

public static class AccessTokenOptions
{
    public const string Issuer = "AccessDreamcoreHorrorGameAPIServer";

    public const string Audience = "AccessDreamcoreHorrorGameUser";

    private const string Key = "project_eaglex_9bk24v6qx9rv9tr8N4qSU";

    public const int LifetimeMinutes = 5;

    public static SymmetricSecurityKey SecurityKey => new(Encoding.ASCII.GetBytes(Key));
}
