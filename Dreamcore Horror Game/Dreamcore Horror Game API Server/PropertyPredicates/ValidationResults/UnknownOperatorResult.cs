namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public class UnknownOperatorResult : PropertyPredicateValidationResult
{
    public override string Message => "One of the predicates contained an unknown operator.";
}
