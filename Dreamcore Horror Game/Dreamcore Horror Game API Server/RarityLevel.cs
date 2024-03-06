using System;
using System.Collections.Generic;

namespace Dreamcore_Horror_Game_API_Server;

public partial class RarityLevel
{
    public Guid Id { get; set; }

    public string AssetName { get; set; } = null!;

    public float Probability { get; set; }

    public virtual ICollection<Artifact> Artifacts { get; set; } = new List<Artifact>();
}
