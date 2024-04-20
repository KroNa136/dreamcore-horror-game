namespace DreamcoreHorrorGameApiServer.Models.Database;

public interface IDatabaseEntity
{
    public Guid Id { get; set; }
    public string DisplayName { get; }
}
