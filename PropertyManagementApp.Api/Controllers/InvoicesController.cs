using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PropertyManagementApp.Api.Data;
using PropertyManagementApp.Shared.Models;
using System.Text;

namespace PropertyManagementApp.Api.Controllers;

/// <summary>Invoices — billing, line items, and payment workflow.</summary>
[ApiController]
[Route("api/[controller]")]
public class InvoicesController : ControllerBase
{
    private readonly AppDbContext _db;
    public InvoicesController(AppDbContext db) => _db = db;

    /// <summary>List invoices, optionally filtered by status or project.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(List<InvoiceDto>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] string? status, [FromQuery] int? projectId)
    {
        IQueryable<InvoiceDto> query = _db.Invoices.Include(i => i.LineItems);
        if (!string.IsNullOrEmpty(status))
            query = query.Where(x => x.Status == status);
        if (projectId.HasValue)
            query = query.Where(x => x.MaintenanceProjectId == projectId.Value);
        return Ok(query.ToList());
    }

    /// <summary>Get a single invoice with its line items.</summary>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(int id)
    {
        var inv = _db.Invoices.Include(i => i.LineItems).FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        return Ok(inv);
    }

    /// <summary>Create a new invoice (Draft) with line items.</summary>
    [HttpPost]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status201Created)]
    public IActionResult Create([FromBody] InvoiceDto dto)
    {
        var lineItems = dto.LineItems?.ToList() ?? new List<InvoiceLineItemDto>();
        dto.LineItems = new List<InvoiceLineItemDto>();
        dto.Id = 0;
        dto.Status = "Draft";
        _db.Invoices.Add(dto);
        _db.SaveChanges();

        dto.InvoiceNumber = $"INV-{dto.Id:D4}";
        foreach (var item in lineItems)
        {
            item.Id = 0;
            item.InvoiceId = dto.Id;
            _db.InvoiceLineItems.Add(item);
        }
        _db.SaveChanges();

        var saved = _db.Invoices.Include(i => i.LineItems).First(x => x.Id == dto.Id);
        return CreatedAtAction(nameof(GetById), new { id = saved.Id }, saved);
    }

    /// <summary>Update an existing invoice and replace its line items.</summary>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(InvoiceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update(int id, [FromBody] InvoiceDto dto)
    {
        var existing = _db.Invoices.Include(i => i.LineItems).FirstOrDefault(x => x.Id == id);
        if (existing == null) return NotFound();

        existing.InvoiceNumber = dto.InvoiceNumber;
        existing.TenantId = dto.TenantId;
        existing.CustomerName = dto.CustomerName;
        existing.PropertyId = dto.PropertyId;
        existing.PropertyAddress = dto.PropertyAddress;
        existing.MaintenanceProjectId = dto.MaintenanceProjectId;
        existing.InvoiceDate = dto.InvoiceDate;
        existing.DueDate = dto.DueDate;
        existing.TaxRate = dto.TaxRate;
        existing.Discount = dto.Discount;
        existing.Notes = dto.Notes;
        existing.Status = dto.Status;

        _db.InvoiceLineItems.RemoveRange(existing.LineItems);
        _db.SaveChanges();

        foreach (var item in dto.LineItems ?? new List<InvoiceLineItemDto>())
        {
            item.Id = 0;
            item.InvoiceId = id;
            _db.InvoiceLineItems.Add(item);
        }
        _db.SaveChanges();

        var saved = _db.Invoices.Include(i => i.LineItems).First(x => x.Id == id);
        return Ok(saved);
    }

    /// <summary>Delete an invoice and its line items.</summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete(int id)
    {
        var inv = _db.Invoices.Include(i => i.LineItems).FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        _db.InvoiceLineItems.RemoveRange(inv.LineItems);
        _db.Invoices.Remove(inv);
        _db.SaveChanges();
        return NoContent();
    }

    /// <summary>Mark an invoice as Sent and return a payment link.</summary>
    [HttpPost("{id}/send")]
    public IActionResult Send(int id)
    {
        var inv = _db.Invoices.FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        inv.Status = "Sent";
        _db.SaveChanges();
        return Ok(new { payUrl = $"https://pay.example.com/inv/{id}", status = "Sent" });
    }

    /// <summary>Record a manual payment against an invoice.</summary>
    [HttpPost("{id}/payments")]
    public IActionResult RecordPayment(int id, [FromBody] ManualPaymentDto payment)
    {
        var inv = _db.Invoices.FirstOrDefault(x => x.Id == id);
        if (inv == null) return NotFound();
        inv.Status = "Paid";
        _db.SaveChanges();
        return Ok(inv);
    }

    /// <summary>Export invoices as a CSV file.</summary>
    [HttpGet("export")]
    public IActionResult Export()
    {
        var sb = new StringBuilder();
        sb.AppendLine("Invoice #,Date,Customer,Property,Amount,Status");
        var invoices = _db.Invoices.Include(i => i.LineItems).ToList();
        foreach (var inv in invoices)
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
