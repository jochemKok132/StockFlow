using StockFlow.Shared.DTOs.CashRegister;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Customer;
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
        private CustomerDto? _customer;
        private decimal _total;
        private decimal _discount;
        private int _pointsUsed;
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

        public void Begin(IReadOnlyList<RegisterItemDetailDto> lines, decimal total, decimal discount, int pointsUsed, CustomerDto? customer)
        {
            _lines = [.. lines];
            _total = total;
            _discount = discount;
            _pointsUsed = customer is null ? 0 : pointsUsed;
            _customer = customer;
            _tendered = 0m;
            _method = "";

            Theme.ShowMessage(lblMessage, BuildCustomerMessage());
            UpdateView();
        }

        private string? BuildCustomerMessage()
        {
            if (_customer is null) return null;

            List<string> parts = [$"Klant: {_customer.FullName} ({_customer.CustomerId})"];
            if (_discount > 0) parts.Add($"Korting: {Theme.Money(_discount)}");
            if (_pointsUsed > 0) parts.Add($"Punten: {_pointsUsed} (-{Theme.Money(LoyaltyPoints.ToEuros(_pointsUsed))})");
            if (PointsToEarn > 0) parts.Add($"Spaart: +{PointsToEarn} punten");
            return string.Join("   ·   ", parts);
        }

        private int PointsToEarn => _customer is null ? 0 : LoyaltyPoints.PointsEarned(_total);

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
                string receipt = string.IsNullOrWhiteSpace(result) ? "" : $" Bonnummer: {result}.";
                if (PointsToEarn > 0) receipt += $" +{PointsToEarn} punten gespaard.";
                _mainForm.ScanView.ShowNotice(
                    change > 0 && _method == Cash
                        ? $"Transactie afgerond.{receipt} Teruggeld: {Theme.Money(change)}"
                        : $"Transactie afgerond.{receipt}");
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
            TotalOff = (double)_discount,
            CustomerId = _customer?.Id,
            PointsUsed = _pointsUsed,
            ProductsSold = [.. _lines]
        };

        private void UpdateView()
        {
            decimal difference = _tendered - _total;
            bool paid = _total == 0 || (_method != "" && difference >= 0);

            lblTotalDue.Text = Theme.Money(_total);
            lblTenderedCaption.Text = _method == "" ? "ONTVANGEN" : $"ONTVANGEN ({_method})";
            lblTendered.Text = Theme.Money(_tendered);
            lblChangeCaption.Text = paid ? "TERUGGELD" : "NOG TE BETALEN";
            lblChange.Text = Theme.Money(Math.Abs(difference));
            lblChange.ForeColor = paid ? Theme.AccentColor : Theme.DangerText;

            Theme.SetSelected(btnPin, _method == Pin);
            Theme.SetSelected(btnCash, _method == Cash);

            btnComplete.Enabled = paid && !_busy && (_total > 0 || _pointsUsed > 0);
            btnBack.Enabled = !_busy;
        }
    }
}