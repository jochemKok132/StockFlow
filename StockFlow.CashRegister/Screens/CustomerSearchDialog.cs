using System.Drawing;
using System.Windows.Forms;
using StockFlow.CashRegister.Interfaces;
using StockFlow.Shared.Common;
using StockFlow.Shared.DTOs.Customer;

namespace StockFlow.CashRegister.Screens
{
    public static class CustomerSearchDialog
    {
        public static Task<CustomerDto?> AskAsync(IWin32Window owner, ICashRegisterService cashRegister)
        {
            using var form = new Form
            {
                Text = "Find customer by address",
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(420, 384)
            };
            Theme.ApplyForm(form);

            var lblPostalCode = new Label { Text = "Postal code", Location = new Point(24, 20), Size = new Size(180, 20) };
            var txtPostalCode = new TextBox { Location = new Point(24, 42), Size = new Size(180, 27), MaxLength = FieldLimits.PostalCode, CharacterCasing = CharacterCasing.Upper };
            var lblHouseNumber = new Label { Text = "House number", Location = new Point(216, 20), Size = new Size(180, 20) };
            var txtHouseNumber = new TextBox { Location = new Point(216, 42), Size = new Size(180, 27), MaxLength = FieldLimits.HouseNumber, CharacterCasing = CharacterCasing.Upper };
            var btnSearch = new Button { Text = "Search", Location = new Point(24, 84), Size = new Size(372, 36) };
            var lblResult = new Label { Text = "Enter a postal code and house number.", Location = new Point(24, 130), Size = new Size(372, 20) };
            var lstCustomers = new ListBox { Location = new Point(24, 154), Size = new Size(372, 150), IntegralHeight = false };
            var btnOk = new Button { Text = "Link", Location = new Point(24, 320), Size = new Size(180, 40), Enabled = false };
            var btnCancel = new Button { Text = "Cancel", Location = new Point(216, 320), Size = new Size(180, 40), DialogResult = DialogResult.Cancel };

            Theme.ApplyCaption(lblPostalCode);
            Theme.ApplyCaption(lblHouseNumber);
            Theme.ApplyInput(txtPostalCode, 12);
            Theme.ApplyInput(txtHouseNumber, 12);
            Theme.ApplySecondary(btnSearch);
            Theme.ApplyMuted(lblResult);
            Theme.ApplyPrimary(btnOk);
            Theme.ApplySecondary(btnCancel);
            lstCustomers.BorderStyle = BorderStyle.FixedSingle;
            lstCustomers.Font = new Font(form.Font.FontFamily, 11);

            List<CustomerDto> found = [];
            CustomerDto? chosen = null;

            async Task SearchAsync()
            {
                btnSearch.Enabled = false;
                lstCustomers.Items.Clear();
                btnOk.Enabled = false;
                lblResult.Text = "Searching...";

                var (customers, error) = await cashRegister.FindCustomersByAddressAsync(txtPostalCode.Text.Trim(), txtHouseNumber.Text.Trim());
                found = customers;
                btnSearch.Enabled = true;

                if (!string.IsNullOrEmpty(error))
                {
                    lblResult.Text = error.StartsWith("Error:") ? error["Error:".Length..].Trim() : error;
                    return;
                }

                lblResult.Text = found.Count switch
                {
                    0 => "No customer found at this address.",
                    1 => "1 customer found.",
                    _ => $"{found.Count} customers found. Choose the right customer.",
                };

                foreach (var customer in found)
                    lstCustomers.Items.Add($"{customer.FullName}   ·   {customer.CustomerId}   ·   {customer.Email}");

                if (found.Count > 0)
                {
                    lstCustomers.SelectedIndex = 0;
                    lstCustomers.Focus();
                }
            }

            void Choose()
            {
                if (lstCustomers.SelectedIndex < 0 || lstCustomers.SelectedIndex >= found.Count) return;
                chosen = found[lstCustomers.SelectedIndex];
                form.DialogResult = DialogResult.OK;
            }

            btnSearch.Click += async (_, _) => await SearchAsync();
            lstCustomers.SelectedIndexChanged += (_, _) => btnOk.Enabled = lstCustomers.SelectedIndex >= 0;
            lstCustomers.DoubleClick += (_, _) => Choose();
            lstCustomers.KeyDown += (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                Choose();
            };
            btnOk.Click += (_, _) => Choose();
            txtHouseNumber.KeyDown += async (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                await SearchAsync();
            };
            txtPostalCode.KeyDown += (_, e) =>
            {
                if (e.KeyCode != Keys.Enter) return;
                e.SuppressKeyPress = true;
                txtHouseNumber.Focus();
            };

            form.CancelButton = btnCancel;
            form.Controls.AddRange([lblPostalCode, txtPostalCode, lblHouseNumber, txtHouseNumber, btnSearch, lblResult, lstCustomers, btnOk, btnCancel]);
            form.Shown += (_, _) => txtPostalCode.Focus();

            return Task.FromResult(form.ShowDialog(owner) == DialogResult.OK ? chosen : null);
        }
    }
}
