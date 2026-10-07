using StockFlow.Application.DTOs.CashRegister;
using StockFlow.CashRegister.Interfaces;
using StockFlow.CashRegister.Screens;

namespace StockFlow.CashRegister
{
    public partial class ScanScreen : UserControl, IKeyScreen
    {
        private const decimal VatRate = 0.21m;   // prices are treated as VAT-inclusive
        private const int BarcodeLength = 9;     // the API only accepts 9-digit barcodes

        private readonly MainForm _mainForm;
        private readonly ICashRegisterService _cashRegister;
        private readonly List<RegisterItemDetailDto> _lines = [];
        private List<RegisterItemDetailDto>? _parked;
        private bool _busy;

        public IReadOnlyList<RegisterItemDetailDto> Lines => _lines;
        public decimal CurrentTotal => _lines.Sum(LineTotal);
        public bool HasItems => _lines.Count > 0;

        public ScanScreen(MainForm parent, ICashRegisterService cashRegister)
        {
            _mainForm = parent;
            _cashRegister = cashRegister;

            InitializeComponent();
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
            txtBarcode.KeyDown += async (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                await AddProductAsync();
            };

            btnAdd.Click += async (_, _) => await AddProductAsync();
            btnSearch.Click += (_, _) => { txtBarcode.Focus(); txtBarcode.SelectAll(); };
            btnQuantity.Click += (_, _) => ChangeQuantity();
            btnDeleteLine.Click += (_, _) => DeleteSelectedLine();
            btnCancel.Click += (_, _) => CancelSale();
            btnPark.Click += (_, _) => ParkSale();
            btnRecall.Click += (_, _) => RecallSale();
            btnCheckout.Click += (_, _) => _mainForm.ShowPurchasingScreen();
        }

        // ---------- public API used by MainForm / PurchasingScreen ----------

        public void FocusScannerInput() => txtBarcode.Focus();

        public void Clear()
        {
            _lines.Clear();
            RefreshView();
        }

        public void ShowNotice(string text) => Theme.ShowMessage(lblMessage, text);

        public bool HandleKey(Keys key)
        {
            Button? target = key switch
            {
                Keys.F1 => btnSearch,
                Keys.F2 => btnQuantity,
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

        // ---------- actions ----------

        private async Task AddProductAsync()
        {
            if (_busy) return;

            string input = txtBarcode.Text.Trim();
            if (input.Length != BarcodeLength || !input.All(char.IsDigit))
            {
                ShowError($"Voer een geldige barcode van {BarcodeLength} cijfers in.");
                txtBarcode.SelectAll();
                return;
            }

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
                    line = new RegisterItemDetailDto
                    {
                        Product = product,
                        PriceAtTimeOfSale = product.Price
                    };
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
            string? answer = InputDialog.Ask(FindForm()!, "Aantal wijzigen",
                $"Nieuw aantal voor {line.Product.ProductName}", line.Amount.ToString());

            if (answer is null) return;

            if (!int.TryParse(answer, out int quantity) || quantity < 1 || quantity > 999)
            {
                ShowError("Voer een aantal tussen 1 en 999 in.");
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
            if (!HasItems) return;

            var answer = MessageBox.Show(FindForm(), "De huidige bon annuleren?", "StockFlow Kassa",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);

            if (answer != DialogResult.Yes) return;

            Clear();
            ShowNotice("Bon geannuleerd.");
        }

        private void ParkSale()
        {
            if (!HasItems || _parked is not null) return;

            _parked = [.. _lines];
            Clear();
            ShowNotice("Bon geparkeerd. Gebruik Heropenen [F8] om verder te gaan.");
        }

        private void RecallSale()
        {
            if (_parked is null || HasItems) return;

            _lines.AddRange(_parked);
            _parked = null;
            Theme.ShowMessage(lblMessage, null);
            RefreshView();
        }

        // ---------- view ----------

        private static decimal LineTotal(RegisterItemDetailDto line) => line.PriceAtTimeOfSale * line.Amount;

        private int SelectedIndex => gridItems.CurrentRow?.Index ?? -1;

        private void RefreshView(int? select = null)
        {
            gridItems.Rows.Clear();
            foreach (var line in _lines)
            {
                gridItems.Rows.Add(
                    line.Product.Barcode.ToString(),
                    line.Product.ProductName,
                    line.Amount,
                    Theme.Money(line.PriceAtTimeOfSale),
                    Theme.Money(LineTotal(line)));
            }

            if (gridItems.Rows.Count > 0)
            {
                int index = Math.Clamp(select ?? gridItems.Rows.Count - 1, 0, gridItems.Rows.Count - 1);
                gridItems.CurrentCell = gridItems.Rows[index].Cells[0];
                gridItems.Rows[index].Selected = true;
            }

            decimal total = CurrentTotal;
            decimal vat = Math.Round(total - total / (1 + VatRate), 2);

            lblItemCount.Text = $"Artikelen: {_lines.Sum(l => l.Amount)}";
            lblSubtotal.Text = $"Subtotaal (excl. BTW): {Theme.Money(total - vat)}";
            lblVat.Text = $"BTW ({VatRate:P0}): {Theme.Money(vat)}";
            lblTotalAmount.Text = Theme.Money(total);

            btnQuantity.Enabled = HasItems;
            btnDeleteLine.Enabled = HasItems;
            btnCancel.Enabled = HasItems;
            btnPark.Enabled = HasItems && _parked is null;
            btnRecall.Enabled = _parked is not null && !HasItems;
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