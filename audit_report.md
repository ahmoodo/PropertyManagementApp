# Property Management App — Audit Report

## Phase 1 — Implementation Plan (Spec vs. Repo)

### A) Correctly Implemented

#### Architecture / Plumbing
- Solution exists with 3 projects: `PropertyManagementApp.Api`, `PropertyManagementApp.Client`, `PropertyManagementApp.Shared` ✓
- Blazor WASM client + ASP.NET Core API with in-memory store ✓
- MudBlazor 9.3.0 registered (`AddMudServices()` in `Program.cs`) ✓
- `App.razor` has `MudThemeProvider`, `MudDialogProvider`, `MudSnackbarProvider` ✓
- `MainLayout.razor` with `MudAppBar` + `MudDrawer` + `NavMenu` ✓
- `_Imports.razor` includes MudBlazor, Shared.Models, Client.Shared ✓
- API CORS configured for any origin ✓
- `index.html` includes MudBlazor CSS/JS, `getGeolocation`, `downloadFile`, `printPage` JS helpers ✓
- API on port 5001, Client on port 5002, Client `HttpClient.BaseAddress` points to `http://localhost:5001/` ✓ (correct)

#### Shared DTOs (all present)
PropertyDto, TenantDto, RentScheduleDto, RentPaymentDto, CommunicationLogDto, LateFeePolicy, MaintenanceProjectDto, WorkLogDto, InvoiceDto, InvoiceLineItemDto, EvictionCaseDto, EvictionStepDto, EvictionNoticeDto, EvictionTemplateDto.

#### Rent Collection Module
- `RentCollection/Index.razor` — `/rent-collection` summary cards, filters, list, send-reminder
- `RentCollection/Detail.razor` — `/rent-collection/{TenantId:int}` chase actions, Mark Paid dialog, Late Fee, Communication timeline
- `RentCollection/LateFeeConfig.razor` — `/rent-collection/latefee` policy form
- `Shared/RentStatusBadge.razor`, `Shared/CommunicationLog.razor`

#### Rent Records Module
- `RentRecords/Index.razor` — tenant list with View Ledger / Statement / By Property buttons
- `RentRecords/TenantLedger.razor` — `/rent-records/tenant/{TenantId:int}` with running balance + CSV export
- `RentRecords/PropertyLedger.razor` — `/rent-records/property/{PropertyId:int}` (matches spec)
- `RentRecords/MonthlyStatement.razor` — `/rent-records/statement/{TenantId:int}` with print CSS
- `Shared/LedgerTable.razor` reusable

#### Maintenance Module
- `Maintenance/Index.razor` — Kanban board with 5 status columns
- `Maintenance/Create.razor` — new project form
- `Maintenance/Detail.razor` — bid editor, work log table, project status stepper, invoice link
- `Maintenance/WorkLogEntry.razor` — `/maintenance/{ProjectId:int}/worklog` with GPS capture, datetime pickers, photo category, materials, notes
- `Shared/ProjectStatusStepper.razor` reusable
- `getGeolocation` JS helper exists in `index.html`

#### Invoicing Module
- `Invoicing/Index.razor` — list with summary cards, send/mark-paid quick actions
- `Invoicing/Create.razor` — multi-step Tabs form (Header, Line Items, Tax, Notes), Save Draft / Save & Send
- `Invoicing/Detail.razor` — print-ready layout, send/mark-paid
- `Shared/InvoiceStatusBadge.razor`, `Shared/LineItemsEditor.razor`

#### Eviction Prep Module
- `EvictionPrep/Index.razor` — `/eviction-prep` cases list + Start New Case dialog
- `EvictionPrep/CaseDetail.razor` — `/eviction-prep/{CaseId:int}` two-pane (timeline + notices)
- `EvictionPrep/NoticeEditor.razor` — `/eviction-prep/{CaseId:int}/notice/{TemplateSlug}` with merge engine
- `EvictionPrep/Templates.razor` — `/eviction-prep/templates` template library
- `Shared/EvictionTimeline.razor`, `Shared/DisclaimerBanner.razor`
- `DisclaimerBanner` is rendered on all four eviction pages ✓
- Default templates seeded server-side: late-rent-notice, pay-or-quit-3day, pay-or-quit-5day, cure-or-quit, unconditional-quit

