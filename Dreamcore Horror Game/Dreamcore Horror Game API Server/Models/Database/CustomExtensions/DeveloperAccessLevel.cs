using System.ComponentModel.DataAnnotations.Schema;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class DeveloperAccessLevel : IDatabaseEntity, IEquatable<DeveloperAccessLevel>
{
    [NotMapped]
    public string DisplayName => Name;
    [NotMapped]
    public static string DatabaseTableName => "developer_access_levels";

    public bool Equals(DeveloperAccessLevel? other)
    {
        if (other is null)
            return false;

        return Id.Equals(other.Id)
            && Name.Equals(other.Name);
    }

    public override bool Equals(object? obj)
    {
        return Equals(obj as DeveloperAccessLevel);
    }

    public override int GetHashCode()
    {
        throw new NotImplementedException();
    }
}
