using StockFlow.CashRegister.Screens;
using System.Drawing;
using System.Windows.Forms;

namespace StockFlow.CashRegister
{
    partial class ScanScreen
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
            scanBoxPanel = new TableLayoutPanel();
            lblScan = new Label();
            txtBarcode = new TextBox();
            btnAdd = new Button();
            lblMessage = new Label();
            gridCard = new CardPanel();
            gridItems = new DataGridView();
            colBarcode = new DataGridViewTextBoxColumn();
            colDescription = new DataGridViewTextBoxColumn();
            colQty = new DataGridViewTextBoxColumn();
            colPrice = new DataGridViewTextBoxColumn();
            colTotal = new DataGridViewTextBoxColumn();
            totalCard = new CardPanel();
            lblItemCount = new Label();
            lblSubtotal = new Label();
            lblVat = new Label();
            lblTotalAmount = new Label();
            rightPanel = new Panel();
            buttonGrid = new TableLayoutPanel();
            btnSearch = new Button();
            btnQuantity = new Button();
            btnDeleteLine = new Button();
            btnCancel = new Button();
            btnPark = new Button();
            btnRecall = new Button();
            btnCheckout = new Button();
            mainLayout.SuspendLayout();
            leftPanel.SuspendLayout();
            scanBoxPanel.SuspendLayout();
            gridCard.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridItems).BeginInit();
            totalCard.SuspendLayout();
            rightPanel.SuspendLayout();
            buttonGrid.SuspendLayout();
            SuspendLayout();
            //
            // mainLayout
            //
            mainLayout.ColumnCount = 2;
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 64F));
            mainLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 36F));
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
            leftPanel.Controls.Add(scanBoxPanel, 0, 0);
            leftPanel.Controls.Add(lblMessage, 0, 1);
            leftPanel.Controls.Add(gridCard, 0, 2);
            leftPanel.Controls.Add(totalCard, 0, 3);
            leftPanel.Dock = DockStyle.Fill;
            leftPanel.Margin = new Padding(0, 0, 16, 0);
            leftPanel.Name = "leftPanel";
            leftPanel.RowCount = 4;
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 76F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.AutoSize));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            leftPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 124F));
            leftPanel.TabIndex = 0;
            //
            // scanBoxPanel
            //
            scanBoxPanel.ColumnCount = 2;
            scanBoxPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            scanBoxPanel.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130F));
            scanBoxPanel.Controls.Add(lblScan, 0, 0);
            scanBoxPanel.SetColumnSpan(lblScan, 2);
            scanBoxPanel.Controls.Add(txtBarcode, 0, 1);
            scanBoxPanel.Controls.Add(btnAdd, 1, 1);
            scanBoxPanel.Dock = DockStyle.Fill;
            scanBoxPanel.Margin = new Padding(0, 0, 0, 8);
            scanBoxPanel.Name = "scanBoxPanel";
            scanBoxPanel.RowCount = 2;
            scanBoxPanel.RowStyles.Add(new RowStyle(SizeType.Absolute, 22F));
            scanBoxPanel.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            scanBoxPanel.TabIndex = 0;
            //
            // lblScan
            //
            lblScan.Dock = DockStyle.Fill;
            lblScan.Margin = new Padding(0);
            lblScan.Name = "lblScan";
            lblScan.Text = "SCAN / INVOER";
            lblScan.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txtBarcode
            //
            txtBarcode.Anchor = AnchorStyles.Left | AnchorStyles.Right;
            txtBarcode.Margin = new Padding(0, 0, 8, 0);
            txtBarcode.MaxLength = 9;
            txtBarcode.Name = "txtBarcode";
            txtBarcode.TabIndex = 0;
            //
            // btnAdd
            //
            btnAdd.Dock = DockStyle.Fill;
            btnAdd.Margin = new Padding(0);
            btnAdd.Name = "btnAdd";
            btnAdd.TabIndex = 1;
            btnAdd.Text = "Toevoegen";
            //
            // lblMessage
            //
            lblMessage.Dock = DockStyle.Fill;
            lblMessage.Margin = new Padding(0, 0, 0, 8);
            lblMessage.Name = "lblMessage";
            lblMessage.Size = new Size(0, 40);
            //
            // gridCard
            //
            gridCard.Controls.Add(gridItems);
            gridCard.Dock = DockStyle.Fill;
            gridCard.Margin = new Padding(0, 0, 0, 8);
            gridCard.Name = "gridCard";
            gridCard.Padding = new Padding(1);
            gridCard.TabIndex = 2;
            //
            // gridItems
            //
            gridItems.Columns.AddRange(new DataGridViewColumn[] { colBarcode, colDescription, colQty, colPrice, colTotal });
            gridItems.Dock = DockStyle.Fill;
            gridItems.Name = "gridItems";
            gridItems.TabIndex = 0;
            //
            // colBarcode
            //
            colBarcode.FillWeight = 70F;
            colBarcode.HeaderText = "BARCODE";
            colBarcode.Name = "colBarcode";
            //
            // colDescription
            //
            colDescription.FillWeight = 160F;
            colDescription.HeaderText = "OMSCHRIJVING";
            colDescription.Name = "colDescription";
            //
            // colQty
            //
            colQty.FillWeight = 45F;
            colQty.HeaderText = "AANTAL";
            colQty.Name = "colQty";
            //
            // colPrice
            //
            colPrice.FillWeight = 65F;
            colPrice.HeaderText = "PRIJS";
            colPrice.Name = "colPrice";
            //
            // colTotal
            //
            colTotal.FillWeight = 65F;
            colTotal.HeaderText = "TOTAAL";
            colTotal.Name = "colTotal";
            //
            // totalCard
            //
            totalCard.Controls.Add(lblItemCount);
            totalCard.Controls.Add(lblSubtotal);
            totalCard.Controls.Add(lblVat);
            totalCard.Controls.Add(lblTotalAmount);
            totalCard.Dock = DockStyle.Fill;
            totalCard.Margin = new Padding(0);
            totalCard.Name = "totalCard";
            totalCard.Padding = new Padding(20);
            totalCard.TabIndex = 3;
            //
            // lblItemCount
            //
            lblItemCount.AutoSize = true;
            lblItemCount.Location = new Point(20, 18);
            lblItemCount.Name = "lblItemCount";
            //
            // lblSubtotal
            //
            lblSubtotal.AutoSize = true;
            lblSubtotal.Location = new Point(20, 46);
            lblSubtotal.Name = "lblSubtotal";
            //
            // lblVat
            //
            lblVat.AutoSize = true;
            lblVat.Location = new Point(20, 74);
            lblVat.Name = "lblVat";
            //
            // lblTotalAmount
            //
            lblTotalAmount.Dock = DockStyle.Right;
            lblTotalAmount.Name = "lblTotalAmount";
            lblTotalAmount.Size = new Size(300, 84);
            lblTotalAmount.TextAlign = ContentAlignment.MiddleRight;
            //
            // rightPanel
            //
            rightPanel.Controls.Add(buttonGrid);
            rightPanel.Dock = DockStyle.Fill;
            rightPanel.Margin = new Padding(0);
            rightPanel.Name = "rightPanel";
            rightPanel.TabIndex = 1;
            //
            // buttonGrid
            //
            buttonGrid.ColumnCount = 2;
            buttonGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 50F));
            buttonGrid.Controls.Add(btnSearch, 0, 0);
            buttonGrid.Controls.Add(btnQuantity, 1, 0);
            buttonGrid.Controls.Add(btnDeleteLine, 0, 1);
            buttonGrid.Controls.Add(btnCancel, 1, 1);
            buttonGrid.Controls.Add(btnPark, 0, 2);
            buttonGrid.Controls.Add(btnRecall, 1, 2);
            buttonGrid.Controls.Add(btnCheckout, 0, 3);
            buttonGrid.SetColumnSpan(btnCheckout, 2);
            buttonGrid.Dock = DockStyle.Fill;
            buttonGrid.Name = "buttonGrid";
            buttonGrid.RowCount = 4;
            buttonGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            buttonGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            buttonGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 20F));
            buttonGrid.RowStyles.Add(new RowStyle(SizeType.Percent, 40F));
            buttonGrid.TabIndex = 0;
            //
            // btnSearch
            //
            btnSearch.Dock = DockStyle.Fill;
            btnSearch.Margin = new Padding(0, 0, 6, 12);
            btnSearch.Name = "btnSearch";
            btnSearch.TabIndex = 0;
            btnSearch.Text = "Zoeken\r\n[F1]";
            //
            // btnQuantity
            //
            btnQuantity.Dock = DockStyle.Fill;
            btnQuantity.Margin = new Padding(6, 0, 0, 12);
            btnQuantity.Name = "btnQuantity";
            btnQuantity.TabIndex = 1;
            btnQuantity.Text = "Aantal\r\n[F2]";
            //
            // btnDeleteLine
            //
            btnDeleteLine.Dock = DockStyle.Fill;
            btnDeleteLine.Margin = new Padding(0, 0, 6, 12);
            btnDeleteLine.Name = "btnDeleteLine";
            btnDeleteLine.TabIndex = 2;
            btnDeleteLine.Text = "Wis regel\r\n[F6]";
            //
            // btnCancel
            //
            btnCancel.Dock = DockStyle.Fill;
            btnCancel.Margin = new Padding(6, 0, 0, 12);
            btnCancel.Name = "btnCancel";
            btnCancel.TabIndex = 3;
            btnCancel.Text = "Annuleren\r\n[F5]";
            //
            // btnPark
            //
            btnPark.Dock = DockStyle.Fill;
            btnPark.Margin = new Padding(0, 0, 6, 12);
            btnPark.Name = "btnPark";
            btnPark.TabIndex = 4;
            btnPark.Text = "Parkeren\r\n[F7]";
            //
            // btnRecall
            //
            btnRecall.Dock = DockStyle.Fill;
            btnRecall.Margin = new Padding(6, 0, 0, 12);
            btnRecall.Name = "btnRecall";
            btnRecall.TabIndex = 5;
            btnRecall.Text = "Heropenen\r\n[F8]";
            //
            // btnCheckout
            //
            btnCheckout.Dock = DockStyle.Fill;
            btnCheckout.Margin = new Padding(0);
            btnCheckout.Name = "btnCheckout";
            btnCheckout.TabIndex = 6;
            btnCheckout.Text = "AFREKENEN  [F10]";
            //
            // ScanScreen
            //
            Controls.Add(mainLayout);
            Name = "ScanScreen";
            Size = new Size(1280, 740);
            mainLayout.ResumeLayout(false);
            leftPanel.ResumeLayout(false);
            leftPanel.PerformLayout();
            scanBoxPanel.ResumeLayout(false);
            scanBoxPanel.PerformLayout();
            gridCard.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridItems).EndInit();
            totalCard.ResumeLayout(false);
            totalCard.PerformLayout();
            rightPanel.ResumeLayout(false);
            buttonGrid.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel mainLayout;
        private TableLayoutPanel leftPanel;
        private TableLayoutPanel scanBoxPanel;
        private Label lblScan;
        private TextBox txtBarcode;
        private Button btnAdd;
        private Label lblMessage;
        private CardPanel gridCard;
        private DataGridView gridItems;
        private DataGridViewTextBoxColumn colBarcode;
        private DataGridViewTextBoxColumn colDescription;
        private DataGridViewTextBoxColumn colQty;
        private DataGridViewTextBoxColumn colPrice;
        private DataGridViewTextBoxColumn colTotal;
        private CardPanel totalCard;
        private Label lblItemCount;
        private Label lblSubtotal;
        private Label lblVat;
        private Label lblTotalAmount;
        private Panel rightPanel;
        private TableLayoutPanel buttonGrid;
        private Button btnSearch;
        private Button btnQuantity;
        private Button btnDeleteLine;
        private Button btnCancel;
        private Button btnPark;
        private Button btnRecall;
        private Button btnCheckout;
    }
}