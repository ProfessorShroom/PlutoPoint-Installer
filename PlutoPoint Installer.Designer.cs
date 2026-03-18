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
            this.installerTextBox = new System.Windows.Forms.Label();
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
            this.aiCheck = new System.Windows.Forms.CheckBox();
            this.shutdownToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.testToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.test = new PlutoPoint_Installer.RoundedButton();
            this.taskbarToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.taskbarCheck = new System.Windows.Forms.CheckBox();
            this.installerLogPanel = new System.Windows.Forms.Panel();
            this.roundedGroupBox2 = new PlutoPoint_Installer.RoundedGroupBox();
            this.restart = new PlutoPoint_Installer.RoundedButton();
            this.close = new PlutoPoint_Installer.RoundedButton();
            this.progressBar = new PlutoPoint_Installer.RoundedGradientProgressBar();
            this.install = new PlutoPoint_Installer.RoundedButton();
            this.roundedGroupBox1 = new PlutoPoint_Installer.RoundedGroupBox();
            this.installerLogPanel.SuspendLayout();
            this.roundedGroupBox2.SuspendLayout();
            this.roundedGroupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // installerTextBox
            // 
            this.installerTextBox.AutoSize = true;
            this.installerTextBox.BackColor = System.Drawing.Color.Transparent;
            this.installerTextBox.ForeColor = System.Drawing.Color.White;
            this.installerTextBox.Location = new System.Drawing.Point(0, 0);
            this.installerTextBox.MaximumSize = new System.Drawing.Size(500, 0);
            this.installerTextBox.Name = "installerTextBox";
            this.installerTextBox.Size = new System.Drawing.Size(0, 13);
            this.installerTextBox.TabIndex = 30;
            // 
            // versionLabel
            // 
            this.versionLabel.AutoSize = true;
            this.versionLabel.BackColor = System.Drawing.Color.Transparent;
            this.versionLabel.Font = new System.Drawing.Font("Ubuntu", 8.25F, System.Drawing.FontStyle.Bold);
            this.versionLabel.LinkColor = System.Drawing.Color.White;
            this.versionLabel.Location = new System.Drawing.Point(12, 425);
            this.versionLabel.Name = "versionLabel";
            this.versionLabel.Size = new System.Drawing.Size(0, 16);
            this.versionLabel.TabIndex = 9;
            this.versionLabel.TabStop = true;
            this.versionLabel.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.versionLabel_LinkClicked);
            // 
            // locationLabel
            // 
            this.locationLabel.AutoSize = true;
            this.locationLabel.BackColor = System.Drawing.Color.Transparent;
            this.locationLabel.Font = new System.Drawing.Font("Ubuntu", 8.25F, System.Drawing.FontStyle.Bold);
            this.locationLabel.ForeColor = System.Drawing.Color.White;
            this.locationLabel.Location = new System.Drawing.Point(120, 425);
            this.locationLabel.Name = "locationLabel";
            this.locationLabel.Size = new System.Drawing.Size(0, 16);
            this.locationLabel.TabIndex = 9;
            this.locationLabel.TabStop = true;
            // 
            // restartCheck
            // 
            this.restartCheck.BackColor = System.Drawing.Color.Transparent;
            this.restartCheck.Image = global::PlutoPoint_Installer.Properties.Resources.restart;
            this.restartCheck.Location = new System.Drawing.Point(687, 415);
            this.restartCheck.Name = "restartCheck";
            this.restartCheck.Size = new System.Drawing.Size(48, 34);
            this.restartCheck.TabIndex = 10;
            this.restartToolTip.SetToolTip(this.restartCheck, "Restart Computer on Install Completion");
            this.restartCheck.UseVisualStyleBackColor = false;
            // 
            // powerCheck
            // 
            this.powerCheck.BackColor = System.Drawing.Color.Transparent;
            this.powerCheck.Image = global::PlutoPoint_Installer.Properties.Resources.power;
            this.powerCheck.Location = new System.Drawing.Point(633, 415);
            this.powerCheck.Name = "powerCheck";
            this.powerCheck.Size = new System.Drawing.Size(48, 34);
            this.powerCheck.TabIndex = 12;
            this.powerToolTip.SetToolTip(this.powerCheck, "Disable Sleep on AC Power");
            this.powerCheck.UseVisualStyleBackColor = false;
            // 
            // recycleBinCheck
            // 
            this.recycleBinCheck.BackColor = System.Drawing.Color.Transparent;
            this.recycleBinCheck.Image = global::PlutoPoint_Installer.Properties.Resources.recycleBin;
            this.recycleBinCheck.Location = new System.Drawing.Point(579, 415);
            this.recycleBinCheck.Name = "recycleBinCheck";
            this.recycleBinCheck.Size = new System.Drawing.Size(48, 34);
            this.recycleBinCheck.TabIndex = 22;
            this.recycleBinToolTip.SetToolTip(this.recycleBinCheck, "Empty Recycle Bin");
            this.recycleBinCheck.UseVisualStyleBackColor = false;
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
            this.libreOfficeToolTip.SetToolTip(this.libreOfficeCheck, "LibreOffice (Free Microsoft Office alternative)");
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
            this.nvidiaAppCheck.Location = new System.Drawing.Point(10, 237);
            this.nvidiaAppCheck.Name = "nvidiaAppCheck";
            this.nvidiaAppCheck.Size = new System.Drawing.Size(57, 50);
            this.nvidiaAppCheck.TabIndex = 21;
            this.nvidiaAppToolTip.SetToolTip(this.nvidiaAppCheck, "Nvidia Graphics App");
            this.nvidiaAppCheck.UseVisualStyleBackColor = true;
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
            this.nanaZipCheck.Location = new System.Drawing.Point(10, 192);
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
            this.bingWallpaperstoolTip.SetToolTip(this.bingWallpapersCheck, "Bing Wallpapers (New wallpaper everyday)");
            this.bingWallpapersCheck.UseVisualStyleBackColor = false;
            // 
            // shutdownCheck
            // 
            this.shutdownCheck.BackColor = System.Drawing.Color.Transparent;
            this.shutdownCheck.Image = global::PlutoPoint_Installer.Properties.Resources.shutdown;
            this.shutdownCheck.Location = new System.Drawing.Point(740, 415);
            this.shutdownCheck.Name = "shutdownCheck";
            this.shutdownCheck.Size = new System.Drawing.Size(48, 34);
            this.shutdownCheck.TabIndex = 26;
            this.shutdownToolTip.SetToolTip(this.shutdownCheck, "Shutdown Computer on Install Completion");
            this.shutdownCheck.UseVisualStyleBackColor = false;
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
            // test
            // 
            this.test.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.test.CornerRadius = 4;
            this.test.Cursor = System.Windows.Forms.Cursors.Hand;
            this.test.Font = new System.Drawing.Font("Ubuntu", 8F, System.Drawing.FontStyle.Bold);
            this.test.ForeColor = System.Drawing.Color.White;
            this.test.HoverShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.test.Location = new System.Drawing.Point(800, 15);
            this.test.Name = "test";
            this.test.PressedShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.test.Size = new System.Drawing.Size(66, 24);
            this.test.TabIndex = 28;
            this.test.Text = "Test";
            this.testToolTip.SetToolTip(this.test, "You\'ve found my test button, you sneaky bastard.");
            this.test.Click += new System.EventHandler(this.test_Click);
            // 
            // taskbarCheck
            // 
            this.taskbarCheck.Checked = true;
            this.taskbarCheck.CheckState = System.Windows.Forms.CheckState.Checked;
            this.taskbarCheck.Image = global::PlutoPoint_Installer.Properties.Resources.taskbar;
            this.taskbarCheck.Location = new System.Drawing.Point(10, 147);
            this.taskbarCheck.Name = "taskbarCheck";
            this.taskbarCheck.Size = new System.Drawing.Size(57, 50);
            this.taskbarCheck.TabIndex = 23;
            this.taskbarToolTip.SetToolTip(this.taskbarCheck, "Move Windows taskbar to the left like Windows 10");
            this.taskbarCheck.UseVisualStyleBackColor = true;
            // 
            // installerLogPanel
            // 
            this.installerLogPanel.AutoScroll = true;
            this.installerLogPanel.BackColor = System.Drawing.Color.Transparent;
            this.installerLogPanel.Controls.Add(this.installerTextBox);
            this.installerLogPanel.Location = new System.Drawing.Point(271, 54);
            this.installerLogPanel.Name = "installerLogPanel";
            this.installerLogPanel.Size = new System.Drawing.Size(517, 355);
            this.installerLogPanel.TabIndex = 29;
            // 
            // roundedGroupBox2
            // 
            this.roundedGroupBox2.BackColor = System.Drawing.Color.Transparent;
            this.roundedGroupBox2.BorderColor = System.Drawing.Color.DodgerBlue;
            this.roundedGroupBox2.BorderColorOverride = null;
            this.roundedGroupBox2.BorderShadowBottom = 80;
            this.roundedGroupBox2.BorderShadowTop = 50;
            this.roundedGroupBox2.BorderThickness = 2F;
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
            this.roundedGroupBox2.CornerRadius = 3;
            this.roundedGroupBox2.Font = new System.Drawing.Font("Ubuntu", 8.25F, System.Drawing.FontStyle.Bold);
            this.roundedGroupBox2.ForeColor = System.Drawing.Color.White;
            this.roundedGroupBox2.Location = new System.Drawing.Point(12, 54);
            this.roundedGroupBox2.Name = "roundedGroupBox2";
            this.roundedGroupBox2.Size = new System.Drawing.Size(149, 296);
            this.roundedGroupBox2.TabIndex = 25;
            this.roundedGroupBox2.TabStop = false;
            this.roundedGroupBox2.Text = "Software";
            this.roundedGroupBox2.TextColorOverride = null;
            // 
            // restart
            // 
            this.restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(80)))), ((int)(((byte)(80)))));
            this.restart.CornerRadius = 3;
            this.restart.Cursor = System.Windows.Forms.Cursors.Hand;
            this.restart.Font = new System.Drawing.Font("Ubuntu", 8F, System.Drawing.FontStyle.Bold);
            this.restart.ForeColor = System.Drawing.Color.White;
            this.restart.HoverShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.restart.Location = new System.Drawing.Point(123, 15);
            this.restart.Name = "restart";
            this.restart.PressedShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.restart.Size = new System.Drawing.Size(66, 24);
            this.restart.TabIndex = 7;
            this.restart.Text = "Restart";
            this.restart.Click += new System.EventHandler(this.restart_Click);
            // 
            // close
            // 
            this.close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            this.close.CornerRadius = 3;
            this.close.Cursor = System.Windows.Forms.Cursors.Hand;
            this.close.Font = new System.Drawing.Font("Ubuntu", 8F, System.Drawing.FontStyle.Bold);
            this.close.ForeColor = System.Drawing.Color.White;
            this.close.HoverShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.close.Location = new System.Drawing.Point(195, 15);
            this.close.Name = "close";
            this.close.PressedShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.close.Size = new System.Drawing.Size(66, 24);
            this.close.TabIndex = 4;
            this.close.Text = "Close";
            this.close.Click += new System.EventHandler(this.close_Click);
            // 
            // progressBar
            // 
            this.progressBar.BackColor = System.Drawing.Color.Transparent;
            this.progressBar.BackShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(20)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.progressBar.CornerRadius = 3;
            this.progressBar.FillShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(70)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.progressBar.Location = new System.Drawing.Point(267, 14);
            this.progressBar.Maximum = 1;
            this.progressBar.Name = "progressBar";
            this.progressBar.ShineShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(30)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.progressBar.ShineWidth = 32;
            this.progressBar.Size = new System.Drawing.Size(521, 24);
            this.progressBar.StepSize = 1;
            this.progressBar.TabIndex = 3;
            this.progressBar.Value = 0;
            // 
            // install
            // 
            this.install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(80)))), ((int)(((byte)(80)))), ((int)(((byte)(255)))));
            this.install.CornerRadius = 3;
            this.install.Cursor = System.Windows.Forms.Cursors.Hand;
            this.install.Font = new System.Drawing.Font("Ubuntu", 12F, System.Drawing.FontStyle.Bold);
            this.install.ForeColor = System.Drawing.Color.White;
            this.install.HoverShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(25)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(255)))));
            this.install.Location = new System.Drawing.Point(12, 15);
            this.install.Name = "install";
            this.install.PressedShadeColor = System.Drawing.Color.FromArgb(((int)(((byte)(50)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))), ((int)(((byte)(0)))));
            this.install.Size = new System.Drawing.Size(105, 24);
            this.install.TabIndex = 0;
            this.install.Text = "Install";
            this.install.Click += new System.EventHandler(this.install_Click);
            // 
            // roundedGroupBox1
            // 
            this.roundedGroupBox1.BackColor = System.Drawing.Color.Transparent;
            this.roundedGroupBox1.BorderColor = System.Drawing.Color.DodgerBlue;
            this.roundedGroupBox1.BorderColorOverride = null;
            this.roundedGroupBox1.BorderShadowBottom = 80;
            this.roundedGroupBox1.BorderShadowTop = 50;
            this.roundedGroupBox1.BorderThickness = 2F;
            this.roundedGroupBox1.Controls.Add(this.taskbarCheck);
            this.roundedGroupBox1.Controls.Add(this.aiCheck);
            this.roundedGroupBox1.Controls.Add(this.crcCheck);
            this.roundedGroupBox1.Controls.Add(this.nvidiaAppCheck);
            this.roundedGroupBox1.Controls.Add(this.nanaZipCheck);
            this.roundedGroupBox1.Controls.Add(this.bingWallpapersCheck);
            this.roundedGroupBox1.CornerRadius = 3;
            this.roundedGroupBox1.Font = new System.Drawing.Font("Ubuntu", 8.25F, System.Drawing.FontStyle.Bold);
            this.roundedGroupBox1.ForeColor = System.Drawing.Color.White;
            this.roundedGroupBox1.Location = new System.Drawing.Point(177, 54);
            this.roundedGroupBox1.Name = "roundedGroupBox1";
            this.roundedGroupBox1.Size = new System.Drawing.Size(75, 296);
            this.roundedGroupBox1.TabIndex = 24;
            this.roundedGroupBox1.TabStop = false;
            this.roundedGroupBox1.Text = "Utilities";
            this.roundedGroupBox1.TextColorOverride = null;
            // 
            // installerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(41)))), ((int)(((byte)(41)))), ((int)(((byte)(41)))));
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.installerLogPanel);
            this.Controls.Add(this.test);
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
            this.Controls.Add(this.install);
            this.Controls.Add(this.roundedGroupBox1);
            this.Icon = global::PlutoPoint_Installer.Properties.Resources.computerRepairCentreIcon;
            this.Name = "installerForm";
            this.Text = "Computer Repair Centre Installer";
            this.installerLogPanel.ResumeLayout(false);
            this.installerLogPanel.PerformLayout();
            this.roundedGroupBox2.ResumeLayout(false);
            this.roundedGroupBox1.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

            }

        #endregion

        private RoundedButton install;
        private Label installerTextBox;
        private RoundedGradientProgressBar progressBar;
        private CheckBox libreOfficeCheck;
        private CheckBox mozillaFirefoxCheck;
        private CheckBox googleChromeCheck;
        private RoundedButton close;
        private RoundedButton restart;
        private CheckBox bingWallpapersCheck;
        private LinkLabel versionLabel;
        private Label locationLabel;
        private CheckBox restartCheck;
        private CheckBox crcCheck;
        private CheckBox powerCheck;
        private CheckBox anyDeskCheck;
        private CheckBox mozillaThunderbirdCheck;
        private CheckBox bitDefenderCheck;
        private CheckBox discordCheck;
        private CheckBox steamCheck;
        private CheckBox nanaZipCheck;
        private CheckBox microsoftOffice2007Check;
        private CheckBox vlcMediaPlayerCheck;
        private CheckBox nvidiaAppCheck;
        private CheckBox recycleBinCheck;
        private ToolTip crcToolTip;
        private ToolTip amdToolTip;
        private ToolTip anyDeskToolTip;
        private ToolTip bitDefenderToolTip;
        private ToolTip discordToolTip;
        private ToolTip googleChromeToolTip;
        private ToolTip libreOfficeToolTip;
        private ToolTip microsoftOffice2007ToolTip;
        private ToolTip nvidiaAppToolTip;
        private ToolTip mozillaFirefoxToolTip;
        private ToolTip mozillaThunderbirdToolTip;
        private ToolTip nanaZipToolTip;
        private ToolTip steamToolTip;
        private ToolTip vlcMediaPlayerToolTip;
        private ToolTip bingWallpaperstoolTip;
        private ToolTip recycleBinToolTip;
        private ToolTip powerToolTip;
        private ToolTip restartToolTip;
        private ToolTip aiToolTip;
        private RoundedGroupBox roundedGroupBox1;
        private RoundedGroupBox roundedGroupBox2;
        private CheckBox aiCheck;
        private CheckBox shutdownCheck;
        private ToolTip shutdownToolTip;
        private RoundedButton test;
        private ToolTip testToolTip;
        private CheckBox taskbarCheck;
        private ToolTip taskbarToolTip;
        private Panel installerLogPanel;
    }
}
