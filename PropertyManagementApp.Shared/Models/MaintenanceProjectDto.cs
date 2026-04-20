namespace PropertyManagementApp.Shared.Models;

public class MaintenanceProjectDto
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public int PropertyId { get; set; }
    public string PropertyAddress { get; set; } = "";
    public string Vendor { get; set; } = "";
    public decimal BidAmount { get; set; }
    public string Status { get; set; } = "Bid"; // Bid, Approved, Work Order, Invoiced, Closed
    public DateTime CreatedAt { get; set; } = DateTime.Today;
    public int? LinkedInvoiceId { get; set; }
}
