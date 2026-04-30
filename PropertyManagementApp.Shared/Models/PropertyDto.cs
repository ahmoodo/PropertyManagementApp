using System.ComponentModel.DataAnnotations;

namespace PropertyManagementApp.Shared.Models;

public class PropertyDto
{
    public int Id { get; set; }
    [Required]
    public string Address { get; set; } = "";
    public string City { get; set; } = "";
    public string State { get; set; } = "";
    public string Zip { get; set; } = "";
    [Required]
    public string Name { get; set; } = "";
    public int TotalUnits { get; set; }
}
