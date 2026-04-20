using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public TenantsController(InMemoryDataStore store) => _store = store;

    [HttpGet]
    public IActionResult GetAll() => Ok(_store.Tenants);

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var t = _store.Tenants.FirstOrDefault(x => x.Id == id);
        if (t == null) return NotFound();
        return Ok(t);
    }

    [HttpPost]
    public IActionResult Create([FromBody] TenantDto dto)
    {
        dto.Id = _store.NextId();
        _store.Tenants.Add(dto);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] TenantDto dto)
    {
        var existing = _store.Tenants.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        var idx = _store.Tenants.IndexOf(existing);
        dto.Id = id;
        _store.Tenants[idx] = dto;
        return Ok(dto);
    }
}
