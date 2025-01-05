namespace DreamcoreHorrorGameStatisticsServer.Models.DTO;

public class RequestDTO
{
    public Guid Id { get; set; }
    public DateTime ReceptionTimestamp { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string ControllerName { get; set; } = string.Empty;
    public string MethodName { get; set; } = string.Empty;
}
