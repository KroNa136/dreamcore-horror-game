using System;
using System.Collections.Generic;

namespace Dreamcore_Horror_Game_API_Server;

public partial class Player
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public DateTime RegistrationTimestamp { get; set; }

    public bool CollectOptionalData { get; set; }

    public bool IsOnline { get; set; }

    public Guid ExperienceLevelId { get; set; }

    public short ExperiencePoints { get; set; }

    public short AbilityPoints { get; set; }

    public short SpiritEnergyPoints { get; set; }

    public virtual ICollection<AcquiredAbility> AcquiredAbilities { get; set; } = new List<AcquiredAbility>();

    public virtual ICollection<CollectedArtifact> CollectedArtifacts { get; set; } = new List<CollectedArtifact>();

    public virtual ExperienceLevel ExperienceLevel { get; set; } = null!;

    public virtual ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();
}
