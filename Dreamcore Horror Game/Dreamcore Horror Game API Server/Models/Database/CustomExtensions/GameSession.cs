namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class GameSession : IDatabaseEntity, IEquatable<GameSession>
{
    public bool Equals(GameSession? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && ServerId.Equals(other.ServerId)
            && GameModeId.Equals(other.GameModeId)
            && StartTimestamp.Equals(other.StartTimestamp)
            && EndTimestamp.Equals(other.EndTimestamp);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as GameSession);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
