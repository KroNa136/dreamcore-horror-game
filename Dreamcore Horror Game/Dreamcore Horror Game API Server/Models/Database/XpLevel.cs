using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class XpLevel
{
    public Guid Id { get; set; }

    public short Number { get; set; }

    public short RequiredXp { get; set; }

    public virtual ICollection<Creature> Creatures { get; set; } = new List<Creature>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
