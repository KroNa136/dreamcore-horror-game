using DreamcoreHorrorGameApiServer.ConstantValues;
using DreamcoreHorrorGameApiServer.Extensions;
using DreamcoreHorrorGameApiServer.PropertyPredicates;
using System.Collections.Immutable;
using System.Reflection;
using System.Text.Json;

namespace DreamcoreHorrorGameApiServer.Services;

public class PropertyPredicateValidator : IPropertyPredicateValidator
{
    protected readonly List<UnaryOperator> _unaryOperators = [
        new UnaryOperator
        (
            Name: "is null",
            IgnoreTypes: true,
            Operation: value => value is null
        ),
        new UnaryOperator
        (
            Name: "is not null",
            IgnoreTypes: true,
            Operation: value => value is not null
        )
    ];

    protected readonly List<BinaryOperator> _binaryOperators = [
        new BinaryOperator
        (
            Name: "equals",
            IgnoreTypes: false,
            Operation: (firstValue, secondValue)
                => firstValue is not null
                && secondValue is not null
                && firstValue.Equals(secondValue)
        ),
        new BinaryOperator
        (
            Name: "in",
            IgnoreTypes: false,
            Operation: (value, range)
                => value is not null
                && range is not null
                && (range is IEnumerable<object?> collection
                    && collection.Contains(value)
                    || value.Equals(range)
                )
        ),
        new BinaryOperator
        (
            Name: "starts with",
            IgnoreTypes: true,
            Operation: (value, start)
                => value is not null
                && start is not null
                && value.ToString() is string _value
                && start.ToString() is string _start
                && _value.StartsWith(_start)
        ),
        new BinaryOperator
        (
            Name: "starts with ignore case",
            IgnoreTypes: true,
            Operation: (value, start)
                => value is not null
                && start is not null
                && value.ToString() is string _value
                && start.ToString() is string _start
                && _value.ToLower().StartsWith(_start.ToLower())
        ),
        new BinaryOperator
        (
            Name: "contains substring",
            IgnoreTypes: true,
            Operation: (value, substring)
                => value is not null
                && substring is not null
                && value.ToString() is string _value
                && substring.ToString() is string _substring
                && _value.Contains(_substring)
        ),
        new BinaryOperator
        (
            Name: "contains substring ignore case",
            IgnoreTypes: true,
            Operation: (value, substring)
                => value is not null
                && substring is not null
                && value.ToString() is string _value
                && substring.ToString() is string _substring
                && _value.ToLower().Contains(_substring.ToLower())
        ),
        new BinaryOperator
        (
            Name: "ends with",
            IgnoreTypes: true,
            Operation: (value, end)
                => value is not null
                && end is not null
                && value.ToString() is string _value
                && end.ToString() is string _end
                && _value.EndsWith(_end)
        ),
        new BinaryOperator
        (
            Name: "ends with ignore case",
            IgnoreTypes: true,
            Operation: (value, end)
                => value is not null
                && end is not null
                && value.ToString() is string _value
                && end.ToString() is string _end
                && _value.ToLower().EndsWith(_end.ToLower())
        ),
        new BinaryOperator
        (
            Name: "less than",
            IgnoreTypes: false,
            Operation: (firstValue, secondValue)
                => firstValue is IComparable _firstValue
                && secondValue is IComparable _secondValue
                && _firstValue.CompareTo(_secondValue) is < 0
        ),
        new BinaryOperator
        (
            Name: "less than or equal to",
            IgnoreTypes: false,
            Operation: (firstValue, secondValue)
                => firstValue is IComparable _firstValue
                && secondValue is IComparable _secondValue
                && _firstValue.CompareTo(_secondValue) is <= 0
        ),
        new BinaryOperator
        (
            Name: "equal to",
            IgnoreTypes: false,
            Operation: (firstValue, secondValue)
                => firstValue is IComparable _firstValue
                && secondValue is IComparable _secondValue
                && _firstValue.CompareTo(_secondValue) is 0
        ),
        new BinaryOperator
        (
            Name: "greater than or equal to",
            IgnoreTypes: false,
            Operation: (firstValue, secondValue)
                => firstValue is IComparable _firstValue
                && secondValue is IComparable _secondValue
                && _firstValue.CompareTo(_secondValue) is >= 0
        ),
        new BinaryOperator
        (
            Name: "greater than",
            IgnoreTypes: false,
            Operation: (firstValue, secondValue)
                => firstValue is IComparable _firstValue
                && secondValue is IComparable _secondValue
                && _firstValue.CompareTo(_secondValue) is > 0
        )
    ];

