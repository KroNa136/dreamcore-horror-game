namespace DreamcoreHorrorGameApiServer.Models.Database;

public interface IUser
{
    public string Login { get; }
    public string Password { get; set; }
    public string? RefreshToken { get; set; }
    public string Role { get; }
    public bool IsOnline { get; set; }
}
