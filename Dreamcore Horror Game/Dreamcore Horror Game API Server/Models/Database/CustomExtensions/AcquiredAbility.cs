using System.ComponentModel.DataAnnotations.Schema;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class AcquiredAbility : IDatabaseEntity, IEquatable<AcquiredAbility>
{
    [NotMapped]
    public string DisplayName => $"{Ability?.DisplayName ?? "NULL"} у игрока {Player?.DisplayName ?? "NULL"}";
    [NotMapped]
    public static string DatabaseTableName => "acquired_abilities";

    public bool Equals(AcquiredAbility? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && PlayerId.Equals(other.PlayerId)
            && AbilityId.Equals(other.AbilityId)
            && AcquirementTimestamp.Equals(other.AcquirementTimestamp);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as AcquiredAbility);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
