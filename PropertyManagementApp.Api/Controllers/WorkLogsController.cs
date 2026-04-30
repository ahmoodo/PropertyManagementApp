using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkLogsController : ControllerBase
{
    private readonly AppDbContext _db;
    public WorkLogsController(AppDbContext db) => _db = db;

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? projectId)
    {
        IQueryable<WorkLogDto> query = _db.WorkLogs;
        if (projectId.HasValue)
            query = query.Where(x => x.ProjectId == projectId.Value);
        return Ok(query.ToList());
    }

    [HttpPost]
    public IActionResult Create([FromBody] WorkLogDto dto)
    {
        dto.Id = 0;
        _db.WorkLogs.Add(dto);
        _db.SaveChanges();
        return Ok(dto);
    }

    [HttpPost("{id}/photos")]
    public IActionResult UploadPhoto(int id, [FromBody] PhotoUploadDto upload)
    {
        var log = _db.WorkLogs.FirstOrDefault(x => x.Id == id);
        if (log == null) return NotFound();
        log.ProofPhotoUrl = upload.DataUrl;
        log.PhotoCategory = upload.Category;
        _db.SaveChanges();
        return Ok(new { url = upload.DataUrl });
    }
}

public class PhotoUploadDto
{
    public string DataUrl { get; set; } = "";
    public string Category { get; set; } = "During";
}
