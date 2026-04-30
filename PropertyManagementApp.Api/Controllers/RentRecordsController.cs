using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;
using System.Text;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentRecordsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RentRecordsController(AppDbContext db) => _db = db;

    [HttpGet("export")]
    public IActionResult Export([FromQuery] int? tenantId, [FromQuery] int? propertyId, [FromQuery] string format = "csv")
    {
        var sb = new StringBuilder();
        sb.AppendLine("Date,Description,Charge,Payment,Balance");

        IQueryable<RentScheduleDto> schedules = _db.RentSchedules;
        IQueryable<RentPaymentDto> payments = _db.RentPayments;

        if (tenantId.HasValue)
        {
            schedules = schedules.Where(s => s.TenantId == tenantId.Value);
            payments = payments.Where(p => p.TenantId == tenantId.Value);
        }
        if (propertyId.HasValue)
        {
            schedules = schedules.Where(s => s.PropertyId == propertyId.Value);
        }

        decimal balance = 0;
        var schedList = schedules.OrderBy(s => s.DueDate).ToList();
        var payList = payments.OrderBy(p => p.PaymentDate).ToList();

        foreach (var s in schedList)
        {
            balance += s.BaseRent + s.LateFee;
            sb.AppendLine($"{s.DueDate:yyyy-MM-dd},Monthly Rent,{s.BaseRent:F2},,{balance:F2}");
            if (s.LateFee > 0)
                sb.AppendLine($"{s.DueDate:yyyy-MM-dd},Late Fee,{s.LateFee:F2},,{balance:F2}");
        }
        foreach (var p in payList)
        {
            balance -= p.AmountPaid;
            sb.AppendLine($"{p.PaymentDate:yyyy-MM-dd},{p.PaymentMethod} Payment,,{p.AmountPaid:F2},{balance:F2}");
        }

        var bytes = Encoding.UTF8.GetBytes(sb.ToString());
        return File(bytes, "text/csv", "rent-records.csv");
    }
}
