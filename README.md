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
15. [CI](#ci)
16. [Known limitations](#known-limitations)

---

## Solution structure

| Project | Type | Responsibility |
|---|---|---|
| `StockFlow.Domain` | Class library | Entities (`Product`, `ProductBrand`, `Shelf`, `Sales`, `Employee`, `Customer`, audit logs, `RegisterItemDetail`) and enums. Depends on nothing. |
| `StockFlow.Application` | Class library | Services, DTOs, mappings (`Extensions/`), validations (`Validations/`), repository and service interfaces, `ApiResponse<T>`, `IdentifierFormats`. |
| `StockFlow.Infrastructure` | Class library | EF Core `ApplicationDBContext` (SQLite), repositories, migrations, seeders, BCrypt password hashing. |
| `StockFlow.API` | ASP.NET Core Web API | Controllers, JWT authentication, `ErrorController`, Swagger. |
| `StockFlow.ManagementApp` | Blazor WebAssembly | Office application: inventory, brands, sales, shelves, logs, employees and customers. |
| `StockFlow.CashRegister` | WinForms (.NET, Windows) | Point of sale: scan products, attach customers, take payment, take returns. |

The solution file is `StockFlow.slnx` (folders `Backend` and `Frontend`).

```
StockFlow.API ──► StockFlow.Application ──► StockFlow.Domain
      │                    ▲
      └──► StockFlow.Infrastructure ┘
StockFlow.ManagementApp ──► StockFlow.Application (DTOs, ApiResponse)
StockFlow.CashRegister  ──► StockFlow.Application (DTOs, ApiResponse)
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

All rules live in `StockFlow.Application/Common/IdentifierFormats.cs` and are shared by the API, the cash register and the seeders.

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
| Inventory | `/inventory` | Paged and sortable product list with filters. Click a row for the product details panel (image, price incl. sale, location, stock, tags, brand, description). Create, edit and delete for managers. |
| Brands | `/productbrands` | List, create, edit, delete brands. |
| Sales | `/sales` | List, create, edit, delete sales (percentage off + sales tags). |
| Shelves | `/shelves` | List, create, edit, delete shelves. |
| Cash register logs | `/cashregisterlogs` | Every purchase with its receipt id, customer and employee; filterable by receipt id. The products column also shows how many items were returned. Click **Products** to see what was sold, the amounts, how many were returned per product, the price per item, applied sales, the discount and the total. Click a product to open its details. |
| Stock logs | `/stocklogs` | Every create / update / sale / return / delete of products, brands, sales and shelves. **Open message** shows what changed (old → new). |
| Product history | `/producthistory?barcode=…` | Enter a barcode to see the product, current stock, units sold, units returned, units added and the full history. Toggle between all changes and stock changes only. The barcode is kept in the URL so searches can be bookmarked or linked. |
| Customer logs | `/customerlogs` | Customer changes, filterable by customer id, name, email and log type. |
| Employee logs | `/employeelogs` | Employee changes and logins. |
| Employees | `/employees` | List, create (with password), edit (optional password reset). |
| Customers | `/customers` | Customer list with customer id, filterable and sortable. |

Forms are generated by the reusable `EditItem` component from the DTO type. Validation errors from the API are shown above the form and the form stays open.

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
- **Spending:** **100 points = €1**, so 1 point = €0.01. The rules live in `StockFlow.Application/Common/LoyaltyPoints.cs`.
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

- **Refund**: the price per item on the receipt (including sales). If points were used on the receipt, the refund is reduced in the same proportion as the points reduced the total (`ReturnRules` in `StockFlow.Application/Common/ReturnRules.cs`), so points are never paid out as cash. Points are not given back.
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

Every response, including errors, uses the same envelope (`StockFlow.Application/Common/ApiResponse.cs`):

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
| `DataSource` | SQLite file name inside `StockFlow.Infrastructure/Data` (default `app.db`) |
| `JWT:Issuer` | Token issuer |
| `JWT:Key` | Signing key; keep it secret and long enough for HMAC-SHA256 |
| `JWT:ExpiryMinutes` | Token lifetime |

- CORS allows the management app at `https://localhost:7158`.
- Both clients use `https://localhost:7037/` as the API base address (`Program.cs` of each client).

---

## Styling conventions

The management app follows a house style (`wwwroot/css/app.css` plus scoped `.razor.css` files):

- Every color is a CSS variable on `:root` (`--TaskbarColor`, `--DashboardColor`, `--AccentColor`, …); no raw colors in component rules.
- Units are `px` and `%` only.
- Corners are at most 4 px (`--RadiusSmall` 2 px, `--RadiusLarge` 4 px).
- No transitions or transforms; flat surfaces with 1 px borders.
- Light color scheme.
- Shared building blocks: `.card`, `.tasks-table`, `.filters-bar`, `.pagination`, `.modal-overlay` / `.modal-box` / `.drawer`, `.panel-header`.
- Buttons: `.btn-save` (primary), `.btn-cancel` / `.btn-edit` (secondary), `.btn-delete` (destructive), `.btn-icon` (close).

The cash register mirrors the same palette in `Screens/Theme.cs`.

---

## CI

`.github/workflows/ci-build-test.yml` runs on pull requests to `master`: restore, build (Release) and test with .NET 10.

---

## Known limitations

- Customer login and registration exist in the API, but there is no customer portal (front end) yet.
- Points earned before `AddPointsEarned` existed are unknown (0), so returns on those older receipts don't take points back.
