namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Creature : IDatabaseEntity, IEquatable<Creature>
{
    public bool Equals(Creature? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && AssetName.Equals(other.AssetName)
            && RequiredXpLevelId.Equals(other.RequiredXpLevelId)
            && Health.Equals(other.Health)
            && MovementSpeed.Equals(other.MovementSpeed);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Creature);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
