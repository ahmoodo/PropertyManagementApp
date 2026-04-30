using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceProjectsController : ControllerBase
{
    private readonly AppDbContext _db;
    public MaintenanceProjectsController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? propertyId, [FromQuery] string? status)
    {
        IQueryable<MaintenanceProjectDto> query = _db.MaintenanceProjects;
        if (propertyId.HasValue)
            query = query.Where(x => x.PropertyId == propertyId.Value);
        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);
        return Ok(query.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var p = _db.MaintenanceProjects.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpPost]
    public IActionResult Create([FromBody] MaintenanceProjectDto dto)
    {
        dto.Id = 0;
        dto.CreatedAt = DateTime.Today;
        _db.MaintenanceProjects.Add(dto);
        _db.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] MaintenanceProjectDto dto)
    {
        var existing = _db.MaintenanceProjects.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        existing.Title = dto.Title;
        existing.Description = dto.Description;
        existing.PropertyId = dto.PropertyId;
        existing.PropertyAddress = dto.PropertyAddress;
        existing.Vendor = dto.Vendor;
        existing.BidAmount = dto.BidAmount;
        existing.Status = dto.Status;
        existing.LinkedInvoiceId = dto.LinkedInvoiceId;
        _db.SaveChanges();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var p = _db.MaintenanceProjects.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        _db.MaintenanceProjects.Remove(p);
        _db.SaveChanges();
        return NoContent();
    }
}
