using System.Drawing;
using System.Windows.Forms;

namespace StockFlow.CashRegister.Screens
{
    /// <summary>Small themed replacement for the missing WinForms input box.</summary>
    public static class InputDialog
    {
        public static string? Ask(IWin32Window owner, string title, string prompt, string initial = "")
        {
            using var form = new Form
            {
                Text = title,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                StartPosition = FormStartPosition.CenterParent,
                MaximizeBox = false,
                MinimizeBox = false,
                ShowInTaskbar = false,
                ClientSize = new Size(360, 176)
            };
            Theme.ApplyForm(form);

            var lblPrompt = new Label { Text = prompt, Location = new Point(24, 20), Size = new Size(312, 40) };
            Theme.ApplyMuted(lblPrompt);

            var txtValue = new TextBox { Text = initial, Location = new Point(24, 68), Size = new Size(312, 27) };
            Theme.ApplyInput(txtValue, 12);

            var btnOk = new Button { Text = "OK", Location = new Point(24, 120), Size = new Size(150, 40), DialogResult = DialogResult.OK };
            Theme.ApplyPrimary(btnOk);

            var btnCancel = new Button { Text = "Annuleren", Location = new Point(186, 120), Size = new Size(150, 40), DialogResult = DialogResult.Cancel };
            Theme.ApplySecondary(btnCancel);

            form.AcceptButton = btnOk;
            form.CancelButton = btnCancel;
            form.Controls.AddRange([lblPrompt, txtValue, btnOk, btnCancel]);
            form.Shown += (_, _) => { txtValue.Focus(); txtValue.SelectAll(); };

            return form.ShowDialog(owner) == DialogResult.OK ? txtValue.Text.Trim() : null;
        }
    }
}