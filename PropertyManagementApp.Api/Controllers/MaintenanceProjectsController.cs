using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MaintenanceProjectsController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public MaintenanceProjectsController(InMemoryDataStore store) => _store = store;

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? propertyId, [FromQuery] string? status)
    {
        var query = _store.MaintenanceProjects.AsEnumerable();
        if (propertyId.HasValue)
            query = query.Where(x => x.PropertyId == propertyId.Value);
        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);
        return Ok(query.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var p = _store.MaintenanceProjects.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpPost]
    public IActionResult Create([FromBody] MaintenanceProjectDto dto)
    {
        dto.Id = _store.NextId();
        dto.CreatedAt = DateTime.Today;
        _store.MaintenanceProjects.Add(dto);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] MaintenanceProjectDto dto)
    {
        var existing = _store.MaintenanceProjects.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        var idx = _store.MaintenanceProjects.IndexOf(existing);
        dto.Id = id;
        _store.MaintenanceProjects[idx] = dto;
        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var p = _store.MaintenanceProjects.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        _store.MaintenanceProjects.Remove(p);
        return NoContent();
    }
}
