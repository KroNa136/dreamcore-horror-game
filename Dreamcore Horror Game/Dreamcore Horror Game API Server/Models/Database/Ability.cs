using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Ability
{
    public Guid Id { get; set; }

    public string AssetName { get; set; } = null!;

    public virtual ICollection<AcquiredAbility> AcquiredAbilities { get; set; } = new List<AcquiredAbility>();
}
