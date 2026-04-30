using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

/// <summary>Tenants — CRUD operations for residents/lessees.</summary>
[ApiController]
[Route("api/[controller]")]
public class TenantsController : ControllerBase
{
    private readonly AppDbContext _db;
    public TenantsController(AppDbContext db) => _db = db;

    /// <summary>List all tenants.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<TenantDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(_db.Tenants.ToList());

    /// <summary>Get a tenant by ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var t = _db.Tenants.FirstOrDefault(x => x.Id == id);
        if (t == null) return NotFound();
        return Ok(t);
    }

    /// <summary>Create a new tenant.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status201Created)]
    public IActionResult Create([FromBody] TenantDto dto)
    {
        dto.Id = 0;
        _db.Tenants.Add(dto);
        _db.SaveChanges();
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    /// <summary>Update an existing tenant.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(TenantDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] TenantDto dto)
    {
        var existing = _db.Tenants.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        existing.FirstName = dto.FirstName;
        existing.LastName = dto.LastName;
        existing.Email = dto.Email;
        existing.Phone = dto.Phone;
        existing.PropertyId = dto.PropertyId;
        existing.Unit = dto.Unit;
        _db.SaveChanges();
        return Ok(existing);
    }
}
