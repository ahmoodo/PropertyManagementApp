using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommunicationsController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public CommunicationsController(InMemoryDataStore store) => _store = store;

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? tenantId)
    {
        var query = _store.CommunicationLogs.AsEnumerable();
        if (tenantId.HasValue)
            query = query.Where(x => x.TenantId == tenantId.Value);
        return Ok(query.OrderByDescending(x => x.SentAt).ToList());
    }

    [HttpPost]
    public IActionResult Create([FromBody] CommunicationLogDto dto)
    {
        dto.Id = _store.NextId();
        dto.SentAt = DateTime.Now;
        dto.SentBy = "Gongjun Yan";
        dto.Status = "Sent";
        _store.CommunicationLogs.Add(dto);
        return Ok(dto);
    }
}
