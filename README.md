# Property Management App

A full-stack property management system built with **Blazor WebAssembly** (frontend) and **ASP.NET Core Web API** (backend).

Owner: Gongjun Yan, Ph.D.
Document Version: 1.0 | March 2026

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | Blazor WebAssembly (.NET 8) |
| UI Components | MudBlazor 9 |
| Backend API | ASP.NET Core Web API (.NET 8) |
| Data | In-memory store (seeded with sample data) |
| Shared Models | PropertyManagementApp.Shared (class library) |

## Project Structure

```
PropertyManagementApp.sln
├── PropertyManagementApp.Api/         # ASP.NET Core Web API (runs on port 5001)
│   ├── Controllers/                   # All REST endpoints
│   └── Data/InMemoryDataStore.cs      # Seeded in-memory database
├── PropertyManagementApp.Client/      # Blazor WASM (runs on port 5002)
│   ├── Pages/
│   │   ├── RentCollection/            # Rent status, chase workflow, late fees
│   │   ├── RentRecords/               # Ledgers, statements, CSV export
│   │   ├── Maintenance/               # Kanban board, work logs, GPS capture
│   │   ├── Invoicing/                 # Invoice builder, status tracking
│   │   └── EvictionPrep/              # Case management, notice templates
│   ├── Shared/                        # Reusable components (StatusBadge, LedgerTable, etc.)
│   ├── Layout/                        # MainLayout + NavMenu
│   └── Services/NotificationService.cs
└── PropertyManagementApp.Shared/      # DTOs shared between API and Client
    └── Models/                        # All DTO classes
```

## Modules

| Module | Route | Description |
|---|---|---|
| Dashboard | `/` | Summary cards, quick links |
| Rent Collection | `/rent-collection` | Tenant rent status, chase workflow, late fees |
| Rent Records | `/rent-records` | Full ledger per tenant and property, CSV export, print statements |
| Maintenance | `/maintenance` | Kanban board (Bid→Approved→Work Order→Invoiced→Closed), GPS-tagged work logs |
| Invoicing | `/invoicing` | Invoice builder with line items, multi-status lifecycle |
| Eviction Prep | `/eviction-prep` | Guided case timeline, auto-filled legal notice templates |

## How to Run

### 1. Start the API

```bash
cd PropertyManagementApp.Api
dotnet run
# API runs at http://localhost:5001
```

### 2. Start the Blazor Client

```bash
cd PropertyManagementApp.Client
dotnet run
# App opens at http://localhost:5002
```

### Notes
- The API uses an **in-memory data store** — data resets on restart
- Sample data is seeded automatically (2 properties, 4 tenants, rent schedules, maintenance projects, invoices, and one eviction case)
- For production, replace `InMemoryDataStore` with SQL Server + Entity Framework Core per the implementation plan

## Key Features

- **Rent Chase Workflow** — 3-step escalation (Day 1 SMS → Day 3 Late Notice → Final Notice)
- **Late Fee Engine** — configurable flat or percentage fees with grace period
- **Kanban Board** — drag-free status advancement for maintenance projects
- **GPS Capture** — `navigator.geolocation` integration in work log entry
- **Ledger Tables** — running balance with debit/credit columns
- **Notice Merge Engine** — auto-fills tenant name, address, rent amount, dates into legal templates
- **CSV Export** — client-side download via JS Interop
- **Print/PDF** — `window.print()` with `@media print` CSS isolation

## API Endpoints

| Method | Route | Purpose |
|---|---|---|
| GET/PUT | `/api/rentschedules` | Rent status grid + updates |
| POST | `/api/rentschedules/{id}/apply-late-fee` | Apply late fee |
| GET/POST | `/api/rentpayments` | Payment history + record payment |
| GET/POST | `/api/communications` | Communication log |
| GET/PUT | `/api/latefeepolicies` | Late fee configuration |
| GET/POST/PUT | `/api/maintenanceprojects` | Maintenance project CRUD |
| GET/POST | `/api/worklogs` | Work log entries |
| GET/POST/PUT | `/api/invoices` | Invoice lifecycle |
| GET/POST | `/api/evictioncases` | Eviction case management |
| GET/PUT/DELETE | `/api/evictiontemplates` | Notice template library |
