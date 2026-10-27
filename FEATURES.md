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
- Support role that handles the support requests of other employees, and can view employees, customers, statistics and all logs and restore deleted items
- The admin and Tester accounts are hidden from everyone except the admin
- Passwords stored as BCrypt hashes only, never in a log
- SQLite database that migrates and seeds itself on startup
- Seed data: 24 employees (including a Tester and a Support account), 15 customers, 150 products with price history, expiry batches and every status, 25 brands with contact information, 15 shelves, 5 suppliers, 6 sales of every type plus the moving sale, 81 receipts, 4 supplier orders, 4 customer orders, 2 stock counts, 4 tickets, 13 weekly shifts with contract hours and availability, 5 schedule weeks and 4 swap / day-off requests, and stock logs, customer logs and return records that match all of it
- Soft delete for products, brands, sales and shelves
- Users never see a GUID: forms, error messages and logs always use an identifier (barcode, brand name, shelf name, receipt id, employee id)
- Maximum length on every text field, checked in the apps and again in the API

## Identifiers

- Product barcodes: 9 digits, next free number on create
- Customer ids: 8 digits starting with 8
- Receipt ids: 10 digits starting with 9, unique, given to every purchase
- Customer order numbers: 7 digits starting with 7, scanned at the cash register to pick up the order
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
- Scan products, customers, customer orders and receipts in one input field (max 10 characters)
- Hand over a customer order: scan its number, the customer and products are loaded, pay as usual
- Recalled, draft and disabled products can't be sold
- Sales only apply when a customer is attached
- Find a customer by postal code and house number (F9), with a choice when more people live at that address
- Sale prices for every sale type and the moving sale, the cheapest sale per line wins
- Change quantity, delete a line, cancel the receipt
- Park and recall a receipt (the customer is parked too)
- PIN or cash payment, confirmation shows the receipt id and points earned
- Stock lowered on every sale (stock may go negative)
- Keyboard shortcuts for everything (F1 – F12, Esc)
- English UI with the same colours as the management app
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
- Every return saved as one record: receipt id, employee, customer, date and time, products with amount, price paid and refund, total refund and points taken back

## Management app

