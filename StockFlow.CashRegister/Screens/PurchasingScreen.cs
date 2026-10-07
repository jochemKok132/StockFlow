using StockFlow.Application.DTOs.CashRegister;
using StockFlow.CashRegister.Interfaces;
using StockFlow.CashRegister.Screens;

namespace StockFlow.CashRegister
{
    public partial class PurchasingScreen : UserControl, IKeyScreen
    {
        private const string Pin = "PIN";
        private const string Cash = "CONTANT";

        private readonly MainForm _mainForm;
        private readonly ICashRegisterService _cashRegister;

        private List<RegisterItemDetailDto> _lines = [];
        private decimal _total;
        private decimal _tendered;
        private string _method = "";
        private bool _busy;

        public PurchasingScreen(MainForm parent, ICashRegisterService cashRegister)
        {
            _mainForm = parent;
            _cashRegister = cashRegister;

            InitializeComponent();
            ApplyTheme();
            SetupLogic();
            UpdateView();
        }

        private void ApplyTheme()
        {
            Theme.ApplyPage(this);

            Theme.ApplyCaption(lblMethodsHeader);
            Theme.ApplyCaption(lblQuickCashHeader);

            Theme.ApplySecondary(btnPin, 12);
            Theme.ApplySecondary(btnCash, 12);

            foreach (var button in new[] { btnExact, btn5, btn10, btn20, btn50, btn100, btnClear })
                Theme.ApplySecondary(button, 11);

            Theme.ApplyCaption(lblTotalDueCaption);
            Theme.ApplyCaption(lblTenderedCaption);
            Theme.ApplyCaption(lblChangeCaption);
            Theme.ApplyValue(lblTotalDue, 20);
            Theme.ApplyValue(lblTendered, 16);
            Theme.ApplyValue(lblChange, 22);
            Theme.ApplyMessage(lblMessage);

            Theme.ApplySecondary(btnBack, 11);
            Theme.ApplyPrimary(btnComplete, 13);
        }

        private void SetupLogic()
        {
            btnPin.Click += (_, _) => SelectMethod(Pin);
            btnCash.Click += (_, _) => SelectMethod(Cash);

            btnExact.Click += (_, _) => { _method = Cash; _tendered = _total; UpdateView(); };
            btn5.Click += (_, _) => AddCash(5m);
            btn10.Click += (_, _) => AddCash(10m);
            btn20.Click += (_, _) => AddCash(20m);
            btn50.Click += (_, _) => AddCash(50m);
            btn100.Click += (_, _) => AddCash(100m);
            btnClear.Click += (_, _) => { _method = ""; _tendered = 0m; UpdateView(); };

            btnBack.Click += (_, _) => _mainForm.ShowScanScreen();
            btnComplete.Click += async (_, _) => await CompleteAsync();
        }

        // ---------- public API used by MainForm ----------

        public void Begin(IReadOnlyList<RegisterItemDetailDto> lines, decimal total)
        {
            _lines = [.. lines];
            _total = total;
            _tendered = 0m;
            _method = "";

            Theme.ShowMessage(lblMessage, null);
            UpdateView();
        }

        public bool HandleKey(Keys key)
        {
            Button? target = key switch
            {
                Keys.F1 => btnPin,
                Keys.F2 => btnCash,
                Keys.F12 => btnComplete,
                Keys.Escape => btnBack,
                _ => null
            };

            if (target is null) return false;
            if (target.Enabled) target.PerformClick();
            return true;
        }

        // ---------- payment ----------

        private void SelectMethod(string method)
        {
            _method = method;
            _tendered = method == Pin ? _total : 0m;
            UpdateView();
        }

        private void AddCash(decimal amount)
        {
            if (_method != Cash)
            {
                _method = Cash;
                _tendered = 0m;
            }

            _tendered += amount;
            UpdateView();
        }

        private async Task CompleteAsync()
        {
            if (_busy) return;

            _busy = true;
            UpdateView();

            try
            {
                string result = await _cashRegister.FinalizePurchaseAsync(BuildPurchase());

                const string prefix = "Error:";
                if (result.StartsWith(prefix))
                {
                    Theme.ShowMessage(lblMessage, result[prefix.Length..].Trim(), true);
                    return;
                }

                decimal change = _tendered - _total;

                _mainForm.ScanView.Clear();
                _mainForm.ShowScanScreen();
                _mainForm.ScanView.ShowNotice(
                    change > 0 && _method == Cash
                        ? $"Transactie afgerond. Teruggeld: {Theme.Money(change)}"
                        : "Transactie afgerond.");
            }
            catch (Exception ex)
            {
                Theme.ShowMessage(lblMessage, ex.Message, true);
            }
            finally
            {
                _busy = false;
                UpdateView();
            }
        }

        private Purchase BuildPurchase() => new()
        {
            TotalPrice = (double)_total,
            TotalOff = 0,               // no discount function on the register yet
            ProductsSold = [.. _lines]
        };

        // ---------- view ----------

        private void UpdateView()
        {
            decimal difference = _tendered - _total;
            bool paid = _method != "" && difference >= 0;

            lblTotalDue.Text = Theme.Money(_total);
            lblTenderedCaption.Text = _method == "" ? "ONTVANGEN" : $"ONTVANGEN ({_method})";
            lblTendered.Text = Theme.Money(_tendered);
            lblChangeCaption.Text = paid ? "TERUGGELD" : "NOG TE BETALEN";
            lblChange.Text = Theme.Money(Math.Abs(difference));
            lblChange.ForeColor = paid ? Theme.AccentColor : Theme.DangerText;

            Theme.SetSelected(btnPin, _method == Pin);
            Theme.SetSelected(btnCash, _method == Cash);

            btnComplete.Enabled = paid && !_busy && _total > 0;
            btnBack.Enabled = !_busy;
        }
    }
}