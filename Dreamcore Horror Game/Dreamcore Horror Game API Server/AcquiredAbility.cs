using System;
using System.Collections.Generic;

namespace Dreamcore_Horror_Game_API_Server;

public partial class AcquiredAbility
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public Guid AbilityId { get; set; }

    public DateTime AcquirementTimestamp { get; set; }

    public virtual Ability Ability { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