- Sidebar only shows the pages your role can use, and buttons only show for roles that may use them
- Goes to the login page automatically when the session ends
- Inventory: paged, sortable and filterable, with a product details panel (image, sale price, location, stock, tags)
- Inventory shows the regular price with the sale price under it; the brand filter is a searchable select and the sale filter only lists active sales
- Product details: a button per section (sold, price history, recall, order block, write off) that opens only that section, and a button that opens the product on the public site
- Number fields always use a dot as decimal separator, so editing a price works in every browser language
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
- Product images are downsized in the browser to at most 800 px before upload
- Product details show the barcode as text and as a scannable Code 128 barcode
- Write off stock from the product details with a reason (damaged, expired, stolen, lost, other) and a note, for every employee except the Tester
- Employees: create with password, edit with optional password reset, only roles below your own
- Customers: list, filter, sort and edit
- Cash register logs with receipt filter, products overlay, returned amounts and a points column
- Return logs page: every return with its receipt, employee, customer, refund and points taken back, filter on receipt, employee and customer, and the returned products per return
- Download any receipt as a till-style PDF, with a centered, scannable barcode of the receipt id at the bottom to start a return
- Moving sales page to approve or reject requests
- Statistics page: revenue, purchases, average receipt, % of purchases with a customer card, discounts, points, returns, new customers, revenue per day and top 10 products
- Stock logs with old → new values
- Product history per barcode (sold, returned, added, all changes or stock only), bookmarkable URL, paged on the server with totals over the whole history, and the receipt id under every sale (click it to open the receipt)
- Customer logs and employee logs (including logins)
- Releases page: management app and cash register versions (no API version), *Up to date* / *Update available*, cash register download, release notes
- Version shown in the sidebar
- Confirmation popup before every delete or update (an edited item lists the fields that changed)
- Help & feedback page for every employee: send a support request, bug report or feature request (ticket #1001 and up) and follow its status and answer
- Support requests page for the Support role, and a Bugs & features overview for the admin, to set the status and answer
- Deleted items page for Support and the admin: every deleted product, brand, sale and shelf, with Restore (a product only after its brand and shelf)
- The logged-in employee (name, employee id and role) shown in the sidebar

## Validation

- Paging: page 1 or higher, page size 1 – 100
- Products: price 0 – 100,000 with at most 2 decimals, image up to 5 MB and a real image file, brand and shelf must exist
- Sales: valid type and its fields (1 – 99 % off, multi-buy amounts, bundle size and price), end date not before the start, at least one tag
- Sales only count on days inside their period
- Purchases: 1 – 999 per line, no negative prices, all points rules checked again on the server
- Returns: every rule checked again on the server

## Audit logging

- Stock logs for every create, update, delete, sale (with the receipt id), return, write-off and restore
- Cash register logs with prices at the time of sale, sales, discount, points used and earned
- Return logs: one record per return with the refund per product and the points taken back
- Employee logs for create, update and login
- Customer logs for registration, login, update, password change, points changes and moving sale requests, approvals and rejections
- Update logs show `Field: New: x. Old: y`
- Brand and shelf changes are logged by name, never by internal id

## Styling

- Light colour scheme, every colour a `:root` variable
- Only `px` and `%` units, corners 4 px or less
- No transitions, flat surfaces with 1 px borders
- StockFlow logo (stencil S) in the sidebar, on the login page, as the browser tab icon, and as the cash register's exe and window icon

## Products, ordering and stock counts

- Product status: active, draft or disabled; only active products can be sold or ordered
- Minimum and maximum stock per product, with the products below their minimum and a suggested amount on the orders page
- Non-orderable products, never ordered but still for sale
- Order automatically when the free stock drops below the minimum (up to the maximum)
- Recall a product with a reason, optionally writing off all its stock; it can't be sold or ordered and is taken off open orders
- Order blocks (*blokkades*) with a reason and an optional end date
- Products that can expire, with expiry batches per delivery; sales and write-offs use the first expiring batch
- Forced stock count of a shelf's expirable products one week before the first one expires
- Inventory filters: brand, on sale (yes / no), a specific sale, status, supplier and below minimum; sort on price
- Sales graph of the last 30 days per product
- Price history, plan a new price for a future date, the latest started price is used automatically
- More brand information: logo, description, website, country, contact email and customer service phone
- Suppliers with contact details, a fixed order day and a lead time
- Orders page: one open order per supplier, sent automatically on the order day, change or cancel it, receive the delivery
- Customer orders: products reserved from stock, the rest ordered from the supplier, ready for pickup when everything is in, picked up and paid at the cash register
- Manual customer orders made by an employee
- Stock counts (*tellingen*) per shelf with counted and expired amounts and the differences
- Stock automation every 15 minutes: planned prices, forced counts, automatic orders and sending orders on the order day

## Staff schedule

- Shifts per week per role (days, times, number of employees), with the hours needed per role against the contract hours
- Contract hours and availability per day for every employee
- Schedule generated automatically 4 weeks ahead: the right role, within contract hours and availability, one shift a day, no days off, fair spread; shifts nobody fits stay open
- Week view per day with open shifts and your own shifts highlighted, and your next shifts at the top
- Shift leaders change draft weeks (generate again, add, change or remove shifts) and see planned against contract hours
- The manager confirms a week; employees only see confirmed weeks, and only a manager can change a confirmed week
- Swap a shift with a colleague (take-over or exchange) and ask for a day off; the manager approves or rejects with a note, and the schedule changes straight away

## Tests and CI

- 725 xUnit + Moq unit tests: services, validations, mappings, shared rules, repositories (in-memory SQLite), controllers and the GitHub release service
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

## Products

- [ ] Online customer orders through the public site (paid on the public site)

## Services

- [ ] Show models
- [ ] Renting
- [ ] Custom wood sawing
- [ ] Paint mixed in a colour the customer specifies


## Login

- [ ] Refresh tokens

## New apps and services

- [ ] Public site for customers (`jochemkok.nl/stockflow/public`)
- [ ] Mobile management app with products, orders, sales and stock counts
- [ ] Schedule app for employees to see the schedule
- [ ] Email service

## Look and feel

- [ ] A house style that is clean and welcoming, but also modern
