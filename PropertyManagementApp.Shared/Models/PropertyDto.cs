namespace PropertyManagementApp.Shared.Models;

public class PropertyDto
{
    public int Id { get; set; }
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string Zip { get; set; } = "";
    public string Name { get; set; } = ""; // e.g. "Maple Apartments"
    public int TotalUnits { get; set; }
}
