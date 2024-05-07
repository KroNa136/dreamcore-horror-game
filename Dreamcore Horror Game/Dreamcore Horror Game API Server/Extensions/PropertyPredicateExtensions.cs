using DreamcoreHorrorGameApiServer.PropertyPredicates;

namespace DreamcoreHorrorGameApiServer.Extensions;

public static class PropertyPredicateExtensions
{
    public static bool IsEmptyProperty(this PropertyPredicate propertyPredicate)
        => string.IsNullOrEmpty(propertyPredicate.Property);

    public static bool IsNotEmptyProperty(this PropertyPredicate propertyPredicate)
        => string.IsNullOrEmpty(propertyPredicate.Property) is false;

    public static bool IsEmptyOperator(this PropertyPredicate propertyPredicate)
        => string.IsNullOrEmpty(propertyPredicate.Operator);

    public static bool IsNotEmptyOperator(this PropertyPredicate propertyPredicate)
        => string.IsNullOrEmpty(propertyPredicate.Operator) is false;
}
