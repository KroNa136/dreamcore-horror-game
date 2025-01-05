using System.ComponentModel.DataAnnotations.Schema;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class XpLevel : IDatabaseEntity, IEquatable<XpLevel>
{
    [NotMapped]
    public string DisplayName => Number.ToString();
    [NotMapped]
    public static string DatabaseTableName => "xp_levels";

    public bool Equals(XpLevel? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && Number.Equals(other.Number)
            && RequiredXp.Equals(other.RequiredXp);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as XpLevel);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
