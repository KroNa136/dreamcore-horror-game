using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DreamcoreHorrorGameApiServer.ConstantValues.TokenOptions;

public static class RefreshTokenOptions
{
    public const string Issuer = "RefreshDreamcoreHorrorGameAPIServer";

    public const string Audience = "RefreshDreamcoreHorrorGameUser";

    private const string Key = "z8Ntg7ISsrLXCHVhIOaKSfvrGoqYmZHQ5m7nBReMXnAE9lFdUB44to2aT80Xp9Ub";

    public const int LifetimeMinutes = 60 * 24 * 30;

    public static SymmetricSecurityKey SecurityKey => new(Encoding.ASCII.GetBytes(Key));
}
