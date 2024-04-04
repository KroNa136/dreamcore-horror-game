using System;
using System.Collections.Generic;

namespace Dreamcore_Horror_Game_API_Server.Models.Database;

public partial class GameSession
{
    public Guid Id { get; set; }

    public Guid? ServerId { get; set; }

    public Guid GameModeId { get; set; }

    public DateTime StartTimestamp { get; set; }

    public DateTime? EndTimestamp { get; set; }

    public virtual GameMode GameMode { get; set; } = null!;

    public virtual ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();

    public virtual Server? Server { get; set; }
}
