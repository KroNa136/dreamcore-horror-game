namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class GameMode : IDatabaseEntity, IEquatable<GameMode>
{
    public bool Equals(GameMode? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && AssetName.Equals(other.AssetName)
            && MaxPlayers.Equals(other.MaxPlayers)
            && TimeLimit.Equals(other.TimeLimit)
            && IsActive.Equals(other.IsActive);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as GameMode);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
