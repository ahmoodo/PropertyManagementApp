namespace PropertyManagementApp.Shared.Models;

public class LateFeePolicy
{
    public int Id { get; set; }
    public int GracePeriodDays { get; set; } = 3;
    public string FeeType { get; set; } = "Flat"; // Flat or Percentage
    public decimal FeeValue { get; set; } = 50m;
    public bool PerDayAccrual { get; set; } = false;
}
