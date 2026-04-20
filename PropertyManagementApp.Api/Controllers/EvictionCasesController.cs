using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class EvictionCasesController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public EvictionCasesController(InMemoryDataStore store) => _store = store;

    [HttpGet]
    public IActionResult GetAll() => Ok(_store.EvictionCases);

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var c = _store.EvictionCases.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        return Ok(c);
    }

    [HttpPost]
    public IActionResult Create([FromBody] EvictionCaseDto dto)
    {
        dto.Id = _store.NextId();
        dto.CaseOpenDate = DateTime.Today;
        // seed the default steps
        var defaultSteps = new[]
        {
            new EvictionStepDto { Order = 1, Label = "Issue Late Rent Notice", TypicalTiming = "Day 1-3", ActionRequired = "Generate & send Late Rent Notice PDF" },
            new EvictionStepDto { Order = 2, Label = "Issue Pay-or-Quit Notice", TypicalTiming = "Day 3-5", ActionRequired = "Generate Pay-or-Quit Notice; record delivery" },
            new EvictionStepDto { Order = 3, Label = "Document Non-Payment Evidence", TypicalTiming = "Day 5-10", ActionRequired = "Attach rent ledger export + communication logs" },
            new EvictionStepDto { Order = 4, Label = "Consult Attorney", TypicalTiming = "Day 7-10", ActionRequired = "Attorney contact notes" },
            new EvictionStepDto { Order = 5, Label = "File Eviction Petition", TypicalTiming = "Day 10-14", ActionRequired = "Export full case document package" },
            new EvictionStepDto { Order = 6, Label = "Court Hearing", TypicalTiming = "Per court schedule", ActionRequired = "Log hearing date + outcome" },
            new EvictionStepDto { Order = 7, Label = "Possession / Resolution", TypicalTiming = "Per judgment", ActionRequired = "Close or resolve case" },
        };
        int stepId = _store.NextId();
        foreach (var s in defaultSteps)
        {
            s.Id = stepId++;
            s.CaseId = dto.Id;
        }
        dto.Steps = defaultSteps.ToList();
        _store.EvictionCases.Add(dto);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] EvictionCaseDto dto)
    {
        var existing = _store.EvictionCases.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        var idx = _store.EvictionCases.IndexOf(existing);
        dto.Id = id;
        _store.EvictionCases[idx] = dto;
        return Ok(dto);
    }

    [HttpPut("{id}/steps/{stepId}")]
    public IActionResult CompleteStep(int id, int stepId)
    {
        var c = _store.EvictionCases.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        var step = c.Steps.FirstOrDefault(s => s.Id == stepId);
        if (step == null) return NotFound();
        step.IsComplete = !step.IsComplete;
        step.CompletedAt = step.IsComplete ? DateTime.Now : null;
        return Ok(step);
    }

    [HttpPost("{id}/notices")]
    public IActionResult SaveNotice(int id, [FromBody] EvictionNoticeDto dto)
    {
        var c = _store.EvictionCases.FirstOrDefault(x => x.Id == id);
        if (c == null) return NotFound();
        dto.Id = _store.NextId();
        dto.CaseId = id;
        dto.CreatedAt = DateTime.Now;
        c.Notices.Add(dto);
        return Ok(dto);
    }
}
