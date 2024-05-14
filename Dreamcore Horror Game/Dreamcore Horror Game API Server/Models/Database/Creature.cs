using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Creature
{
    public Guid Id { get; set; }

    public string AssetName { get; set; } = null!;

    public Guid RequiredXpLevelId { get; set; }

    public float Health { get; set; }

    public float MovementSpeed { get; set; }

    public virtual ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();

    public virtual XpLevel? RequiredXpLevel { get; set; }
}
