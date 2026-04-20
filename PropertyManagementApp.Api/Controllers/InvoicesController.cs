using Microsoft.AspNetCore.Mvc;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;
using System.Text;

namespace PropertyManagementApp.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly InMemoryDataStore _store;
    public InvoicesController(InMemoryDataStore store) => _store = store;

    [HttpGet]
    public IActionResult GetAll([FromQuery] string? status, [FromQuery] int? projectId)
    {
        var query = _store.Invoices.AsEnumerable();
        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);
        if (projectId.HasValue)
            query = query.Where(x => x.MaintenanceProjectId == projectId.Value);
        return Ok(query.ToList());
    }

    [HttpGet("{id}")]
    public IActionResult GetById(int id)
    {
        var inv = _store.Invoices.FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        return Ok(inv);
    }

    [HttpPost]
    public IActionResult Create([FromBody] InvoiceDto dto)
    {
        dto.Id = _store.NextId();
        dto.InvoiceNumber = $"INV-{dto.Id:D4}";
        dto.Status = "Draft";
        // give each line item an id
        foreach (var item in dto.LineItems)
        {
            item.Id = _store.NextId();
            item.InvoiceId = dto.Id;
        }
        _store.Invoices.Add(dto);
        return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
    }

    [HttpPut("{id}")]
    public IActionResult Update(int id, [FromBody] InvoiceDto dto)
    {
        var existing = _store.Invoices.FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();
        var idx = _store.Invoices.IndexOf(existing);
        dto.Id = id;
        _store.Invoices[idx] = dto;
        return Ok(dto);
    }

    [HttpDelete("{id}")]
    public IActionResult Delete(int id)
    {
        var inv = _store.Invoices.FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        _store.Invoices.Remove(inv);
        return NoContent();
    }

    [HttpPost("{id}/send")]
    public IActionResult Send(int id)
    {
        var inv = _store.Invoices.FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        inv.Status = "Sent";
        return Ok(new { payUrl = $"https://pay.example.com/inv/{id}", status = "Sent" });
    }

    [HttpPost("{id}/payments")]
    public IActionResult RecordPayment(int id, [FromBody] ManualPaymentDto payment)
    {
        var inv = _store.Invoices.FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        inv.Status = "Paid";
        return Ok(inv);
    }

    [HttpGet("export")]
    public IActionResult Export()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Invoice #,Date,Customer,Property,Amount,Status");
        foreach (var inv in _store.Invoices)
        {
            sb.AppendLine($"{inv.InvoiceNumber},{inv.InvoiceDate:yyyy-MM-dd},{inv.CustomerName},{inv.PropertyAddress},{inv.TotalAmount:F2},{inv.Status}");
        }
        return File(Encoding.UTF8.GetBytes(sb.ToString()), "text/csv", "invoices.csv");
    }
}

public class ManualPaymentDto
{
    public string PaymentMethod { get; set; } = "";
    public decimal Amount { get; set; }
    public string Reference { get; set; } = "";
}
