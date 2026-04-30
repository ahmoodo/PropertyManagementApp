using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LateFeePoliciesController : ControllerBase
{
    private readonly AppDbContext _db;
    public LateFeePoliciesController(AppDbContext db) => _db = db;

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var p = _db.LateFeePolicies.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpGet]
    public IActionResult GetFirst() => Ok(_db.LateFeePolicies.FirstOrDefault());

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] LateFeePolicy dto)
    {
        var existing = _db.LateFeePolicies.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        existing.GracePeriodDays = dto.GracePeriodDays;
        existing.FeeType = dto.FeeType;
        existing.FeeValue = dto.FeeValue;
        existing.PerDayAccrual = dto.PerDayAccrual;
        _db.SaveChanges();
        return Ok(existing);
    }
}
