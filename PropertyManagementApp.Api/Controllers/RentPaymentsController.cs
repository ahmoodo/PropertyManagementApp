using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentPaymentsController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public RentPaymentsController(InMemoryDataStore store) => _store = store;

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? tenantId)
    {
        var query = _store.RentPayments.AsEnumerable();
        if (tenantId.HasValue)
            query = query.Where(x => x.TenantId == tenantId.Value);
        return Ok(query.ToList());
    }

    [HttpPost]
    public IActionResult Create([FromBody] RentPaymentDto dto)
    {
        dto.Id = _store.NextId();
        dto.PaymentDate = DateTime.Now;
        _store.RentPayments.Add(dto);

        // update the schedule status
        var sched = _store.RentSchedules.FirstOrDefault(x => x.Id == dto.RentScheduleId);
        if (sched != null)
        {
            decimal total = _store.RentPayments
                .Where(p => p.RentScheduleId == sched.Id)
                .Sum(p => p.AmountPaid);
            decimal owed = sched.BaseRent + sched.LateFee;
            sched.Status = total >= owed ? "Paid" : "Partial";
        }

        return Ok(dto);
    }
}
