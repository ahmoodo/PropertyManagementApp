using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentPaymentsController : ControllerBase
{
    private readonly AppDbContext _db;
    public RentPaymentsController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? tenantId)
    {
        IQueryable<RentPaymentDto> query = _db.RentPayments;
        if (tenantId.HasValue)
            query = query.Where(x => x.TenantId == tenantId.Value);
        return Ok(query.ToList());
    }

    [HttpPost]
    public IActionResult Create([FromBody] RentPaymentDto dto)
    {
        dto.Id = 0;
        dto.PaymentDate = DateTime.Now;
        _db.RentPayments.Add(dto);
        _db.SaveChanges();

        var sched = _db.RentSchedules.FirstOrDefault(x => x.Id == dto.RentScheduleId);
        if (sched != null)
        {
            decimal total = _db.RentPayments
                .Where(p => p.RentScheduleId == sched.Id)
                .Sum(p => p.AmountPaid);
            decimal owed = sched.BaseRent + sched.LateFee;
            sched.Status = total >= owed ? "Paid" : "Partial";
            _db.SaveChanges();
        }

        return Ok(dto);
    }
}
