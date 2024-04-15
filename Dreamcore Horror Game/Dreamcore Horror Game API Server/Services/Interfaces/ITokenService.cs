namespace DreamcoreHorrorGameApiServer.Services;

public interface ITokenService
{
    public string CreateAccessToken(string login, string role);
    public string CreateRefreshToken(string login, string role);
}
