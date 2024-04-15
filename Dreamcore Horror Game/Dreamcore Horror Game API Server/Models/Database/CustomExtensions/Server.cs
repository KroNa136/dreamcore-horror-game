using DreamcoreHorrorGameApiServer.ConstantValues;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Server : IDatabaseEntity, IUser
{
    public string Login => IpAddress.ToString();
    public string Role => AuthenticationRoles.Server;
}
