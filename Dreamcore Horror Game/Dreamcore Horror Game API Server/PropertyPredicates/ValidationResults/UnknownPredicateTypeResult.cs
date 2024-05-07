namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record UnknownPredicateTypeResult()
    : PropertyPredicateValidationResult("One of the predicates was of unknown type.");
