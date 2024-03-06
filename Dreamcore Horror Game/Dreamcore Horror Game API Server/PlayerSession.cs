using System;
using System.Collections.Generic;

namespace Dreamcore_Horror_Game_API_Server;

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

    public virtual GameSession GameSession { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;

    public virtual Creature? UsedCreature { get; set; }
}
