using StockFlow.Shared.DTOs.Ordering;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.AuditLogs.CashRegister;
using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.DTOs.Customer;
using StockFlow.Shared.DTOs.Stock.Product;
using StockFlow.Shared.DTOs.Stock.Sales;
using StockFlow.CashRegister.Interfaces;
using StockFlow.CashRegister.Screens;

namespace StockFlow.CashRegister
{
    public partial class ScanScreen : UserControl, IKeyScreen
    {
        private const decimal VatRate = 0.21m;

        private readonly MainForm _mainForm;
        private readonly ICashRegisterService _cashRegister;
        private readonly List<RegisterItemDetailDto> _lines = [];
        private List<RegisterItemDetailDto>? _parked;
        private CustomerDto? _parkedCustomer;
        private CustomerDto? _customer;
        private CashRegisterLogsDto? _receipt;
        private CustomerOrderDto? _customerOrder;
        private bool _usePoints;
        private bool _busy;
        private string _scanCaption = "";

        public IReadOnlyList<RegisterItemDetailDto> Lines => _lines;
        public decimal LinesTotal => _lines.Sum(LineTotal);
        public int PointsToUse => _usePoints && _customer is not null && !IsReturning
            ? LoyaltyPoints.Redeemable(_customer.SavedPoints, LinesTotal)
            : 0;
        public decimal CurrentTotal => IsReturning ? RefundTotal : LinesTotal - LoyaltyPoints.ToEuros(PointsToUse);
        public decimal CurrentDiscount => IsReturning ? 0m : _lines.Sum(l => SalePricing.Total(l.Product.Price, l.Amount) - LineTotal(l));
        public bool IsReturning => _receipt is not null;
        public int PointsToEarn => _customer is null || IsReturning ? 0 : LoyaltyPoints.PointsEarned(CurrentTotal);
        private decimal ReceiptLinesTotal => _receipt?.ProductsSold.Sum(p => SalePricing.Total(p.PriceAtTimeOfSale, p.Amount)) ?? 0m;
        private decimal RefundTotal => _lines.Sum(RefundFor);
        public bool HasItems => _lines.Count > 0;
        public CustomerDto? Customer => _customer;
        public int? CustomerOrderNumber => _customerOrder?.OrderNumber;

        public ScanScreen(MainForm parent, ICashRegisterService cashRegister)
        {
            _mainForm = parent;
            _cashRegister = cashRegister;

            InitializeComponent();
            _scanCaption = lblScan.Text;
            ApplyTheme();
            SetupLogic();
            RefreshView();
        }

        private void ApplyTheme()
        {
            Theme.ApplyPage(this);

            Theme.ApplyCaption(lblScan);
            Theme.ApplyInput(txtBarcode, 14);
            Theme.ApplyPrimary(btnAdd, 10);
            Theme.ApplyMessage(lblMessage);

            Theme.ApplyValue(lblCustomer, 11);
            Theme.ApplySecondary(btnRemoveCustomer, 10);
            Theme.ApplySecondary(btnUsePoints, 10);

            Theme.ApplyGrid(gridItems);
            foreach (var column in new[] { colQty, colPrice, colTotal })
            {
                column.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
                column.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
            }

            Theme.ApplyMuted(lblItemCount);
            Theme.ApplyMuted(lblSubtotal);
            Theme.ApplyMuted(lblVat);
            Theme.ApplyValue(lblTotalAmount, 30);

            foreach (var button in new[] { btnSearch, btnQuantity, btnDeleteLine, btnCancel, btnPark, btnRecall })
                Theme.ApplySecondary(button, 11);

            btnCancel.ForeColor = Theme.DangerText;
            Theme.ApplyPrimary(btnCheckout, 15);
        }

