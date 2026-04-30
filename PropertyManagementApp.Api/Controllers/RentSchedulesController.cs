using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

/// <summary>Rent Schedules — monthly rent rows per tenant with status tracking.</summary>
[ApiController]
[Route("api/[controller]")]
public class RentSchedulesController : ControllerBase
{
    private readonly AppDbContext _db;
    public RentSchedulesController(AppDbContext db) => _db = db;

    /// <summary>List rent schedules, optionally filtered by month/tenant/property.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<RentScheduleDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] string? month, [FromQuery] int? tenantId, [FromQuery] int? propertyId)
    {
        IQueryable<RentScheduleDto> query = _db.RentSchedules;
        if (!string.IsNullOrEmpty(month))
            query = query.Where(x => x.Month == month);
        if (tenantId.HasValue)
            query = query.Where(x => x.TenantId == tenantId.Value);
        if (propertyId.HasValue)
            query = query.Where(x => x.PropertyId == propertyId.Value);
        return Ok(query.ToList());
    }

    /// <summary>Get a single rent schedule by ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(RentScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var s = _db.RentSchedules.FirstOrDefault(x => x.Id == id);
        if (s == null) return NotFound();
        return Ok(s);
    }

    /// <summary>Update a rent schedule entry.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(RentScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] RentScheduleDto dto)
    {
        var existing = _db.RentSchedules.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        existing.TenantId = dto.TenantId;
        existing.TenantName = dto.TenantName;
        existing.PropertyId = dto.PropertyId;
        existing.PropertyAddress = dto.PropertyAddress;
        existing.Unit = dto.Unit;
        existing.DueDate = dto.DueDate;
        existing.BaseRent = dto.BaseRent;
        existing.LateFee = dto.LateFee;
        existing.Status = dto.Status;
        existing.Month = dto.Month;
        _db.SaveChanges();
        return Ok(existing);
    }

    /// <summary>Apply the configured late fee policy to a schedule.</summary>
    [HttpPost("{id}/apply-late-fee")]
    [ProducesResponseType(typeof(RentScheduleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public IActionResult ApplyLateFee(int id)
    {
        var schedule = _db.RentSchedules.FirstOrDefault(x => x.Id == id);
        if (schedule == null) return NotFound();
        var policy = _db.LateFeePolicies.FirstOrDefault();
        if (policy == null) return BadRequest("No late fee policy configured.");

        decimal fee = policy.FeeType == "Flat"
            ? policy.FeeValue
            : schedule.BaseRent * (policy.FeeValue / 100m);

        schedule.LateFee = fee;
        if (schedule.Status != "Paid")
            schedule.Status = "Late";

        _db.SaveChanges();
        return Ok(schedule);
    }
}
