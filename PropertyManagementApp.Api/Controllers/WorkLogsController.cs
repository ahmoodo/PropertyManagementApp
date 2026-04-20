using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WorkLogsController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public WorkLogsController(InMemoryDataStore store) => _store = store;

    [HttpGet]
    public IActionResult GetAll([FromQuery] int? projectId)
    {
        var query = _store.WorkLogs.AsEnumerable();
        if (projectId.HasValue)
            query = query.Where(x => x.ProjectId == projectId.Value);
        return Ok(query.ToList());
    }

    [HttpPost]
    public IActionResult Create([FromBody] WorkLogDto dto)
    {
        dto.Id = _store.NextId();
        _store.WorkLogs.Add(dto);
        return Ok(dto);
    }

    [HttpPost("{id}/photos")]
    public IActionResult UploadPhoto(int id, [FromBody] PhotoUploadDto upload)
    {
        var log = _store.WorkLogs.FirstOrDefault(x => x.Id == id);
        if (log == null) return NotFound();
        // in real app this would go to blob storage, here we just accept the base64
        log.ProofPhotoUrl = upload.DataUrl;
        log.PhotoCategory = upload.Category;
        return Ok(new { url = upload.DataUrl });
    }
}

public class PhotoUploadDto
{
    public string DataUrl { get; set; } = "";
    public string Category { get; set; } = "During";
}
