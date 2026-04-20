using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Data;

// simple in-memory store acts as our database for this project
public class InMemoryDataStore
{
    public List<PropertyDto> Properties { get; set; } = new();
    public List<TenantDto> Tenants { get; set; } = new();
    public List<RentScheduleDto> RentSchedules { get; set; } = new();
    public List<RentPaymentDto> RentPayments { get; set; } = new();
    public List<CommunicationLogDto> CommunicationLogs { get; set; } = new();
    public List<LateFeePolicy> LateFeePolicies { get; set; } = new();
    public List<MaintenanceProjectDto> MaintenanceProjects { get; set; } = new();
    public List<WorkLogDto> WorkLogs { get; set; } = new();
    public List<InvoiceDto> Invoices { get; set; } = new();
    public List<EvictionCaseDto> EvictionCases { get; set; } = new();
    public List<EvictionTemplateDto> EvictionTemplates { get; set; } = new();

    private int _nextId = 100;
    public int NextId() => _nextId++;

    public void Seed()
    {
        // properties
        Properties.Add(new PropertyDto { Id = 1, Name = "Maple Apartments", Address = "123 Maple St", City = "Evansville", State = "IN", Zip = "47701", TotalUnits = 4 });
        Properties.Add(new PropertyDto { Id = 2, Name = "Oak Grove", Address = "456 Oak Ave", City = "Evansville", State = "IN", Zip = "47702", TotalUnits = 2 });

        // tenants
        Tenants.Add(new TenantDto { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", Phone = "812-555-0101", PropertyId = 1, Unit = "1A" });
        Tenants.Add(new TenantDto { Id = 2, FirstName = "Bob", LastName = "Smith", Email = "bob@example.com", Phone = "812-555-0102", PropertyId = 1, Unit = "1B" });
        Tenants.Add(new TenantDto { Id = 3, FirstName = "Carol", LastName = "White", Email = "carol@example.com", Phone = "812-555-0103", PropertyId = 2, Unit = "2A" });
        Tenants.Add(new TenantDto { Id = 4, FirstName = "David", LastName = "Brown", Email = "david@example.com", Phone = "812-555-0104", PropertyId = 2, Unit = "2B" });

        // rent schedules for current month
        var month = "2026-04";
        RentSchedules.Add(new RentScheduleDto { Id = 1, TenantId = 1, TenantName = "Alice Johnson", PropertyId = 1, PropertyAddress = "123 Maple St", Unit = "1A", DueDate = new DateTime(2026, 4, 1), BaseRent = 1250m, LateFee = 0, Status = "Paid", Month = month });
        RentSchedules.Add(new RentScheduleDto { Id = 2, TenantId = 2, TenantName = "Bob Smith", PropertyId = 1, PropertyAddress = "123 Maple St", Unit = "1B", DueDate = new DateTime(2026, 4, 1), BaseRent = 1100m, LateFee = 75m, Status = "Late", Month = month });
        RentSchedules.Add(new RentScheduleDto { Id = 3, TenantId = 3, TenantName = "Carol White", PropertyId = 2, PropertyAddress = "456 Oak Ave", Unit = "2A", DueDate = new DateTime(2026, 4, 1), BaseRent = 950m, LateFee = 0, Status = "Partial", Month = month });
        RentSchedules.Add(new RentScheduleDto { Id = 4, TenantId = 4, TenantName = "David Brown", PropertyId = 2, PropertyAddress = "456 Oak Ave", Unit = "2B", DueDate = new DateTime(2026, 4, 1), BaseRent = 1000m, LateFee = 0, Status = "Unpaid", Month = month });

        // some payment history
        RentPayments.Add(new RentPaymentDto { Id = 1, TenantId = 1, RentScheduleId = 1, AmountPaid = 1250m, PaymentDate = new DateTime(2026, 4, 1), PaymentMethod = "ACH", TransactionReference = "TXN001" });
        RentPayments.Add(new RentPaymentDto { Id = 2, TenantId = 3, RentScheduleId = 3, AmountPaid = 500m, PaymentDate = new DateTime(2026, 4, 5), PaymentMethod = "Check", TransactionReference = "CHK1234" });

        // communication logs
        CommunicationLogs.Add(new CommunicationLogDto { Id = 1, TenantId = 2, Type = "SMS", Message = "Hi Bob, your rent of $1,100 was due April 1st. Please pay ASAP.", SentAt = new DateTime(2026, 4, 5), SentBy = "Gongjun Yan", Status = "Sent" });
        CommunicationLogs.Add(new CommunicationLogDto { Id = 2, TenantId = 4, Type = "Email", Message = "Dear David, this is a reminder that rent is overdue.", SentAt = new DateTime(2026, 4, 8), SentBy = "Gongjun Yan", Status = "Sent" });

        // late fee policy
        LateFeePolicies.Add(new LateFeePolicy { Id = 1, GracePeriodDays = 3, FeeType = "Flat", FeeValue = 75m, PerDayAccrual = false });

        // maintenance projects
        MaintenanceProjects.Add(new MaintenanceProjectDto { Id = 1, Title = "HVAC Repair Unit 1A", PropertyId = 1, PropertyAddress = "123 Maple St", Vendor = "Cool Air LLC", BidAmount = 850m, Status = "Approved", CreatedAt = new DateTime(2026, 4, 2) });
        MaintenanceProjects.Add(new MaintenanceProjectDto { Id = 2, Title = "Roof Leak Fix", PropertyId = 2, PropertyAddress = "456 Oak Ave", Vendor = "Apex Roofing", BidAmount = 2200m, Status = "Bid", CreatedAt = new DateTime(2026, 4, 10) });
        MaintenanceProjects.Add(new MaintenanceProjectDto { Id = 3, Title = "Parking Lot Repaving", PropertyId = 1, PropertyAddress = "123 Maple St", Vendor = "PaveRight Co", BidAmount = 5000m, Status = "Work Order", CreatedAt = new DateTime(2026, 3, 20) });
        MaintenanceProjects.Add(new MaintenanceProjectDto { Id = 4, Title = "Fence Installation", PropertyId = 2, PropertyAddress = "456 Oak Ave", Vendor = "SecureIt Fencing", BidAmount = 1500m, Status = "Closed", CreatedAt = new DateTime(2026, 2, 15) });

        // work logs
        WorkLogs.Add(new WorkLogDto { Id = 1, ProjectId = 3, ClockIn = new DateTime(2026, 4, 12, 8, 0, 0), ClockOut = new DateTime(2026, 4, 12, 16, 0, 0), GpsLocation = "37.9716,-87.5711", MaterialsUsed = "Asphalt mix, 2 tons", Notes = "Completed east section of lot", PhotoCategory = "During" });

        // invoices
        var inv = new InvoiceDto
        {
            Id = 1,
            InvoiceNumber = "INV-0001",
            TenantId = 1,
            CustomerName = "Alice Johnson",
            PropertyId = 1,
            PropertyAddress = "123 Maple St",
            InvoiceDate = new DateTime(2026, 4, 1),
            DueDate = new DateTime(2026, 4, 30),
            Status = "Paid",
            Notes = "April rent"
        };
        inv.LineItems.Add(new InvoiceLineItemDto { Id = 1, InvoiceId = 1, Description = "April Rent - Unit 1A", Type = "Labor", Quantity = 1, UnitPrice = 1250m });
        Invoices.Add(inv);

        var inv2 = new InvoiceDto
        {
            Id = 2,
            InvoiceNumber = "INV-0002",
            CustomerName = "Cool Air LLC",
            MaintenanceProjectId = 1,
            PropertyId = 1,
            PropertyAddress = "123 Maple St",
            InvoiceDate = new DateTime(2026, 4, 5),
            DueDate = new DateTime(2026, 4, 19),
            Status = "Overdue",
            Notes = "HVAC repair"
        };
        inv2.LineItems.Add(new InvoiceLineItemDto { Id = 2, InvoiceId = 2, Description = "HVAC Labor", Type = "Labor", Quantity = 4, UnitPrice = 125m });
        inv2.LineItems.Add(new InvoiceLineItemDto { Id = 3, InvoiceId = 2, Description = "Refrigerant R-410A", Type = "Material", Quantity = 2, UnitPrice = 85m });
        Invoices.Add(inv2);

        // eviction cases
        var evCase = new EvictionCaseDto
        {
            Id = 1,
            TenantId = 2,
            TenantName = "Bob Smith",
            PropertyId = 1,
            PropertyAddress = "123 Maple St",
            Reason = "Late Rent",
            CaseOpenDate = new DateTime(2026, 4, 10),
            Status = "Open"
        };
        evCase.Steps.AddRange(new[]
        {
            new EvictionStepDto { Id = 1, CaseId = 1, Order = 1, Label = "Issue Late Rent Notice", TypicalTiming = "Day 1-3", ActionRequired = "Generate & send Late Rent Notice PDF", IsComplete = true, CompletedAt = new DateTime(2026, 4, 10) },
            new EvictionStepDto { Id = 2, CaseId = 1, Order = 2, Label = "Issue Pay-or-Quit Notice", TypicalTiming = "Day 3-5", ActionRequired = "Generate Pay-or-Quit Notice; record delivery", IsComplete = false },
            new EvictionStepDto { Id = 3, CaseId = 1, Order = 3, Label = "Document Non-Payment Evidence", TypicalTiming = "Day 5-10", ActionRequired = "Attach rent ledger export + communication logs", IsComplete = false },
            new EvictionStepDto { Id = 4, CaseId = 1, Order = 4, Label = "Consult Attorney", TypicalTiming = "Day 7-10", ActionRequired = "Checkbox + attorney contact notes", IsComplete = false },
            new EvictionStepDto { Id = 5, CaseId = 1, Order = 5, Label = "File Eviction Petition", TypicalTiming = "Day 10-14", ActionRequired = "Export full case document package", IsComplete = false },
            new EvictionStepDto { Id = 6, CaseId = 1, Order = 6, Label = "Court Hearing", TypicalTiming = "Per court schedule", ActionRequired = "Log hearing date + outcome", IsComplete = false },
            new EvictionStepDto { Id = 7, CaseId = 1, Order = 7, Label = "Possession / Resolution", TypicalTiming = "Per judgment", ActionRequired = "Close or resolve case", IsComplete = false },
        });
        evCase.Notices.Add(new EvictionNoticeDto { Id = 1, CaseId = 1, Type = "late-rent-notice", NoticeText = "Dear Bob Smith, your rent of $1,100 was due April 1, 2026...", CreatedAt = new DateTime(2026, 4, 10), DeliveryMethod = "Email" });
        EvictionCases.Add(evCase);

        // eviction templates
        EvictionTemplates.Add(new EvictionTemplateDto { Id = 1, Slug = "late-rent-notice", Title = "Late Rent Notice", Description = "Friendly first reminder", Body = "Dear {{TenantFullName}},\n\nThis is a friendly reminder that your rent payment of {{RentAmountDue}} was due on {{DueDate}} for the property at {{PropertyAddress}}.\n\nPlease remit payment as soon as possible to avoid further action.\n\nSincerely,\n{{LandlordName}}" });
        EvictionTemplates.Add(new EvictionTemplateDto { Id = 2, Slug = "pay-or-quit-3day", Title = "3-Day Pay or Quit Notice", Description = "Formal 3-day pay-or-quit notice", Body = "NOTICE TO PAY OR QUIT\n\nDate: {{NoticeDate}}\n\nTo: {{TenantFullName}}\nProperty: {{PropertyAddress}}\n\nYou are hereby notified that you are in default of your rent obligation in the amount of {{RentAmountDue}}, which was due on {{DueDate}}.\n\nYou must pay the full amount or vacate the premises within THREE (3) DAYS of this notice (by {{QuitByDate}}).\n\nIf you fail to do so, legal proceedings will be initiated.\n\n{{LandlordName}}" });
        EvictionTemplates.Add(new EvictionTemplateDto { Id = 3, Slug = "pay-or-quit-5day", Title = "5-Day Pay or Quit Notice", Description = "5-day variant", Body = "NOTICE TO PAY OR QUIT\n\nDate: {{NoticeDate}}\n\nTo: {{TenantFullName}}\nProperty: {{PropertyAddress}}\n\nYou have FIVE (5) DAYS from {{NoticeDate}} to pay {{RentAmountDue}} or vacate.\n\n{{LandlordName}}" });
        EvictionTemplates.Add(new EvictionTemplateDto { Id = 4, Slug = "cure-or-quit", Title = "Cure or Quit Notice", Description = "Lease violation remedy notice", Body = "NOTICE TO CURE OR QUIT\n\nTo: {{TenantFullName}}\nProperty: {{PropertyAddress}}\nDate: {{NoticeDate}}\n\nYou are in violation of your lease agreement. You must cure this violation within the time specified or vacate the premises.\n\n{{LandlordName}}" });
        EvictionTemplates.Add(new EvictionTemplateDto { Id = 5, Slug = "unconditional-quit", Title = "Unconditional Quit Notice", Description = "Final notice before filing", Body = "UNCONDITIONAL NOTICE TO QUIT\n\nTo: {{TenantFullName}}\nProperty: {{PropertyAddress}}\nDate: {{NoticeDate}}\n\nYou are hereby required to vacate the premises. No opportunity to pay or cure is offered. Failure to vacate will result in immediate eviction proceedings.\n\n{{LandlordName}}" });
    }
}