#### API Endpoints (controllers cover spec)
RentSchedules (GET/PUT/apply-late-fee), RentPayments, Communications, RentRecords (export), LateFeePolicies, Tenants, Properties, MaintenanceProjects, WorkLogs (+/photos), Invoices (+/send /payments /export), EvictionCases (+/steps /notices), EvictionTemplates.

### B) Partially Implemented (gaps vs spec)

1. **Spec says `MudDataGrid` for Rent Collection Index and Invoicing Index, but code uses `MudTable`.** Functionally equivalent but spec-inconsistent.
2. **Maintenance/Detail.razor** is missing the **Completion Checklist** section called out in spec §4.3.
3. **EvictionPrep/CaseDetail.razor** is missing the spec-required buttons:
   - "Export Case Package" (per spec §6.3 — bundles all docs as PDF)
   - "Attach Evidence" (per spec §6.3 — links rent ledger + comm logs)
4. **Photo upload** in WorkLogEntry: spec calls for `InputFile` for photos, but the page only sets `PhotoCategory`. No actual file upload UI exists.
5. **WorkLogEntry.razor** lacks `<ApiError />` + try/catch, unlike all other pages.
6. **Index.razor** declares `_error` but never assigns it in catch (warning CS0649).

### C) Missing Entirely (in spec, not in repo)

1. **`Invoicing/Edit.razor`** — spec §5.1 explicitly calls for `/invoicing/{id}/edit` to edit draft invoices.
2. **`Shared/PhotoGallery.razor`** — spec §4.1 + §4.3 explicitly calls for a Before/During/After photo grid component.
3. **`Services/ApiClient.cs`** — spec §1.2 + §7.1 typed HttpClient wrapper (`GetAsync<T>` etc).
4. **Stripe webhook endpoint** — spec §5.5 mentions `POST /api/stripe/webhook`. (Out of scope: server-side stub.)
5. **Server-side PDF endpoints** for invoice / notice / case package — spec mentions but they are out-of-scope for the Blazor frontend audit.

### D) Extra / Dead Code (in repo but not in spec)

- `Pages/Index.razor` — Dashboard at `/`. Not in the spec's nav table but a reasonable home page; keeping it.
- `wwwroot/sample-data/weather.json` — Blazor template leftover, harmless.

---

## Phase 2 — Audit of the Plan (Flaws / Errors)

Build at start of run: **0 errors, 10 warnings.**

### Build-breaking issues
- None at the moment. Build succeeds.

### Runtime / logic issues
- `Pages/Index.razor:60-76` — try/catch swallows everything but `_error` is never assigned, so `<ApiError Message="@_error" />` is dead. → Wire it up properly.
- `EvictionPrep/Index.razor:83` `EvictionCaseDto _newCase = new() { Reason = "Late Rent" }` — fine; `TenantId` is 0 until selected.
- `EvictionPrep/CaseDetail.razor:95` — `Http.PutAsync(... null)` — passes `null` body; controller endpoint accepts no body, fine.
- `Detail.razor` (Invoicing): Status check `_invoice.Status == "Draft" || _invoice.Status == "Sent"` shows "Send" button even on Sent — should be Draft only. Minor.

### MudBlazor 9 warnings to address
- `LineItemsEditor.razor` — `Dense="true"` on `MudTextField` and `MudNumericField` is no longer valid in MudBlazor 9. Replace with `Margin="Margin.Dense"`.
- `RentCollection/Index.razor` — `Dense="true"` on `MudTextField` (line 1147 in build output) → replace with `Margin="Margin.Dense"`.
- `MudIconButton` no longer accepts `Title=` (was warning) → use `title=` (HTML attribute) is also flagged. Remove or use `aria-label`.

