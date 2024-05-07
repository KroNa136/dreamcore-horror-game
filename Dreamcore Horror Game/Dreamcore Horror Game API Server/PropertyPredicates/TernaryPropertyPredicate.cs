namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record TernaryPropertyPredicate(string Property, string Operator, object? FirstValue, object? SecondValue)
    : PropertyPredicate(Property, Operator);
