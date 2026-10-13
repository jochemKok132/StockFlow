using StockFlow.Shared.DTOs.Employee.EmployeeAuthentication;
using StockFlow.CashRegister.Interfaces;
using StockFlow.Shared.Common;

namespace StockFlow.CashRegister.Screens
{
    public partial class LoginForm : Form
    {
        private readonly IAuthService _authService;
        private readonly ICashRegisterService _cashRegister;

        public LoginForm(IAuthService authService, ICashRegisterService cashRegister)
        {
            InitializeComponent();
            _authService = authService;
            _cashRegister = cashRegister;
            ApplyTheme();

            Text = $"StockFlow Kassa - Inloggen ({AppVersion.Display(Program.Version)})";
            lblSubtitle.Text = $"Log in om de kassa te openen   ·   {AppVersion.Display(Program.Version)}";
            Shown += async (_, _) => await CheckForUpdateAsync();

            KeyPreview = true;
            Resize += (_, _) => CenterCard();
            Load += (_, _) => { CenterCard(); txtUsername.Focus(); };
            KeyDown += (_, e) =>
            {
                if (e.KeyCode == Keys.Escape)
                    DialogResult = DialogResult.Cancel;
            };
        }

        private void CenterCard()
        {
            card.Location = new Point(
                (ClientSize.Width - card.Width) / 2,
                (ClientSize.Height - card.Height) / 2);
        }

        private void ApplyTheme()
        {
            Theme.ApplyForm(this);
            Theme.ApplyTitle(lblBrand, 18);
            Theme.ApplyMuted(lblSubtitle);
            Theme.ApplyCaption(lblUsername);
            Theme.ApplyCaption(lblPassword);
            Theme.ApplyInput(txtUsername);
            Theme.ApplyInput(txtPassword);
            Theme.ApplyMessage(lblError);
            Theme.ApplyPrimary(btnLogin, 11);
        }

        private async void btnLogin_Click(object? sender, EventArgs e)
        {
            string username = txtUsername.Text.Trim();
            string password = txtPassword.Text;

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            {
                Theme.ShowMessage(lblError, "Gebruikersnaam en wachtwoord zijn verplicht.", true);
                return;
            }

            SetLoading(true);

            try
            {
                var request = new LoginRequest
                {
                    EmployeeId = username,
                    Password = password
                };

                var response = await _authService.LoginAsync(request);

                if (response.Succeeded && response.Value is not null)
                {
                    DialogResult = DialogResult.OK;
                    Close();
                    return;
                }

                Theme.ShowMessage(lblError, response.Message ?? "Ongeldige inloggegevens.", true);
            }
            catch (Exception ex)
            {
                Theme.ShowMessage(lblError, $"Inloggen mislukt: {ex.Message}", true);
            }
            finally
            {
                SetLoading(false);
            }
        }

        private async Task CheckForUpdateAsync()
        {
            try
            {
                var latest = await _cashRegister.GetLatestReleaseAsync();
                if (latest is null || !AppVersion.IsNewer(latest.Version, Program.Version))
                    return;

                if (lblError.Visible)
                    return;

                Theme.ShowMessage(lblError,
                    $"Nieuwe versie {AppVersion.Display(latest.Version)} beschikbaar. Download deze via Releases in de management app.");
            }
            catch (Exception)
            {
            }
        }

        private void SetLoading(bool isLoading)
        {
            btnLogin.Enabled = !isLoading;
            txtUsername.Enabled = !isLoading;
            txtPassword.Enabled = !isLoading;
            btnLogin.Text = isLoading ? "Bezig..." : "Inloggen";

            if (isLoading)
                lblError.Visible = false;
        }
    }
}