using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvictionTemplatesController : ControllerBase
{
    private readonly AppDbContext _db;
    public EvictionTemplatesController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult GetAll() => Ok(_db.EvictionTemplates.ToList());

    [HttpGet("{slug}")]
    public IActionResult GetBySlug(string slug)
    {
        var t = _db.EvictionTemplates.FirstOrDefault(x => x.Slug == slug);
        if (t == null && int.TryParse(slug, out int id))
            t = _db.EvictionTemplates.FirstOrDefault(x => x.Id == id);
        if (t == null) return NotFound();
        return Ok(t);
    }

    [HttpPost]
    public IActionResult Create([FromBody] EvictionTemplateDto dto)
    {
        dto.Id = 0;
        _db.EvictionTemplates.Add(dto);
        _db.SaveChanges();
        return Ok(dto);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] EvictionTemplateDto dto)
    {
        var existing = _db.EvictionTemplates.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        existing.Slug = dto.Slug;
        existing.Title = dto.Title;
        existing.Body = dto.Body;
        existing.Description = dto.Description;
        _db.SaveChanges();
        return Ok(existing);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var t = _db.EvictionTemplates.FirstOrDefault(x => x.Id == id);
        if (t == null) return NotFound();
        _db.EvictionTemplates.Remove(t);
        _db.SaveChanges();
        return NoContent();
    }
}
