namespace PropertyManagementApp.Shared.Models;

public class EvictionStepDto
{
    public int Id { get; set; }
    public int CaseId { get; set; }
    public string Label { get; set; } = "";
    public string TypicalTiming { get; set; } = "";
    public string ActionRequired { get; set; } = "";
    public bool IsComplete { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int Order { get; set; }
}
