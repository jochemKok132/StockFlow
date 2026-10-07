using StockFlow.CashRegister.Authentication;
using StockFlow.CashRegister.Interfaces;
using StockFlow.CashRegister.Screens;

namespace StockFlow.CashRegister
{
    /// <summary>Implemented by screens that react to function keys.</summary>
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

        /// <summary>True when the form closed because the employee logged out.</summary>
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

            ScanView = new ScanScreen(this, cashRegister) { Dock = DockStyle.Fill };
            PurchasingView = new PurchasingScreen(this, cashRegister) { Dock = DockStyle.Fill };

            _clockTimer.Tick += (_, _) => UpdateStatus();
            btnLogout.Click += btnLogout_Click;

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
                ?? "Kassier";

            UpdateStatus();
        }

        private void UpdateStatus()
        {
            string employee = string.IsNullOrEmpty(_employeeName) ? "" : $"{_employeeName}   |   ";
            lblStatusInfo.Text = $"{employee}{DateTime.Now:dd-MM-yyyy HH:mm:ss}";
        }

        private async void btnLogout_Click(object? sender, EventArgs e)
        {
            if (ScanView.HasItems)
            {
                var answer = MessageBox.Show(this, "Er staat nog een bon open. Toch uitloggen?",
                    "StockFlow Kassa", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

                if (answer != DialogResult.Yes)
                    return;
            }

            await _authStateProvider.MarkUserAsLoggedOutAsync();
            LoggedOut = true;
            Close();
        }

        // Route function keys to the visible screen, even while a textbox has focus.
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
            PurchasingView.Begin(ScanView.Lines, ScanView.CurrentTotal);
            SetScreen(PurchasingView);
        }
    }
}