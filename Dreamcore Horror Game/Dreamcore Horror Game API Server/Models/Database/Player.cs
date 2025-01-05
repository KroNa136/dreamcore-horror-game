﻿using System;
using System.Collections.Generic;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Player
{
    public Guid Id { get; set; }

    public string Username { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Password { get; set; } = null!;

    public string? RefreshToken { get; set; }

    public DateTime RegistrationTimestamp { get; set; }

    public bool CollectOptionalData { get; set; }

    public bool IsOnline { get; set; }

    public Guid XpLevelId { get; set; }

    public short Xp { get; set; }

    public short AbilityPoints { get; set; }

    public short SpiritEnergyPoints { get; set; }

    public bool EmailVerified { get; set; }

    public Guid? EmailVerificationTokenId { get; set; }

    public virtual ICollection<AcquiredAbility> AcquiredAbilities { get; set; } = new List<AcquiredAbility>();

    public virtual ICollection<CollectedArtifact> CollectedArtifacts { get; set; } = new List<CollectedArtifact>();

    public virtual EmailVerificationToken? EmailVerificationToken { get; set; }

    public virtual ICollection<PlayerSession> PlayerSessions { get; set; } = new List<PlayerSession>();

    public virtual XpLevel? XpLevel { get; set; }
}
