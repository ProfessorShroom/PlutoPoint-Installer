using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Security.Policy;

namespace PlutoPoint_Installer
{
    partial class installerForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.install = new System.Windows.Forms.Button();
            this.installerTextBox = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.libreOfficeCheck = new System.Windows.Forms.CheckBox();
            this.mozillaFirefoxCheck = new System.Windows.Forms.CheckBox();
            this.googleChromeCheck = new System.Windows.Forms.CheckBox();
            this.close = new System.Windows.Forms.Button();
            this.restart = new System.Windows.Forms.Button();
            this.bingWallpapersCheck = new System.Windows.Forms.CheckBox();
            this.versionLabel = new System.Windows.Forms.LinkLabel();
            this.restartCheck = new System.Windows.Forms.CheckBox();
            this.crcCheck = new System.Windows.Forms.CheckBox();
            this.powerCheck = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // install
            // 
            this.install.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.install.FlatAppearance.BorderSize = 0;
            this.install.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.install.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.install.ForeColor = System.Drawing.Color.White;
            this.install.Location = new System.Drawing.Point(12, 14);
            this.install.Name = "install";
            this.install.Size = new System.Drawing.Size(105, 48);
            this.install.TabIndex = 0;
            this.install.Text = "Install";
            this.install.UseVisualStyleBackColor = false;
            this.install.Click += new System.EventHandler(this.install_Click);
            // 
            // installerTextBox
            // 
            this.installerTextBox.AcceptsReturn = true;
            this.installerTextBox.BackColor = System.Drawing.SystemColors.ControlDarkDark;
            this.installerTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.installerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.installerTextBox.ForeColor = System.Drawing.SystemColors.Info;
            this.installerTextBox.Location = new System.Drawing.Point(344, 54);
            this.installerTextBox.Multiline = true;
            this.installerTextBox.Name = "installerTextBox";
            this.installerTextBox.ReadOnly = true;
            this.installerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.installerTextBox.Size = new System.Drawing.Size(444, 360);
            this.installerTextBox.TabIndex = 1;
            // 
            // progressBar
            // 
            this.progressBar.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(156)))), ((int)(((byte)(224)))), ((int)(((byte)(250)))));
            this.progressBar.Location = new System.Drawing.Point(267, 14);
            this.progressBar.Maximum = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(521, 24);
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 3;
            // 
            // libreOfficeCheck
            // 
            this.libreOfficeCheck.Checked = true;
            this.libreOfficeCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.libreOfficeCheck.Image = global::PlutoPoint_Installer.Properties.Resources.libreOffice;
            this.libreOfficeCheck.Location = new System.Drawing.Point(12, 205);
            this.libreOfficeCheck.Name = "libreOfficeCheck";
            this.libreOfficeCheck.Size = new System.Drawing.Size(57, 50);
            this.libreOfficeCheck.TabIndex = 4;
            this.libreOfficeCheck.UseVisualStyleBackColor = true;
            // 
            // mozillaFirefoxCheck
            // 
            this.mozillaFirefoxCheck.Checked = true;
            this.mozillaFirefoxCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mozillaFirefoxCheck.Image = global::PlutoPoint_Installer.Properties.Resources.mozillaFirefox;
            this.mozillaFirefoxCheck.Location = new System.Drawing.Point(12, 250);
            this.mozillaFirefoxCheck.Name = "mozillaFirefoxCheck";
            this.mozillaFirefoxCheck.Size = new System.Drawing.Size(57, 50);
            this.mozillaFirefoxCheck.TabIndex = 5;
            this.mozillaFirefoxCheck.UseVisualStyleBackColor = true;
            // 
            // googleChromeCheck
            // 
            this.googleChromeCheck.Checked = true;
            this.googleChromeCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.googleChromeCheck.Image = global::PlutoPoint_Installer.Properties.Resources.googleChrome;
            this.googleChromeCheck.Location = new System.Drawing.Point(12, 160);
            this.googleChromeCheck.Name = "googleChromeCheck";
            this.googleChromeCheck.Size = new System.Drawing.Size(57, 50);
            this.googleChromeCheck.TabIndex = 5;
            this.googleChromeCheck.UseVisualStyleBackColor = true;
            // 
            // close
            // 
            this.close.BackColor = System.Drawing.SystemColors.MenuHighlight;
            this.close.FlatAppearance.BorderSize = 0;
            this.close.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.close.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.close.ForeColor = System.Drawing.Color.White;
            this.close.Location = new System.Drawing.Point(195, 14);
            this.close.Name = "close";
            this.close.Size = new System.Drawing.Size(66, 24);
            this.close.TabIndex = 6;
            this.close.Text = "Close";
            this.close.UseVisualStyleBackColor = false;
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // restart
            // 
            this.restart.BackColor = System.Drawing.Color.OrangeRed;
            this.restart.FlatAppearance.BorderSize = 0;
            this.restart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.restart.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.restart.ForeColor = System.Drawing.Color.White;
            this.restart.Location = new System.Drawing.Point(123, 14);
            this.restart.Name = "restart";
            this.restart.Size = new System.Drawing.Size(66, 24);
            this.restart.TabIndex = 7;
            this.restart.Text = "Restart";
            this.restart.UseVisualStyleBackColor = false;
            this.restart.Click += new System.EventHandler(this.restart_Click);
            // 
            // bingWallpapersCheck
            // 
            this.bingWallpapersCheck.Checked = true;
            this.bingWallpapersCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bingWallpapersCheck.Image = global::PlutoPoint_Installer.Properties.Resources.bingWallpaper;
            this.bingWallpapersCheck.Location = new System.Drawing.Point(12, 115);
            this.bingWallpapersCheck.Name = "bingWallpapersCheck";
            this.bingWallpapersCheck.Size = new System.Drawing.Size(57, 50);
            this.bingWallpapersCheck.TabIndex = 8;
            this.bingWallpapersCheck.UseVisualStyleBackColor = true;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.LinkColor = System.Drawing.Color.White;
            this.versionLabel.Location = new System.Drawing.Point(12, 425);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(84, 13);
            this.versionLabel.TabIndex = 9;
            this.versionLabel.TabStop = true;
            this.versionLabel.Text = "Version 6.0.3.1b";
            this.versionLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.versionLabel_LinkClicked);
            // 
            // restartCheck
            // 
            this.restartCheck.Image = global::PlutoPoint_Installer.Properties.Resources.restart;
            this.restartCheck.Location = new System.Drawing.Point(751, 417);
            this.restartCheck.Name = "restartCheck";
            this.restartCheck.Size = new System.Drawing.Size(46, 32);
            this.restartCheck.TabIndex = 10;
            this.restartCheck.UseVisualStyleBackColor = true;
            // 
            // crcCheck
            // 
            this.crcCheck.Checked = true;
            this.crcCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.crcCheck.Image = global::PlutoPoint_Installer.Properties.Resources.computerRepairCentre;
            this.crcCheck.Location = new System.Drawing.Point(12, 70);
            this.crcCheck.Name = "crcCheck";
            this.crcCheck.Size = new System.Drawing.Size(57, 50);
            this.crcCheck.TabIndex = 11;
            this.crcCheck.UseVisualStyleBackColor = true;
            // 
            // powerCheck
            // 
            this.powerCheck.Image = global::PlutoPoint_Installer.Properties.Resources.power;
            this.powerCheck.Location = new System.Drawing.Point(699, 417);
            this.powerCheck.Name = "powerCheck";
            this.powerCheck.Size = new System.Drawing.Size(46, 32);
            this.powerCheck.TabIndex = 12;
            this.powerCheck.UseVisualStyleBackColor = true;
            // 
            // installerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.powerCheck);
            this.Controls.Add(this.crcCheck);
            this.Controls.Add(this.restartCheck);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.bingWallpapersCheck);
            this.Controls.Add(this.restart);
            this.Controls.Add(this.close);
            this.Controls.Add(this.libreOfficeCheck);
            this.Controls.Add(this.mozillaFirefoxCheck);
            this.Controls.Add(this.googleChromeCheck);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.installerTextBox);
            this.Controls.Add(this.install);
            this.Icon = global::PlutoPoint_Installer.Properties.Resources.computerRepairCentreIcon;
            this.Name = "installerForm";
            this.Text = "Computer Repair Centre Installer";
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Button install;
        private System.Windows.Forms.TextBox installerTextBox;
        private System.Windows.Forms.ProgressBar progressBar;
        private System.Windows.Forms.CheckBox libreOfficeCheck;
        private System.Windows.Forms.CheckBox mozillaFirefoxCheck;
        private System.Windows.Forms.CheckBox googleChromeCheck;
        private System.Windows.Forms.Button close;
        private System.Windows.Forms.Button restart;
        private System.Windows.Forms.CheckBox bingWallpapersCheck;
        private System.Windows.Forms.LinkLabel versionLabel;
        private System.Windows.Forms.CheckBox restartCheck;
        private System.Windows.Forms.CheckBox crcCheck;
        private System.Windows.Forms.CheckBox powerCheck;
    }
}

