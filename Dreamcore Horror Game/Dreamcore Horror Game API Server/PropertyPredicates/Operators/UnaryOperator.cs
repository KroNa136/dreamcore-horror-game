namespace DreamcoreHorrorGameApiServer.PropertyPredicates;

public class UnaryOperator : PropertyPredicateOperator
{
    public Func<object?, bool> Operation { get; init; } = x => throw new NotImplementedException();
}
