using System.ComponentModel.DataAnnotations;

namespace PropertyManagementApp.Shared.Models;

public class InvoiceDto
{
    public int Id { get; set; }
    [Required]
    public string InvoiceNumber { get; set; } = "";
    public int? TenantId { get; set; }
    [Required]
    public string CustomerName { get; set; } = "";
    public int? PropertyId { get; set; }
    public string PropertyAddress { get; set; } = "";
    public int? MaintenanceProjectId { get; set; }
    public DateTime InvoiceDate { get; set; } = DateTime.Today;
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(30);
    public decimal TaxRate { get; set; } = 0;
    public decimal Discount { get; set; } = 0;
    public string Notes { get; set; } = "";
    public string Status { get; set; } = "Draft"; // Draft, Sent, Paid, Overdue
    public List<InvoiceLineItemDto> LineItems { get; set; } = new();
    public decimal Subtotal => LineItems.Sum(i => i.Total);
    public decimal TaxAmount => Subtotal * (TaxRate / 100);
    public decimal TotalAmount => Subtotal + TaxAmount - Discount;
}
