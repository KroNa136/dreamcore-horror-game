namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record UnaryOperator(string Name, bool IgnoreTypes, Func<object?, bool> Operation)
    : PropertyPredicateOperator(Name, IgnoreTypes);
