namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class AcquiredAbility : IDatabaseEntity, IEquatable<AcquiredAbility>
{
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
