namespace DreamcoreHorrorGameApiServer.Models.PropertyPredicates;

public class BinaryOperator : PropertyPredicateOperator
{
    public Func<object?, object?, bool> Operation { get; init; } = (x, y) => throw new NotImplementedException();
}