        private void SetupLogic()
        {
            txtBarcode.MaxLength = IdentifierFormats.ReceiptIdLength;

            txtBarcode.KeyDown += async (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                await HandleInputAsync();
            };

            btnAdd.Click += async (_, _) => await HandleInputAsync();
            btnRemoveCustomer.Click += (_, _) =>
            {
                if (IsReturning || _customerOrder is not null) CancelSale();
                else SetCustomer(null);
            };
            btnUsePoints.Click += (_, _) => TogglePoints();
            btnSearch.Click += (_, _) => { txtBarcode.Focus(); txtBarcode.SelectAll(); };
            btnQuantity.Click += (_, _) => ChangeQuantity();
            btnDeleteLine.Click += (_, _) => DeleteSelectedLine();
            btnCancel.Click += (_, _) => CancelSale();
            btnPark.Click += (_, _) => ParkSale();
            btnRecall.Click += (_, _) => RecallSale();
            btnFindCustomer.Click += async (_, _) => await FindCustomerAsync();
            btnCheckout.Click += async (_, _) =>
            {
                if (IsReturning) await CompleteReturnAsync();
                else _mainForm.ShowPurchasingScreen();
            };
        }

        public void FocusScannerInput() => txtBarcode.Focus();

        public void Clear()
        {
            _receipt = null;
            _customerOrder = null;
            _lines.Clear();
            SetCustomer(null);
            RefreshView();
        }

        public void ShowNotice(string text) => Theme.ShowMessage(lblMessage, text);

        public bool HandleKey(Keys key)
        {
            Button? target = key switch
            {
                Keys.F1 => btnSearch,
                Keys.F2 => btnQuantity,
                Keys.F3 => btnRemoveCustomer,
                Keys.F4 => btnUsePoints,
                Keys.F5 => btnCancel,
                Keys.F6 => btnDeleteLine,
                Keys.F7 => btnPark,
                Keys.F8 => btnRecall,
                Keys.F9 => btnFindCustomer,
                Keys.F10 => btnCheckout,
                _ => null
            };

            if (target is null) return false;
            if (target.Enabled) target.PerformClick();
            return true;
        }

        private async Task HandleInputAsync()
        {
            if (_busy) return;

            string input = txtBarcode.Text.Trim();

            if (IdentifierFormats.IsProductBarcode(input))
            {
                if (IsReturning) AddReturnProduct(input);
                else await AddProductAsync(input);
            }
            else if (IdentifierFormats.IsCustomerId(input))
            {
                if (IsReturning)
                {
                    ShowError("A customer can't be linked during a return.");
                    txtBarcode.SelectAll();
                }
                else await AddCustomerAsync(input);
            }
            else if (IdentifierFormats.IsReceiptId(input))
                await LoadReceiptAsync(input);
            else if (IdentifierFormats.IsCustomerOrderNumber(input))
                await LoadCustomerOrderAsync(input);
            else
            {
                ShowError($"Enter a barcode of {IdentifierFormats.ProductBarcodeLength} digits, " +
                          $"a customer id of {IdentifierFormats.CustomerIdLength} digits " +
                          $"(starting with {IdentifierFormats.CustomerIdPrefix}), a customer order number of " +
                          $"{IdentifierFormats.CustomerOrderNumberLength} digits (starting with {IdentifierFormats.CustomerOrderNumberPrefix}) or a receipt id of " +
                          $"{IdentifierFormats.ReceiptIdLength} digits (starting with {IdentifierFormats.ReceiptIdPrefix}).");
                txtBarcode.SelectAll();
            }
        }

