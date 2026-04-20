using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LateFeePoliciesController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public LateFeePoliciesController(InMemoryDataStore store) => _store = store;

    [HttpGet("{id}")]
    public IActionResult Get(int id)
    {
        var p = _store.LateFeePolicies.FirstOrDefault(x => x.Id == id);
        if (p == null) return NotFound();
        return Ok(p);
    }

    [HttpGet]
    public IActionResult GetFirst() => Ok(_store.LateFeePolicies.FirstOrDefault());

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] LateFeePolicy dto)
    {
        var existing = _store.LateFeePolicies.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        var idx = _store.LateFeePolicies.IndexOf(existing);
        dto.Id = id;
        _store.LateFeePolicies[idx] = dto;
        return Ok(dto);
    }
}
