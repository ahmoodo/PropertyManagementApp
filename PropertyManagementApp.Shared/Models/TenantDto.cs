using System.ComponentModel.DataAnnotations;

namespace PropertyManagementApp.Shared.Models;

public class TenantDto
{
    public int Id { get; set; }
    [Required]
    public string FirstName { get; set; } = "";
    [Required]
    public string LastName { get; set; } = "";
    [EmailAddress]
    public string Email { get; set; } = "";
    public string Phone { get; set; } = "";
    public int PropertyId { get; set; }
    public string Unit { get; set; } = "";
    public string FullName => $"{FirstName} {LastName}";
}
