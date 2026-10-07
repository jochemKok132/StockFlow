using StockFlow.CashRegister.Screens;
using System.Drawing;
using System.Windows.Forms;

namespace StockFlow.CashRegister
{
    partial class PurchasingScreen
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

        #region Component Designer generated code

        private void InitializeComponent()
        {
            mainLayout = new TableLayoutPanel();
            leftPanel = new TableLayoutPanel();
            lblMethodsHeader = new Label();
            methodsGrid = new TableLayoutPanel();
            btnPin = new Button();
            btnCash = new Button();
            lblQuickCashHeader = new Label();
            quickCashGrid = new TableLayoutPanel();
            btnExact = new Button();
            btn5 = new Button();
            btn10 = new Button();
            btn20 = new Button();
            btn50 = new Button();
            btn100 = new Button();
            btnClear = new Button();
            rightPanel = new TableLayoutPanel();
            summaryCard = new CardPanel();
            summaryTable = new TableLayoutPanel();
            lblTotalDueCaption = new Label();
            lblTotalDue = new Label();
            lblTenderedCaption = new Label();
            lblTendered = new Label();
            lblChangeCaption = new Label();
            lblChange = new Label();
            lblMessage = new Label();
            btnBack = new Button();
            btnComplete = new Button();
            mainLayout.SuspendLayout();
            leftPanel.SuspendLayout();
            methodsGrid.SuspendLayout();
            quickCashGrid.SuspendLayout();
            rightPanel.SuspendLayout();
            summaryCard.SuspendLayout();
            summaryTable.SuspendLayout();
            SuspendLayout();
            //
            // mainLayout
            //
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 55F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 45F));
            mainLayout.Controls.Add(leftPanel, 0, 0);
            mainLayout.Controls.Add(rightPanel, 1, 0);
            mainLayout.Dock = DockStyle.Fill;
            mainLayout.Name = "mainLayout";
            mainLayout.Padding = new Padding(16);
            mainLayout.RowCount = 1;
            mainLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            mainLayout.TabIndex = 0;
            //
            // leftPanel
            //
            leftPanel.ColumnCount = 1;
            leftPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            leftPanel.Controls.Add(lblMethodsHeader, 0, 0);
            leftPanel.Controls.Add(methodsGrid, 0, 1);
            leftPanel.Controls.Add(lblQuickCashHeader, 0, 2);
            leftPanel.Controls.Add(quickCashGrid, 0, 3);
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.Margin = new Padding(0, 0, 16, 0);
            leftPanel.Name = "leftPanel";
            leftPanel.RowCount = 5;
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 28F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 100F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 52F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 140F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            leftPanel.TabIndex = 0;
            //
            // lblMethodsHeader
            //
            lblMethodsHeader.Dock = DockStyle.Fill;
            lblMethodsHeader.Margin = new Padding(0);
            lblMethodsHeader.Name = "lblMethodsHeader";
            lblMethodsHeader.Text = "BETAALWIJZE";
            lblMethodsHeader.TextAlign = ContentAlignment.MiddleLeft;
            //
            // methodsGrid
            //
            methodsGrid.ColumnCount = 2;
            methodsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            methodsGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            methodsGrid.Controls.Add(btnPin, 0, 0);
            methodsGrid.Controls.Add(btnCash, 1, 0);
            methodsGrid.Dock = DockStyle.Fill;
            methodsGrid.Margin = new Padding(0);
            methodsGrid.Name = "methodsGrid";
            methodsGrid.RowCount = 1;
            methodsGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            methodsGrid.TabIndex = 1;
            //
            // btnPin
            //
            btnPin.Dock = DockStyle.Fill;
            btnPin.Margin = new Padding(0, 0, 6, 0);
            btnPin.Name = "btnPin";
            btnPin.TabIndex = 0;
            btnPin.Text = "PIN\r\n[F1]";
            //
            // btnCash
            //
            btnCash.Dock = DockStyle.Fill;
            btnCash.Margin = new Padding(6, 0, 0, 0);
            btnCash.Name = "btnCash";
            btnCash.TabIndex = 1;
            btnCash.Text = "CONTANT\r\n[F2]";
            //
            // lblQuickCashHeader
            //
            lblQuickCashHeader.Dock = DockStyle.Fill;
            lblQuickCashHeader.Margin = new Padding(0);
            lblQuickCashHeader.Name = "lblQuickCashHeader";
            lblQuickCashHeader.Text = "SNEL CONTANT";
            lblQuickCashHeader.TextAlign = ContentAlignment.BottomLeft;
            //
            // quickCashGrid
            //
            quickCashGrid.ColumnCount = 4;
            for (int i = 0; i < 4; i++)
                quickCashGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            quickCashGrid.Controls.Add(btnExact, 0, 0);
            quickCashGrid.Controls.Add(btn5, 1, 0);
            quickCashGrid.Controls.Add(btn10, 2, 0);
            quickCashGrid.Controls.Add(btn20, 3, 0);
            quickCashGrid.Controls.Add(btn50, 0, 1);
            quickCashGrid.Controls.Add(btn100, 1, 1);
            quickCashGrid.Controls.Add(btnClear, 2, 1);
            quickCashGrid.SetColumnSpan(btnClear, 2);
            quickCashGrid.Dock = DockStyle.Fill;
            quickCashGrid.Margin = new Padding(0);
            quickCashGrid.Name = "quickCashGrid";
            quickCashGrid.RowCount = 2;
            quickCashGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            quickCashGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 50F));
            quickCashGrid.TabIndex = 3;
            //
            // quick cash buttons
            //
            ConfigureQuickButton(btnExact, "btnExact", "Exact", 0);
            ConfigureQuickButton(btn5, "btn5", "€ 5", 1);
            ConfigureQuickButton(btn10, "btn10", "€ 10", 2);
            ConfigureQuickButton(btn20, "btn20", "€ 20", 3);
            ConfigureQuickButton(btn50, "btn50", "€ 50", 4);
            ConfigureQuickButton(btn100, "btn100", "€ 100", 5);
            ConfigureQuickButton(btnClear, "btnClear", "Wissen", 6);
            //
            // rightPanel
            //
            rightPanel.ColumnCount = 1;
            rightPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            rightPanel.Controls.Add(summaryCard, 0, 0);
            rightPanel.Controls.Add(btnBack, 0, 2);
            rightPanel.Controls.Add(btnComplete, 0, 3);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Margin = new Padding(0);
            rightPanel.Name = "rightPanel";
            rightPanel.RowCount = 4;
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 280F));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 56F));
            rightPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 68F));
            rightPanel.TabIndex = 1;
            //
            // summaryCard
            //
            summaryCard.Controls.Add(summaryTable);
            summaryCard.Controls.Add(lblMessage);
            summaryCard.Dock = DockStyle.Fill;
            summaryCard.Margin = new Padding(0);
            summaryCard.Name = "summaryCard";
            summaryCard.Padding = new Padding(24);
            summaryCard.TabIndex = 0;
            //
            // summaryTable
            //
            summaryTable.ColumnCount = 2;
            summaryTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            summaryTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            summaryTable.Controls.Add(lblTotalDueCaption, 0, 0);
            summaryTable.Controls.Add(lblTotalDue, 1, 0);
            summaryTable.Controls.Add(lblTenderedCaption, 0, 1);
            summaryTable.Controls.Add(lblTendered, 1, 1);
            summaryTable.Controls.Add(lblChangeCaption, 0, 2);
            summaryTable.Controls.Add(lblChange, 1, 2);
            summaryTable.Dock = DockStyle.Fill;
            summaryTable.Name = "summaryTable";
            summaryTable.RowCount = 3;
            summaryTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            summaryTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33.33F));
            summaryTable.RowStyles.Add(new RowStyle(SizeType.Percent, 33.34F));
            summaryTable.TabIndex = 0;
            //
            // summary labels
            //
            ConfigureSummaryLabel(lblTotalDueCaption, "lblTotalDueCaption", "TE BETALEN", ContentAlignment.MiddleLeft);
            ConfigureSummaryLabel(lblTotalDue, "lblTotalDue", "", ContentAlignment.MiddleRight);
            ConfigureSummaryLabel(lblTenderedCaption, "lblTenderedCaption", "ONTVANGEN", ContentAlignment.MiddleLeft);
            ConfigureSummaryLabel(lblTendered, "lblTendered", "", ContentAlignment.MiddleRight);
            ConfigureSummaryLabel(lblChangeCaption, "lblChangeCaption", "TERUGGELD", ContentAlignment.MiddleLeft);
            ConfigureSummaryLabel(lblChange, "lblChange", "", ContentAlignment.MiddleRight);
            //
            // lblMessage
            //
            lblMessage.Dock = DockStyle.Bottom;
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 40);
            //
            // btnBack
            //
            btnBack.Dock = DockStyle.Fill;
            btnBack.Margin = new Padding(0, 0, 0, 8);
            btnBack.Name = "btnBack";
            btnBack.TabIndex = 1;
            btnBack.Text = "Terug naar scannen  [ESC]";
            //
            // btnComplete
            //
            btnComplete.Dock = DockStyle.Fill;
            btnComplete.Margin = new Padding(0);
            btnComplete.Name = "btnComplete";
            btnComplete.TabIndex = 2;
            btnComplete.Text = "TRANSACTIE VOLTOOIEN  [F12]";
            //
            // PurchasingScreen
            //
            Controls.Add(mainLayout);
            Name = "PurchasingScreen";
            Size = new Size(1280, 740);
            mainLayout.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            methodsGrid.ResumeLayout(false);
            quickCashGrid.ResumeLayout(false);
            rightPanel.ResumeLayout(false);
            summaryCard.ResumeLayout(false);
            summaryTable.ResumeLayout(false);
            ResumeLayout(false);
        }

        private static void ConfigureQuickButton(Button button, string name, string text, int tabIndex)
        {
            button.Dock = DockStyle.Fill;
            button.Margin = new Padding(0, 0, 8, 8);
            button.Name = name;
            button.TabIndex = tabIndex;
            button.Text = text;
        }

        private static void ConfigureSummaryLabel(Label label, string name, string text, ContentAlignment alignment)
        {
            label.Dock = DockStyle.Fill;
            label.Margin = new Padding(0);
            label.Name = name;
            label.Text = text;
            label.TextAlign = alignment;
        }

        #endregion

        private TableLayoutPanel mainLayout;
        private TableLayoutPanel leftPanel;
        private Label lblMethodsHeader;
        private TableLayoutPanel methodsGrid;
        private Button btnPin;
        private Button btnCash;
        private Label lblQuickCashHeader;
        private TableLayoutPanel quickCashGrid;
        private Button btnExact;
        private Button btn5;
        private Button btn10;
        private Button btn20;
        private Button btn50;
        private Button btn100;
        private Button btnClear;
        private TableLayoutPanel rightPanel;
        private CardPanel summaryCard;
        private TableLayoutPanel summaryTable;
        private Label lblTotalDueCaption;
        private Label lblTotalDue;
        private Label lblTenderedCaption;
        private Label lblTendered;
        private Label lblChangeCaption;
        private Label lblChange;
        private Label lblMessage;
        private Button btnBack;
        private Button btnComplete;
    }
}