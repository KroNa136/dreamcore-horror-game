﻿namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Ability : IDatabaseEntity, IEquatable<Ability>
{
    public bool Equals(Ability? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && AssetName.Equals(other.AssetName);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Ability);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
