namespace DreamcoreHorrorGameEmailServer.Models;

public class VerificationEmailDTO
{
    public string To { get; set; } = string.Empty;
    public string VerificationToken { get; set; } = string.Empty;
}
