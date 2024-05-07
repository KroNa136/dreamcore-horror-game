namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record UnknownOperatorResult()
    : PropertyPredicateValidationResult("One of the predicates contained an unknown operator.");
