using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvictionTemplatesController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public EvictionTemplatesController(InMemoryDataStore store) => _store = store;

    [HttpGet]
    public IActionResult GetAll() => Ok(_store.EvictionTemplates);

    [HttpGet("{slug}")]
    public IActionResult GetBySlug(string slug)
    {
        // try by slug first, then by id
        var t = _store.EvictionTemplates.FirstOrDefault(x => x.Slug == slug);
        if (t == null && int.TryParse(slug, out int id))
            t = _store.EvictionTemplates.FirstOrDefault(x => x.Id == id);
        if (t == null) return NotFound();
        return Ok(t);
    }

    [HttpPost]
    public IActionResult Create([FromBody] EvictionTemplateDto dto)
    {
        dto.Id = _store.NextId();
        _store.EvictionTemplates.Add(dto);
        return Ok(dto);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] EvictionTemplateDto dto)
    {
        var existing = _store.EvictionTemplates.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        var idx = _store.EvictionTemplates.IndexOf(existing);
        dto.Id = id;
        _store.EvictionTemplates[idx] = dto;
        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var t = _store.EvictionTemplates.FirstOrDefault(x => x.Id == id);
        if (t == null) return NotFound();
        _store.EvictionTemplates.Remove(t);
        return NoContent();
    }
}
