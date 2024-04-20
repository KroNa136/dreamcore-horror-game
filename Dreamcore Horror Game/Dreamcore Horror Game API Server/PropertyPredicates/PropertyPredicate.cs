using DreamcoreHorrorGameApiServer.ConstantValues;
using System.Text.Json.Serialization;

namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(derivedType: typeof(PropertyPredicate), typeDiscriminator: PropertyPredicateTypes.Base)]
[JsonDerivedType(derivedType: typeof(UnaryPropertyPredicate), typeDiscriminator: PropertyPredicateTypes.Unary)]
[JsonDerivedType(derivedType: typeof(BinaryPropertyPredicate), typeDiscriminator: PropertyPredicateTypes.Binary)]
[JsonDerivedType(derivedType: typeof(TernaryPropertyPredicate), typeDiscriminator: PropertyPredicateTypes.Ternary)]
public class PropertyPredicate
{
    public string Property { get; set; } = string.Empty;
    public string Operator { get; set; } = string.Empty;

    public PropertyPredicate(string property, string Operator)
    {
        Property = property;
        this.Operator = Operator;
    }

    [JsonIgnore]
    public bool IsEmptyProperty => string.IsNullOrEmpty(Property);
    [JsonIgnore]
    public bool IsNotEmptyProperty => string.IsNullOrEmpty(Property) is false;

    [JsonIgnore]
    public bool IsEmptyOperator => string.IsNullOrEmpty(Operator);
    [JsonIgnore]
    public bool IsNotEmptyOperator => string.IsNullOrEmpty(Operator) is false;
}
