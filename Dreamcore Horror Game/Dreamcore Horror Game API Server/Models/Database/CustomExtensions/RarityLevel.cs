namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class RarityLevel : IDatabaseEntity, IEquatable<RarityLevel>
{
    public bool Equals(RarityLevel? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && AssetName.Equals(other.AssetName)
            && Probability.Equals(other.Probability);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as RarityLevel);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
