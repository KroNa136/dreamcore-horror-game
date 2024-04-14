using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Models.Database.Interfaces;

namespace DreamcoreHorrorGameApiServer.Models.Database;

public partial class Player : IDatabaseEntity, IUser
{
    public string Login => Email;
    public string Role => AuthenticationRoles.Player;
}
