using System;
using System.Collections.Generic;
using System.Net;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Server
{
    public Guid Id { get; set; }

    public IPAddress IpAddress { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public short PlayerCapacity { get; set; }

    public bool IsOnline { get; set; }

    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}
