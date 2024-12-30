using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class GameMode
{
    public Guid Id { get; set; }

    public string AssetName { get; set; } = null!;

    public short? MaxPlayers { get; set; }

    public TimeOnly? TimeLimit { get; set; }

    public bool IsActive { get; set; }

    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}
