﻿namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class CollectedArtifact : IDatabaseEntity, IEquatable<CollectedArtifact>
{
    public bool Equals(CollectedArtifact? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && PlayerId.Equals(other.PlayerId)
            && ArtifactId.Equals(other.ArtifactId)
            && CollectionTimestamp.Equals(other.CollectionTimestamp);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as CollectedArtifact);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
