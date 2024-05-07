using DreamcoreHorrorGameApiServer.ConstantValues;
using System.Text.Json.Serialization;

namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

[JsonPolymorphic(UnknownDerivedTypeHandling = JsonUnknownDerivedTypeHandling.FallBackToNearestAncestor)]
[JsonDerivedType(derivedType: typeof(PropertyPredicate), typeDiscriminator: PropertyPredicateTypes.Base)]
[JsonDerivedType(derivedType: typeof(UnaryPropertyPredicate), typeDiscriminator: PropertyPredicateTypes.Unary)]
[JsonDerivedType(derivedType: typeof(BinaryPropertyPredicate), typeDiscriminator: PropertyPredicateTypes.Binary)]
[JsonDerivedType(derivedType: typeof(TernaryPropertyPredicate), typeDiscriminator: PropertyPredicateTypes.Ternary)]
public record PropertyPredicate (string Property, string Operator);
