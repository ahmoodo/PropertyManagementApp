namespace PropertyManagementApp.Client.Shared;

public class LedgerRow
{
    public DateTime Date { get; set; }
    public string Description { get; set; } = "";
    public decimal Charge { get; set; }
    public decimal Payment { get; set; }
    public decimal Balance { get; set; }
}
