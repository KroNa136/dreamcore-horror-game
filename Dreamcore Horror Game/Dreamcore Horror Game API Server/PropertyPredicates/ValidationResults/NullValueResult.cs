namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record NullValueResult()
    : PropertyPredicateValidationResult("One of the predicates contained a null value.");
