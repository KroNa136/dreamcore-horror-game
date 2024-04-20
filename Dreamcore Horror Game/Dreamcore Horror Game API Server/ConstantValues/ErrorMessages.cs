namespace DreamcoreHorrorGameApiServer.ConstantValues;

public static class ErrorMessages
{
    // This message is misleading on purpose: to protect the app from potential request forgery attempts.
    // The required cors header value is a randomly generated 64-character string which resembles an ID.
    // An attacker might think that any client application must send its own ID with each request, and that
    // faking a request is impossible due to these IDs being validated here on the backend side.
    public const string CorsHeaderMissing = "Request sender ID is required to access this resource.";

    public const string IdMismatch = "Received parameter 'id' does not match the 'id' value of the received object.";

    public const string InvalidModelData = "An error occured while binding the received data to the database model.";

    public const string RelatedEntityDoesNotExist = "Related entity with requested ID does not exist.";

    public const string CreateConflict = "Couldn't create the requested entity due to some conflict in the database. Please try again later.";

    public const string PlayerRegisterConflict = "Couldn't complete the registration process due to some conflict in the database. Please try again later.";

    public const string EditConflict = "Couldn't edit the requested entity due to some conflict in the database. Please try again later.";

    public const string DeleteConflict = "Couldn't delete the requested entity due to some conflict in the database. Please try again later.";

    public const string DeleteConstraintViolation = "Couldn't delete the requested entity due to the database constraints.";

    public const string PlayerAlreadyExists = "A player with the same email address already exists.";

    public const string DeveloperAlreadyExists = "A developer with the same login already exists.";

    public const string ServerAlreadyExists = "A server with the same IP address already exists.";

    public const string UserIsAlreadyLoggedIn = "Requested user is already logged in.";

    public const string UserIsAlreadyLoggedOut = "Requested user is already logged out.";

    public const string UnacceptableParameterValue = "One or more parameters of the request have unacceptable values.";

    public const string EmptyPropertyInPropertyPredicate = "One of the predicates contained an empty property.";

    public const string EmptyOperatorInPropertyPredicate = "One of the predicates contained an empty operator.";

    public const string UnknownPropertyInPropertyPredicate = "One of the predicates contained an unknown property.";
}
