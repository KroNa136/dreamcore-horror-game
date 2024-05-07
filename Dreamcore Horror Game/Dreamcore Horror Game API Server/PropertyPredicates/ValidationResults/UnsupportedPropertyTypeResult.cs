namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record UnsupportedPropertyTypeResult()
    : PropertyPredicateValidationResult("One of the predicates contained a property of unsupported type.");
