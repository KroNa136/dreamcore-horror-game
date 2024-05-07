namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record BinaryPropertyPredicate(string Property, string Operator, object? Value)
    : PropertyPredicate(Property, Operator);
