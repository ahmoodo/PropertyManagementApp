namespace PropertyManagementApp.Shared.Models;

public class InvoiceLineItemDto
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public string Description { get; set; } = "";
    public string Type { get; set; } = "Labor"; // Labor, Material, Tax
    public decimal Quantity { get; set; } = 1;
    public decimal UnitPrice { get; set; }
    public decimal Total => Quantity * UnitPrice;
}
