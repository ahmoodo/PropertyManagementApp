namespace PropertyManagementApp.Shared.Models;

public class EvictionNoticeDto
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string Type { get; set; } = "";
    public string NoticeText { get; set; } = "";
    public DateTime CreatedAt { get; set; }
    public string DeliveryMethod { get; set; } = "Email";
}
