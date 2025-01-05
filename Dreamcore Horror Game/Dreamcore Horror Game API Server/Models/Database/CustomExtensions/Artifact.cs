using System.ComponentModel.DataAnnotations.Schema;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Artifact : IDatabaseEntity, IEquatable<Artifact>
{
    [NotMapped]
    public string DisplayName => AssetName;
    [NotMapped]
    public static string DatabaseTableName => "artifacts";

    public bool Equals(Artifact? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && AssetName.Equals(other.AssetName)
            && RarityLevelId.Equals(other.RarityLevelId);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as Artifact);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
