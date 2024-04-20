using System.Text.Json.Serialization;

namespace DreamcoreHorrorGameApiServer.Models.PropertyPredicates;

public class UnaryPropertyPredicate : PropertyPredicate
{
    public UnaryPropertyPredicate(string property, string Operator) : base(property, Operator) { }
}
