namespace PropertyManagementApp.Shared.Models;

public class EvictionTemplateDto
{
    public int Id { get; set; }
    public string Slug { get; set; } = ""; // e.g. "late-rent-notice"
    public string Title { get; set; } = "";
    public string Body { get; set; } = "";
    public string Description { get; set; } = "";
}
