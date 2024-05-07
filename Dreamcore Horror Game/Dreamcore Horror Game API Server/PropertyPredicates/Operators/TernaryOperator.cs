namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record TernaryOperator(string Name, bool IgnoreTypes, Func<object?, object?, object?, bool> Operation)
    : PropertyPredicateOperator(Name, IgnoreTypes);
