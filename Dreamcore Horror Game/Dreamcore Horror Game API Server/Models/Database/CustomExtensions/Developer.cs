using System.ComponentModel.DataAnnotations.Schema;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Developer : IDatabaseEntity, IUser, IEquatable<Developer>
{
    [NotMapped]
    public string DisplayName => Login;
    [NotMapped]
    public static string DatabaseTableName => "developers";
    [NotMapped]
    public string Role => DeveloperAccessLevel?.Name ?? string.Empty;

    public bool Equals(Developer? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && Login.Equals(other.Login)
            && Password.Equals(other.Password)
            && (RefreshToken is null && other.RefreshToken is null || RefreshToken is not null && RefreshToken.Equals(other.RefreshToken))
            && DeveloperAccessLevelId.Equals(other.DeveloperAccessLevelId)
            && IsOnline.Equals(other.IsOnline);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Developer);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
