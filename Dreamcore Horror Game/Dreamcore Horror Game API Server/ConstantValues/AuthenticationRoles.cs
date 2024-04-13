namespace DreamcoreHorrorGameApiServer.ConstantValues;

public static class AuthenticationRoles
{
    public const string Server = "Server";

    public const string Player = "Player";
    public const string PlayerOrServer = "Player,Server";

    public const string LowAccessDeveloper = "Low Access Developer";
    public const string LowAccessDeveloperOrPlayer = "Low Access Developer,Player";
    public const string LowAccessDeveloperOrServer = "Low Access Developer,Server";
    public const string LowAccessDeveloperOrPlayerOrServer = "Low Access Developer,Player,Server";

    public const string MediumAccessDeveloper = "Medium Access Developer";
    public const string MediumAccessDeveloperOrPlayer = "Medium Access Developer,Player";
    public const string MediumAccessDeveloperOrServer = "Medium Access Developer,Server";
    public const string MediumAccessDeveloperOrPlayerOrServer = "Medium Access Developer,Player,Server";

    public const string FullAccessDeveloper = "Full Access Developer";
    public const string FullAccessDeveloperOrPlayer = "Full Access Developer,Player";
    public const string FullAccessDeveloperOrServer = "Full Access Developer,Server";
    public const string FullAccessDeveloperOrPlayerOrServer = "Full Access Developer,Player,Server";

    public const string LowOrMediumAccessDeveloper = "Low Access Developer,Medium Access Developer";
    public const string LowOrMediumAccessDeveloperOrPlayer = "Low Access Developer,Medium Access Developer,Player";
    public const string LowOrMediumAccessDeveloperOrServer = "Low Access Developer,Medium Access Developer,Server";
    public const string LowOrMediumAccessDeveloperOrPlayerOrServer = "Low Access Developer,Medium Access Developer,Player,Server";

    public const string LowOrFullAccessDeveloper = "Low Access Developer,Full Access Developer";
    public const string LowOrFullAccessDeveloperOrPlayer = "Low Access Developer,Full Access Developer,Player";
    public const string LowOrFullAccessDeveloperOrServer = "Low Access Developer,Full Access Developer,Server";
    public const string LowOrFullAccessDeveloperOrPlayerOrServer = "Low Access Developer,Full Access Developer,Player,Server";

    public const string MediumOrFullAccessDeveloper = "Medium Access Developer,Full Access Developer";
    public const string MediumOrFullAccessDeveloperOrPlayer = "Medium Access Developer,Full Access Developer,Player";
    public const string MediumOrFullAccessDeveloperOrServer = "Medium Access Developer,Full Access Developer,Server";
    public const string MediumOrFullAccessDeveloperOrPlayerOrServer = "Medium Access Developer,Full Access Developer,Player,Server";

    public const string Developer = "Low Access Developer,Medium Access Developer,Full Access Developer";
    public const string DeveloperOrPlayer = "Low Access Developer,Medium Access Developer,Full Access Developer,Player";
    public const string DeveloperOrServer = "Low Access Developer,Medium Access Developer,Full Access Developer,Server";
    public const string DeveloperOrPlayerOrServer = "Low Access Developer,Medium Access Developer,Full Access Developer,Player,Server";
}