    protected readonly List<TernaryOperator> _ternaryOperators = [
        new TernaryOperator
        (
            Name: "between",
            IgnoreTypes: false,
            Operation: (value, start, end)
                => value is IComparable _value
                && start is IComparable _start
                && end is IComparable _end
                && _start.CompareTo(_end) is <= 0
                && _value.CompareTo(_start) is >= 0
                && _value.CompareTo(_end) is <= 0
        ),
        new TernaryOperator
        (
            Name: "between exclusive",
            IgnoreTypes: false,
            Operation: (value, start, end)
                => value is IComparable _value
                && start is IComparable _start
                && end is IComparable _end
                && _start.CompareTo(_end) is < 0
                && _value.CompareTo(_start) is > 0
                && _value.CompareTo(_end) is < 0
        ),
        new TernaryOperator
        (
            Name: "starts with and ends with",
            IgnoreTypes: true,
            Operation: (value, start, end)
                => value is not null
                && start is not null
                && end is not null
                && value.ToString() is string _value
                && start.ToString() is string _start
                && end.ToString() is string _end
                && _value.StartsWith(_start)
                && _value.EndsWith(_end)
        ),
        new TernaryOperator
        (
            Name: "starts with and ends with ignore case",
            IgnoreTypes: true,
            Operation: (value, start, end)
                => value is not null
                && start is not null
                && end is not null
                && value.ToString() is string _value
                && start.ToString() is string _start
                && end.ToString() is string _end
                && _value.ToLower().StartsWith(_start.ToLower())
                && _value.ToLower().EndsWith(_end.ToLower())
        )
    ];

    public PropertyPredicateOperator? GetOperator(string name)
        => _unaryOperators.Find(op => op.Name.Equals(name)) as PropertyPredicateOperator
        ?? _binaryOperators.Find(op => op.Name.Equals(name)) as PropertyPredicateOperator
        ?? _ternaryOperators.Find(op => op.Name.Equals(name)) as PropertyPredicateOperator
        ?? null;

    public object GetValidValue(object value, Type propertyType, bool ignoreTypes)
    {
        if (value is JsonElement jsonElement)
            value = GetValueFromJsonElement(jsonElement);

        if (ignoreTypes)
            return value;

        return value is IEnumerable<object> collection
            ? collection.Select(item => GetValidValue(item, propertyType, false))
            : value.ParseToObjectOfType(propertyType) ?? throw new InvalidCastException();
    }