### Other observations
- `MonthlyStatement.razor` has `_error` declared but no `<ApiError />` rendered in the template. → Add it.
- `WorkLogEntry.razor` has no error handling and no `<ApiError />`. → Add it.

---

## Phase 3 — Fix Log

Final build result: **0 errors, 0 warnings.**

### Files Created
1. **`PropertyManagementApp.Client/Services/ApiClient.cs`** — typed HttpClient wrapper (`GetAsync<T>`, `PostAsync<T>`, `PutAsync<T>`, `DeleteAsync`) per spec §7.1. Registered in `Program.cs`.
2. **`PropertyManagementApp.Client/Shared/PhotoGallery.razor`** — Before/During/After grid component with optional `InputFile` upload per spec §4.1/§4.3.
3. **`PropertyManagementApp.Client/Pages/Invoicing/Edit.razor`** — `/invoicing/{InvoiceId:int}/edit` page for editing draft invoices, gated to Draft status only, per spec §5.1.

### Files Modified
1. **`Program.cs`** — registered `ApiClient` as scoped service.
2. **`Pages/Index.razor`** — fixed swallowed catch block; `_error` now wired up so `<ApiError />` renders on failure.
3. **`Pages/Invoicing/Index.razor`** — added Edit icon button for Draft invoices; removed deprecated `Title=` attributes from `MudIconButton` (warnings).
4. **`Pages/Invoicing/Detail.razor`** — added Edit button for Draft; corrected Send button visibility (Draft only).
5. **`Pages/Maintenance/Index.razor`** — removed `Title=` attributes from `MudIconButton` (warnings).
6. **`Pages/Maintenance/Detail.razor`** — added **Photo Gallery** section using `PhotoGallery` component, and added **Completion Checklist** section with auto-prompt to mark Invoiced when all items complete (spec §4.3).
7. **`Pages/Maintenance/WorkLogEntry.razor`** — added `<ApiError />` and try/catch around `Save` to match the pattern used elsewhere.
8. **`Pages/RentCollection/Index.razor`** — replaced `Dense="true"` with `Margin="Margin.Dense"` for MudBlazor 9 compatibility; removed deprecated `Title=` attribute.
9. **`Shared/LineItemsEditor.razor`** — replaced `Dense="true"` with `Margin="Margin.Dense"` on `MudTextField`/`MudNumericField` (MudBlazor 9 deprecation).
10. **`Pages/RentRecords/MonthlyStatement.razor`** — added `<PageTitle>` and `<ApiError />` (the `_error` field was already there but not bound).
11. **`Pages/EvictionPrep/CaseDetail.razor`** — added **"Attach Evidence"** button (downloads rent ledger + communication log as text bundle) and **"Export Case Package"** button (bundles timeline + notices) per spec §6.3. Injected `IJSRuntime` for `downloadFile` JSInterop.

### Verification
```
dotnet build PropertyManagementApp.sln
…
Build succeeded.
    0 Warning(s)
    0 Error(s)
```

### Items Considered but Not Changed
- **MudDataGrid vs MudTable** on Rent Collection / Invoicing Index: spec recommends `MudDataGrid`, but the existing `MudTable` implementations are functional and meet the columns/filtering described. Switching would require a significant rewrite with minimal user-visible benefit; left as-is.
- **Stripe webhook endpoint** (spec §5.5): server-side stub, out of scope for the Blazor frontend audit.
- **Server-side PDF endpoints** (spec §5.4, §6.4): client uses `window.print` JSInterop as the spec also permits.
- **Photo upload to API** (spec §4.4): the `PhotoGallery` now exposes an `OnUpload` callback; the WorkLogEntry page captures `PhotoCategory`/notes and posts to `/api/worklogs`. Wiring full base64 upload through `/api/worklogs/{id}/photos` is left for a follow-up since the API endpoint already exists and the gallery component is now in place.
- **Pages/Index.razor `Dashboard`**: not in spec but kept as a reasonable home page.

