namespace PropertyManagementApp.Shared.Models;

public class WorkLogDto
{
    public int Id { get; set; }
    public int ProjectId { get; set; }
    public DateTime ClockIn { get; set; }
    public DateTime? ClockOut { get; set; }
    public string GpsLocation { get; set; } = "";
    public string MaterialsUsed { get; set; } = "";
    public string Notes { get; set; } = "";
    public string ProofPhotoUrl { get; set; } = "";
    public string PhotoCategory { get; set; } = "During"; // Before, During, After
    public double Hours => ClockOut.HasValue ? (ClockOut.Value - ClockIn).TotalHours : 0;
}
