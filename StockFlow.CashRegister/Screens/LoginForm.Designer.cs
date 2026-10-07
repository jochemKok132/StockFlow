using System.Drawing;
using System.Windows.Forms;

namespace StockFlow.CashRegister.Screens
{
    partial class LoginForm
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
            card = new CardPanel();
            lblBrand = new Label();
            lblSubtitle = new Label();
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblPassword = new Label();
            txtPassword = new TextBox();
            lblError = new Label();
            btnLogin = new Button();
            card.SuspendLayout();
            SuspendLayout();
            //
            // card
            //
            card.Controls.Add(lblBrand);
            card.Controls.Add(lblSubtitle);
            card.Controls.Add(lblUsername);
            card.Controls.Add(txtUsername);
            card.Controls.Add(lblPassword);
            card.Controls.Add(txtPassword);
            card.Controls.Add(lblError);
            card.Controls.Add(btnLogin);
            card.Location = new Point(30, 30);
            card.Name = "card";
            card.Size = new Size(340, 348);
            card.TabIndex = 0;
            //
            // lblBrand
            //
            lblBrand.AutoSize = true;
            lblBrand.Location = new Point(24, 24);
            lblBrand.Name = "lblBrand";
            lblBrand.Text = "StockFlow Kassa";
            //
            // lblSubtitle
            //
            lblSubtitle.AutoSize = true;
            lblSubtitle.Location = new Point(24, 58);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Text = "Log in om de kassa te openen";
            //
            // lblUsername
            //
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(24, 102);
            lblUsername.Name = "lblUsername";
            lblUsername.Text = "GEBRUIKERSNAAM";
            //
            // txtUsername
            //
            txtUsername.Location = new Point(24, 124);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(292, 27);
            txtUsername.TabIndex = 1;
            //
            // lblPassword
            //
            lblPassword.AutoSize = true;
            lblPassword.Location = new Point(24, 166);
            lblPassword.Name = "lblPassword";
            lblPassword.Text = "WACHTWOORD";
            //
            // txtPassword
            //
            txtPassword.Location = new Point(24, 188);
            txtPassword.Name = "txtPassword";
            txtPassword.Size = new Size(292, 27);
            txtPassword.TabIndex = 2;
            txtPassword.UseSystemPasswordChar = true;
            //
            // lblError
            //
            lblError.Location = new Point(24, 232);
            lblError.Name = "lblError";
            lblError.Size = new Size(292, 36);
            lblError.Visible = false;
            //
            // btnLogin
            //
            btnLogin.Location = new Point(24, 280);
            btnLogin.Name = "btnLogin";
            btnLogin.Size = new Size(292, 44);
            btnLogin.TabIndex = 3;
            btnLogin.Text = "Inloggen";
            btnLogin.Click += btnLogin_Click;
            //
            // LoginForm
            //
            AcceptButton = btnLogin;
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(400, 408);
            Controls.Add(card);
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
            Name = "LoginForm";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "StockFlow Kassa - Inloggen";
            card.ResumeLayout(false);
            card.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private CardPanel card;
        private Label lblBrand;
        private Label lblSubtitle;
        private Label lblUsername;
        private Label lblPassword;
        private TextBox txtUsername;
        private TextBox txtPassword;
        private Button btnLogin;
        private Label lblError;
    }
}