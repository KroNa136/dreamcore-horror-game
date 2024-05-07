namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public record UnaryPropertyPredicate(string Property, string Operator)
    : PropertyPredicate(Property, Operator);
