namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public class TernaryOperator : PropertyPredicateOperator
{
    public Func<object?, object?, object?, bool> Operation { get; init; } = (x, y, z) => throw new NotImplementedException();
}
