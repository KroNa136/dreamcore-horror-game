namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public class BinaryPropertyPredicate : PropertyPredicate
{
    public object? Value { get; set; } = null;

    public BinaryPropertyPredicate(string property, string Operator, object? value)
        : base(property, Operator)
    {
        Value = value;
    }
}
