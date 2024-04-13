namespace DreamcoreHorrorGameApiServer.ConstantValues;

public static class ErrorMessages
{
    public const string EntitySetIsNull = "Requested entity set is null.";

    public const string IdMismatch = "Received parameter \"id\" does not match the \"id\" value of the received object.";

    public const string InvalidModelData = "An error occured while binding the received data to the database model.";

    public const string HeaderMissing = "An additional HTTP header is required to access this resource.";

    public const string PlayerAlreadyExists = "A player with the same email address already exists.";

    public const string DeveloperAlreadyExists = "A developer with the same login already exists.";

    public const string ServerAlreadyExists = "A server with the same IP address already exists.";

    public const string UnacceptableParameterValue = "One or more parameters of the request have unacceptable values.";
}
