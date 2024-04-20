namespace DreamcoreHorrorGameApiServer.Models.PropertyPredicates;

public class NullValueResult : PropertyPredicateValidationResult
{
    public override string Message => "One of the predicates contained a null value.";
}
