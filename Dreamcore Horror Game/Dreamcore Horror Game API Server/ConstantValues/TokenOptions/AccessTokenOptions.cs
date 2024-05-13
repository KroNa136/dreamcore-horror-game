using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace DreamcoreHorrorGameApiServer.ConstantValues.TokenOptions;

public static class AccessTokenOptions
{
    public const string Issuer = "AccessDreamcoreHorrorGameAPIServer";

    public const string Audience = "AccessDreamcoreHorrorGameUser";

    private const string Key = "exj7v0YETeNGkep43NaaYRI00Hrqx4yVTFDwoMLBLqtHQpZbvgu60GtZzCEHf18Y";

    public const int LifetimeMinutes = 1;

    public static SymmetricSecurityKey SecurityKey => new(Encoding.ASCII.GetBytes(Key));
}
