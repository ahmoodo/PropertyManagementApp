using PropertyManagementApp.Shared.Models;

namespace PropertyManagementApp.Api.Data;

public static class DbSeeder
{
    public static void Seed(AppDbContext db)
    {
        if (db.Properties.Any()) return;

        db.Properties.AddRange(
            new PropertyDto { Id = 1, Name = "Maple Apartments", Address = "123 Maple St", City = "Evansville", State = "IN", Zip = "47701", TotalUnits = 4 },
            new PropertyDto { Id = 2, Name = "Oak Grove", Address = "456 Oak Ave", City = "Evansville", State = "IN", Zip = "47702", TotalUnits = 2 }
        );

        db.Tenants.AddRange(
            new TenantDto { Id = 1, FirstName = "Alice", LastName = "Johnson", Email = "alice@example.com", Phone = "812-555-0101", PropertyId = 1, Unit = "1A" },
            new TenantDto { Id = 2, FirstName = "Bob", LastName = "Smith", Email = "bob@example.com", Phone = "812-555-0102", PropertyId = 1, Unit = "1B" },
            new TenantDto { Id = 3, FirstName = "Carol", LastName = "White", Email = "carol@example.com", Phone = "812-555-0103", PropertyId = 2, Unit = "2A" },
            new TenantDto { Id = 4, FirstName = "David", LastName = "Brown", Email = "david@example.com", Phone = "812-555-0104", PropertyId = 2, Unit = "2B" }
        );

        var currentMonth = DateTime.Today.ToString("yyyy-MM");
        var dueDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);

        db.RentSchedules.AddRange(
            new RentScheduleDto { Id = 1, TenantId = 1, TenantName = "Alice Johnson", PropertyId = 1, PropertyAddress = "123 Maple St", Unit = "1A", DueDate = dueDate, BaseRent = 1250m, LateFee = 0, Status = "Paid", Month = currentMonth },
            new RentScheduleDto { Id = 2, TenantId = 2, TenantName = "Bob Smith", PropertyId = 1, PropertyAddress = "123 Maple St", Unit = "1B", DueDate = dueDate, BaseRent = 1100m, LateFee = 75m, Status = "Late", Month = currentMonth },
            new RentScheduleDto { Id = 3, TenantId = 3, TenantName = "Carol White", PropertyId = 2, PropertyAddress = "456 Oak Ave", Unit = "2A", DueDate = dueDate, BaseRent = 950m, LateFee = 0, Status = "Partial", Month = currentMonth },
            new RentScheduleDto { Id = 4, TenantId = 4, TenantName = "David Brown", PropertyId = 2, PropertyAddress = "456 Oak Ave", Unit = "2B", DueDate = dueDate, BaseRent = 1000m, LateFee = 0, Status = "Unpaid", Month = currentMonth }
        );

        db.RentPayments.AddRange(
            new RentPaymentDto { Id = 1, TenantId = 1, RentScheduleId = 1, AmountPaid = 1250m, PaymentDate = dueDate, PaymentMethod = "ACH", TransactionReference = "TXN001" },
            new RentPaymentDto { Id = 2, TenantId = 3, RentScheduleId = 3, AmountPaid = 500m, PaymentDate = dueDate.AddDays(5), PaymentMethod = "Check", TransactionReference = "CHK1234" }
        );

        db.CommunicationLogs.AddRange(
            new CommunicationLogDto { Id = 1, TenantId = 2, Type = "SMS", Message = "Hi Bob, your rent was due on the 1st. Please pay as soon as possible.", SentAt = dueDate.AddDays(4), SentBy = "Property Manager", Status = "Sent" },
            new CommunicationLogDto { Id = 2, TenantId = 4, Type = "Email", Message = "Dear David, this is a reminder that rent is overdue.", SentAt = dueDate.AddDays(7), SentBy = "Property Manager", Status = "Sent" }
        );

        db.LateFeePolicies.Add(new LateFeePolicy { Id = 1, GracePeriodDays = 3, FeeType = "Flat", FeeValue = 75m, PerDayAccrual = false });

