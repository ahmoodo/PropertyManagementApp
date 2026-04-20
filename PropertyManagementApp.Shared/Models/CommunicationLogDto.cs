namespace PropertyManagementApp.Shared.Models;

public class CommunicationLogDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string Type { get; set; } = "SMS"; // SMS, Email, Call
    public string Message { get; set; } = "";
    public DateTime SentAt { get; set; }
    public string SentBy { get; set; } = "";
    public string Status { get; set; } = "Sent";
}
