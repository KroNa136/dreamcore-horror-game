using DreamcoreHorrorGameApiServer.ConstantValues;
using System.ComponentModel.DataAnnotations.Schema;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Player : IDatabaseEntity, IUser, IEquatable<Player>
{
    [NotMapped]
    public string Login => Email;
    [NotMapped]
    public string Role => AuthenticationRoles.Player;

    public bool Equals(Player? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && Username.Equals(other.Username)
            && Email.Equals(other.Email)
            && Password.Equals(other.Password)
            && (RefreshToken is null && other.RefreshToken is null || RefreshToken is not null && RefreshToken.Equals(other.RefreshToken))
            && RegistrationTimestamp.Equals(other.RegistrationTimestamp)
            && CollectOptionalData.Equals(other.CollectOptionalData)
            && IsOnline.Equals(other.IsOnline)
            && XpLevelId.Equals(other.XpLevelId)
            && Xp.Equals(other.Xp)
            && AbilityPoints.Equals(other.AbilityPoints)
            && SpiritEnergyPoints.Equals(other.SpiritEnergyPoints);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Player);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
