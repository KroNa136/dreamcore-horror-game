namespace DreamcoreHorrorGameApiServer.Models.PropertyPredicates;

public class InvalidValueTypeResult : PropertyPredicateValidationResult
{
    public override string Message => "One of the predicates had a value of invalid type.";
}
