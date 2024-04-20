namespace DreamcoreHorrorGameApiServer.Models.PropertyPredicates;

public abstract class PropertyPredicateOperator
{
    public string Name { get; init; } = string.Empty;
    public bool IgnoreTypes { get; init; } = false;
}
