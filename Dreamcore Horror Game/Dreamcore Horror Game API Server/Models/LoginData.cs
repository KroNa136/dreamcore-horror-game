namespace DreamcoreHorrorGameApiServer.Models;

public class LoginData
{
    public string? Login { get; set; }
    public string? Password { get; set; }

    public LoginData(string? login, string? password)
    {
        Login = login;
        Password = password;
    }

    public bool IsEmptyLogin => string.IsNullOrEmpty(Login);
    public bool IsNotEmptyLogin => !string.IsNullOrEmpty(Login);

    public bool IsEmptyPassword => string.IsNullOrEmpty(Password);
    public bool IsNotEmptyPassword => !string.IsNullOrEmpty(Password);
}