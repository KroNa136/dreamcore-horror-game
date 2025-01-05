using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class EmailVerificationToken
{
    public Guid Id { get; set; }

    public string Token { get; set; } = null!;

    public DateTime ExpirationTimestamp { get; set; }

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
