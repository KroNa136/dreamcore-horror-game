using System.ComponentModel.DataAnnotations.Schema;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class EmailVerificationToken
{
    [NotMapped]
    public string DisplayName => Token;
    [NotMapped]
    public static string DatabaseTableName => "email_verification_tokens";

    public bool Equals(EmailVerificationToken? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && Token.Equals(other.Token)
            && ExpirationTimestamp.Equals(other.ExpirationTimestamp);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as EmailVerificationToken);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
