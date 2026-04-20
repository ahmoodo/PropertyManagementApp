namespace PropertyManagementApp.Shared.Models;

public class RentPaymentDto
{
    public int Id { get; set; }
    public int TenantId { get; set; }
    public int RentScheduleId { get; set; }
    public decimal AmountPaid { get; set; }
    public DateTime PaymentDate { get; set; }
    public string PaymentMethod { get; set; } = "ACH"; // ACH, Card, Check, Cash
    public string TransactionReference { get; set; } = "";
}
