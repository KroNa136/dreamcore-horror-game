namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public class UnknownPredicateTypeResult : PropertyPredicateValidationResult
{
    public override string Message => "One of the predicates was of unknown type.";
}
