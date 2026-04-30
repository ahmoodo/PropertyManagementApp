using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CommunicationsController : ControllerBase
{
    private readonly AppDbContext _db;
    public CommunicationsController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? tenantId)
    {
        IQueryable<CommunicationLogDto> query = _db.CommunicationLogs;
        if (tenantId.HasValue)
            query = query.Where(x => x.TenantId == tenantId.Value);
        return Ok(query.OrderByDescending(x => x.SentAt).ToList());
    }

    [HttpPost]
    public IActionResult Create([FromBody] CommunicationLogDto dto)
    {
        dto.Id = 0;
        dto.SentAt = DateTime.Now;
        dto.SentBy = "Property Manager";
        dto.Status = "Sent";
        _db.CommunicationLogs.Add(dto);
        _db.SaveChanges();
        return Ok(dto);
    }
}
