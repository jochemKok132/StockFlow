using StockFlow.Application.DTOs.Employee.EmployeeAuthentication;
using StockFlow.CashRegister.Interfaces;

namespace StockFlow.CashRegister.Screens
{
    public partial class LoginForm : Form
    {
        private readonly IAuthService _authService;

        public LoginForm(IAuthService authService)
        {
            InitializeComponent();
            _authService = authService;
            ApplyTheme();

            // Fullscreen: keep the card centered and let Esc close the app.
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