        private async Task LoadCustomerOrderAsync(string orderNumber)
        {
            if (IsReturning)
            {
                ShowError("A customer order can't be picked up during a return.");
                txtBarcode.SelectAll();
                return;
            }

            if (HasItems || _customerOrder is not null)
            {
                ShowError("Finish or cancel the current receipt before you pick up a customer order.");
                txtBarcode.SelectAll();
                return;
            }

            SetBusy(true);
            try
            {
                var (order, error) = await _cashRegister.GetCustomerOrderAsync(orderNumber);
                if (order is null)
                {
                    ShowError(error.StartsWith("Error:") ? error["Error:".Length..].Trim() : error);
                    txtBarcode.SelectAll();
                    return;
                }

                var (customer, customerError) = await _cashRegister.GetCustomerAsync(order.CustomerNumber);
                if (customer is null)
                {
                    ShowError(customerError.StartsWith("Error:") ? customerError["Error:".Length..].Trim() : customerError);
                    txtBarcode.SelectAll();
                    return;
                }

                foreach (var orderLine in order.Lines.Where(l => l.Product is not null))
                    _lines.Add(new RegisterItemDetailDto { Product = orderLine.Product!, Amount = orderLine.Amount });

                _customerOrder = order;
                SetCustomer(customer);
                txtBarcode.Clear();
                ShowNotice($"Customer order {order.OrderNumber} of {customer.FullName}: {order.Lines.Sum(l => l.Amount)} item(s). Hand over the products and press Checkout [F10].");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task LoadReceiptAsync(string receiptId)
        {
            if (IsReturning)
            {
                ShowError($"A return is already open for receipt {_receipt!.ReceiptId}. Finish it first or stop the return [F3].");
                txtBarcode.SelectAll();
                return;
            }

            if (HasItems)
            {
                ShowError("Finish or cancel the current receipt before you start a return.");
                txtBarcode.SelectAll();
                return;
            }

            SetBusy(true);
            try
            {
                var (receipt, error) = await _cashRegister.GetReceiptAsync(receiptId);
                if (receipt is null)
                {
                    ShowError(error);
                    txtBarcode.SelectAll();
                    return;
                }

                int returnable = receipt.ProductsSold.Sum(p => p.Amount - p.ReturnedAmount);
                if (returnable <= 0)
                {
                    ShowError($"All items on receipt {receipt.ReceiptId} have already been returned.");
                    txtBarcode.SelectAll();
                    return;
                }

                SetCustomer(null);
                _receipt = receipt;
                txtBarcode.Clear();
                ShowNotice($"Return for receipt {receipt.ReceiptId}: {returnable} item(s) can still be returned. Scan the items that come back.");
                RefreshView();
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private IEnumerable<RegisterItemDetailDto> ReceiptLinesFor(ProductDto product) =>
            _receipt?.ProductsSold.Where(p => p.Product is not null && p.Product.Id == product.Id) ?? [];

        private int Returnable(ProductDto product) =>
            ReceiptLinesFor(product).Sum(p => p.Amount - p.ReturnedAmount);

        private decimal RefundFor(RegisterItemDetailDto line) =>
            _receipt is null
                ? 0m
                : ReturnRules.RefundFor(line.PriceAtTimeOfSale, line.Amount, (decimal)_receipt.TotalPrice, ReceiptLinesTotal);

        private void AddReturnProduct(string input)
        {
            if (_receipt is null) return;

            int barcode = int.Parse(input);
            var sold = _receipt.ProductsSold.FirstOrDefault(p => p.Product is not null && p.Product.Barcode == barcode);
            if (sold is null)
            {
                ShowError($"This product is not on receipt {_receipt.ReceiptId}.");
                txtBarcode.SelectAll();
                return;
            }

            var receiptLines = ReceiptLinesFor(sold.Product).ToList();
            int bought = receiptLines.Sum(p => p.Amount);
            int alreadyReturned = receiptLines.Sum(p => p.ReturnedAmount);
            int max = bought - alreadyReturned;

            var line = _lines.FirstOrDefault(l => l.Product.Id == sold.Product.Id);
            if ((line?.Amount ?? 0) >= max)
            {
                ShowError(max <= 0
                    ? $"{sold.Product.ProductName} from receipt {_receipt.ReceiptId} has already been fully returned."
                    : $"At most {max} of {sold.Product.ProductName} can be returned (bought: {bought}, already returned: {alreadyReturned}).");
                txtBarcode.SelectAll();
                return;
            }

            if (line is null)
            {
                line = new RegisterItemDetailDto
                {
                    Product = sold.Product,
                    PriceAtTimeOfSale = sold.PriceAtTimeOfSale,
                    Sale = sold.Sale,
                };
                _lines.Add(line);
            }
            line.Amount++;

            txtBarcode.Clear();
            Theme.ShowMessage(lblMessage, null);
            RefreshView(_lines.IndexOf(line));
        }

        private async Task CompleteReturnAsync()
        {
            if (_receipt is null || !HasItems || _busy) return;

            int count = _lines.Sum(l => l.Amount);
            var answer = MessageBox.Show(FindForm(),
                $"Return {count} item(s) from receipt {_receipt.ReceiptId}?\r\nTo refund: {Theme.Money(RefundTotal)}",
                "StockFlow Cash Register", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            SetBusy(true);
            try
            {
                var request = new ReturnRequest
                {
                    ReceiptId = _receipt.ReceiptId,
                    Items = _lines.Select(l => new ReturnItem { ProductId = l.Product.Id, Amount = l.Amount }).ToList(),
                };

                var (result, error) = await _cashRegister.ProcessReturnAsync(request);
                if (result is null)
                {
                    ShowError(error);
                    return;
                }

                Clear();
                string points = result.PointsDeducted > 0 ? $"   ·   {result.PointsDeducted} saved points deducted" : "";
                ShowNotice($"Return processed for receipt {result.ReceiptId}: {result.ItemsReturned} item(s). Refund: {Theme.Money(result.RefundAmount)}{points}");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private async Task AddCustomerAsync(string customerId)
        {
            SetBusy(true);
            try
            {
                var (customer, error) = await _cashRegister.GetCustomerAsync(customerId);
                if (customer is null)
                {
                    ShowError(error);
                    txtBarcode.SelectAll();
                    return;
                }

                SetCustomer(customer);
                txtBarcode.Clear();
                ShowNotice(CurrentDiscount > 0
                    ? $"Customer {customer.FullName} linked. Sales applied: {Theme.Money(CurrentDiscount)} discount."
                    : $"Customer {customer.FullName} linked to this receipt.");
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void SetCustomer(CustomerDto? customer)
        {
            _customer = customer;
            _usePoints = false;
            customerCard.Visible = customer is not null;
            btnRemoveCustomer.Enabled = customer is not null;

            foreach (var line in _lines)
                ApplyPricing(line);
            RefreshView();
        }

        private void TogglePoints()
        {
            if (_customer is null || IsReturning) return;

            if (!_usePoints && _customer.SavedPoints <= 0)
            {
                ShowError("The customer has no points to use.");
                return;
            }

            _usePoints = !_usePoints;
            Theme.ShowMessage(lblMessage, null);
            RefreshView();
        }

        private void UpdateCustomerView()
        {
            customerCard.Visible = _customer is not null || IsReturning;
            btnRemoveCustomer.Enabled = customerCard.Visible;

            if (_receipt is not null)
            {
                string customer = _receipt.Customer is null ? "No customer" : $"Customer: {_receipt.Customer.FullName}";
                string pointsText = _receipt.PointsUsed > 0 ? $"   ·   {_receipt.PointsUsed} points used" : "";
                lblCustomer.Text = $"RETURN   ·   Receipt {_receipt.ReceiptId}   ·   {_receipt.CreatedAt.ToLocalTime():dd-MM-yyyy HH:mm}   ·   {customer}{pointsText}";
                btnRemoveCustomer.Text = "Stop return [F3]";
                btnUsePoints.Visible = false;
                return;
            }

            btnRemoveCustomer.Text = _customerOrder is not null ? "Stop pickup [F3]" : "Remove customer [F3]";
            btnUsePoints.Visible = true;

            if (_customer is null)
            {
                lblCustomer.Text = "";
                return;
            }

            int points = PointsToUse;
            string movingSale = _customer.MovingSale is { } sale
                ? $"   ·   moving sale {sale.PercentageOff}% until {_customer.MovingSaleUntil:dd-MM-yyyy}"
                : "";
            string pickup = _customerOrder is not null ? $"Customer order {_customerOrder.OrderNumber}   ·   " : "";
            lblCustomer.Text = pickup + (points > 0
                ? $"Customer: {_customer.FullName}   ·   {_customer.CustomerId}   ·   {points} of {_customer.SavedPoints} points used (-{Theme.Money(LoyaltyPoints.ToEuros(points))})"
                : $"Customer: {_customer.FullName}   ·   {_customer.CustomerId}   ·   {_customer.SavedPoints} points ({Theme.Money(LoyaltyPoints.ToEuros(_customer.SavedPoints))})") + movingSale;

            btnUsePoints.Text = _usePoints ? "Don't use points [F4]" : "Use points [F4]";
            btnUsePoints.Enabled = _customer.SavedPoints > 0;
            Theme.SetSelected(btnUsePoints, _usePoints);
        }

        private void ApplyPricing(RegisterItemDetailDto line)
        {
            int amount = Math.Max(line.Amount, 1);
            SalesDto? sale = _customer is null ? null : SalePricing.BestSale(line.Product, amount, DateTime.Today, _customer.MovingSale);

            line.Sale = sale;
            line.PriceAtTimeOfSale = SalePricing.UnitPrice(line.Product.Price, amount, sale);
        }

        private async Task FindCustomerAsync()
        {
            if (IsReturning || _busy) return;

            var customer = await CustomerSearchDialog.AskAsync(FindForm()!, _cashRegister);
            if (customer is null)
            {
                txtBarcode.Focus();
                return;
            }

            SetCustomer(customer);
            ShowNotice(CurrentDiscount > 0
                ? $"Customer {customer.FullName} linked. Sales applied: {Theme.Money(CurrentDiscount)} discount."
                : $"Customer {customer.FullName} linked to this receipt.");
            txtBarcode.Focus();
        }

        private async Task AddProductAsync(string input)
        {
            SetBusy(true);
            try
            {
                var (product, error) = await _cashRegister.GetProductAsync(input);
                if (!string.IsNullOrEmpty(error))
                {
                    ShowError(error);
                    txtBarcode.SelectAll();
                    return;
                }

                var line = _lines.FirstOrDefault(l => l.Product.Id == product.Id);
                if (line is null)
                {
                    line = new RegisterItemDetailDto { Product = product };
                    ApplyPricing(line);
                    _lines.Add(line);
                }
                line.Amount++;

                txtBarcode.Clear();
                Theme.ShowMessage(lblMessage, null);
                RefreshView(_lines.IndexOf(line));
            }
            catch (Exception ex)
            {
                ShowError(ex.Message);
            }
            finally
            {
                SetBusy(false);
            }
        }

        private void ChangeQuantity()
        {
            int index = SelectedIndex;
            if (index < 0) return;

            var line = _lines[index];
            int max = IsReturning ? Returnable(line.Product) : 999;
            string? answer = InputDialog.Ask(FindForm()!, "Change quantity",
                IsReturning
                    ? $"Quantity to return for {line.Product.ProductName} (at most {max})"
                    : $"New quantity for {line.Product.ProductName}",
                line.Amount.ToString(),
                FieldLimits.Quantity);

            if (answer is null) return;

            if (!int.TryParse(answer, out int quantity) || quantity < 1 || quantity > max)
            {
                ShowError($"Enter a quantity between 1 and {max}.");
                return;
            }

            line.Amount = quantity;
            Theme.ShowMessage(lblMessage, null);
            RefreshView(index);
        }

        private void DeleteSelectedLine()
        {
            int index = SelectedIndex;
            if (index < 0) return;

            _lines.RemoveAt(index);
            Theme.ShowMessage(lblMessage, null);
            RefreshView(index);
        }

        private void CancelSale()
        {
            if (!HasItems && !IsReturning) return;

            bool returning = IsReturning;
            if (HasItems)
            {
                var answer = MessageBox.Show(FindForm(), returning ? "Cancel the return?" : "Cancel the current receipt?",
                    "StockFlow Cash Register", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (answer != DialogResult.Yes) return;
            }

            Clear();
            ShowNotice(returning ? "Return cancelled." : "Receipt cancelled.");
        }

        private void ParkSale()
        {
            if (!HasItems || _parked is not null || IsReturning) return;
            if (_customerOrder is not null)
            {
                ShowError("A customer order pickup can't be parked. Finish or cancel it first.");
                return;
            }

            _parked = [.. _lines];
            _parkedCustomer = _customer;
            Clear();
            ShowNotice("Receipt parked. Use Reopen [F8] to continue.");
        }

        private void RecallSale()
        {
            if (_parked is null || HasItems || IsReturning) return;

            _lines.AddRange(_parked);
            _parked = null;
            SetCustomer(_parkedCustomer);
            _parkedCustomer = null;
            Theme.ShowMessage(lblMessage, null);
            RefreshView();
        }

        private static decimal LineTotal(RegisterItemDetailDto line) => SalePricing.Total(line.PriceAtTimeOfSale, line.Amount);

        private int SelectedIndex => gridItems.CurrentRow?.Index ?? -1;

        private void RefreshView(int? select = null)
        {
            if (!IsReturning)
                foreach (var line in _lines)
                    ApplyPricing(line);

            gridItems.Rows.Clear();
            foreach (var line in _lines)
            {
                string description = line.Sale is null
                    ? line.Product.ProductName
                    : $"{line.Product.ProductName}   ({SalePricing.Describe(line.Sale)} {line.Sale.SaleName})";

                if (IsReturning)
                    description += $"   (return, max {Returnable(line.Product)})";

                gridItems.Rows.Add(
                    line.Product.Barcode.ToString(),
                    description,
                    line.Amount,
                    Theme.Money(line.PriceAtTimeOfSale),
                    Theme.Money(IsReturning ? RefundFor(line) : LineTotal(line)));
            }

            if (gridItems.Rows.Count > 0)
            {
                int index = Math.Clamp(select ?? gridItems.Rows.Count - 1, 0, gridItems.Rows.Count - 1);
                gridItems.CurrentCell = gridItems.Rows[index].Cells[0];
                gridItems.Rows[index].Selected = true;
            }

            decimal total = CurrentTotal;
            decimal vat = Math.Round(total - total / (1 + VatRate), 2);

            decimal discount = CurrentDiscount;
            int points = PointsToUse;
            var summary = IsReturning
                ? $"Return: {_lines.Sum(l => l.Amount)} item(s)"
                : $"Items: {_lines.Sum(l => l.Amount)}";
            if (IsReturning && _receipt!.PointsUsed > 0)
                summary += "   ·   Refund pro rata (points used)";
            if (discount > 0)
                summary += $"   ·   Discount: {Theme.Money(discount)}";
            if (points > 0)
                summary += $"   ·   Points: -{Theme.Money(LoyaltyPoints.ToEuros(points))}";
            if (PointsToEarn > 0)
                summary += $"   ·   Earns: +{PointsToEarn} points";
            lblItemCount.Text = summary;

            UpdateCustomerView();
            lblSubtotal.Text = $"Subtotal (excl. VAT): {Theme.Money(total - vat)}";
            lblVat.Text = $"VAT ({VatRate:P0}): {Theme.Money(vat)}";
            lblTotalAmount.Text = IsReturning && total > 0 ? $"-{Theme.Money(total)}" : Theme.Money(total);
            lblScan.Text = IsReturning ? $"RETURN RECEIPT {_receipt!.ReceiptId}  ·  SCAN THE ITEMS (9 DIGITS)" : _scanCaption;
            btnCheckout.Text = IsReturning ? "FINISH RETURN  [F10]" : "CHECKOUT  [F10]";

            btnQuantity.Enabled = HasItems;
            btnDeleteLine.Enabled = HasItems;
            btnCancel.Enabled = HasItems || IsReturning;
            btnPark.Enabled = HasItems && _parked is null && !IsReturning;
            btnRecall.Enabled = _parked is not null && !HasItems && !IsReturning;
            btnFindCustomer.Enabled = !IsReturning;
            btnCheckout.Enabled = HasItems;
        }

        private void SetBusy(bool busy)
        {
            _busy = busy;
            txtBarcode.Enabled = !busy;
            btnAdd.Enabled = !busy;

            if (!busy)
                txtBarcode.Focus();
        }

        private void ShowError(string message)
        {
            const string prefix = "Error:";
            if (message.StartsWith(prefix))
                message = message[prefix.Length..].Trim();

            Theme.ShowMessage(lblMessage, message, true);
        }
    }
}