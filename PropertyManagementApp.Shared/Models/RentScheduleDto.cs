using System.ComponentModel.DataAnnotations;

namespace PropertyManagementApp.Shared.Models;

public class RentScheduleDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public string TenantName { get; set; } = "";
    public int PropertyId { get; set; }
    public string PropertyAddress { get; set; } = "";
    public string Unit { get; set; } = "";
    public DateTime DueDate { get; set; }
    [Range(0, double.MaxValue)]
    public decimal BaseRent { get; set; }
    [Range(0, double.MaxValue)]
    public decimal LateFee { get; set; }
    public string Status { get; set; } = "Unpaid"; // Paid, Unpaid, Late, Partial
    public string Month { get; set; } = ""; // e.g. "2026-03"
}
