namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record BinaryOperator(string Name, bool IgnoreTypes, Func<object?, object?, bool> Operation)
    : PropertyPredicateOperator(Name, IgnoreTypes);
