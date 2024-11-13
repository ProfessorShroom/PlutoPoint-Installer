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
            this.anyDeskCheck = new System.Windows.Forms.CheckBox();
            this.mozillaThunderbirdCheck = new System.Windows.Forms.CheckBox();
            this.bitDefenderCheck = new System.Windows.Forms.CheckBox();
            this.discordCheck = new System.Windows.Forms.CheckBox();
            this.steamCheck = new System.Windows.Forms.CheckBox();
            this.nanaZipCheck = new System.Windows.Forms.CheckBox();
            this.microsoftOffice2007Check = new System.Windows.Forms.CheckBox();
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
            this.installerTextBox.Font = new System.Drawing.Font("Microsoft Sans Serif", 11F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.installerTextBox.ForeColor = System.Drawing.SystemColors.Info;
            this.installerTextBox.Location = new System.Drawing.Point(267, 54);
            this.installerTextBox.Multiline = true;
            this.installerTextBox.Name = "installerTextBox";
            this.installerTextBox.ReadOnly = true;
            this.installerTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.installerTextBox.Size = new System.Drawing.Size(521, 360);
            this.installerTextBox.TabIndex = 1;
            // 
            // progressBar
            // 
            this.progressBar.Location = new System.Drawing.Point(267, 14);
            this.progressBar.Maximum = 0;
            this.progressBar.Name = "progressBar";
            this.progressBar.Size = new System.Drawing.Size(521, 24);
            this.progressBar.Step = 1;
            this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Continuous;
            this.progressBar.TabIndex = 3;
            // 
            // libreOfficeCheck
            // 
            this.libreOfficeCheck.Checked = true;
            this.libreOfficeCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.libreOfficeCheck.Image = global::PlutoPoint_Installer.Properties.Resources.libreOffice;
            this.libreOfficeCheck.Location = new System.Drawing.Point(12, 340);
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
            this.mozillaFirefoxCheck.Location = new System.Drawing.Point(75, 115);
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
            this.googleChromeCheck.Location = new System.Drawing.Point(12, 295);
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
            this.bingWallpapersCheck.Location = new System.Drawing.Point(12, 160);
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
            this.versionLabel.Size = new System.Drawing.Size(78, 13);
            this.versionLabel.TabIndex = 9;
            this.versionLabel.TabStop = true;
            this.versionLabel.Text = "Version 6.1.2.4";
            this.versionLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.versionLabel_LinkClicked);
            // 
            // restartCheck
            // 
            this.restartCheck.Checked = true;
            this.restartCheck.CheckState = System.Windows.Forms.CheckState.Checked;
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
            // anyDeskCheck
            // 
            this.anyDeskCheck.Checked = true;
            this.anyDeskCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.anyDeskCheck.Image = global::PlutoPoint_Installer.Properties.Resources.anyDesk;
            this.anyDeskCheck.Location = new System.Drawing.Point(12, 115);
            this.anyDeskCheck.Name = "anyDeskCheck";
            this.anyDeskCheck.Size = new System.Drawing.Size(57, 50);
            this.anyDeskCheck.TabIndex = 13;
            this.anyDeskCheck.UseVisualStyleBackColor = true;
            // 
            // mozillaThunderbirdCheck
            // 
            this.mozillaThunderbirdCheck.Image = global::PlutoPoint_Installer.Properties.Resources.mozillaThunderbird;
            this.mozillaThunderbirdCheck.Location = new System.Drawing.Point(75, 160);
            this.mozillaThunderbirdCheck.Name = "mozillaThunderbirdCheck";
            this.mozillaThunderbirdCheck.Size = new System.Drawing.Size(57, 50);
            this.mozillaThunderbirdCheck.TabIndex = 14;
            this.mozillaThunderbirdCheck.UseVisualStyleBackColor = true;
            // 
            // bitDefenderCheck
            // 
            this.bitDefenderCheck.Image = global::PlutoPoint_Installer.Properties.Resources.bitDefender;
            this.bitDefenderCheck.Location = new System.Drawing.Point(12, 205);
            this.bitDefenderCheck.Name = "bitDefenderCheck";
            this.bitDefenderCheck.Size = new System.Drawing.Size(57, 50);
            this.bitDefenderCheck.TabIndex = 15;
            this.bitDefenderCheck.UseVisualStyleBackColor = true;
            // 
            // discordCheck
            // 
            this.discordCheck.Image = global::PlutoPoint_Installer.Properties.Resources.discord;
            this.discordCheck.Location = new System.Drawing.Point(12, 250);
            this.discordCheck.Name = "discordCheck";
            this.discordCheck.Size = new System.Drawing.Size(57, 50);
            this.discordCheck.TabIndex = 16;
            this.discordCheck.UseVisualStyleBackColor = true;
            // 
            // steamCheck
            // 
            this.steamCheck.Image = global::PlutoPoint_Installer.Properties.Resources.steam;
            this.steamCheck.Location = new System.Drawing.Point(75, 250);
            this.steamCheck.Name = "steamCheck";
            this.steamCheck.Size = new System.Drawing.Size(57, 50);
            this.steamCheck.TabIndex = 17;
            this.steamCheck.UseVisualStyleBackColor = true;
            // 
            // nanaZipCheck
            // 
            this.nanaZipCheck.Checked = true;
            this.nanaZipCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.nanaZipCheck.Image = global::PlutoPoint_Installer.Properties.Resources.nanaZip;
            this.nanaZipCheck.Location = new System.Drawing.Point(75, 205);
            this.nanaZipCheck.Name = "nanaZipCheck";
            this.nanaZipCheck.Size = new System.Drawing.Size(57, 50);
            this.nanaZipCheck.TabIndex = 18;
            this.nanaZipCheck.UseVisualStyleBackColor = true;
            // 
            // microsoftOffice2007Check
            // 
            this.microsoftOffice2007Check.Image = global::PlutoPoint_Installer.Properties.Resources.microsoftOffice2007;
            this.microsoftOffice2007Check.Location = new System.Drawing.Point(75, 70);
            this.microsoftOffice2007Check.Name = "microsoftOffice2007Check";
            this.microsoftOffice2007Check.Size = new System.Drawing.Size(57, 50);
            this.microsoftOffice2007Check.TabIndex = 19;
            this.microsoftOffice2007Check.UseVisualStyleBackColor = true;
            // 
            // installerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.microsoftOffice2007Check);
            this.Controls.Add(this.nanaZipCheck);
            this.Controls.Add(this.steamCheck);
            this.Controls.Add(this.discordCheck);
            this.Controls.Add(this.bitDefenderCheck);
            this.Controls.Add(this.mozillaThunderbirdCheck);
            this.Controls.Add(this.anyDeskCheck);
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
        private System.Windows.Forms.CheckBox anyDeskCheck;
        private System.Windows.Forms.CheckBox mozillaThunderbirdCheck;
        private System.Windows.Forms.CheckBox bitDefenderCheck;
        private System.Windows.Forms.CheckBox discordCheck;
        private System.Windows.Forms.CheckBox steamCheck;
        private System.Windows.Forms.CheckBox nanaZipCheck;
        private System.Windows.Forms.CheckBox microsoftOffice2007Check;
    }
}

