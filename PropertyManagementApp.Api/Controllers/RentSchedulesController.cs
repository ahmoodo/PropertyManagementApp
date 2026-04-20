using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class RentSchedulesController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public RentSchedulesController(InMemoryDataStore store) => _store = store;

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? month, [FromQuery] int? tenantId, [FromQuery] int? propertyId)
    {
        var query = _store.RentSchedules.AsEnumerable();
        if (!string.IsNullOrEmpty(month))
            query = query.Where(x => x.Month == month);
        if (tenantId.HasValue)
            query = query.Where(x => x.TenantId == tenantId.Value);
        if (propertyId.HasValue)
            query = query.Where(x => x.PropertyId == propertyId.Value);
        return Ok(query.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var s = _store.RentSchedules.FirstOrDefault(x => x.Id == id);
        if (s == null) return NotFound();
        return Ok(s);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] RentScheduleDto dto)
    {
        var existing = _store.RentSchedules.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        var idx = _store.RentSchedules.IndexOf(existing);
        dto.Id = id;
        _store.RentSchedules[idx] = dto;
        return Ok(dto);
    }

    [HttpPost("{id}/apply-late-fee")]
    public IActionResult ApplyLateFee(int id)
    {
        var schedule = _store.RentSchedules.FirstOrDefault(x => x.Id == id);
        if (schedule == null) return NotFound();
        var policy = _store.LateFeePolicies.FirstOrDefault();
        if (policy == null) return BadRequest("No late fee policy configured.");

        decimal fee = policy.FeeType == "Flat"
            ? policy.FeeValue
            : schedule.BaseRent * (policy.FeeValue / 100m);

        schedule.LateFee = fee;
        if (schedule.Status != "Paid")
            schedule.Status = "Late";

        return Ok(schedule);
    }
}
