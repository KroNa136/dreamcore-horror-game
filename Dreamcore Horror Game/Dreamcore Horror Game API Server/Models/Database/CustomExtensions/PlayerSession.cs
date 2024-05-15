using System.ComponentModel.DataAnnotations.Schema;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class PlayerSession : IDatabaseEntity, IEquatable<PlayerSession>
{
    [NotMapped]
    public string DisplayName => $"У игрока {Player?.DisplayName ?? "NULL"} на сервере {GameSession?.Server?.DisplayName ?? "NULL"} {StartTimestamp}";

    public bool Equals(PlayerSession? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && GameSessionId.Equals(other.GameSessionId)
            && PlayerId.Equals(other.PlayerId)
            && StartTimestamp.Equals(other.StartTimestamp)
            && EndTimestamp.Equals(other.EndTimestamp)
            && IsCompleted.Equals(other.IsCompleted)
            && IsWon.Equals(other.IsWon)
            && TimeAlive.Equals(other.TimeAlive)
            && PlayedAsCreature.Equals(other.PlayedAsCreature)
            && UsedCreatureId.Equals(other.UsedCreatureId)
            && SelfReviveCount.Equals(other.SelfReviveCount)
            && AllyReviveCount.Equals(other.AllyReviveCount);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as PlayerSession);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
