# StockFlow

StockFlow is an inventory management system for a (Dutch) hardware store. It consists of a REST API, a Blazor WebAssembly management app for the office and a WinForms cash register for the shop floor. Every change to stock, employees and customers is written to an audit log.

---

## Contents

1. [Solution structure](#solution-structure)
2. [Tech stack](#tech-stack)
3. [Getting started](#getting-started)
4. [Seed data and logins](#seed-data-and-logins)
5. [Identifiers](#identifiers)
6. [Roles and permissions](#roles-and-permissions)
7. [Management app](#management-app)
8. [Cash register](#cash-register)
9. [API](#api)
10. [Validation rules](#validation-rules)
11. [Audit logging](#audit-logging)
12. [Database and migrations](#database-and-migrations)
13. [Configuration](#configuration)
14. [Styling conventions](#styling-conventions)
15. [Tests](#tests)
16. [CI](#ci)
17. [Releases and deployment](#releases-and-deployment)
18. [Known limitations](#known-limitations)

---

## Solution structure

| Project | Type | Responsibility |
|---|---|---|
| `StockFlow.Domain` | Class library | Entities (`Product`, `ProductBrand`, `Shelf`, `Sales`, `Employee`, `Customer`, audit logs, `RegisterItemDetail`) and enums. Depends on nothing. |
| `StockFlow.Application` | Class library | Services, mappings (`Extensions/`), validations (`Validations/`), repository and service interfaces. Depends on Domain and Shared. |
| `StockFlow.Shared` | Class library | Everything that crosses a process boundary: request/response DTOs (`DTOs/`), DTO enums, pagination types and order-by enums, `ApiResponse<T>`, and the rules both server and cash register need (`Common/`: `IdentifierFormats`, `LoyaltyPoints`, `ReturnRules`). References nothing. |
| `StockFlow.Infrastructure` | Class library | EF Core `ApplicationDBContext` (SQLite), repositories, migrations, seeders, BCrypt password hashing. |
| `StockFlow.API` | ASP.NET Core Web API | Controllers, JWT authentication, `ErrorController`, Swagger. |
| `StockFlow.ManagementApp` | Blazor WebAssembly | Office application: inventory, brands, sales, shelves, logs, employees and customers. |
| `StockFlow.CashRegister` | WinForms (.NET, Windows) | Point of sale: scan products, attach customers, take payment, take returns. |
| `StockFlow.UnitTests` | xUnit + Moq test project | Tests for every service, validator, mapping, shared rule, repository (in-memory SQLite) and the main controllers. |

The solution file is `StockFlow.slnx` (folders `Backend` and `Frontend`; `StockFlow.Shared` is under `Backend`).

```
StockFlow.Domain          ──► (nothing)
StockFlow.Shared          ──► (nothing)
StockFlow.Application     ──► Domain, Shared
StockFlow.Infrastructure  ──► Application, Domain
StockFlow.API             ──► Application, Infrastructure, Shared
StockFlow.ManagementApp   ──► Shared
StockFlow.CashRegister    ──► Shared
StockFlow.UnitTests       ──► API, Application, Domain, Infrastructure, Shared
```

---

## Tech stack

- .NET 10 / C#
- ASP.NET Core Web API with JWT bearer authentication
- Entity Framework Core 10 with SQLite
- BCrypt.Net-Next for password hashing
- Blazor WebAssembly (management app), Bootstrap Icons
- Windows Forms (cash register)
- Swashbuckle (Swagger UI)

---

## Getting started

### Prerequisites

- .NET 10 SDK
- Visual Studio with .NET 10 support and the ASP.NET and Windows desktop workloads. The cash register is a Windows-only WinForms app.

### Run

1. Open `StockFlow.slnx` in Visual Studio.
2. Set multiple startup projects: `StockFlow.API`, `StockFlow.ManagementApp` and/or `StockFlow.CashRegister`.
3. Start with the **https** profiles.

| App | URL |
|---|---|
| API | https://localhost:7037 (http://localhost:5077) |
| Swagger UI | https://localhost:7037/api |
| Management app | https://localhost:7158 (http://localhost:5120) |
| Cash register | Desktop window, talks to `https://localhost:7037/` |

On startup the API applies all pending migrations and runs the seeders (`Database.Migrate()` in `Program.cs`). The database file is created at `StockFlow.Infrastructure/Data/app.db`. Seeders skip tables that already contain data, so restarting is safe.

---

## Seed data and logins

### Employees

All employee passwords are **`001273`**.

| Employee Id | Role |
|---|---|
| 000001 | Admin |
| 000002 | Manager |
| 000003 – 000005 | ShiftLeader |
| 000006 – 000009 | ServiceDesk |
| 000010 – 000012 | Cashier |
| 000013 – 000022 | FloorWorker |

### Customers

15 customers with customer ids **80000001 – 80000015** (Dutch names, postal codes, house numbers and some saved points). They share the same password hash as the employees (`001273`), so every seeded customer can log in through `Customer/Login` with their email and `001273`. New customers can sign up through `Customer/Register` and start with **250 welcome points** (€2.50).

### Stock

- 150 products with barcodes **500000001 – 500000150**
- 25 brands
- 15 shelves. Product locations follow `meterNumber * 1000 + productNumberInMeter`.
- 2 sales: *Paint Promotion* (10 % off, tag `paint`) and *Hardware Promotion* (5 % off, tag `hardware`). Products are linked to sales through matching sales tags.

---

## Identifiers

All rules live in `StockFlow.Shared/Common/IdentifierFormats.cs` and are shared by the API, the cash register and the seeders.

| Identifier | Format | Example |
|---|---|---|
| Product barcode | 9 digits | `500000001` |
| Customer id | 8 digits, starts with `8` | `80000001` |
| Receipt id | 10 digits, starts with `9` | `9000000001` |
| Employee id | 6 digits | `000001` |

- New products get the next barcode after the highest existing one.
- New employees get the next free employee id.
- Every cash register log (receipt) gets the next receipt id after the highest existing one, starting at `9000000001`. Receipt ids are unique.
- On the cash register the **number of digits decides** what was scanned: 9 digits is a product, 8 digits (starting with 8) is a customer, 10 digits (starting with 9) is a receipt for a return.

---

## Roles and permissions

Roles: `Admin`, `Manager`, `ShiftLeader`, `ServiceDesk`, `Cashier`, `FloorWorker` (employees) and `Customer` (given to customers when they log in or register).

| Area | Who |
|---|---|
| View inventory, brands, sales, shelves | Every logged-in employee |
| Create / update / delete products, brands, sales, shelves | Manager, Admin |
| Cash register (scan, look up customers, purchase) | Cashier, ServiceDesk, ShiftLeader, Manager, Admin |
| Cash register logs | Cashier, ServiceDesk, ShiftLeader, Manager, Admin |
| Stock logs, product history | ServiceDesk, ShiftLeader, Manager, Admin |
| Employee logs | ServiceDesk, ShiftLeader, Manager, Admin (menu: Manager, Admin) |
| Customer logs | Manager, Admin |
| View employees | ShiftLeader, Manager, Admin |
| Create / update employees | Manager, Admin |
| View customers | ShiftLeader, Manager, Admin |
| Update customers | Manager, Admin |
| Register a customer account | Anyone (no login) |
| View own customer account (`Customer/Me`), change own customer password | Customer |
| Look up customers or employees by id / email through the API | Employees only (a customer token is refused) |

---

## Management app

Blazor WebAssembly app (`StockFlow.ManagementApp`). Log in with an employee id and password; the sidebar only shows the pages your role can use.

| Page | Route | Features |
|---|---|---|
| Releases | `/` (home, also the version link at the bottom of the sidebar) | The versions of the management app and the API next to the latest release (*Up to date* / *Update available*), a download card for the newest cash register zip, and every GitHub release with its notes, badges (*Latest*, *Pre-release*, *Running here*) and download links. |
| Inventory | `/inventory` | Paged and sortable product list with filters. Click a row for the product details panel (image, price incl. sale, location, stock, tags, brand, description). Create, edit and delete for managers. In the create / edit form, brand and shelf are searchable selects and sales tags are a searchable multi-select that shows which sale each tag belongs to. |
| Brands | `/productbrands` | List, create, edit, delete brands. |
| Sales | `/sales` | List, create, edit, delete sales (percentage off + sales tags). Sales tags are a searchable multi-select with the tags already in use; new tags can be added by typing them. |
| Shelves | `/shelves` | List, create, edit, delete shelves. |
| Cash register logs | `/cashregisterlogs` | Every purchase with its receipt id, customer and employee; filterable by receipt id. The products column also shows how many items were returned. Click **Products** to see what was sold, the amounts, how many were returned per product, the price per item, applied sales, the discount and the total. Click a product to open its details. |
| Stock logs | `/stocklogs` | Every create / update / sale / return / delete of products, brands, sales and shelves. **Open message** shows what changed (old → new). |
| Product history | `/producthistory?barcode=…` | Enter a barcode to see the product, current stock, units sold, units returned, units added and the full history. Toggle between all changes and stock changes only. The barcode is kept in the URL so searches can be bookmarked or linked. |
| Customer logs | `/customerlogs` | Customer changes, filterable by customer id, name, email and log type. |
| Employee logs | `/employeelogs` | Employee changes and logins. |
| Employees | `/employees` | List, create (with password), edit (optional password reset). The role is a searchable select. |
| Customers | `/customers` | Customer list with customer id, filterable and sortable. |

Forms are generated by the reusable `EditItem` component from the DTO type. Validation errors from the API are shown above the form and the form stays open.

**Updates only with changes:** when an edit form opens (any DTO with a non-empty `Id`), `EditItem` takes a snapshot of the editable fields. **Save** stays disabled with the hint *No changes to save.* until something actually differs from that snapshot, and a save without differences is never sent to the API. The comparison ignores leading/trailing spaces, the order of tags, empty tags and trailing zeros in prices (12.50 = 12.5), so typing a value back to what it was counts as no change. While you are typing in a text field Save is clickable, because the value is only applied when the field loses focus; the check runs again when you click it. Create forms are not affected.

---

## Cash register

WinForms app (`StockFlow.CashRegister`), Dutch UI.

### Flow

1. Log in with an employee id and password.
2. Scan or type a **product barcode** (9 digits) or a **customer id** (8 digits, starts with 8) in the same input and press Enter.
3. Optionally attach a customer. **Sales only apply when a customer is attached.** Attaching a customer reprices the receipt with each product's best active sale; removing the customer restores the normal prices.
4. Optionally press **Punten gebruiken [F4]** to pay part of the receipt with the customer's saved points.
5. Press **Afrekenen [F10]**, choose PIN or cash, complete with **F12**. The confirmation shows the receipt id.

On completion the stock of every product is lowered by the sold amount (stock may go negative), a cash register log is written, and a stock log entry of type *Sold* is written per product.

### Loyalty points

- **Earning:** a customer attached to a purchase earns **1 point per whole euro paid**, rounded down: €1.99 gives 1 point, €2.00 gives 2. "Paid" is the receipt total after sales and after points used, so points spent don't earn new points. The server calculates this from the receipt lines, stores it as `PointsEarned` on the cash register log and adds it to the customer's saved points. Walk-in purchases earn nothing.
- **Welcome points:** every new account made through `Customer/Register` starts with 250 points (`LoyaltyPoints.WelcomePoints`). The *Create* entry in the customer logs mentions them.
- **Spending:** **100 points = €1**, so 1 point = €0.01. The rules live in `StockFlow.Shared/Common/LoyaltyPoints.cs`.
- **All or nothing, no minimum:** pressing F4 spends as many points as possible: all of the customer's points, or exactly the receipt total if the customer has more. The cashier can't pick a smaller amount. Examples: 1,250 points on an €8.40 receipt uses 840 points and leaves €0.00 to pay (410 points stay); 50 points on a €10.00 receipt uses all 50 (−€0.50); any number of points can pay a €0.05 receipt (5 points).
- When the points cover the whole receipt, the payment screen can be completed without choosing PIN or cash.
- The button is enabled when a customer with at least 1 point is attached. Pressing F4 again turns it off; changing or removing the customer resets it.
- The summary shows `Punten: -€x` and `Spaart: +n punten`; the payment screen shows the points used and the points that will be earned next to the customer, and the confirmation shows the points earned.
- The server checks everything again (see [Validation rules](#validation-rules)), applies `earned − used` to the customer's saved points in one step and stores `PointsUsed` and `PointsEarned` on the cash register log.
- Every purchase where points were used or earned writes one customer log entry (type *Update*) with `SavedPoints: New: x. Old: y` (or `SavedPoints: Unchanged (x)` when used and earned cancel out), `Receipt: <receipt id>`, `PointsUsed: n (-€x)` and `PointsEarned: n`. Open it on the customer logs page with **Open message**.
- The cash register logs page has a **Points** column (`-used` in red, `+earned` in green), and the products overlay shows the points earned, the points used and their euro value.

### Returns

Items can only be returned with the receipt.

1. With an empty receipt on screen, type or scan the **receipt id** (10 digits, starts with 9) and press Enter. The register loads that receipt and switches to return mode (the card shows `RETOUR`, the receipt id, date and customer).
2. Scan every item that comes back. Only products that are on that receipt are accepted, and never more than `bought − already returned` per product. F2 changes the amount within that limit, F6 removes a line.
3. Press **Retour afronden [F10]** and confirm. The refund amount is shown.
4. F3 or F5 stops the return without changing anything.

When a return is completed the server checks everything again, raises `ReturnedAmount` on the receipt lines (so the same items can't be returned twice), raises the stock of each product, writes a stock log entry of type *Returned* per product and takes back the points earned on the returned items.

- **Refund**: the price per item on the receipt (including sales). If points were used on the receipt, the refund is reduced in the same proportion as the points reduced the total (`ReturnRules` in `StockFlow.Shared/Common/ReturnRules.cs`), so points are never paid out as cash. Points are not given back.
- **Points on returns:** the points earned on the receipt are recalculated for the items the customer keeps (1 per whole euro of the value kept), and the difference is taken off the customer's saved points (never below 0). `PointsEarned` on the receipt is lowered to match, so returning in several steps never takes off more than was earned. The confirmation shows how many points were taken off, and the customer log entry contains `SavedPoints: New: x. Old: y`, `Return: Receipt <receipt id>` and `PointsDeducted: n`.
- Customer ids, other receipts, parking and points can't be used while in return mode.
- A receipt whose items have all been returned can't be opened for a return again.

### Keys

| Screen | Key | Action |
|---|---|---|
| Scan | F1 | Focus the input |
| Scan | F2 | Change quantity |
| Scan | F3 | Remove customer (in return mode: stop the return) |
| Scan | F4 | Use / stop using saved points |
| Scan | F5 | Cancel receipt or return |
| Scan | F6 | Delete line |
| Scan | F7 | Park receipt (customer is parked too) |
| Scan | F8 | Recall parked receipt |
| Scan | F10 | Checkout (in return mode: complete the return) |
| Payment | F1 / F2 | PIN / cash |
| Payment | F12 | Complete |
| Payment | Esc | Back |

---

## API

Base URL `https://localhost:7037`, Swagger at `/api`. All endpoints except the logins and `Customer/Register` require a JWT (`Authorization: Bearer <token>`).

### Response format

Every response, including errors, uses the same envelope (`StockFlow.Shared/Common/ApiResponse.cs`):

```json
{
  "value": { },
  "succeeded": true,
  "message": "Shelf created."
}
```

| Situation | Status | Body |
|---|---|---|
| Success | 200 | `ApiResponse<T>.Ok(value, message)` |
| Validation error (`ArgumentException`) | 400 | `Fail(message)` |
| Invalid body / query (model binding) | 400 | `Fail(first validation message)` |
| Not found (`KeyNotFoundException`) | 404 | `Fail(message)` |
| Not logged in / wrong login | 401 | `Fail("You are not logged in.")` or a login error |
| Wrong role | 403 | `Fail("You don't have access to this function.")` |
| Unhandled exception | 500 | `Fail("An unexpected error occurred.")` via `ErrorController` (`/error`) |

Both clients read this envelope in their `HttpService.ReadAnswer`.

### Endpoints

| Controller | Endpoints |
|---|---|
| `Product` | `GET GetAll`, `GET GetById/{id}`, `POST Create`, `PUT Update`, `DELETE SoftDelete/{id}` |
| `ProductBrand` | `GET GetAll`, `POST Create`, `PUT Update`, `DELETE SoftDelete/{id}` |
| `Sales` | `GET GetAll`, `POST Create`, `PUT Update`, `DELETE SoftDelete/{id}` |
| `Shelf` | `GET GetAll`, `POST Create`, `PUT Update`, `DELETE SoftDelete/{id}` |
| `Employee` | `GET GetAll`, `GET GetById/{id}`, `GET GetByEmployeeId/{employeeId}`, `POST Create`, `PUT Update`, `POST Login` |
| `Customer` | `GET GetAll`, `GET GetById/{id}`, `GET GetByEmail/{email}`, `GET Me`, `PUT Update`, `POST ChangePassword`, `POST Login`, `POST Register` |
| `CashRegister` | `GET {barcode}`, `GET Customer/{customerId}`, `GET Receipt/{receiptId}`, `POST Purchase` (returns the receipt id), `POST Return` (returns receipt id, items returned, refund amount and points deducted) |
| `StockLogs` | `GET GetAll`, `GET ProductHistory/{barcode}`, `POST Create` |
| `CashRegisterLogs` | `GET GetAll` (filters include `receiptId`), `POST Create` (returns the receipt id) |
| `CustomerLogs` | `GET GetAll`, `POST Create` |
| `EmployeeLogs` | `GET GetAll`, `POST Create` |
| `Releases` | `GET` (employees: all GitHub releases with notes and assets), `GET Latest` (anonymous: newest non-pre-release, used by the cash register's update check), `GET Version` (anonymous: the running API version) |

List endpoints take paging via the query string: `pageNumber`, `pageSize`, `orderBy`, `orderType` plus the filters of that list (for example `customerId`, `customerName`, `email`).

---

## Validation rules

Validations are extension methods in `StockFlow.Application/Validations` that throw `ArgumentException`. Services call them before doing any work.

| Subject | Rules |
|---|---|
| Paging | Page number ≥ 1, page size 1 – 100 |
| Product | Name and description required, price 0 – 100,000 with at most 2 decimals, location ≥ 0, image ≤ 5 MB, brand and shelf must exist. Stock may be negative. |
| Brand | Name required |
| Shelf | Name and location required |
| Sale | Name and description required, 1 – 99 % off, at least one sales tag |
| Employee | Name required, a real role (not `None`), password 4 – 12 digits on create, optional new password (same rule) on update |
| Employee login | Employee id digits only, password required |
| Customer | Name required, valid email, Dutch postal code (`1234 AB`), saved points ≥ 0 |
| Customer registration | Name at least 2 characters, valid email that is not in use yet, password and confirmation must match and follow the customer password rule, Dutch postal code (stored as `1234 AB`), house number like `12`, `12A` or `12bis` |
| Customer password | At least 8 characters with upper case, lower case, digit and special character; new and confirm must match and differ from the current password |
| Purchase | At least one line, amount 1 – 999 per line, no negative prices or totals, the customer must exist, sales only with a customer; points not negative, only with a customer, not more than the customer has, not worth more than the receipt, and all or nothing (exactly `min(customer points, receipt total in cents)`) |
| Stock change | Not 0 |
| Product history | Barcode must be 9 digits |
| Return | Receipt id must be 10 digits starting with 9 and exist, at least one item, amount 1 – 999 per item, every product must be on the receipt, never more than bought minus already returned |

### Passwords

- Passwords are only ever stored as BCrypt hashes (`IPassword.Hash`).
- Update DTOs never contain a password hash.
- Employees get a password on create; a manager can reset it with the optional **New password** field when editing.
- Customers change their own password through `Customer/ChangePassword`.
- Customers log in with their email address (case-insensitive) and password. The token carries the `Customer` role, the customer's id, name, email and customer id.
- Accounts without a password hash cannot log in.

---

## Audit logging

| Log | Written when |
|---|---|
| Stock logs | Product, brand, sale or shelf is created, updated or deleted; a product is sold (*Sold*) or returned (*Returned*) |
| Cash register logs | A purchase is completed (receipt id, lines, prices at time of sale, applied sales, total, discount, points used, points earned, customer). A return raises `ReturnedAmount` on the lines of the original receipt. |
| Employee logs | Employee created, updated, logged in |
| Customer logs | Customer registered, logged in, updated, password changed, saved points changed at the register (purchase or return) |

- Update logs contain the changed fields, built by `DifferenceFinder` *before* the change is applied, in the form `Field: New: x. Old: y`.
- Navigation properties (`Shelf`, `Brand`, …) are skipped because the matching id (`ShelfId`, `BrandId`) is already compared.
- Passwords are never written to a log; a password change is logged as `Password: Changed.`.

---

## Database and migrations

- SQLite database at `StockFlow.Infrastructure/Data/app.db`; migrations in `StockFlow.Infrastructure/Data/Migrations`.
- The API migrates and seeds automatically on startup.
- `AddPointsUsed` adds the `PointsUsed` column (default 0) to `CashRegisterLogs`.
- `AddReceiptsAndReturns` adds `ReceiptId` (unique) to `CashRegisterLogs` and `ReturnedAmount` (default 0) to `RegisterItemDetails`. Existing logs get receipt ids from `9000000001` upwards in order of creation.
- `AddPointsEarned` adds the `PointsEarned` column (default 0) to `CashRegisterLogs`.

Adding a migration in the Package Manager Console (set **Default project** to `StockFlow.Infrastructure`, startup project `StockFlow.API`):

```powershell
Add-Migration <Name>
Update-Database
```

Or with the CLI from the solution folder:

```bash
dotnet ef migrations add <Name> --project StockFlow.Infrastructure --startup-project StockFlow.API --output-dir Data/Migrations
dotnet ef database update --project StockFlow.Infrastructure --startup-project StockFlow.API
```

Seeders: `EmployeeSeeder`, `CustomerSeeder`, `ShelfSeeder`, `ProductBrandSeeder`, `ProductSeeder`, `SalesSeeder` (registered in `ApplicationDBContext.OnConfiguring` through `UseSeeding`).

---

## Configuration

`StockFlow.API/appsettings.json`:

| Key | Meaning |
|---|---|
| `DataSource` | SQLite file name (default `app.db`) |
| `DataFolder` | Folder for the database. Empty = `StockFlow.Infrastructure/Data` (local development); the Docker image sets `/app/data`. |
| `Cors:AllowedOrigins` | Browser origins allowed to call the API (default `https://localhost:7158`) |
| `JWT:Issuer` | Token issuer |
| `JWT:Key` | Signing key; keep it secret and long enough for HMAC-SHA256 |
| `JWT:ExpiryMinutes` | Token lifetime |
| `Releases:Repository` | GitHub repository the releases page reads (`owner/name`, default `jochemKok132/StockFlow`) |
| `Releases:Token` | Optional GitHub token. Only needed if the repository is private (a fine-grained token with read access to *Contents*); it also raises GitHub's rate limit |
| `Releases:CacheMinutes` | How long the API keeps the release list before asking GitHub again (default 10). If GitHub is unreachable the last known list is used |

- Every key can be overridden with an environment variable using `__`, for example `JWT__Key` or `Cors__AllowedOrigins__0`. Production secrets go in environment variables, never in `appsettings.json`.
- **Management app:** the API address is `ApiBaseUrl` in `wwwroot/appsettings.json` (`/stockflow/api/`; the Docker image writes this file from the `API_BASE_URL` build argument) and `wwwroot/appsettings.Development.json` (`https://localhost:7037/`, used by the dev server).
- **Cash register:** the API address is `ApiBaseUrl` in `appsettings.json` next to the `.exe` (default `https://localhost:7037/`), read by `RegisterSettings`. The file is not bundled into the single-file exe, so it can be edited after unzipping.

---

## Styling conventions

The management app follows a house style (`wwwroot/css/app.css` plus scoped `.razor.css` files):

- Every color is a CSS variable on `:root` (`--TaskbarColor`, `--DashboardColor`, `--AccentColor`, …); no raw colors in component rules.
- Units are `px` and `%` only.
- Corners are at most 4 px (`--RadiusSmall` 2 px, `--RadiusLarge` 4 px).
- No transitions or transforms; flat surfaces with 1 px borders.
- Light color scheme.
- Shared building blocks: `.card`, `.tasks-table`, `.filters-bar`, `.pagination`, `.modal-overlay` / `.modal-box` / `.drawer`, `.panel-header`.
- Searchable selects live in `Components/`: `SearchableSelect` (one value) and `SearchableMultiSelect` (chips, optional custom values), both fed with `SelectOption(Value, Label, Detail)`. Type to filter on label and detail, arrow keys to move, Enter to pick, Esc to close, Backspace on an empty multi-select removes the last chip.
- `EditItem` picks the control per property: enums always become a `SearchableSelect`; a property listed in its `Lookups` parameter (for example `BrandId`, `ShelfId`) becomes a `SearchableSelect`; string collections (`SalesTags`) become a `SearchableMultiSelect`. Lookup lists are loaded by `ILookupService` (`Services/LookupService.cs`), which pages through the brands, shelves and sales endpoints (100 per page) and skips soft-deleted items.
- Buttons: `.btn-save` (primary), `.btn-cancel` / `.btn-edit` (secondary), `.btn-delete` (destructive), `.btn-icon` (close).

The cash register mirrors the same palette in `Screens/Theme.cs`.

---

## Tests

`StockFlow.UnitTests` (xUnit + Moq, about 300 tests). Run with `dotnet test` from the repository root or the Test Explorer. The release workflow runs them before anything is published.

| Folder | Covers |
|---|---|
| `Services/` | Every Application service with mocked repositories: `CustomerService` (login, registration with welcome points, password change, update), `EmployeeService` (create, login, update, password reset), `ProductService`, `ProductBrandService`, `SalesService`, `ShelfService` (create, update, soft delete, not-found paths, sales/product re-linking), `RegisterDataService` (purchases, stock changes, points earned/used, returns, refunds, points taken back) and `AuthenticationService` (JWT claims, expiry, missing configuration) |
| `Services/AuditLogs/` | `CashRegisterLogsService` (receipt ids), `CustomerLogsService`, `EmployeeLogsService`, `StockLogsService` (product history) |
| `Validations/` | All validation rules: customers, employees, passwords, products, sales, shelves, brands, pagination, logs, purchases and returns |
| `Mappings/` | `DifferenceFinder` (log messages), enum mappings, entity ↔ DTO mappings |
| `Common/` | Shared rules: `LoyaltyPoints`, `ReturnRules`, `IdentifierFormats`, `AppVersion` (display, comparing versions) |
| `Infrastructure/` | `GitHubReleaseService` against a fake GitHub (mapping, drafts skipped, cash register asset, caching, stale fallback, errors), BCrypt `Password`, and the repositories against a real in-memory SQLite database (new customer / employee / receipt ids, barcodes, tag matching, soft delete, receipt filter, unique receipt ids) |
| `Api/` | Controllers: status codes and `ApiResponse` envelopes for the cash register, customer, employee and releases endpoints (200 / 400 / 401 / 404 / 503) |

Conventions: one `Mock<T>` per constructor dependency, system under test in `_sut`, names like `MethodName_Scenario_ExpectedResult`, Arrange / Act / Assert blocks, `[Theory]` for input ranges. `Helpers/TestData.cs` builds the sample entities. Repository tests create only the tables (`CreateTables()`), so the seeders do not run and every test starts with an empty database.

The Blazor management app and the WinForms cash register have no automated UI tests; their logic lives in the services and shared rules that are tested here.

---

## CI

`.github/workflows/ci-build-test.yml` runs on pull requests to `master` and on pushes to `master`: restore, build (Release) and test with .NET 10. The cash register project sets `EnableWindowsTargeting`, so the whole solution also builds on the Linux runner.

The standard Visual Studio `.gitignore` ignores every folder named `Releases` or `bootstrap`. Source folders with those names are re-included at the bottom of `.gitignore` (`StockFlow.Shared/DTOs/Releases/`, `StockFlow.Infrastructure/Releases/` and `StockFlow.ManagementApp/wwwroot/lib/bootstrap/`). If you add another source folder with an ignored name, add a matching `!` rule, or CI will fail on missing types.

---

## Releases and deployment

Versions are controlled with **GitHub Releases**. Publishing a release starts `.github/workflows/release.yml`:

1. **Build and test**: restore, build and run all unit tests. If anything fails, nothing is published.
2. **Docker images**: built from `StockFlow.API/Dockerfile` and `StockFlow.ManagementApp/Dockerfile` and pushed to the GitHub Container Registry:
   - `ghcr.io/<owner>/stockflow-api`
   - `ghcr.io/<owner>/stockflow-web` (the management app as static files in a small nginx container)

   Tags: the version (`1.2.0`), the minor line (`1.2`) and `latest` (not for pre-releases).
3. **Cash register**: published on a Windows runner as a self-contained single-file `.exe` (no .NET install needed), zipped and attached to the same release as `StockFlow.CashRegister-v1.2.0-win-x64.zip`.

### Making a release

1. Merge everything into `master`.
2. On GitHub: **Releases → Draft a new release → Choose a tag**, type a new tag like `v1.2.0` (always `v` + major.minor.patch), target `master`, write the notes and click **Publish release**.
3. Follow the progress under **Actions**. When it is done, the images are under **Packages** and the zip is on the release page.

Mark a release as **pre-release** to test it: it gets its version tags but does not move `latest`.

### Versions

- The version comes from the release tag: the workflow builds with `-p:Version=1.2.0` for `v1.2.0`. Local builds use `0.0.0-dev` from `Directory.Build.props` and show as *dev build*.
- `AppVersion` (`StockFlow.Shared/Common/AppVersion.cs`) reads the version from the assembly and compares versions.
- **Management app:** the sidebar shows `StockFlow v1.2.0` (links to the releases page); the releases page shows the management app and API versions.
- **Cash register:** the window title, the login screen and the status bar show the version. On the login screen it asks the API for the latest release (`Releases/Latest`) and shows *Nieuwe versie … beschikbaar* when a newer version exists.
- The releases page and the update check read GitHub through the API (`GitHubReleaseService`), cached for `Releases:CacheMinutes`, so browsers and registers never call GitHub directly.

### Where it runs

Everything runs on the existing jochemkok.nl server, next to the other sites, behind the nginx + Certbot setup that already serves the domain:

| URL | Container | Local port |
|---|---|---|
| `https://jochemkok.nl/stockflow/api/` | `api` (image `stockflow-api`) | `127.0.0.1:5080` |
| `https://jochemkok.nl/stockflow/management/` | `management` (image `stockflow-web`) | `127.0.0.1:5081` |
| `https://jochemkok.nl/stockflow/public/` | public customer site (later) | `127.0.0.1:5082` |

The containers only listen on `127.0.0.1`, so they are reachable through nginx only. nginx strips the `/stockflow/...` prefix before passing a request on, so the API itself still serves `/Product/GetAll` and so on. The management app image is built with `<base href="/stockflow/management/">` and `ApiBaseUrl` `/stockflow/api/` (build arguments `BASE_PATH` and `API_BASE_URL` in `StockFlow.ManagementApp/Dockerfile`). Inside the app all links are relative, so it works under any base path.

### One-time GitHub setup

- **Settings → Actions → General → Workflow permissions**: *Read and write permissions* (the workflow pushes images and uploads the zip with `GITHUB_TOKEN`).
- **Settings → Secrets and variables → Actions → Variables**: add `REGISTER_API_URL` = `https://jochemkok.nl/stockflow/api/`, so the downloaded cash register talks to the server straight away.
- After the first release the packages are private. Either make them public (**Packages → stockflow-api / stockflow-web → Package settings → Change visibility**) or log in on the server (see below).

### One-time server setup

On the server (over SSH):

1. **Install Docker** (if it isn't there yet):
   ```bash
   curl -fsSL https://get.docker.com | sudo sh
   sudo usermod -aG docker $USER     # log out and back in afterwards
   ```
2. **Get the deploy files** into `/opt/stockflow`:
   ```bash
   sudo mkdir -p /opt/stockflow && sudo chown $USER /opt/stockflow && cd /opt/stockflow
   curl -fsSLO https://raw.githubusercontent.com/jochemKok132/StockFlow/master/deploy/docker-compose.yml
   curl -fsSLO https://raw.githubusercontent.com/jochemKok132/StockFlow/master/deploy/nginx-stockflow.conf
   curl -fsSL  https://raw.githubusercontent.com/jochemKok132/StockFlow/master/deploy/.env.example -o .env
   ```
   (If the repository is private, copy the files with `scp` from your computer instead.)
3. **Fill in `.env`**: `nano .env`. `DOMAIN=jochemkok.nl`, `GHCR_OWNER=jochemkok132`, and a new `JWT_KEY` from `openssl rand -base64 64`. `GITHUB_RELEASES_TOKEN` stays empty unless the repository is private.
4. **Log in to GitHub's registry** (only needed while the packages are private), with a personal access token that has `read:packages`:
   ```bash
   echo <token> | docker login ghcr.io -u jochemKok132 --password-stdin
   ```
5. **Start the containers**:
   ```bash
   docker compose up -d
   docker compose ps          # api and management should be "running"
   curl -I http://127.0.0.1:5081/
   ```
6. **Hook it into nginx**: copy the snippet and include it in the existing `server` block for jochemkok.nl, the one with `listen 443 ssl` that Certbot manages, next to the `/vue` location:
   ```bash
   sudo cp nginx-stockflow.conf /etc/nginx/snippets/stockflow.conf
   sudo nano /etc/nginx/sites-available/jochemkok.nl     # add: include /etc/nginx/snippets/stockflow.conf;
   sudo nginx -t && sudo systemctl reload nginx
   ```
   No new certificate is needed, because everything lives under the existing domain.
7. Open `https://jochemkok.nl/stockflow/management/` and log in with a seeded employee. **Change the seeded passwords**: the seed data uses the shared dev password from this README.

The SQLite database lives in the Docker volume `stockflow_stockflow-data`, so it survives updates. The API migrates and seeds it on startup.

### Updating the server

**Automatic (recommended):** let the release workflow deploy for you. Add these in **Settings → Secrets and variables → Actions**:

| Kind | Name | Value |
|---|---|---|
| Variable | `DEPLOY_ENABLED` | `true` |
| Secret | `DEPLOY_HOST` | the server's host name or IP |
| Secret | `DEPLOY_USER` | the SSH user (must be in the `docker` group) |
| Secret | `DEPLOY_SSH_KEY` | a private key whose public key is in that user's `~/.ssh/authorized_keys` (make a separate key just for this) |
| Secret | `DEPLOY_PORT` | optional, if SSH isn't on port 22 |

After the images are pushed, the `Deploy to server` job connects over SSH and runs `docker compose pull && docker compose up -d` in `/opt/stockflow`. Pre-releases are never deployed automatically.

**Manual:**

```bash
cd /opt/stockflow && docker compose pull && docker compose up -d
```

With `STOCKFLOW_VERSION=latest` this picks up the newest release. Pin a version (`STOCKFLOW_VERSION=1.2.0`) to stay on it, or set it back to an older one to roll back. A rollback across a database migration may need a backup, so back up the volume before big updates:

```bash
docker run --rm -v stockflow_stockflow-data:/data -v "$PWD":/backup alpine tar czf /backup/stockflow-data.tgz -C /data .
```

### Cash register on the shop computers

1. Download the zip from the **Releases** page in the management app (or the GitHub release page).
2. Unzip it anywhere, for example `C:\StockFlow\CashRegister`.
3. If `REGISTER_API_URL` was not set, open `appsettings.json` and set `ApiBaseUrl` to `https://jochemkok.nl/stockflow/api/`.
4. Start `StockFlow.CashRegister.exe`. To update, download the new zip and replace the files (keep your `appsettings.json`). The login screen tells you when a newer version is available.

---

## Known limitations

- Customer login and registration exist in the API, but there is no customer portal (front end) yet. It is planned at `https://jochemkok.nl/stockflow/public/`: add it as a third image in the release workflow and `docker-compose.yml` (port 5082), and uncomment the `/stockflow/public/` block in `deploy/nginx-stockflow.conf`.
- The cash register does not update itself; it only shows that a new version exists. Shop computers need the new zip after each release.
- If the GitHub repository is private, the download links on the releases page only work for people who are logged in to GitHub with access to the repository (and the API needs `Releases:Token`).
- The default Docker setup is single-instance (SQLite + migrate on startup). Moving to multiple API instances needs a server database and migrations as a pipeline step.
- Points earned before `AddPointsEarned` existed are unknown (0), so returns on those older receipts don't take points back.
