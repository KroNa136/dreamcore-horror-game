using DreamcoreHorrorGameApiServer.Models.PropertyPredicates;

namespace DreamcoreHorrorGameApiServer.Services;

public interface IPropertyPredicateValidator
{
    public PropertyPredicateOperator? GetOperator(string name);
    public bool ValidatePropertyPredicateCollection(IEnumerable<PropertyPredicate> propertyPredicateCollection, Type predicateTargetType, out string errorMessage);
    public object? GetValidValue(object value, Type propertyType, bool ignoreTypes);
}