    public bool ValidatePropertyPredicateCollection
    (
        IEnumerable<PropertyPredicate> propertyPredicateCollection,
        Type predicateTargetType,
        out string errorMessage
    )
    {
        foreach (PropertyPredicate predicate in propertyPredicateCollection)
        {
            if (predicate.IsEmptyProperty())
            {
                errorMessage = ErrorMessages.EmptyPropertyInPropertyPredicate;
                return false;
            }

            if (predicate.IsEmptyOperator())
            {
                errorMessage = ErrorMessages.EmptyOperatorInPropertyPredicate;
                return false;
            }

            PropertyInfo? property = GetProperty(predicateTargetType, predicate.Property);

            if (property is null)
            {
                errorMessage = ErrorMessages.UnknownPropertyInPropertyPredicate;
                return false;
            }

            PropertyPredicateValidationResult validationResult = predicate switch
            {
                UnaryPropertyPredicate unaryPredicate => ValidatePropertyPredicate(unaryPredicate),
                BinaryPropertyPredicate binaryPredicate => ValidatePropertyPredicate(binaryPredicate, property.PropertyType),
                TernaryPropertyPredicate ternaryPredicate => ValidatePropertyPredicate(ternaryPredicate, property.PropertyType),
                _ => new UnknownPredicateTypeResult()
            };

            if (validationResult is not SuccessResult)
            {
                errorMessage = validationResult.Message;
                return false;
            }
        }

        errorMessage = string.Empty;
        return true;
    }

    private static PropertyInfo? GetProperty(Type type, string name)
        => type.GetProperties()
            .FirstOrDefault(property => property.Name.ToLower().Equals(name.ToLower().Replace("_", string.Empty)));

    private PropertyPredicateValidationResult ValidatePropertyPredicate(UnaryPropertyPredicate predicate)
    {
        var _operator = _unaryOperators.Find(op => op.Name.Equals(predicate.Operator));

        if (_operator is null)
            return new UnknownOperatorResult();

        return new SuccessResult();
    }

    private PropertyPredicateValidationResult ValidatePropertyPredicate(BinaryPropertyPredicate predicate, Type propertyType)
    {
        var _operator = _binaryOperators.Find(op => op.Name.Equals(predicate.Operator));

        if (_operator is null)
            return new UnknownOperatorResult();

        if (predicate.Value is null)
            return new NullValueResult();

        if (_operator.IgnoreTypes)
            return new SuccessResult();

        return ValidateValueType(predicate.Value, propertyType);
    }

    private PropertyPredicateValidationResult ValidatePropertyPredicate(TernaryPropertyPredicate predicate, Type propertyType)
    {
        var _operator = _ternaryOperators.Find(op => op.Name.Equals(predicate.Operator));

        if (_operator is null)
            return new UnknownOperatorResult();

        if (predicate.FirstValue is null || predicate.SecondValue is null)
            return new NullValueResult();

        if (_operator.IgnoreTypes)
            return new SuccessResult();

        var firstValueValidationResult = ValidateValueType(predicate.FirstValue, propertyType);
        var secondValueValidationResult = ValidateValueType(predicate.SecondValue, propertyType);

        if (firstValueValidationResult is not SuccessResult)
            return firstValueValidationResult;

        if (secondValueValidationResult is not SuccessResult)
            return secondValueValidationResult;

        return new SuccessResult();
    }

    private static PropertyPredicateValidationResult ValidateValueType(object value, Type type)
    {
        if (value is JsonElement jsonElement)
            value = GetValueFromJsonElement(jsonElement);

        try
        {
            if (value is IEnumerable<object> collection)
            {
                return collection.All(item => item.CanBeParsedToType(type))
                    ? new SuccessResult()
                    : new InvalidValueTypeResult();
            }

            return value.CanBeParsedToType(type)
                ? new SuccessResult()
                : new InvalidValueTypeResult();
        }
        catch (NotSupportedException)
        {
            return new UnsupportedPropertyTypeResult();
        }
    }

    private static object GetValueFromJsonElement(JsonElement jsonElement)
        => jsonElement.ValueKind switch
        {
            JsonValueKind.Object => jsonElement.GetRawText(),
            JsonValueKind.Array => jsonElement.EnumerateArray().Cast<object>()
                .Select(item => item is JsonElement innerJsonElement
                    ? GetValueFromJsonElement(innerJsonElement)
                    : item
                ),
            JsonValueKind.String => jsonElement.GetString()!,
            JsonValueKind.Number => jsonElement.GetDouble(),
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => throw new ArgumentNullException(nameof(jsonElement))
        };
}