        db.MaintenanceProjects.AddRange(
            new MaintenanceProjectDto { Id = 1, Title = "HVAC Repair Unit 1A", PropertyId = 1, PropertyAddress = "123 Maple St", Vendor = "Cool Air LLC", BidAmount = 850m, Status = "Approved", CreatedAt = DateTime.Today.AddDays(-18) },
            new MaintenanceProjectDto { Id = 2, Title = "Roof Leak Fix", PropertyId = 2, PropertyAddress = "456 Oak Ave", Vendor = "Apex Roofing", BidAmount = 2200m, Status = "Bid", CreatedAt = DateTime.Today.AddDays(-10) },
            new MaintenanceProjectDto { Id = 3, Title = "Parking Lot Repaving", PropertyId = 1, PropertyAddress = "123 Maple St", Vendor = "PaveRight Co", BidAmount = 5000m, Status = "Work Order", CreatedAt = DateTime.Today.AddDays(-40) },
            new MaintenanceProjectDto { Id = 4, Title = "Fence Installation", PropertyId = 2, PropertyAddress = "456 Oak Ave", Vendor = "SecureIt Fencing", BidAmount = 1500m, Status = "Closed", CreatedAt = DateTime.Today.AddDays(-75) }
        );

        db.WorkLogs.Add(new WorkLogDto { Id = 1, ProjectId = 3, ClockIn = DateTime.Today.AddDays(-8).AddHours(8), ClockOut = DateTime.Today.AddDays(-8).AddHours(16), GpsLocation = "37.9716,-87.5711", MaterialsUsed = "Asphalt mix, 2 tons", Notes = "Completed east section of lot", PhotoCategory = "During" });

        var inv1 = new InvoiceDto
        {
            Id = 1, InvoiceNumber = "INV-0001", TenantId = 1, CustomerName = "Alice Johnson",
            PropertyId = 1, PropertyAddress = "123 Maple St",
            InvoiceDate = dueDate, DueDate = dueDate.AddDays(29), Status = "Paid", Notes = "Monthly rent"
        };
        db.Invoices.Add(inv1);
        db.InvoiceLineItems.Add(new InvoiceLineItemDto { Id = 1, InvoiceId = 1, Description = "Monthly Rent - Unit 1A", Type = "Labor", Quantity = 1, UnitPrice = 1250m });

        var inv2 = new InvoiceDto
        {
            Id = 2, InvoiceNumber = "INV-0002", CustomerName = "Cool Air LLC",
            MaintenanceProjectId = 1, PropertyId = 1, PropertyAddress = "123 Maple St",
            InvoiceDate = dueDate.AddDays(4), DueDate = dueDate.AddDays(18), Status = "Overdue", Notes = "HVAC repair"
        };
        db.Invoices.Add(inv2);
        db.InvoiceLineItems.AddRange(
            new InvoiceLineItemDto { Id = 2, InvoiceId = 2, Description = "HVAC Labor", Type = "Labor", Quantity = 4, UnitPrice = 125m },
            new InvoiceLineItemDto { Id = 3, InvoiceId = 2, Description = "Refrigerant R-410A", Type = "Material", Quantity = 2, UnitPrice = 85m }
        );

        var evCase = new EvictionCaseDto
        {
            Id = 1, TenantId = 2, TenantName = "Bob Smith", PropertyId = 1,
            PropertyAddress = "123 Maple St", Reason = "Late Rent",
            CaseOpenDate = DateTime.Today.AddDays(-10), Status = "Open"
        };
        db.EvictionCases.Add(evCase);

