using DreamcoreHorrorGameApiServer.Models.Database;

namespace DreamcoreHorrorGameApiServer.Services;

public interface ITokenService
{
    public EmailVerificationToken CreateEmailVerificationToken();
    public string CreateAccessToken(string login, string role);
    public string CreateRefreshToken(string login, string role);
}
