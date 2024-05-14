using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class GameSession
{
    public Guid Id { get; set; }

    public Guid? ServerId { get; set; }

    public Guid GameModeId { get; set; }

    public DateTime StartTimestamp { get; set; }

    public DateTime? EndTimestamp { get; set; }

    public virtual GameMode? GameMode { get; set; }

    public virtual ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();

    public virtual Server? Server { get; set; }
}
