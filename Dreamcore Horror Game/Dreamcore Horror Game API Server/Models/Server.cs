using System;
using System.Collections.Generic;
using System.Net;

namespace Dreamcore_Horror_Game_API_Server.Models;

public partial class Server
{
    public Guid Id { get; set; }

    public IPAddress IpAddress { get; set; } = null!;

    public short PlayerCapacity { get; set; }

    public bool IsOnline { get; set; }

    public virtual ICollection<GameSession> GameSessions { get; set; } = new List<GameSession>();
}
