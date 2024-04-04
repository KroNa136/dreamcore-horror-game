using System;
using System.Collections.Generic;

namespace Dreamcore_Horror_Game_API_Server.Models.Database;

public partial class XpLevel
{
    public Guid Id { get; set; }

    public short Number { get; set; }

    public short RequiredXp { get; set; }

    public virtual ICollection<Creature> Creatures { get; set; } = new List<Creature>();

    public virtual ICollection<Player> Players { get; set; } = new List<Player>();
}
