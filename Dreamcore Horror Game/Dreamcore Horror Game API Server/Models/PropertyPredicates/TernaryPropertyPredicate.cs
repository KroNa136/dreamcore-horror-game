namespace DreamcoreHorrorGameApiServer.Models.PropertyPredicates;

public class TernaryPropertyPredicate : PropertyPredicate
{
    public object? FirstValue { get; set; } = null;
    public object? SecondValue { get; set; } = null;

    public TernaryPropertyPredicate(string property, string Operator, object? firstValue, object? secondValue) : base(property, Operator)
    {
        FirstValue = firstValue;
        SecondValue = secondValue;
    }
}
