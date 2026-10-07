using StockFlow.CashRegister.Screens;
using System.Drawing;
using System.Windows.Forms;

namespace StockFlow.CashRegister
{
    partial class MainForm
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            headerPanel = new CardPanel();
            headerLayout = new TableLayoutPanel();
            lblTitle = new Label();
            lblStatusInfo = new Label();
            btnLogout = new Button();
            contentPanel = new Panel();
            headerPanel.SuspendLayout();
            headerLayout.SuspendLayout();
            SuspendLayout();
            //
            // headerPanel
            //
            headerPanel.BottomBorderOnly = true;
            headerPanel.Controls.Add(headerLayout);
            headerPanel.Dock = DockStyle.Top;
            headerPanel.Name = "headerPanel";
            headerPanel.Padding = new Padding(20, 0, 20, 1);
            headerPanel.Size = new Size(1280, 60);
            headerPanel.TabIndex = 0;
            //
            // headerLayout
            //
            headerLayout.ColumnCount = 3;
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            headerLayout.Controls.Add(lblTitle, 0, 0);
            headerLayout.Controls.Add(lblStatusInfo, 1, 0);
            headerLayout.Controls.Add(btnLogout, 2, 0);
            headerLayout.Dock = DockStyle.Fill;
            headerLayout.Name = "headerLayout";
            headerLayout.RowCount = 1;
            headerLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            headerLayout.TabIndex = 0;
            //
            // lblTitle
            //
            lblTitle.Dock = DockStyle.Fill;
            lblTitle.Name = "lblTitle";
            lblTitle.Text = "StockFlow Kassa";
            lblTitle.TextAlign = ContentAlignment.MiddleLeft;
            //
            // lblStatusInfo
            //
            lblStatusInfo.AutoSize = true;
            lblStatusInfo.Dock = DockStyle.Fill;
            lblStatusInfo.Margin = new Padding(3, 3, 16, 3);
            lblStatusInfo.Name = "lblStatusInfo";
            lblStatusInfo.TextAlign = ContentAlignment.MiddleRight;
            //
            // btnLogout
            //
            btnLogout.Anchor = AnchorStyles.None;
            btnLogout.Name = "btnLogout";
            btnLogout.Size = new Size(110, 36);
            btnLogout.TabIndex = 0;
            btnLogout.Text = "Uitloggen";
            //
            // contentPanel
            //
            contentPanel.Dock = DockStyle.Fill;
            contentPanel.Name = "contentPanel";
            contentPanel.TabIndex = 1;
            //
            // MainForm
            //
            ClientSize = new Size(1280, 800);
            Controls.Add(contentPanel);
            Controls.Add(headerPanel);
            FormBorderStyle = FormBorderStyle.None;
            MinimumSize = new Size(1024, 728);
            Name = "MainForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "StockFlow Kassa";
            WindowState = FormWindowState.Maximized;
            headerPanel.ResumeLayout(false);
            headerLayout.ResumeLayout(false);
            headerLayout.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private CardPanel headerPanel;
        private TableLayoutPanel headerLayout;
        private Label lblTitle;
        private Label lblStatusInfo;
        private Button btnLogout;
        private Panel contentPanel;
    }
}