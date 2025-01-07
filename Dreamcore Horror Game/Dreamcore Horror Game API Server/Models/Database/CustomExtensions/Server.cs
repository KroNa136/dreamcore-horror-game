using DreamcoreHorrorGameApiServer.ConstantValues;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Server : IDatabaseEntity, IUser, IEquatable<Server>
{
    [NotMapped]
    public string Login => IpAddress.ToString();
    [NotMapped]
    public string Role => AuthenticationRoles.Server;

    public bool Equals(Server? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && IpAddress.Equals(other.IpAddress)
            && Password.Equals(other.Password)
            && (RefreshToken is null && other.RefreshToken is null || RefreshToken is not null && RefreshToken.Equals(other.RefreshToken))
            && PlayerCapacity.Equals(other.PlayerCapacity)
            && IsOnline.Equals(other.IsOnline);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Server);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
