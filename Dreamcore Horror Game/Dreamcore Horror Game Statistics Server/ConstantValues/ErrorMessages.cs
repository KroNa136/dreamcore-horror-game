namespace DreamcoreHorrorGameStatisticsServer.ConstantValues;

public static class ErrorMessages
{
    public const string IdMismatch = "Received parameter 'id' does not match the 'id' value of the received object.";

    public const string InvalidModelData = "An error occured while binding the received data to the database model.";

    public const string RelatedEntityDoesNotExist = "Related entity with requested ID does not exist.";

    public const string CreateConflict = "Couldn't create the requested entity due to some conflict in the database. Please try again later.";

    public const string EditConflict = "Couldn't edit the requested entity due to some conflict in the database. Please try again later.";

    public const string DeleteConflict = "Couldn't delete the requested entity due to some conflict in the database. Please try again later.";

    public const string DeleteConstraintViolation = "Couldn't delete the requested entity due to the database constraints.";

    public const string UnknownDatabaseError = "Couldn't process the request due to some unknown error in the database. Please try again later.";

    public const string UnacceptableParameterValue = "One or more parameters of the request have unacceptable values.";

    public const string UnknownServerError = "Couldn't process the request due to some unknown server error. Please try again later.";
}
