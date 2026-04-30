using Microsoft.EntityFrameworkCore;
using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<PropertyDto> Properties => Set<PropertyDto>();
    public DbSet<TenantDto> Tenants => Set<TenantDto>();
    public DbSet<RentScheduleDto> RentSchedules => Set<RentScheduleDto>();
    public DbSet<RentPaymentDto> RentPayments => Set<RentPaymentDto>();
    public DbSet<CommunicationLogDto> CommunicationLogs => Set<CommunicationLogDto>();
    public DbSet<LateFeePolicy> LateFeePolicies => Set<LateFeePolicy>();
    public DbSet<MaintenanceProjectDto> MaintenanceProjects => Set<MaintenanceProjectDto>();
    public DbSet<WorkLogDto> WorkLogs => Set<WorkLogDto>();
    public DbSet<InvoiceDto> Invoices => Set<InvoiceDto>();
    public DbSet<InvoiceLineItemDto> InvoiceLineItems => Set<InvoiceLineItemDto>();
    public DbSet<EvictionCaseDto> EvictionCases => Set<EvictionCaseDto>();
    public DbSet<EvictionStepDto> EvictionSteps => Set<EvictionStepDto>();
    public DbSet<EvictionNoticeDto> EvictionNotices => Set<EvictionNoticeDto>();
    public DbSet<EvictionTemplateDto> EvictionTemplates => Set<EvictionTemplateDto>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<InvoiceDto>()
            .Ignore(i => i.Subtotal)
            .Ignore(i => i.TaxAmount)
            .Ignore(i => i.TotalAmount);

        modelBuilder.Entity<InvoiceLineItemDto>()
            .Ignore(i => i.Total);

        modelBuilder.Entity<TenantDto>()
            .Ignore(t => t.FullName);

        modelBuilder.Entity<EvictionCaseDto>()
            .Ignore(e => e.DaysElapsed);

        modelBuilder.Entity<WorkLogDto>()
            .Ignore(w => w.Hours);

        modelBuilder.Entity<InvoiceDto>()
            .HasMany(i => i.LineItems)
            .WithOne()
            .HasForeignKey(li => li.InvoiceId);

        modelBuilder.Entity<EvictionCaseDto>()
            .HasMany(c => c.Steps)
            .WithOne()
            .HasForeignKey(s => s.CaseId);

        modelBuilder.Entity<EvictionCaseDto>()
            .HasMany(c => c.Notices)
            .WithOne()
            .HasForeignKey(n => n.CaseId);
    }
}
