namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record InvalidValueTypeResult()
    : PropertyPredicateValidationResult("One of the predicates had a value of invalid type.");
