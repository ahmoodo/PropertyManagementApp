namespace PropertyManagementApp.Shared.Models;

public class EvictionCaseDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; } = "";
    public int PropertyId { get; set; }
    public string PropertyAddress { get; set; } = "";
    public string Reason { get; set; } = "Late Rent";
    public DateTime CaseOpenDate { get; set; } = DateTime.Today;
    public string Status { get; set; } = "Open"; // Open, Closed, Resolved
    public List<EvictionStepDto> Steps { get; set; } = new();
    public List<EvictionNoticeDto> Notices { get; set; } = new();
    public int DaysElapsed => (DateTime.Today - CaseOpenDate).Days;
}
