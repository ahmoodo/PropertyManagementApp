using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

/// <summary>Properties — list and lookup of managed properties.</summary>
[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly AppDbContext _db;
    public PropertiesController(AppDbContext db) => _db = db;

    /// <summary>List all properties.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<PropertyDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll() => Ok(_db.Properties.ToList());

    /// <summary>Get a property by ID.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(PropertyDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var p = _db.Properties.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        return Ok(p);
    }
}
