using System;
using System.Collections.Generic;

namespace Dreamcore_Horror_Game_API_Server.Models;

public partial class Ability
{
    public Guid Id { get; set; }

    public string AssetName { get; set; } = null!;

    public virtual ICollection<AcquiredAbility> AcquiredAbilities { get; set; } = new List<AcquiredAbility>();
}