        db.EvictionSteps.AddRange(
            new EvictionStepDto { Id = 1, CaseId = 1, Order = 1, Label = "Issue Late Rent Notice", TypicalTiming = "Day 1-3", ActionRequired = "Generate and send Late Rent Notice PDF", IsComplete = true, CompletedAt = DateTime.Today.AddDays(-10) },
            new EvictionStepDto { Id = 2, CaseId = 1, Order = 2, Label = "Issue Pay-or-Quit Notice", TypicalTiming = "Day 3-5", ActionRequired = "Generate Pay-or-Quit Notice; record delivery", IsComplete = false },
            new EvictionStepDto { Id = 3, CaseId = 1, Order = 3, Label = "Document Non-Payment Evidence", TypicalTiming = "Day 5-10", ActionRequired = "Attach rent ledger export and communication logs", IsComplete = false },
            new EvictionStepDto { Id = 4, CaseId = 1, Order = 4, Label = "Consult Attorney", TypicalTiming = "Day 7-10", ActionRequired = "Attorney contact notes", IsComplete = false },
            new EvictionStepDto { Id = 5, CaseId = 1, Order = 5, Label = "File Eviction Petition", TypicalTiming = "Day 10-14", ActionRequired = "Export full case document package", IsComplete = false },
            new EvictionStepDto { Id = 6, CaseId = 1, Order = 6, Label = "Court Hearing", TypicalTiming = "Per court schedule", ActionRequired = "Log hearing date and outcome", IsComplete = false },
            new EvictionStepDto { Id = 7, CaseId = 1, Order = 7, Label = "Possession / Resolution", TypicalTiming = "Per judgment", ActionRequired = "Close or resolve case", IsComplete = false }
        );

        db.EvictionNotices.Add(new EvictionNoticeDto { Id = 1, CaseId = 1, Type = "late-rent-notice", NoticeText = "Dear Bob Smith, your rent was due on the 1st of this month...", CreatedAt = DateTime.Today.AddDays(-10), DeliveryMethod = "Email" });

        db.EvictionTemplates.AddRange(
            new EvictionTemplateDto { Id = 1, Slug = "late-rent-notice", Title = "Late Rent Notice", Description = "Friendly first reminder", Body = "Dear {{TenantFullName}},\n\nThis is a friendly reminder that your rent payment of {{RentAmountDue}} was due on {{DueDate}} for the property at {{PropertyAddress}}.\n\nPlease remit payment as soon as possible to avoid further action.\n\nSincerely,\n{{LandlordName}}" },
            new EvictionTemplateDto { Id = 2, Slug = "pay-or-quit-3day", Title = "3-Day Pay or Quit Notice", Description = "Formal 3-day pay-or-quit notice", Body = "NOTICE TO PAY OR QUIT\n\nDate: {{NoticeDate}}\nTo: {{TenantFullName}}\nProperty: {{PropertyAddress}}\n\nYou are in default of rent in the amount of {{RentAmountDue}}, due on {{DueDate}}.\nYou must pay or vacate within THREE (3) DAYS (by {{QuitByDate}}).\n\n{{LandlordName}}" },
            new EvictionTemplateDto { Id = 3, Slug = "pay-or-quit-5day", Title = "5-Day Pay or Quit Notice", Description = "5-day variant", Body = "NOTICE TO PAY OR QUIT\n\nDate: {{NoticeDate}}\nTo: {{TenantFullName}}\nProperty: {{PropertyAddress}}\n\nYou have FIVE (5) DAYS from {{NoticeDate}} to pay {{RentAmountDue}} or vacate.\n\n{{LandlordName}}" },
            new EvictionTemplateDto { Id = 4, Slug = "cure-or-quit", Title = "Cure or Quit Notice", Description = "Lease violation remedy notice", Body = "NOTICE TO CURE OR QUIT\n\nTo: {{TenantFullName}}\nProperty: {{PropertyAddress}}\nDate: {{NoticeDate}}\n\nYou are in violation of your lease. Cure this violation or vacate the premises.\n\n{{LandlordName}}" },
            new EvictionTemplateDto { Id = 5, Slug = "unconditional-quit", Title = "Unconditional Quit Notice", Description = "Final notice before filing", Body = "UNCONDITIONAL NOTICE TO QUIT\n\nTo: {{TenantFullName}}\nProperty: {{PropertyAddress}}\nDate: {{NoticeDate}}\n\nYou are required to vacate the premises immediately. Failure to vacate will result in eviction proceedings.\n\n{{LandlordName}}" }
        );

        db.SaveChanges();
    }
}
