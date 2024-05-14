using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class PlayerSession
{
    public Guid Id { get; set; }

    public Guid GameSessionId { get; set; }

    public Guid PlayerId { get; set; }

    public DateTime StartTimestamp { get; set; }

    public DateTime? EndTimestamp { get; set; }

    public bool? IsCompleted { get; set; }

    public bool? IsWon { get; set; }

    public TimeOnly? TimeAlive { get; set; }

    public bool? PlayedAsCreature { get; set; }

    public Guid? UsedCreatureId { get; set; }

    public short? SelfReviveCount { get; set; }

    public short? AllyReviveCount { get; set; }

    public virtual GameSession? GameSession { get; set; }

    public virtual Player? Player { get; set; }

    public virtual Creature? UsedCreature { get; set; }
}
