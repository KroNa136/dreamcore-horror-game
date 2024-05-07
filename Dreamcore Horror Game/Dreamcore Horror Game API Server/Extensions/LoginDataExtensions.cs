using DreamcoreHorrorGameApiServer.Models;

namespace DreamcoreHorrorGameApiServer.Extensions;

public static class LoginDataExtensions
{
    public static bool IsEmptyLogin(this LoginData loginData)
        => string.IsNullOrEmpty(loginData.Login);

    public static bool IsNotEmptyLogin(this LoginData loginData)
        => string.IsNullOrEmpty(loginData.Login) is false;

    public static bool IsEmptyPassword(this LoginData loginData)
        => string.IsNullOrEmpty(loginData.Password);

    public static bool IsNotEmptyPassword(this LoginData loginData)
        => string.IsNullOrEmpty(loginData.Password) is false;
}
