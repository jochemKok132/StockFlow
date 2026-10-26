using StockFlow.CashRegister.Authentication;
using StockFlow.Shared.Common;
using StockFlow.CashRegister.Interfaces;
using StockFlow.CashRegister.Screens;

namespace StockFlow.CashRegister
{
    public interface IKeyScreen
    {
        bool HandleKey(Keys key);
    }

    public partial class MainForm : Form
    {
        private readonly AuthStateProvider _authStateProvider;
        private readonly System.Windows.Forms.Timer _clockTimer;
        private string _employeeName = "";
        private Control? _activeScreen;

        public bool LoggedOut { get; private set; }

        public ScanScreen ScanView { get; }
        public PurchasingScreen PurchasingView { get; }

        public MainForm(ICashRegisterService cashRegister, AuthStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;

            InitializeComponent();
            components ??= new System.ComponentModel.Container();
            _clockTimer = new System.Windows.Forms.Timer(components) { Interval = 1000 };

            ApplyTheme();
            Text = $"StockFlow Cash Register {AppVersion.Display(Program.Version)}";

            ScanView = new ScanScreen(this, cashRegister) { Dock = DockStyle.Fill };
            PurchasingView = new PurchasingScreen(this, cashRegister) { Dock = DockStyle.Fill };

            _clockTimer.Tick += (_, _) => UpdateStatus();
            btnLogout.Click += btnLogout_Click;
            _authStateProvider.AuthenticationStateChanged += OnAuthenticationStateChanged;
            FormClosed += (_, _) => _authStateProvider.AuthenticationStateChanged -= OnAuthenticationStateChanged;

            UpdateStatus();
            _clockTimer.Start();
            ShowScanScreen();
        }

        private void ApplyTheme()
        {
            Theme.ApplyForm(this);
            contentPanel.BackColor = Theme.DashboardColor;
            Theme.ApplyTitle(lblTitle, 14);
            Theme.ApplyMuted(lblStatusInfo, 10);
            Theme.ApplySecondary(btnLogout, 9);
        }

        protected override async void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var state = await _authStateProvider.GetAuthenticationStateAsync();
            var user = state.User;

            _employeeName = user.Identity?.Name
                ?? user.FindFirst("name")?.Value
                ?? user.FindFirst("unique_name")?.Value
                ?? "Cashier";

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            string employee = string.IsNullOrEmpty(_employeeName) ? "" : $"{_employeeName}   |   ";
            lblStatusInfo.Text = $"{employee}{AppVersion.Display(Program.Version)}   |   {DateTime.Now:dd-MM-yyyy HH:mm:ss}";
        }

        private async void btnLogout_Click(object? sender, EventArgs e)
        {
            if (ScanView.HasItems)
            {
                var answer = MessageBox.Show(this, "A receipt is still open. Log out anyway?",
                    "StockFlow Cash Register", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (answer != DialogResult.Yes)
                    return;
            }

            LoggedOut = true;
            await _authStateProvider.MarkUserAsLoggedOutAsync();
            Close();
        }

        private async void OnAuthenticationStateChanged(Task<Microsoft.AspNetCore.Components.Authorization.AuthenticationState> task)
        {
            var state = await task;
            if (state.User.Identity?.IsAuthenticated == true || LoggedOut || IsDisposed)
                return;

            LoggedOut = true;
            BeginInvoke(() =>
            {
                MessageBox.Show(this, "Your session has expired. Please log in again.", "StockFlow Cash Register", MessageBoxButtons.OK, MessageBoxIcon.Information);
                Close();
            });
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (_activeScreen is IKeyScreen screen && screen.HandleKey(keyData))
                return true;

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void SetScreen(Control screen)
        {
            contentPanel.Controls.Clear();
            contentPanel.Controls.Add(screen);
            _activeScreen = screen;
        }

        public void ShowScanScreen()
        {
            SetScreen(ScanView);
            ScanView.FocusScannerInput();
        }

        public void ShowPurchasingScreen()
        {
            PurchasingView.Begin(ScanView.Lines, ScanView.CurrentTotal, ScanView.CurrentDiscount, ScanView.PointsToUse, ScanView.Customer);
            SetScreen(PurchasingView);
        }
    }
}