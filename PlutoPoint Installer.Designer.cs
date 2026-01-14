using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Reflection;
using System.Runtime.Remoting.Activation;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Windows.Forms;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer
{
    public class RoundedGroupBox : GroupBox
    {
        public int CornerRadius { get; set; } = 3;
        public Color BorderColor { get; set; } = Color.Silver;
        public Color BackgroundColor { get; set; } = Color.FromArgb(35, 35, 35);

        // Optional overrides
        public Color? BorderColorOverride { get; set; } = null;
        public Color? TextColorOverride { get; set; } = null;
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

            int topPadding = 7;      // space from top border to start of rectangle
            int bottomPadding = 7;   // space from bottom border

            Rectangle rect = new Rectangle(
                0,
                topPadding,
                ClientRectangle.Width - 1,
                ClientRectangle.Height - topPadding - bottomPadding
            );

            using (GraphicsPath path = new GraphicsPath())
            {
                int dia = CornerRadius * 2;

                path.AddArc(rect.X, rect.Y, dia, dia, 180, 90);
                path.AddArc(rect.Right - dia, rect.Y, dia, dia, 270, 90);
                path.AddArc(rect.Right - dia, rect.Bottom - dia, dia, dia, 0, 90);
                path.AddArc(rect.X, rect.Bottom - dia, dia, dia, 90, 90);
                path.CloseFigure();

                using (SolidBrush b = new SolidBrush(Parent?.BackColor ?? BackgroundColor))
                    e.Graphics.FillPath(b, path);

                using (Pen pen = new Pen(BorderColorOverride ?? BorderColor, 2f))
                    e.Graphics.DrawPath(pen, path);
            }

            // Measure text size once
            SizeF textSize = e.Graphics.MeasureString(this.Text, this.Font);

            // Top text
            RectangleF topRect = new RectangleF(
                (ClientRectangle.Width - textSize.Width) / 2f,
                -textSize.Height / 15f,
                textSize.Width,
                textSize.Height
            );

            using (SolidBrush bTop = new SolidBrush(Parent?.BackColor ?? BackgroundColor))
                e.Graphics.FillRectangle(bTop, topRect);

            using (SolidBrush textBrushTop = new SolidBrush(TextColorOverride ?? ForeColor))
                e.Graphics.DrawString(this.Text, this.Font, textBrushTop, topRect.Location);

            // Bottom text
            RectangleF bottomRect = new RectangleF(
                (ClientRectangle.Width - textSize.Width) / 2f,
                ClientRectangle.Height - textSize.Height,
                textSize.Width,
                textSize.Height
            );

            using (SolidBrush bBottom = new SolidBrush(Parent?.BackColor ?? BackgroundColor))
                e.Graphics.FillRectangle(bBottom, bottomRect);

            using (SolidBrush textBrushBottom = new SolidBrush(TextColorOverride ?? ForeColor))
                e.Graphics.DrawString(this.Text, this.Font, textBrushBottom, bottomRect.Location);
        }
    }
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
            this.components = new System.ComponentModel.Container();
            this.install = new System.Windows.Forms.Button();
            this.installerTextBox = new System.Windows.Forms.TextBox();
            this.progressBar = new System.Windows.Forms.ProgressBar();
            this.close = new System.Windows.Forms.Button();
            this.restart = new System.Windows.Forms.Button();
            this.versionLabel = new System.Windows.Forms.LinkLabel();
            this.locationLabel = new System.Windows.Forms.Label();
            this.restartCheck = new System.Windows.Forms.CheckBox();
            this.powerCheck = new System.Windows.Forms.CheckBox();
            this.recycleBinCheck = new System.Windows.Forms.CheckBox();
            this.crcToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.crcCheck = new System.Windows.Forms.CheckBox();
            this.amdToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.anyDeskToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.anyDeskCheck = new System.Windows.Forms.CheckBox();
            this.bitDefenderToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.bitDefenderCheck = new System.Windows.Forms.CheckBox();
            this.discordToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.discordCheck = new System.Windows.Forms.CheckBox();
            this.googleChromeToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.googleChromeCheck = new System.Windows.Forms.CheckBox();
            this.libreOfficeToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.libreOfficeCheck = new System.Windows.Forms.CheckBox();
            this.microsoftOffice2007ToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.microsoftOffice2007Check = new System.Windows.Forms.CheckBox();
            this.nvidiaAppToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.nvidiaAppCheck = new System.Windows.Forms.CheckBox();
            this.aiCheck = new System.Windows.Forms.CheckBox();
            this.mozillaFirefoxToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mozillaFirefoxCheck = new System.Windows.Forms.CheckBox();
            this.mozillaThunderbirdToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.mozillaThunderbirdCheck = new System.Windows.Forms.CheckBox();
            this.nanaZipToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.nanaZipCheck = new System.Windows.Forms.CheckBox();
            this.steamToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.steamCheck = new System.Windows.Forms.CheckBox();
            this.vlcMediaPlayerToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.vlcMediaPlayerCheck = new System.Windows.Forms.CheckBox();
            this.bingWallpaperstoolTip = new System.Windows.Forms.ToolTip(this.components);
            this.bingWallpapersCheck = new System.Windows.Forms.CheckBox();
            this.recycleBinToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.powerToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.restartToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.shutdownCheck = new System.Windows.Forms.CheckBox();
            this.aiToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.roundedGroupBox2 = new PlutoPoint_Installer.RoundedGroupBox();
            this.roundedGroupBox1 = new PlutoPoint_Installer.RoundedGroupBox();
            this.shutdownToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.roundedGroupBox2.SuspendLayout();
            this.roundedGroupBox1.SuspendLayout();
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
            this.installerTextBox.Size = new System.Drawing.Size(521, 355);
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
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.Font = new System.Drawing.Font("Ubuntu", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.versionLabel.LinkColor = System.Drawing.Color.White;
            this.versionLabel.Location = new System.Drawing.Point(12, 425);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(0, 13);
            this.versionLabel.TabIndex = 9;
            this.versionLabel.TabStop = true;
            this.versionLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.versionLabel_LinkClicked);
            // 
            // locationLabel
            // 
            this.locationLabel.AutoSize = true;
            this.locationLabel.Font = new System.Drawing.Font("Ubuntu", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.locationLabel.ForeColor = System.Drawing.Color.White;
            this.locationLabel.Location = new System.Drawing.Point(120, 425);
            this.locationLabel.Name = "locationLabel";
            this.locationLabel.Size = new System.Drawing.Size(0, 13);
            this.locationLabel.TabIndex = 9;
            this.locationLabel.TabStop = true;
            // 
            // restartCheck
            // 
            this.restartCheck.Image = global::PlutoPoint_Installer.Properties.Resources.restart;
            this.restartCheck.Location = new System.Drawing.Point(687, 415);
            this.restartCheck.Name = "restartCheck";
            this.restartCheck.Size = new System.Drawing.Size(48, 34);
            this.restartCheck.TabIndex = 10;
            this.restartToolTip.SetToolTip(this.restartCheck, "Restart Computer on Install Completion");
            this.restartCheck.UseVisualStyleBackColor = true;
            // 
            // powerCheck
            // 
            this.powerCheck.Image = global::PlutoPoint_Installer.Properties.Resources.power;
            this.powerCheck.Location = new System.Drawing.Point(633, 415);
            this.powerCheck.Name = "powerCheck";
            this.powerCheck.Size = new System.Drawing.Size(48, 34);
            this.powerCheck.TabIndex = 12;
            this.powerToolTip.SetToolTip(this.powerCheck, "Disable Sleep on AC Power");
            this.powerCheck.UseVisualStyleBackColor = true;
            // 
            // recycleBinCheck
            // 
            this.recycleBinCheck.Image = global::PlutoPoint_Installer.Properties.Resources.recycleBin;
            this.recycleBinCheck.Location = new System.Drawing.Point(579, 415);
            this.recycleBinCheck.Name = "recycleBinCheck";
            this.recycleBinCheck.Size = new System.Drawing.Size(48, 34);
            this.recycleBinCheck.TabIndex = 22;
            this.recycleBinToolTip.SetToolTip(this.recycleBinCheck, "Empty Recycle Bin");
            this.recycleBinCheck.UseVisualStyleBackColor = true;
            // 
            // crcCheck
            // 
            this.crcCheck.Checked = true;
            this.crcCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.crcCheck.Image = global::PlutoPoint_Installer.Properties.Resources.computerRepairCentre;
            this.crcCheck.Location = new System.Drawing.Point(10, 13);
            this.crcCheck.Name = "crcCheck";
            this.crcCheck.Size = new System.Drawing.Size(57, 50);
            this.crcCheck.TabIndex = 11;
            this.crcToolTip.SetToolTip(this.crcCheck, "Computer Repair Centre OEM Information");
            this.crcCheck.UseVisualStyleBackColor = true;
            // 
            // anyDeskCheck
            // 
            this.anyDeskCheck.BackColor = System.Drawing.Color.Transparent;
            this.anyDeskCheck.Checked = true;
            this.anyDeskCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.anyDeskCheck.Image = global::PlutoPoint_Installer.Properties.Resources.anyDesk;
            this.anyDeskCheck.Location = new System.Drawing.Point(14, 13);
            this.anyDeskCheck.Name = "anyDeskCheck";
            this.anyDeskCheck.Size = new System.Drawing.Size(57, 50);
            this.anyDeskCheck.TabIndex = 13;
            this.anyDeskToolTip.SetToolTip(this.anyDeskCheck, "AnyDesk Remote Support Software");
            this.anyDeskCheck.UseVisualStyleBackColor = false;
            // 
            // bitDefenderCheck
            // 
            this.bitDefenderCheck.Image = global::PlutoPoint_Installer.Properties.Resources.bitDefender;
            this.bitDefenderCheck.Location = new System.Drawing.Point(14, 58);
            this.bitDefenderCheck.Name = "bitDefenderCheck";
            this.bitDefenderCheck.Size = new System.Drawing.Size(57, 50);
            this.bitDefenderCheck.TabIndex = 15;
            this.bitDefenderToolTip.SetToolTip(this.bitDefenderCheck, "BitDefender Anti-Virus");
            this.bitDefenderCheck.UseVisualStyleBackColor = true;
            // 
            // discordCheck
            // 
            this.discordCheck.Image = global::PlutoPoint_Installer.Properties.Resources.discord;
            this.discordCheck.Location = new System.Drawing.Point(14, 101);
            this.discordCheck.Name = "discordCheck";
            this.discordCheck.Size = new System.Drawing.Size(57, 50);
            this.discordCheck.TabIndex = 16;
            this.discordToolTip.SetToolTip(this.discordCheck, "Discord");
            this.discordCheck.UseVisualStyleBackColor = true;
            // 
            // googleChromeCheck
            // 
            this.googleChromeCheck.Checked = true;
            this.googleChromeCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.googleChromeCheck.Image = global::PlutoPoint_Installer.Properties.Resources.googleChrome;
            this.googleChromeCheck.Location = new System.Drawing.Point(14, 147);
            this.googleChromeCheck.Name = "googleChromeCheck";
            this.googleChromeCheck.Size = new System.Drawing.Size(57, 50);
            this.googleChromeCheck.TabIndex = 5;
            this.googleChromeToolTip.SetToolTip(this.googleChromeCheck, "Google Chrome");
            this.googleChromeCheck.UseVisualStyleBackColor = true;
            // 
            // libreOfficeCheck
            // 
            this.libreOfficeCheck.Checked = true;
            this.libreOfficeCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.libreOfficeCheck.Image = global::PlutoPoint_Installer.Properties.Resources.libreOffice;
            this.libreOfficeCheck.Location = new System.Drawing.Point(14, 192);
            this.libreOfficeCheck.Name = "libreOfficeCheck";
            this.libreOfficeCheck.Size = new System.Drawing.Size(57, 50);
            this.libreOfficeCheck.TabIndex = 4;
            this.libreOfficeToolTip.SetToolTip(this.libreOfficeCheck, "LibreOffice");
            this.libreOfficeCheck.UseVisualStyleBackColor = true;
            // 
            // microsoftOffice2007Check
            // 
            this.microsoftOffice2007Check.Image = global::PlutoPoint_Installer.Properties.Resources.microsoftOffice2007;
            this.microsoftOffice2007Check.Location = new System.Drawing.Point(14, 237);
            this.microsoftOffice2007Check.Name = "microsoftOffice2007Check";
            this.microsoftOffice2007Check.Size = new System.Drawing.Size(57, 50);
            this.microsoftOffice2007Check.TabIndex = 19;
            this.microsoftOffice2007ToolTip.SetToolTip(this.microsoftOffice2007Check, "Microsoft Office 2007. Move on grandad");
            this.microsoftOffice2007Check.UseVisualStyleBackColor = true;
            // 
            // nvidiaAppCheck
            // 
            this.nvidiaAppCheck.Image = global::PlutoPoint_Installer.Properties.Resources.nvidiaApp;
            this.nvidiaAppCheck.Location = new System.Drawing.Point(10, 192);
            this.nvidiaAppCheck.Name = "nvidiaAppCheck";
            this.nvidiaAppCheck.Size = new System.Drawing.Size(57, 50);
            this.nvidiaAppCheck.TabIndex = 21;
            this.nvidiaAppToolTip.SetToolTip(this.nvidiaAppCheck, "Nvidia Graphics App");
            this.nvidiaAppCheck.UseVisualStyleBackColor = true;
            // 
            // aiCheck
            // 
            this.aiCheck.Checked = true;
            this.aiCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.aiCheck.Image = global::PlutoPoint_Installer.Properties.Resources.aiRemoval;
            this.aiCheck.Location = new System.Drawing.Point(10, 58);
            this.aiCheck.Name = "aiCheck";
            this.aiCheck.Size = new System.Drawing.Size(57, 50);
            this.aiCheck.TabIndex = 22;
            this.aiToolTip.SetToolTip(this.aiCheck, "Remove Windows AI Bollocks (this can take a few minutes)");
            this.aiCheck.UseVisualStyleBackColor = true;
            // 
            // mozillaFirefoxCheck
            // 
            this.mozillaFirefoxCheck.Checked = true;
            this.mozillaFirefoxCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.mozillaFirefoxCheck.Image = global::PlutoPoint_Installer.Properties.Resources.mozillaFirefox;
            this.mozillaFirefoxCheck.Location = new System.Drawing.Point(82, 13);
            this.mozillaFirefoxCheck.Name = "mozillaFirefoxCheck";
            this.mozillaFirefoxCheck.Size = new System.Drawing.Size(57, 50);
            this.mozillaFirefoxCheck.TabIndex = 5;
            this.mozillaFirefoxToolTip.SetToolTip(this.mozillaFirefoxCheck, "Mozilla Firefox");
            this.mozillaFirefoxCheck.UseVisualStyleBackColor = true;
            // 
            // mozillaThunderbirdCheck
            // 
            this.mozillaThunderbirdCheck.Image = global::PlutoPoint_Installer.Properties.Resources.mozillaThunderbird;
            this.mozillaThunderbirdCheck.Location = new System.Drawing.Point(82, 58);
            this.mozillaThunderbirdCheck.Name = "mozillaThunderbirdCheck";
            this.mozillaThunderbirdCheck.Size = new System.Drawing.Size(57, 50);
            this.mozillaThunderbirdCheck.TabIndex = 14;
            this.mozillaThunderbirdToolTip.SetToolTip(this.mozillaThunderbirdCheck, "Mozilla Thunderbird");
            this.mozillaThunderbirdCheck.UseVisualStyleBackColor = true;
            // 
            // nanaZipCheck
            // 
            this.nanaZipCheck.Checked = true;
            this.nanaZipCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.nanaZipCheck.Image = global::PlutoPoint_Installer.Properties.Resources.nanaZip;
            this.nanaZipCheck.Location = new System.Drawing.Point(10, 147);
            this.nanaZipCheck.Name = "nanaZipCheck";
            this.nanaZipCheck.Size = new System.Drawing.Size(57, 50);
            this.nanaZipCheck.TabIndex = 18;
            this.nanaZipToolTip.SetToolTip(this.nanaZipCheck, "NanaZip Extraction Software");
            this.nanaZipCheck.UseVisualStyleBackColor = true;
            // 
            // steamCheck
            // 
            this.steamCheck.Image = global::PlutoPoint_Installer.Properties.Resources.steam;
            this.steamCheck.Location = new System.Drawing.Point(82, 101);
            this.steamCheck.Name = "steamCheck";
            this.steamCheck.Size = new System.Drawing.Size(57, 50);
            this.steamCheck.TabIndex = 17;
            this.steamToolTip.SetToolTip(this.steamCheck, "Steam");
            this.steamCheck.UseVisualStyleBackColor = true;
            // 
            // vlcMediaPlayerCheck
            // 
            this.vlcMediaPlayerCheck.Image = global::PlutoPoint_Installer.Properties.Resources.vlcMediaPlayer;
            this.vlcMediaPlayerCheck.Location = new System.Drawing.Point(82, 147);
            this.vlcMediaPlayerCheck.Name = "vlcMediaPlayerCheck";
            this.vlcMediaPlayerCheck.Size = new System.Drawing.Size(57, 50);
            this.vlcMediaPlayerCheck.TabIndex = 20;
            this.vlcMediaPlayerToolTip.SetToolTip(this.vlcMediaPlayerCheck, "VLC Media Player");
            this.vlcMediaPlayerCheck.UseVisualStyleBackColor = true;
            // 
            // bingWallpapersCheck
            // 
            this.bingWallpapersCheck.BackColor = System.Drawing.Color.Transparent;
            this.bingWallpapersCheck.Checked = true;
            this.bingWallpapersCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.bingWallpapersCheck.Image = global::PlutoPoint_Installer.Properties.Resources.bingWallpaper;
            this.bingWallpapersCheck.Location = new System.Drawing.Point(10, 104);
            this.bingWallpapersCheck.Name = "bingWallpapersCheck";
            this.bingWallpapersCheck.Size = new System.Drawing.Size(57, 50);
            this.bingWallpapersCheck.TabIndex = 8;
            this.bingWallpaperstoolTip.SetToolTip(this.bingWallpapersCheck, "Bing Wallpapers");
            this.bingWallpapersCheck.UseVisualStyleBackColor = false;
            // 
            // shutdownCheck
            // 
            this.shutdownCheck.Image = global::PlutoPoint_Installer.Properties.Resources.shutdown;
            this.shutdownCheck.Location = new System.Drawing.Point(740, 415);
            this.shutdownCheck.Name = "shutdownCheck";
            this.shutdownCheck.Size = new System.Drawing.Size(48, 34);
            this.shutdownCheck.TabIndex = 26;
            this.shutdownToolTip.SetToolTip(this.shutdownCheck, "Shutdown Computer on Install Completion");
            this.shutdownCheck.UseVisualStyleBackColor = true;
            // 
            // roundedGroupBox2
            // 
            this.roundedGroupBox2.BackColor = System.Drawing.Color.Transparent;
            this.roundedGroupBox2.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.roundedGroupBox2.BorderColor = System.Drawing.Color.DodgerBlue;
            this.roundedGroupBox2.BorderColorOverride = null;
            this.roundedGroupBox2.Controls.Add(this.anyDeskCheck);
            this.roundedGroupBox2.Controls.Add(this.bitDefenderCheck);
            this.roundedGroupBox2.Controls.Add(this.vlcMediaPlayerCheck);
            this.roundedGroupBox2.Controls.Add(this.discordCheck);
            this.roundedGroupBox2.Controls.Add(this.steamCheck);
            this.roundedGroupBox2.Controls.Add(this.microsoftOffice2007Check);
            this.roundedGroupBox2.Controls.Add(this.mozillaThunderbirdCheck);
            this.roundedGroupBox2.Controls.Add(this.googleChromeCheck);
            this.roundedGroupBox2.Controls.Add(this.libreOfficeCheck);
            this.roundedGroupBox2.Controls.Add(this.mozillaFirefoxCheck);
            this.roundedGroupBox2.CornerRadius = 4;
            this.roundedGroupBox2.ForeColor = System.Drawing.Color.White;
            this.roundedGroupBox2.Location = new System.Drawing.Point(12, 70);
            this.roundedGroupBox2.Name = "roundedGroupBox2";
            this.roundedGroupBox2.Size = new System.Drawing.Size(149, 296);
            this.roundedGroupBox2.TabIndex = 25;
            this.roundedGroupBox2.TabStop = false;
            this.roundedGroupBox2.Text = "Software";
            this.roundedGroupBox2.Font = new System.Drawing.Font("Ubuntu", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedGroupBox2.TextColorOverride = null;
            // 
            // roundedGroupBox1
            // 
            this.roundedGroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.roundedGroupBox1.BackgroundColor = System.Drawing.Color.FromArgb(((int)(((byte)(35)))), ((int)(((byte)(35)))), ((int)(((byte)(35)))));
            this.roundedGroupBox1.BorderColor = System.Drawing.Color.DodgerBlue;
            this.roundedGroupBox1.BorderColorOverride = null;
            this.roundedGroupBox1.Controls.Add(this.aiCheck);
            this.roundedGroupBox1.Controls.Add(this.crcCheck);
            this.roundedGroupBox1.Controls.Add(this.nvidiaAppCheck);
            this.roundedGroupBox1.Controls.Add(this.nanaZipCheck);
            this.roundedGroupBox1.Controls.Add(this.bingWallpapersCheck);
            this.roundedGroupBox1.CornerRadius = 4;
            this.roundedGroupBox1.ForeColor = System.Drawing.Color.White;
            this.roundedGroupBox1.Location = new System.Drawing.Point(167, 70);
            this.roundedGroupBox1.Name = "roundedGroupBox1";
            this.roundedGroupBox1.Size = new System.Drawing.Size(75, 250);
            this.roundedGroupBox1.TabIndex = 24;
            this.roundedGroupBox1.TabStop = false;
            this.roundedGroupBox1.Text = "Utilities";
            this.roundedGroupBox1.Font = new System.Drawing.Font("Ubuntu", 8.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.roundedGroupBox1.TextColorOverride = null;
            // 
            // installerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.shutdownCheck);
            this.Controls.Add(this.roundedGroupBox2);
            this.Controls.Add(this.recycleBinCheck);
            this.Controls.Add(this.powerCheck);
            this.Controls.Add(this.restartCheck);
            this.Controls.Add(this.versionLabel);
            this.Controls.Add(this.locationLabel);
            this.Controls.Add(this.restart);
            this.Controls.Add(this.close);
            this.Controls.Add(this.progressBar);
            this.Controls.Add(this.installerTextBox);
            this.Controls.Add(this.install);
            this.Controls.Add(this.roundedGroupBox1);
            this.Icon = global::PlutoPoint_Installer.Properties.Resources.computerRepairCentreIcon;
            this.Name = "installerForm";
            this.Text = "Computer Repair Centre Installer";
            this.roundedGroupBox2.ResumeLayout(false);
            this.roundedGroupBox1.ResumeLayout(false);
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
        private System.Windows.Forms.Label locationLabel;
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
        private System.Windows.Forms.CheckBox vlcMediaPlayerCheck;
        private System.Windows.Forms.CheckBox nvidiaAppCheck;
        private System.Windows.Forms.CheckBox recycleBinCheck;
        private System.Windows.Forms.ToolTip crcToolTip;
        private System.Windows.Forms.ToolTip amdToolTip;
        private System.Windows.Forms.ToolTip anyDeskToolTip;
        private System.Windows.Forms.ToolTip bitDefenderToolTip;
        private System.Windows.Forms.ToolTip discordToolTip;
        private System.Windows.Forms.ToolTip googleChromeToolTip;
        private System.Windows.Forms.ToolTip libreOfficeToolTip;
        private System.Windows.Forms.ToolTip microsoftOffice2007ToolTip;
        private System.Windows.Forms.ToolTip nvidiaAppToolTip;
        private System.Windows.Forms.ToolTip mozillaFirefoxToolTip;
        private System.Windows.Forms.ToolTip mozillaThunderbirdToolTip;
        private System.Windows.Forms.ToolTip nanaZipToolTip;
        private System.Windows.Forms.ToolTip steamToolTip;
        private System.Windows.Forms.ToolTip vlcMediaPlayerToolTip;
        private System.Windows.Forms.ToolTip bingWallpaperstoolTip;
        private System.Windows.Forms.ToolTip recycleBinToolTip;
        private System.Windows.Forms.ToolTip powerToolTip;
        private System.Windows.Forms.ToolTip restartToolTip;
        private System.Windows.Forms.ToolTip aiToolTip;
        private RoundedGroupBox roundedGroupBox1;
        private RoundedGroupBox roundedGroupBox2;
        private CheckBox aiCheck;
        private CheckBox shutdownCheck;
        private ToolTip shutdownToolTip;
    }
}
