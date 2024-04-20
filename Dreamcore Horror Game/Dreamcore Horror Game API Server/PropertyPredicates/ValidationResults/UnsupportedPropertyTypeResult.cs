namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public class UnsupportedPropertyTypeResult : PropertyPredicateValidationResult
{
    public override string Message => "One of the predicates contained a property of unsupported type.";
}
