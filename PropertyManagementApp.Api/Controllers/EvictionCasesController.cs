using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

/// <summary>Eviction Cases — case lifecycle, steps, and notices.</summary>
[ApiController]
[Route("api/[controller]")]
public class EvictionCasesController : ControllerBase
{
    private readonly AppDbContext _db;
    public EvictionCasesController(AppDbContext db) => _db = db;

    /// <summary>List all eviction cases.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<EvictionCaseDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll() =>
        Ok(_db.EvictionCases.Include(c => c.Steps).Include(c => c.Notices).ToList());

    /// <summary>Get one eviction case with steps and notices.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(EvictionCaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var c = _db.EvictionCases.Include(x => x.Steps).Include(x => x.Notices).FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        return Ok(c);
    }

    /// <summary>Open a new eviction case (auto-creates the standard 7-step workflow).</summary>
    [HttpPost]
    [ProducesResponseType(typeof(EvictionCaseDto), StatusCodes.Status201Created)]
    public IActionResult Create([FromBody] EvictionCaseDto dto)
    {
        dto.Id = 0;
        dto.CaseOpenDate = DateTime.Today;
        dto.Steps = new List<EvictionStepDto>();
        dto.Notices = new List<EvictionNoticeDto>();
        _db.EvictionCases.Add(dto);
        _db.SaveChanges();

        var defaultSteps = new[]
        {
            new EvictionStepDto { CaseId = dto.Id, Order = 1, Label = "Issue Late Rent Notice", TypicalTiming = "Day 1-3", ActionRequired = "Generate and send Late Rent Notice PDF" },
            new EvictionStepDto { CaseId = dto.Id, Order = 2, Label = "Issue Pay-or-Quit Notice", TypicalTiming = "Day 3-5", ActionRequired = "Generate Pay-or-Quit Notice; record delivery" },
            new EvictionStepDto { CaseId = dto.Id, Order = 3, Label = "Document Non-Payment Evidence", TypicalTiming = "Day 5-10", ActionRequired = "Attach rent ledger export and communication logs" },
            new EvictionStepDto { CaseId = dto.Id, Order = 4, Label = "Consult Attorney", TypicalTiming = "Day 7-10", ActionRequired = "Attorney contact notes" },
            new EvictionStepDto { CaseId = dto.Id, Order = 5, Label = "File Eviction Petition", TypicalTiming = "Day 10-14", ActionRequired = "Export full case document package" },
            new EvictionStepDto { CaseId = dto.Id, Order = 6, Label = "Court Hearing", TypicalTiming = "Per court schedule", ActionRequired = "Log hearing date and outcome" },
            new EvictionStepDto { CaseId = dto.Id, Order = 7, Label = "Possession / Resolution", TypicalTiming = "Per judgment", ActionRequired = "Close or resolve case" }
        };
        _db.EvictionSteps.AddRange(defaultSteps);
        _db.SaveChanges();

        var saved = _db.EvictionCases.Include(c => c.Steps).Include(c => c.Notices).First(x => x.Id == dto.Id);
        return CreatedAtAction(nameof(GetById), new { id = saved.Id }, saved);
    }

    /// <summary>Update an eviction case (top-level fields only).</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(EvictionCaseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] EvictionCaseDto dto)
    {
        var existing = _db.EvictionCases.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        existing.TenantId = dto.TenantId;
        existing.TenantName = dto.TenantName;
        existing.PropertyId = dto.PropertyId;
        existing.PropertyAddress = dto.PropertyAddress;
        existing.Reason = dto.Reason;
        existing.Status = dto.Status;
        _db.SaveChanges();
        return Ok(existing);
    }

    /// <summary>Toggle completion of a single step.</summary>
    [HttpPut("{id}/steps/{stepId}")]
    [ProducesResponseType(typeof(EvictionStepDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult CompleteStep(int id, int stepId)
    {
        var step = _db.EvictionSteps.FirstOrDefault(s => s.Id == stepId && s.CaseId == id);
        if (step == null) return NotFound();
        step.IsComplete = !step.IsComplete;
        step.CompletedAt = step.IsComplete ? DateTime.Now : null;
        _db.SaveChanges();
        return Ok(step);
    }

    /// <summary>Save a generated notice document under a case.</summary>
    [HttpPost("{id}/notices")]
    [ProducesResponseType(typeof(EvictionNoticeDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult SaveNotice(int id, [FromBody] EvictionNoticeDto dto)
    {
        var c = _db.EvictionCases.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        dto.Id = 0;
        dto.CaseId = id;
        dto.CreatedAt = DateTime.Now;
        _db.EvictionNotices.Add(dto);
        _db.SaveChanges();
        return Ok(dto);
    }
}
