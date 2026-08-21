using System.Drawing;
using System.Windows.Forms;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer.UI
{
    internal class PasswordForm : Form
    {
        public string EnteredPassword { get; private set; }
        private TextBox txtPassword;
        private Button btnOK;
        private Label passwordText;

        public PasswordForm()
        {
            this.Text = "Password Required.";
            this.Width = 300;
            this.Height = 160;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Icon = global::PlutoPoint_Installer.Properties.Resources.computerRepairCentreIcon;
            passwordText = new Label()
            {
                Text = "The installer is not being run from a known location, please enter password to continue.",
                Left = 10,
                Top = 10,
                Width = 260,
                Height = 40,
                ForeColor = Color.Red,
                TextAlign = ContentAlignment.MiddleLeft
            };
            Label lbl = new Label() { Text = "Password:", Left = 10, Top = 55, Width = 70 };
            txtPassword = new TextBox() { Left = 85, Top = 52, Width = 180, PasswordChar = '*' };
            btnOK = new Button() { Text = "OK", Left = 185, Width = 80, Top = 85, DialogResult = DialogResult.OK };
            btnOK.Click += (s, e) => { EnteredPassword = txtPassword.Text; };
            this.Controls.Add(passwordText);
            this.Controls.Add(lbl);
            this.Controls.Add(txtPassword);
            this.Controls.Add(btnOK);
            this.AcceptButton = btnOK;
        }
    }
}
