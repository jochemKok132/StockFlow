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
        private bool _usePoints;
        private bool _busy;
        private string _scanCaption = "";

        public IReadOnlyList<RegisterItemDetailDto> Lines => _lines;
        public decimal LinesTotal => _lines.Sum(LineTotal);
        public int PointsToUse => _usePoints && _customer is not null && !IsReturning
            ? LoyaltyPoints.Redeemable(_customer.SavedPoints, LinesTotal)
            : 0;
        public decimal CurrentTotal => IsReturning ? RefundTotal : LinesTotal - LoyaltyPoints.ToEuros(PointsToUse);
        public decimal CurrentDiscount => IsReturning ? 0m : _lines.Sum(l => (l.Product.Price - l.PriceAtTimeOfSale) * l.Amount);
        public bool IsReturning => _receipt is not null;
        public int PointsToEarn => _customer is null || IsReturning ? 0 : LoyaltyPoints.PointsEarned(CurrentTotal);
        private decimal ReceiptLinesTotal => _receipt?.ProductsSold.Sum(p => p.PriceAtTimeOfSale * p.Amount) ?? 0m;
        private decimal RefundTotal => _lines.Sum(RefundFor);
        public bool HasItems => _lines.Count > 0;
        public CustomerDto? Customer => _customer;

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
                if (IsReturning) CancelSale();
                else SetCustomer(null);
            };
            btnUsePoints.Click += (_, _) => TogglePoints();
            btnSearch.Click += (_, _) => { txtBarcode.Focus(); txtBarcode.SelectAll(); };
            btnQuantity.Click += (_, _) => ChangeQuantity();
            btnDeleteLine.Click += (_, _) => DeleteSelectedLine();
            btnCancel.Click += (_, _) => CancelSale();
            btnPark.Click += (_, _) => ParkSale();
            btnRecall.Click += (_, _) => RecallSale();
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
                    ShowError("Tijdens een retour kan geen klant gekoppeld worden.");
                    txtBarcode.SelectAll();
                }
                else await AddCustomerAsync(input);
            }
            else if (IdentifierFormats.IsReceiptId(input))
                await LoadReceiptAsync(input);
            else
            {
                ShowError($"Voer een barcode van {IdentifierFormats.ProductBarcodeLength} cijfers, " +
                          $"een klantnummer van {IdentifierFormats.CustomerIdLength} cijfers " +
                          $"(begint met {IdentifierFormats.CustomerIdPrefix}) of een bonnummer van " +
                          $"{IdentifierFormats.ReceiptIdLength} cijfers (begint met {IdentifierFormats.ReceiptIdPrefix}) in.");
                txtBarcode.SelectAll();
            }
        }

        private async Task LoadReceiptAsync(string receiptId)
        {
            if (IsReturning)
            {
                ShowError($"Er is al een retour geopend voor bon {_receipt!.ReceiptId}. Rond deze eerst af of stop de retour [F3].");
                txtBarcode.SelectAll();
                return;
            }

            if (HasItems)
            {
                ShowError("Rond eerst de huidige bon af of annuleer deze voordat je een retour start.");
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
                    ShowError($"Alle artikelen van bon {receipt.ReceiptId} zijn al geretourneerd.");
                    txtBarcode.SelectAll();
                    return;
                }

                SetCustomer(null);
                _receipt = receipt;
                txtBarcode.Clear();
                ShowNotice($"Retour voor bon {receipt.ReceiptId}: nog {returnable} artikel(en) terug te nemen. Scan de artikelen die terugkomen.");
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
                ShowError($"Dit product staat niet op bon {_receipt.ReceiptId}.");
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
                    ? $"{sold.Product.ProductName} van bon {_receipt.ReceiptId} is al volledig geretourneerd."
                    : $"Van {sold.Product.ProductName} kunnen maximaal {max} stuks retour (gekocht: {bought}, al retour: {alreadyReturned}).");
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
                $"{count} artikel(en) retour nemen van bon {_receipt.ReceiptId}?\r\nTerug te betalen: {Theme.Money(RefundTotal)}",
                "StockFlow Kassa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

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
                string points = result.PointsDeducted > 0 ? $"   ·   {result.PointsDeducted} gespaarde punten afgeschreven" : "";
                ShowNotice($"Retour verwerkt voor bon {result.ReceiptId}: {result.ItemsReturned} artikel(en). Terugbetalen: {Theme.Money(result.RefundAmount)}{points}");
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
                    ? $"Klant {customer.FullName} gekoppeld. Aanbiedingen toegepast: {Theme.Money(CurrentDiscount)} korting."
                    : $"Klant {customer.FullName} gekoppeld aan deze bon.");
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
                ShowError("De klant heeft geen punten om te gebruiken.");
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
                string customer = _receipt.Customer is null ? "Geen klant" : $"Klant: {_receipt.Customer.FullName}";
                string pointsText = _receipt.PointsUsed > 0 ? $"   ·   {_receipt.PointsUsed} punten gebruikt" : "";
                lblCustomer.Text = $"RETOUR   ·   Bon {_receipt.ReceiptId}   ·   {_receipt.CreatedAt.ToLocalTime():dd-MM-yyyy HH:mm}   ·   {customer}{pointsText}";
                btnRemoveCustomer.Text = "Retour stoppen [F3]";
                btnUsePoints.Visible = false;
                return;
            }

            btnRemoveCustomer.Text = "Klant verwijderen [F3]";
            btnUsePoints.Visible = true;

            if (_customer is null)
            {
                lblCustomer.Text = "";
                return;
            }

            int points = PointsToUse;
            lblCustomer.Text = points > 0
                ? $"Klant: {_customer.FullName}   ·   {_customer.CustomerId}   ·   {points} van {_customer.SavedPoints} punten gebruikt (-{Theme.Money(LoyaltyPoints.ToEuros(points))})"
                : $"Klant: {_customer.FullName}   ·   {_customer.CustomerId}   ·   {_customer.SavedPoints} punten ({Theme.Money(LoyaltyPoints.ToEuros(_customer.SavedPoints))})";

            btnUsePoints.Text = _usePoints ? "Punten niet gebruiken [F4]" : "Punten gebruiken [F4]";
            btnUsePoints.Enabled = _customer.SavedPoints > 0;
            Theme.SetSelected(btnUsePoints, _usePoints);
        }

        private void ApplyPricing(RegisterItemDetailDto line)
        {
            SalesDto? sale = _customer is null ? null : BestSale(line.Product);

            line.Sale = sale;
            line.PriceAtTimeOfSale = sale is null
                ? line.Product.Price
                : Math.Round(line.Product.Price * (1 - sale.PercentageOff / 100m), 2);
        }

        private static SalesDto? BestSale(ProductDto product) =>
            product.Sales
                .Where(s => !s.SoftDeleted && s.PercentageOff > 0)
                .MaxBy(s => s.PercentageOff);

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
            string? answer = InputDialog.Ask(FindForm()!, "Aantal wijzigen",
                IsReturning
                    ? $"Aantal retour voor {line.Product.ProductName} (maximaal {max})"
                    : $"Nieuw aantal voor {line.Product.ProductName}",
                line.Amount.ToString(),
                FieldLimits.Quantity);

            if (answer is null) return;

            if (!int.TryParse(answer, out int quantity) || quantity < 1 || quantity > max)
            {
                ShowError($"Voer een aantal tussen 1 en {max} in.");
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
                var answer = MessageBox.Show(FindForm(), returning ? "De retour annuleren?" : "De huidige bon annuleren?",
                    "StockFlow Kassa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (answer != DialogResult.Yes) return;
            }

            Clear();
            ShowNotice(returning ? "Retour geannuleerd." : "Bon geannuleerd.");
        }

        private void ParkSale()
        {
            if (!HasItems || _parked is not null || IsReturning) return;

            _parked = [.. _lines];
            _parkedCustomer = _customer;
            Clear();
            ShowNotice("Bon geparkeerd. Gebruik Heropenen [F8] om verder te gaan.");
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

        private static decimal LineTotal(RegisterItemDetailDto line) => line.PriceAtTimeOfSale * line.Amount;

        private int SelectedIndex => gridItems.CurrentRow?.Index ?? -1;

        private void RefreshView(int? select = null)
        {
            gridItems.Rows.Clear();
            foreach (var line in _lines)
            {
                string description = line.Sale is null
                    ? line.Product.ProductName
                    : $"{line.Product.ProductName}   (-{line.Sale.PercentageOff}% {line.Sale.SaleName})";

                if (IsReturning)
                    description += $"   (retour, max {Returnable(line.Product)})";

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
                ? $"Retour: {_lines.Sum(l => l.Amount)} artikel(en)"
                : $"Artikelen: {_lines.Sum(l => l.Amount)}";
            if (IsReturning && _receipt!.PointsUsed > 0)
                summary += "   ·   Terugbetaling naar rato (punten gebruikt)";
            if (discount > 0)
                summary += $"   ·   Korting: {Theme.Money(discount)}";
            if (points > 0)
                summary += $"   ·   Punten: -{Theme.Money(LoyaltyPoints.ToEuros(points))}";
            if (PointsToEarn > 0)
                summary += $"   ·   Spaart: +{PointsToEarn} punten";
            lblItemCount.Text = summary;

            UpdateCustomerView();
            lblSubtotal.Text = $"Subtotaal (excl. BTW): {Theme.Money(total - vat)}";
            lblVat.Text = $"BTW ({VatRate:P0}): {Theme.Money(vat)}";
            lblTotalAmount.Text = IsReturning && total > 0 ? $"-{Theme.Money(total)}" : Theme.Money(total);
            lblScan.Text = IsReturning ? $"RETOUR BON {_receipt!.ReceiptId}  ·  SCAN DE ARTIKELEN (9 CIJFERS)" : _scanCaption;
            btnCheckout.Text = IsReturning ? "RETOUR AFRONDEN  [F10]" : "AFREKENEN  [F10]";

            btnQuantity.Enabled = HasItems;
            btnDeleteLine.Enabled = HasItems;
            btnCancel.Enabled = HasItems || IsReturning;
            btnPark.Enabled = HasItems && _parked is null && !IsReturning;
            btnRecall.Enabled = _parked is not null && !HasItems && !IsReturning;
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