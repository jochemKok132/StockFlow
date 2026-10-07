using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace StockFlow.CashRegister.Screens
{
    /// <summary>
    /// WinForms version of the :root tokens from the web stylesheet.
    /// All styling lives here, so the designer files only contain structure.
    /// </summary>
    public static class Theme
    {
        public static readonly Color TaskbarColor = Color.White;
        public static readonly Color DashboardColor = Color.FromArgb(247, 247, 248);
        public static readonly Color SurfaceColor = Color.FromArgb(245, 245, 245);   // rgba(17,17,17,.04) on white
        public static readonly Color BorderColor = Color.FromArgb(236, 236, 236);    // rgba(17,17,17,.08) on white
        public static readonly Color TextColor = Color.FromArgb(17, 17, 17);
        public static readonly Color MutedText = Color.FromArgb(138, 138, 144);
        public static readonly Color AccentColor = Color.FromArgb(91, 141, 239);
        public static readonly Color AccentColorHover = Color.FromArgb(67, 115, 217);
        public static readonly Color AccentColorSoft = Color.FromArgb(230, 238, 253); // rgba(91,141,239,.15) on white
        public static readonly Color ButtonColor = Color.FromArgb(17, 17, 17);
        public static readonly Color ButtonColorHover = Color.FromArgb(42, 42, 42);
        public static readonly Color ButtonTextColor = Color.White;
        public static readonly Color ButtonDisabledColor = Color.FromArgb(195, 200, 209);
        public static readonly Color DangerBg = Color.FromArgb(253, 236, 236);
        public static readonly Color DangerText = Color.FromArgb(185, 28, 28);

        private const string Family = "Segoe UI";
        private static readonly CultureInfo Dutch = new("nl-NL");

        public static string Money(decimal value) => value.ToString("C", Dutch);

        private static Font Font(float size, FontStyle style = FontStyle.Regular) => new(Family, size, style);

        // ---------- containers ----------

        public static void ApplyForm(Form form)
        {
            form.BackColor = DashboardColor;
            form.ForeColor = TextColor;
            form.Font = Font(10);
        }

        public static void ApplyPage(Control control)
        {
            control.BackColor = DashboardColor;
            control.ForeColor = TextColor;
            control.Font = Font(10);
        }

        // ---------- labels ----------

        public static void ApplyTitle(Label label, float size = 18)
        {
            label.Font = Font(size, FontStyle.Bold);
            label.ForeColor = TextColor;
        }

        /// <summary>Small uppercase label, like .filter-group label.</summary>
        public static void ApplyCaption(Label label)
        {
            label.Font = Font(8.5f, FontStyle.Bold);
            label.ForeColor = MutedText;
        }

        public static void ApplyMuted(Label label, float size = 10)
        {
            label.Font = Font(size);
            label.ForeColor = MutedText;
        }

        public static void ApplyValue(Label label, float size, Color? color = null)
        {
            label.Font = Font(size, FontStyle.Bold);
            label.ForeColor = color ?? TextColor;
        }

        /// <summary>Inline alert, like .alert-danger. Hidden until ShowMessage is called.</summary>
        public static void ApplyMessage(Label label)
        {
            label.AutoSize = false;
            label.Font = Font(10);
            label.Padding = new Padding(12, 0, 12, 0);
            label.TextAlign = ContentAlignment.MiddleLeft;
            label.Visible = false;
        }

        public static void ShowMessage(Label label, string? text, bool isError = false)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                label.Visible = false;
                return;
            }

            label.Text = text;
            label.BackColor = isError ? DangerBg : AccentColorSoft;
            label.ForeColor = isError ? DangerText : TextColor;
            label.Visible = true;
        }

        // ---------- inputs ----------

        public static void ApplyInput(TextBox box, float size = 10.5f)
        {
            box.BorderStyle = BorderStyle.FixedSingle;
            box.BackColor = TaskbarColor;
            box.ForeColor = TextColor;
            box.Font = Font(size);
        }

        // ---------- buttons ----------

        private static void FlatButton(Button b, float size)
        {
            b.UseVisualStyleBackColor = false;
            b.FlatStyle = FlatStyle.Flat;
            b.FlatAppearance.BorderSize = 1;
            b.Font = Font(size, FontStyle.Bold);
            b.Cursor = Cursors.Hand;
        }

        /// <summary>Black button, like --ButtonColor.</summary>
        public static void ApplyPrimary(Button b, float size = 10)
        {
            FlatButton(b, size);
            b.BackColor = ButtonColor;
            b.ForeColor = ButtonTextColor;
            b.FlatAppearance.BorderColor = ButtonColor;
            b.FlatAppearance.MouseOverBackColor = ButtonColorHover;
            b.FlatAppearance.MouseDownBackColor = ButtonColorHover;

            b.EnabledChanged += (_, _) =>
            {
                b.BackColor = b.Enabled ? ButtonColor : ButtonDisabledColor;
                b.FlatAppearance.BorderColor = b.BackColor;
            };
        }

        /// <summary>White button with a subtle border.</summary>
        public static void ApplySecondary(Button b, float size = 10)
        {
            FlatButton(b, size);
            b.BackColor = TaskbarColor;
            b.ForeColor = TextColor;
            b.FlatAppearance.BorderColor = BorderColor;
            b.FlatAppearance.MouseOverBackColor = SurfaceColor;
            b.FlatAppearance.MouseDownBackColor = SurfaceColor;
        }

        /// <summary>Secondary button that turns black while selected, like .ml-nav-link.active.</summary>
        public static void SetSelected(Button b, bool selected)
        {
            b.BackColor = selected ? ButtonColor : TaskbarColor;
            b.ForeColor = selected ? ButtonTextColor : TextColor;
            b.FlatAppearance.BorderColor = selected ? ButtonColor : BorderColor;
            b.FlatAppearance.MouseOverBackColor = selected ? ButtonColorHover : SurfaceColor;
        }

        // ---------- grid ----------

        public static void ApplyGrid(DataGridView grid)
        {
            grid.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            grid.BackgroundColor = TaskbarColor;
            grid.BorderStyle = BorderStyle.None;
            grid.GridColor = BorderColor;
            grid.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            grid.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            grid.EnableHeadersVisualStyles = false;
            grid.RowHeadersVisible = false;
            grid.ReadOnly = true;
            grid.AllowUserToAddRows = false;
            grid.AllowUserToDeleteRows = false;
            grid.AllowUserToResizeRows = false;
            grid.MultiSelect = false;
            grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            grid.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            grid.ColumnHeadersHeight = 40;
            grid.RowTemplate.Height = 44;

            grid.ColumnHeadersDefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = DashboardColor,
                ForeColor = TextColor,
                SelectionBackColor = DashboardColor,
                SelectionForeColor = TextColor,
                Font = Font(8.5f, FontStyle.Bold),
                Padding = new Padding(8, 0, 8, 0),
                Alignment = DataGridViewContentAlignment.MiddleLeft
            };

            grid.DefaultCellStyle = new DataGridViewCellStyle
            {
                BackColor = TaskbarColor,
                ForeColor = TextColor,
                SelectionBackColor = AccentColorSoft,
                SelectionForeColor = TextColor,
                Font = Font(10.5f),
                Padding = new Padding(8, 0, 8, 0)
            };
        }
    }

    /// <summary>White card with a 1px border, like .card.</summary>
    public class CardPanel : Panel
    {
        [DefaultValue(false)]
        public bool BottomBorderOnly { get; set; }

        public CardPanel()
        {
            DoubleBuffered = true;
            ResizeRedraw = true;
            BackColor = Theme.TaskbarColor;
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            using var pen = new Pen(Theme.BorderColor);

            if (BottomBorderOnly)
                e.Graphics.DrawLine(pen, 0, Height - 1, Width, Height - 1);
            else
                e.Graphics.DrawRectangle(pen, 0, 0, Width - 1, Height - 1);
        }
    }
}