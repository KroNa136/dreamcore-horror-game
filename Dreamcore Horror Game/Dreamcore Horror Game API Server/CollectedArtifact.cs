using System;
using System.Collections.Generic;

namespace Dreamcore_Horror_Game_API_Server;

public partial class CollectedArtifact
{
    public Guid Id { get; set; }

    public Guid PlayerId { get; set; }

    public Guid ArtifactId { get; set; }

    public DateTime CollectionTimestamp { get; set; }

    public virtual Artifact Artifact { get; set; } = null!;

    public virtual Player Player { get; set; } = null!;
}
