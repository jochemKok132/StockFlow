# StockFlow

StockFlow is a store management system for a (Dutch) hardware store. One ASP.NET Core API serves a Blazor WebAssembly management app for the office and a WinForms cash register for the shop floor. Every change to stock, prices, customers and employees is written to an audit log.

**Full documentation: [the StockFlow wiki](https://github.com/jochemKok132/StockFlow/wiki).** The list of every feature, and the to-do list, is in [FEATURES.md](FEATURES.md).

## What it does

- **Products and prices:** inventory with minimum and maximum stock, price history and planned prices, five sale types, recalls, order blocks and expiry dates per delivery.
- **Ordering:** supplier orders made and sent automatically on each supplier's order day, customer orders that reserve stock and are paid at pickup, and stock counts (forced before products expire).
- **Cash register:** sales, loyalty points, returns with proportional refunds, and receipts as PDFs with a scannable barcode.
- **Staff:** a schedule generated four weeks ahead from contract hours and availability, confirmed by a manager, with swaps and days off.
- **Accountability:** stock, cash register, return, customer and employee logs, and seven roles plus a view-only Tester account.

## Projects

| Project | What it is |
|---|---|
| `StockFlow.API` | ASP.NET Core Web API (JWT, Swagger), runs the stock and schedule automation |
| `StockFlow.ManagementApp` | Blazor WebAssembly office app |
| `StockFlow.CashRegister` | WinForms point of sale (Windows) |
| `StockFlow.Domain`, `.Application`, `.Infrastructure`, `.Shared` | Layered backend: entities, services and rules, EF Core with SQLite, shared DTOs |
| `StockFlow.UnitTests` | xUnit + Moq, about 725 tests |

Built with .NET 10, EF Core 10 (SQLite), Blazor WebAssembly, Windows Forms and QuestPDF. More in [Architecture](https://github.com/jochemKok132/StockFlow/wiki/Architecture).

## Quick start

1. Open `StockFlow.slnx` in Visual Studio (with the ASP.NET and Windows desktop workloads, .NET 10 SDK).
2. Start `StockFlow.API`, `StockFlow.ManagementApp` and/or `StockFlow.CashRegister` with the **https** profiles.
3. Open the management app at https://localhost:7158 (Swagger UI: https://localhost:7037/api).

The API migrates and seeds the database on startup. Every seeded employee has the password **`001273`**: for example `000001` (Admin), `000002` (Manager), `000010` (Cashier) and `000023` (view-only Tester). All logins and seed data are in [Seed data and logins](https://github.com/jochemKok132/StockFlow/wiki/Seed-data-and-logins); the URLs and setup in [Getting started](https://github.com/jochemKok132/StockFlow/wiki/Getting-started).

## Releases

Publishing a GitHub release builds and tests everything, pushes the Docker images and attaches the cash register zip; the server updates itself to the new version. See [Releases and deployment](https://github.com/jochemKok132/StockFlow/wiki/Releases-and-deployment).
