# StockFlow features

A short list of everything StockFlow can do right now, followed by the [to-do list](#to-do). The details are in the [README](README.md).

## General

- REST API, Blazor WebAssembly management app and WinForms cash register, all on .NET 10
- Layered backend (Domain, Application, Infrastructure, API) with a Shared project for DTOs and shared rules
- Same `ApiResponse` envelope for every response, including errors (400 / 401 / 403 / 404 / 500)
- JWT login for employees and customers (30 minutes per login)
- Six employee roles (Admin, Manager, ShiftLeader, ServiceDesk, Cashier, FloorWorker) plus a Customer role
- View-only Tester account to give to visitors: sees every page and form but can't save, refused by the API too; only visible to and created by the admin
- One shared permissions map used by both the API and the management app
- Employee hierarchy: you can only create, edit and give out roles below your own
- The admin and Tester accounts are hidden from everyone except the admin
- Passwords stored as BCrypt hashes only, never in a log
- SQLite database that migrates and seeds itself on startup
- Seed data: 23 employees (including a Tester), 15 customers, 150 products, 25 brands, 15 shelves, 2 sales plus the moving sale
- Soft delete for products, brands, sales and shelves
- Users never see a GUID: forms, error messages and logs always use an identifier (barcode, brand name, shelf name, receipt id, employee id)
- Maximum length on every text field, checked in the apps and again in the API

## Identifiers

- Product barcodes: 9 digits, next free number on create
- Customer ids: 8 digits starting with 8
- Receipt ids: 10 digits starting with 9, unique, given to every purchase
- Employee ids: 6 digits, next free number on create
- The cash register recognises what was scanned by the number of digits

## Customers and loyalty points

- Customer registration and login with email and password
- Strong customer password rule, change own password
- Dutch postal code (`1234 AB`) and house number (`12`, `12A`, `12bis`) checks
- 250 welcome points for every new account
- 1 point per whole euro paid (€1.99 = 1, €2.00 = 2)
- 100 points = €1
- Spending points is all or nothing, with no minimum (even a €0.05 receipt)
- Points can pay the whole receipt, with no PIN or cash needed then
- Points spent don't earn new points
- Every points change is written to the customer log with the receipt id
- Moving sale: a customer who moved asks for it with their new address, a shift leader or higher approves or rejects it; when approved the address is updated and the customer gets 10 % off everything that isn't already on sale for 90 days

## Cash register

- Employee login with version shown and a check for a newer release
- Scan products, customers and receipts in one input field (max 10 characters)
- Sales only apply when a customer is attached
- Find a customer by postal code and house number (F9), with a choice when more people live at that address
- Sale prices for every sale type and the moving sale, the cheapest sale per line wins
- Change quantity, delete a line, cancel the receipt
- Park and recall a receipt (the customer is parked too)
- PIN or cash payment, confirmation shows the receipt id and points earned
- Stock lowered on every sale (stock may go negative)
- Keyboard shortcuts for everything (F1 – F12, Esc)
- Dutch UI with the same colours as the management app
- API address editable in `appsettings.json` next to the exe
- Back to the login screen when the session has expired

## Returns

- Returns only with the receipt id
- Only products from that receipt, never more than bought minus already returned
- Return in several steps, a fully returned receipt can't be opened again
- Refund uses the receipt price including sales
- Refund lowered in proportion when points were used, so points are never paid out as cash
- Points earned on returned items are taken back (never below 0)
- Stock raised and a *Returned* stock log written per product

## Management app

- Sidebar only shows the pages your role can use, and buttons only show for roles that may use them
- Goes to the login page automatically when the session ends
- Inventory: paged, sortable and filterable, with a product details panel (image, sale price, location, stock, tags)
- Create, edit and delete products, brands, sales and shelves (managers)
- Sales types: percentage off, multi-buy (1+1, 2+1, …), second item half price, bundle price (3 for €10)
- Sales with a start and end date, status *Scheduled* / *Active* / *Ended*, filter on status, ended sales kept as history with items sold
- Searchable selects for brand, shelf, role and enums
- Searchable multi-select for sales tags, shows which sale a tag belongs to, new tags can be typed
- Save only enabled when something actually changed (ignores spaces, tag order and trailing zeros)
- API validation errors shown above the form, form stays open
- Live character counter under every text field (orange near the limit, red at the limit)
- Overlays only close when the click starts and ends outside them
- Image picker only accepts PNG, JPEG, GIF and WebP, checked on the file contents
- Employees: create with password, edit with optional password reset, only roles below your own
- Customers: list, filter, sort and edit
- Cash register logs with receipt filter, products overlay, returned amounts and a points column
- Download any receipt as a till-style PDF
- Moving sales page to approve or reject requests
- Statistics page: revenue, purchases, average receipt, % of purchases with a customer card, discounts, points, returns, new customers, revenue per day and top 10 products
- Stock logs with old → new values
- Product history per barcode (sold, returned, added, all changes or stock only), bookmarkable URL
- Customer logs and employee logs (including logins)
- Releases page: management app and cash register versions (no API version), *Up to date* / *Update available*, cash register download, release notes
- Version shown in the sidebar
- The logged-in employee (name, employee id and role) shown in the sidebar

## Validation

- Paging: page 1 or higher, page size 1 – 100
- Products: price 0 – 100,000 with at most 2 decimals, image up to 5 MB and a real image file, brand and shelf must exist
- Sales: valid type and its fields (1 – 99 % off, multi-buy amounts, bundle size and price), end date not before the start, at least one tag
- Sales only count on days inside their period
- Purchases: 1 – 999 per line, no negative prices, all points rules checked again on the server
- Returns: every rule checked again on the server

## Audit logging

- Stock logs for every create, update, delete, sale and return
- Cash register logs with prices at the time of sale, sales, discount, points used and earned
- Employee logs for create, update and login
- Customer logs for registration, login, update, password change, points changes and moving sale requests, approvals and rejections
- Update logs show `Field: New: x. Old: y`
- Brand and shelf changes are logged by name, never by internal id

## Styling

- Light colour scheme, every colour a `:root` variable
- Only `px` and `%` units, corners 4 px or less
- No transitions, flat surfaces with 1 px borders

## Tests and CI

- 483 xUnit + Moq unit tests: services, validations, mappings, shared rules, repositories (in-memory SQLite), controllers and the GitHub release service
- Build with 0 warnings
- CI builds and tests every push and pull request to `master`

## Releases and deployment

- Versions controlled with GitHub Releases (`v1.2.0`)
- Publishing a release builds, tests, pushes Docker images to ghcr.io and attaches the cash register zip
- Cash register as a self-contained single-file exe (no .NET install needed)
- Server updates itself automatically after a release (pre-releases skipped)
- Pin or roll back with `STOCKFLOW_VERSION`
- Runs at `jochemkok.nl/stockflow/api` and `jochemkok.nl/stockflow/management` behind nginx with HTTPS
- Containers restart on their own after a server reboot
- Release list cached in the API, with the last known list used when GitHub is down

---

# To-do

## Bugs and fixes

- [ ] Fix editing a product's price

## Product history

- [ ] Show the barcode under *Sold*
- [ ] Pagination

## Products

- [ ] Write off stock (*afboeken*) with a reason, with a few basic reasons to pick from (damaged, expired, stolen, lost, other); show it in the stock logs and the product history
- [ ] Downsize product images on upload
- [ ] Show the barcode in the product details
- [ ] Minimum and maximum stock per product
- [ ] Non-orderable products
- [ ] Recall products
- [ ] Disable products / save them as a draft
- [ ] Product blocks (*blokkades*)
- [ ] Products that can expire: 1 week before the first one expires, a stock count is forced for that shelf's expirable products
- [ ] Filters: brand, on sale (yes/no), a specific sale
- [ ] Sales graph per product
- [ ] Pricing overhaul: price history, set a new price for a future date, the latest active price is used automatically
- [ ] Button in the product details that opens that product on the public site
- [ ] More brand information


## Ordering and stock counts

- [ ] Order page
- [ ] Per product: order automatically when stock drops below the minimum
- [ ] A fixed order day for automatic and manual orders
- [ ] Customer orders
- [ ] Manual customer orders
- [ ] Stock counts (*tellingen*)

## Services

- [ ] Show models
- [ ] Renting
- [ ] Custom wood sawing
- [ ] Paint mixed in a colour the customer specifies


## Employees and permissions

- [ ] Support role with its own permissions: a page to request support, and a page for the support person to handle the requests
- [ ] Bug report / feature request screen for all employees, with an overview for the admin
- [ ] Admin-only panel with deleted items that can be reinstated
- [ ] Confirmation popup before deleting or updating
- [ ] Staff schedule (*rooster*) management: generated automatically 4 weeks ahead, can be changed, the manager has to confirm it



## Login

- [ ] Refresh tokens

## New apps and services

- [ ] Public site for customers (`jochemkok.nl/stockflow/public`)
- [ ] Mobile management app with products, orders, sales and stock counts
- [ ] Email service

## Look and feel

- [ ] Logo
- [ ] A house style that is clean and welcoming, but also modern
