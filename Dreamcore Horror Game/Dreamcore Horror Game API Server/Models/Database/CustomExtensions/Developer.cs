using DreamcoreHorrorGameApiServer.Models.Database.Interfaces;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Developer : IDatabaseEntity, IUser
{
    public string Role => DeveloperAccessLevel?.Name ?? string.Empty;
}
