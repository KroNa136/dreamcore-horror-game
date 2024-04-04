using System;
using System.Collections.Generic;

namespace Dreamcore_Horror_Game_API_Server.Models.Database;

public partial class Artifact
{
    public Guid Id { get; set; }

    public string AssetName { get; set; } = null!;

    public Guid RarityLevelId { get; set; }

    public virtual ICollection<CollectedArtifact> CollectedArtifacts { get; set; } = new List<CollectedArtifact>();

    public virtual RarityLevel RarityLevel { get; set; } = null!;
}
