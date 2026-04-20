using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PropertiesController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public PropertiesController(InMemoryDataStore store) => _store = store;

    [HttpGet]
    public IActionResult GetAll() => Ok(_store.Properties);

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var p = _store.Properties.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        return Ok(p);
    }
}
