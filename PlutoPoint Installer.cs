using Microsoft.Win32;
using Shell32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using PlutoPoint_Installer.Utilities;
using PlutoPoint_Installer.Attributes;
using PlutoPoint_Installer.UI;
using PlutoPoint_Installer.Models;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer
{
    using System.Drawing;
    using System.Globalization;
    using System.Linq;
    using System.Management;
    using System.Reflection;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Windows.Forms;
    using File = System.IO.File;
    public partial class installerForm : Form
    {
        DateTime buildDate = File.GetLastWriteTime(Assembly.GetExecutingAssembly().Location);
        private Color _gradientTop = Color.FromArgb(30, 200, 255);
        private Color _gradientBottom = Color.FromArgb(140, 0, 255);
        private Task _initialiseUrlsTask;
        private Task _checkIpTask;
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);
        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;
        private string locationLine = "📍 Detecting location...";
        string rootDir;
        string programDataDir;
        private Image _overlayImage;
        private Icon _overlayIcon;
        private float _overlayRotationDegrees;
        public Image OverlayImage { get { return _overlayImage; } set { _overlayImage = value; } }
        public float OverlayRotationDegrees { get { return _overlayRotationDegrees; } set { _overlayRotationDegrees = value; } }
        public installerForm()
        {
            InitializeComponent();
            this.Resize += (s, e) => {
                AdjustInstallerTextBoxSizeForOverlay();
                this.Invalidate();
            };
            ApplyFonts();
            this.AutoScaleMode = AutoScaleMode.Dpi;            
            // Shutdown/restart checks
            shutdownCheck.CheckedChanged += ShutdownCheck_CheckedChanged;
            restartCheck.CheckedChanged += RestartCheck_CheckedChanged;
            // Gradient/transparency
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            // Hover
            install.MouseEnter += (s, e) => AudioEffects.PlayHoverPop();
            restart.MouseEnter += (s, e) => AudioEffects.PlayHoverPop();
            close.MouseEnter += (s, e) => AudioEffects.PlayHoverPop();
            test.MouseEnter += (s, e) => AudioEffects.PlayHoverPop();
            // Click
            install.Click += (s, e) => AudioEffects.PlayClickChime();
            restart.Click += (s, e) => AudioEffects.PlayClickChime();
            close.Click += (s, e) => AudioEffects.PlayClickChime();
            test.Click += (s, e) => AudioEffects.PlayClickChime();
            OverrideRoundedBoxColours();
            // Background tasks only
            _initialiseUrlsTask = InitialiseUrlsAsync();
            _checkIpTask = CheckIPAsync();
            CheckEliteBook();
            // Info checks
            PrintVersion();
            CheckWindowsVersion();
            CheckForIntelHardware();
            CheckforAMDHardware();
            CheckForNvidiaGPU();
            GetLibreOfficeVersion();
            AppendLine(locationLine);
            _ = PrintDayAsync();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            versionLabel.Text = $"Version {version}";
        }
        private UI.ThemeManager _themeManager = new UI.ThemeManager();
        private void ApplyFonts()
        {
            installerTextBox.Font = Program.Ubuntu(12f, FontStyle.Regular);
            close.Font = Program.Ubuntu(9F, FontStyle.Regular);
            restart.Font = Program.Ubuntu(9F, FontStyle.Regular);
            softwareBox.Font = Program.Ubuntu(8F, FontStyle.Regular);
            versionLabel.Font = Program.Ubuntu(8.25f, FontStyle.Regular);
            locationLabel.Font = Program.Ubuntu(8.25f, FontStyle.Regular);
            test.Font = Program.Ubuntu(8F, FontStyle.Regular);
            install.Font = Program.Ubuntu(12, FontStyle.Regular);
            utilitiesBox.Font = Program.Ubuntu(8.25f, FontStyle.Regular);
        }
            protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            _themeManager.ApplyThemeAndMessages(
                (top, bottom, log) => {
                    this.ApplyGradientTheme(top, bottom);
                    this.ApplyLogTheme(log);
                    this.Invalidate();
                },
                (msg) => this.AppendLine(msg)
            );

            _themeManager.UpdateGUIEvent(this);
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (this.OverlayImage == null) return;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle dest = PlutoPoint_Installer.UI.ThemeManager.GetScaledRect(this.OverlayImage, 670, 320, 100, 100);
            using (Matrix m = new Matrix())
            {
                float centerX = dest.Left + dest.Width / 2f;
                float centerY = dest.Top + dest.Height / 2f;
                m.RotateAt(this.OverlayRotationDegrees, new PointF(centerX, centerY));
                e.Graphics.Transform = m;
                e.Graphics.DrawImage(this.OverlayImage, dest);
            }
        }
        public void AdjustInstallerTextBoxSizeForOverlay()
        {
            bool hasOverlayImage = (_overlayImage != null);
            this.installerLogPanel.Size = hasOverlayImage
                ? new System.Drawing.Size(517, 258)
                : new System.Drawing.Size(517, 355);
            installerTextBox.MaximumSize = new Size(installerLogPanel.ClientSize.Width - 10, 0);
        }
        private async Task CheckIPAsync()
        {
            string publicIP = await GetPublicIPAddressAsync();
            if (string.IsNullOrWhiteSpace(publicIP))
                return;
            string publicIPHash = HashIP(publicIP);
            LocationHashes hashes = await GetLocationHashesAsync();
            if (hashes == null)
                return;
            safeLocation = "0";
            romsey = "0";
            chandlersFord = "0";
            highcliffe = "0";
            charlieHome = "0";
            if (publicIPHash == hashes.romsey)
            {
                romsey = "1";
                safeLocation = "1";
            }
            else if (publicIPHash == hashes.chandlersFord)
            {
                chandlersFord = "1";
                safeLocation = "1";
            }
            else if (publicIPHash == hashes.highcliffe)
            {
                highcliffe = "1";
                safeLocation = "1";
            }
            else if (publicIPHash == hashes.charlieHome)
            {
                charlieHome = "1";
                safeLocation = "1";
            }
            AppendLocation();
            UpdateLocation();
        }
        private async Task<string> GetPublicIPAddressAsync()
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string ip = await client.GetStringAsync("https://api.ipify.org");
                    return ip?.Trim();
                }
            }
            catch
            {
                return null;
            }
        }
        // Set strings
        string hpEliteBook = null;
        string safeLocation = null;
        string location = null;
        string romsey = null;
        string chandlersFord = null;
        string highcliffe = null;
        string charlieHome = null;
        string windows7 = null;
        string windows8 = null;
        string windows81 = null;
        string windows10 = null;
        string windows11 = null;
        string amd = null;
        string nvidia = null;
        string intel = null;
        protected override void OnPaintBackground(PaintEventArgs e)
        {
            base.OnPaintBackground(e);
            if (this.ClientSize.Width <= 0 || this.ClientSize.Height <= 0)
                return;
            e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            using (LinearGradientBrush brush = new LinearGradientBrush(
                new Point(0, 0),
                new Point(this.ClientSize.Width, this.ClientSize.Height),
                _gradientTop,
                _gradientBottom))
            {
                e.Graphics.FillRectangle(brush, this.ClientRectangle);
            }
        }
        private void PrintVersion()
        {
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            Func<int, string> WithDaySuffix = day =>
            {
                if (day >= 11 && day <= 13) return day + "th";
                switch (day % 10)
                {
                    case 1: return day + "st";
                    case 2: return day + "nd";
                    case 3: return day + "rd";
                    default: return day + "th";
                }
            };
            var assembly = Assembly.GetExecutingAssembly();
            var updateAttr = assembly
                .GetCustomAttributes(typeof(AssemblyUpdateDateAttribute), false)
                .Cast<AssemblyUpdateDateAttribute>()
                .FirstOrDefault();
            DateTime dateToUse;
            if (updateAttr != null &&
                DateTime.TryParseExact(updateAttr.Date, "dd/MM/yyyy",
                                       System.Globalization.CultureInfo.InvariantCulture,
                                       System.Globalization.DateTimeStyles.None,
                                       out DateTime parsedDate))
            {
                dateToUse = parsedDate;
            }
            else
            {
                dateToUse = DateTime.Today;
            }
            string formatted = string.Format("{0} of {1} {2}",
                WithDaySuffix(dateToUse.Day),
                dateToUse.ToString("MMMM"),
                dateToUse.Year);
            AppendLine($"🛠️ Version {version}");
            AppendLine("📅 Last updated on " + formatted + ".");
        }
        private async Task PrintDayAsync()
        {
            if (_checkIpTask != null)
                await _checkIpTask;
        }
        private void CheckEliteBook()
        {
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Model FROM Win32_ComputerSystem"))
            {
                foreach (ManagementObject computerSystem in searcher.Get())
                {
                    string model = computerSystem["Model"]?.ToString() ?? "";
                    if (model.Contains("EliteBook"))
                    {
                        hpEliteBook = "1";
                        break;
                    }
                }
            }
        }
        private void ApplyButtonTheme(Color backColor, Color foreColor)
        {
            install.BackColor = backColor;
            install.ForeColor = foreColor;
            restart.BackColor = backColor;
            restart.ForeColor = foreColor;
            close.BackColor = backColor;
            close.ForeColor = foreColor;
        }
        private void ApplyGradientTheme(Color topColor, Color bottomColor)
        {
            _gradientTop = topColor;
            _gradientBottom = bottomColor;
        }
        private void ApplyLogTheme(Color foreColor)
        {
            installerTextBox.ForeColor = foreColor;
        }
        private void SyncLabelsWithInstall()
        {
            versionLabel.ForeColor = install.BackColor;
            locationLabel.ForeColor = install.BackColor;
            if (versionLabel is LinkLabel linkLabel)
            {
                linkLabel.LinkColor = install.BackColor;
                linkLabel.ActiveLinkColor = install.BackColor;
                linkLabel.VisitedLinkColor = install.BackColor;
            }
        }
        protected void OverrideRoundedBoxColours()
        {
            softwareBox.BorderColorOverride = versionLabel.LinkColor;
            softwareBox.TextColorOverride = versionLabel.LinkColor;
            utilitiesBox.BorderColorOverride = versionLabel.LinkColor;
            utilitiesBox.TextColorOverride = versionLabel.LinkColor;
        }
        private string HashIP(string ip)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(ip.Trim());
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }
        public class PasswordHashes
        {
            public List<string> allowedHashes { get; set; }
        }
        private async Task<PasswordHashes> GetPasswordHashesAsync()
        {
            try
            {
                string url = "https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/Data/Passwords.json";
                using (var client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    var serializer = new JavaScriptSerializer();
                    return serializer.Deserialize<PasswordHashes>(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load hashes: {ex.Message}");
                return null;
            }
        }
        public class LocationHashes
        {
            public string romsey { get; set; }
            public string chandlersFord { get; set; }
            public string highcliffe { get; set; }
            public string charlieHome { get; set; }
        }
        private async Task<LocationHashes> GetLocationHashesAsync()
        {
            try
            {
                string url = "https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/Data/IPs.json";
                using (var client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    var serializer = new JavaScriptSerializer();
                    return serializer.Deserialize<LocationHashes>(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load hashes: {ex.Message}");
                return null;
            }
        }
        private void AppendLocation()
        {
            if (romsey == "1")
                locationLine = "📍 The installer is being run from the Romsey shop.";
            else if (chandlersFord == "1")
                locationLine = "📍 The installer is being run from the Chandlers Ford shop.";
            else if (highcliffe == "1")
                locationLine = "📍 The installer is being run from the Highcliffe shop.";
            else if (charlieHome == "1")
                locationLine = "📍 The installer is being run from Charlie's house.";
            else
                locationLine = "📍 Location unknown.";
            // rebuild text safely
            var lines = installerTextBox.Text.Split(new[] { Environment.NewLine }, StringSplitOptions.None).ToList();
            for (int i = 0; i < lines.Count; i++)
            {
                if (lines[i].StartsWith("📍"))
                {
                    lines[i] = locationLine;
                    break;
                }
            }
            installerTextBox.Text = string.Join(Environment.NewLine, lines);
            installerLogPanel.PerformLayout();
            installerLogPanel.AutoScrollPosition = new Point(0, installerTextBox.Bottom);
        }
        private string GetPublicIPAddress()
        {
            try
            {
                string url = "https://api.ipify.org";
                using (var webClient = new WebClient())
                {
                    string ip = webClient.DownloadString(url).Trim();
                    return ip;
                }
            }
            catch
            {
                return null;
            }
        }
        private void UpdateLocation()
        {
            if (romsey == "1")
            {
                location = "Romsey";
            }
            else if (chandlersFord == "1")
            {
                location = "Chandler's Ford";
            }
            else if (highcliffe == "1")
            {
                location = "Highcliffe";
            }
            else if (charlieHome == "1")
            {
                location = "Charlie's House";
            }
            else
            {
                location = "Unknown";
            }
            locationLabel.Text = "Current location: " + location;
        }
        public class DownloadUrls
        {
            public string crcOEM { get; set; }
            public string anyDesk { get; set; }
            public string bingWallpapers { get; set; }
            public string bitDefender { get; set; }
            public string discord { get; set; }
            public string googleChrome { get; set; }
            public string mozillaFirefox { get; set; }
            public string mozillaThunderbird { get; set; }
            public string nanaZip { get; set; }
            public string steam { get; set; }
            public string hpHotkeySupport { get; set; }
            public string vlcMediaPlayer { get; set; }
        }
        private DownloadUrls urls;
        private async Task InitialiseUrlsAsync()
        {
            urls = await GetDownloadUrlsAsync();
            if (urls == null)
            {
                MessageBox.Show("Failed to load download URLs.");
            }
        }
        private async Task<DownloadUrls> GetDownloadUrlsAsync()
        {
            try
            {
                string url = "https://raw.githubusercontent.com/professorshroom/PlutoPoint-Installer/main/Data/Downloads.json";
                using (var client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    var serializer = new JavaScriptSerializer();
                    return serializer.Deserialize<DownloadUrls>(json);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to load URLs: {ex.Message}");
                return null;
            }
        }
        private Uri crcOEMURL => new Uri(urls.crcOEM);
        private Uri anyDeskURL => new Uri(urls.anyDesk);
        private Uri bingWallpapersURL => new Uri(urls.bingWallpapers);
        private Uri bitDefenderURL => new Uri(urls.bitDefender);
        private Uri discordURL => new Uri(urls.discord);
        private Uri googleChromeURL => new Uri(urls.googleChrome);
        private Uri mozillaFirefoxURL => new Uri(urls.mozillaFirefox);
        private Uri mozillaThunderbirdURL => new Uri(urls.mozillaThunderbird);
        private Uri nanaZipURL => new Uri(urls.nanaZip);
        private Uri steamURL => new Uri(urls.steam);
        private Uri hpHotkeySupportURL => new Uri(urls.hpHotkeySupport);
        private Uri vlcMediaPlayerURL => new Uri(urls.vlcMediaPlayer);
        public bool IsClickPlaying { get; private set; }
        public class FileDeletionHelper
        {
            public async Task DeleteFilesAndDirectoryAsync(string appsDir, string launcherPath)
            {
                var deleteFileTasks = new List<Task>();
                foreach (var file in Directory.EnumerateFiles(appsDir))
                {
                    deleteFileTasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            System.IO.File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                        }
                    }));
                }
                await Task.WhenAll(deleteFileTasks);
                try
                {
                    await Task.Run(() => Directory.Delete(appsDir, true));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting directory {appsDir}: {ex.Message}");
                }
                if (System.IO.File.Exists(launcherPath))
                {
                    try
                    {
                        await Task.Run(() => System.IO.File.Delete(launcherPath));
                        Console.WriteLine("File deleted successfully.");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting file: {ex.Message}");
                    }
                }
                else
                {
                    Console.WriteLine("File does not exist.");
                }
            }
        }
        private void CheckWindowsVersion()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\\Microsoft\\Windows NT\\CurrentVersion"))
                {
                    if (key != null)
                    {
                        string buildNumber = key.GetValue("CurrentBuild")?.ToString();
                        if (int.TryParse(buildNumber, out int build))
                        {
                            string versionText;
                            if (build >= 22000)
                            {
                                versionText = "🪟 Windows 11 detected.";
                                windows11 = "1";
                            }
                            else if (build >= 10240)
                            {
                                versionText = "🪟 Windows 10 detected. Time to move on grandad.";
                                windows10 = "1";
                            }
                            else if (build >= 9600)
                            {
                                versionText = "🪟 Windows 8.1 detected. Why?";
                                windows81 = "1";
                            }
                            else if (build >= 9200)
                            {
                                versionText = "🪟 Windows 8 detected. No really, why?";
                                windows8 = "1";
                            }
                            else if (build >= 7600)
                            {
                                versionText = "🪟 Windows 7 detected. No it really is time to move on grandad.";
                                windows7 = "1";
                            }
                            else
                            {
                                versionText = "🪟 Older or unknown Windows version detected.";
                            }
                            AppendLine(versionText);
                            return;
                        }
                    }
                    AppendLine("⚠️ Unable to determine Windows version.");
                }
            }
            catch (Exception ex)
            {
                AppendLine("❌ Error checking Windows version: " + ex.Message);
            }
        }
        private void CheckForIntelHardware()
        {
            bool hasIntelGpu = false;
            bool hasIntelCpu = false;
            var gpuSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject queryObj in gpuSearcher.Get())
            {
                if (queryObj["Caption"] is string caption)
                {
                    if (caption.IndexOf("Intel", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        caption.IndexOf("Iris", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        caption.IndexOf("UHD", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        caption.IndexOf("Xe", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        caption.IndexOf("Arc", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        hasIntelGpu = true;
                        break;
                    }
                }
            }
            var cpuSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject queryObj in cpuSearcher.Get())
            {
                if (queryObj["Name"] is string name)
                {
                    if (name.IndexOf("Intel", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        name.IndexOf("Core", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        name.IndexOf("Xeon", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        name.IndexOf("Pentium", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        name.IndexOf("Celeron", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        hasIntelCpu = true;
                        break;
                    }
                }
            }
            if (hasIntelGpu || hasIntelCpu)
            {
                intel = "1";
                if (hasIntelGpu && hasIntelCpu)
                    AppendLine("🧠 + 🎮 Intel CPU and GPU detected.");
                else if (hasIntelGpu)
                    AppendLine("🎮 Intel GPU detected.");
                else
                    AppendLine("🧠 Intel CPU detected.");
            }
            else
            {
            }
        }
        private void CheckForNvidiaGPU()
        {
            var searcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject queryObj in searcher.Get())
            {
                if (queryObj["Caption"] is string caption)
                {
                    if (caption.IndexOf("NVIDIA", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        nvidiaAppCheck.Checked = true;
                        nvidia = "1";
                        AppendLine("🎮 Nvidia GPU detected.");
                        return;
                    }
                }
            }
            nvidiaAppCheck.Checked = false;
        }
        private void CheckforAMDHardware()
        {
            bool hasAmdGpu = false;
            bool hasAmdCpu = false;
            var gpuSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_VideoController");
            foreach (ManagementObject queryObj in gpuSearcher.Get())
            {
                if (queryObj["Caption"] is string caption)
                {
                    if (caption.IndexOf("AMD", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        caption.IndexOf("Radeon", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        caption.IndexOf("Advanced Micro Devices", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        hasAmdGpu = true;
                        break;
                    }
                }
            }
            var cpuSearcher = new ManagementObjectSearcher("SELECT * FROM Win32_Processor");
            foreach (ManagementObject queryObj in cpuSearcher.Get())
            {
                if (queryObj["Name"] is string name)
                {
                    if (name.IndexOf("AMD", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        name.IndexOf("Ryzen", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        name.IndexOf("Threadripper", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        hasAmdCpu = true;
                        break;
                    }
                }
            }
            if (hasAmdGpu || hasAmdCpu)
            {
                amd = "1";
                if (hasAmdGpu && hasAmdCpu)
                    AppendLine("🧠 + 🎮 AMD CPU and GPU detected.");
                else if (hasAmdGpu)
                    AppendLine("🎮 AMD GPU detected.");
                else
                    AppendLine("🧠 AMD CPU detected.");
            }
            else
            {
            }
        }
        private string GetLibreOfficeVersion()
        {
            string url = "https://www.libreoffice.org/download/";
            try
            {
                using (var client = new HttpClient())
                {
                    client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");

                    string html = client.GetStringAsync(url).Result;

                    var match = Regex.Match(
                        html,
                        @"(\d+\.\d+(?:\.\d+)?)[^0-9]{0,100}Windows \(x86-64\)",
                        RegexOptions.IgnoreCase
                    );

                    if (match.Success)
                    {
                        return match.Groups[1].Value;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }

            return null;
        }
        private async void install_Click(object sender, EventArgs e)
        {
            if (safeLocation == "0")
            {
                var hashes = await GetPasswordHashesAsync();
                if (hashes?.allowedHashes == null || hashes.allowedHashes.Count == 0)
                {
                    MessageBox.Show("Unable to load password hashes.");
                    return;
                }
                using (PasswordForm pf = new PasswordForm())
                {
                    if (pf.ShowDialog() == DialogResult.OK)
                    {
                        string enteredHash = ComputeSHA256(pf.EnteredPassword);
                        if (!hashes.allowedHashes.Contains(enteredHash))
                        {
                            MessageBox.Show("Incorrect password. Exiting.");
                            this.Close();
                            return;
                        }
                    }
                    else
                    {
                        MessageBox.Show("Password required. Exiting installer.");
                        Environment.Exit(0);
                        return;
                    }
                }
            }
            progressBar.Maximum = 0;
            // Paths
            rootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ComputerRepairCentre");
            programDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string oemDir = System.IO.Path.Combine(programDataDir, "Computer Repair Centre\\OEM\\");
            string appsDir = System.IO.Path.Combine(rootDir, "apps");
            string windowsAppsPath = @"C:\Program Files\WindowsApps";
            // Installed apps
            string googleChromeExePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            string mozillaFirefoxExePath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
            string mozillaThunderbirdExePath = @"C:\Program Files\Mozilla Thunderbird\thunderbird.exe";
            string nanaZipExe = "NanaZip.Modern.FileManager.exe";
            string nanaZipPath = null;
            // Downloaded installers
            string crcOEMFilename = System.IO.Path.Combine(oemDir, "computerRepairCentreOEM.bmp");
            string googleChromeFilename = System.IO.Path.Combine(appsDir, "googleChrome.msi");
            string mozillaFirefoxFilename = System.IO.Path.Combine(appsDir, "mozillaFirefox.msi");
            string anyDeskFilename = System.IO.Path.Combine(appsDir, "anyDesk.msi");
            string bingWallpapersFilename = System.IO.Path.Combine(appsDir, "bingWallpapers.msi");
            string bitDefenderFilename = System.IO.Path.Combine(appsDir, "bitDefender.exe");
            string discordFilename = System.IO.Path.Combine(appsDir, "discord.exe");
            string libreOfficeFilename = System.IO.Path.Combine(appsDir, "libreOffice.msi");
            string mozillaThunderbirdFilename = System.IO.Path.Combine(appsDir, "mozillaThunderbird.msi");
            string nanaZipFilename = System.IO.Path.Combine(appsDir, "nanaZip.msixbundle");
            string steamFilename = System.IO.Path.Combine(appsDir, "steam.exe");
            string hpHotkeySupportFilename = System.IO.Path.Combine(appsDir, "hpHotkeySupport.zip");
            string vlcMediaPlayerFilename = System.IO.Path.Combine(appsDir, "vlcMediaPlayer.msi");
            string nvidiaAppFilename = System.IO.Path.Combine(appsDir, "nvidiaApp.exe");
            // Other apps
            string bingWallpaperAppPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\BingWallpaperApp\BingWallpaperApp.exe");
            string discordAppPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Discord\Update.exe");
            // Desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string launcherPath = System.IO.Path.Combine(desktopPath, @"Computer Repair Centre Installer Launcher.exe");
            if (!Directory.Exists(rootDir))
            {
                Directory.CreateDirectory(rootDir);
            }
            if (!Directory.Exists(oemDir))
            {
                Directory.CreateDirectory(oemDir);
            }
            if (!Directory.Exists(appsDir))
            {
                Directory.CreateDirectory(appsDir);
            }
            if (windows10 == "1")
            {
                if (romsey == "1") { progressBar.Maximum += 1; };
                if (highcliffe == "1") { progressBar.Maximum += 1; };
            }
            if (windows11 == "1") { progressBar.Maximum += 7; }
            if (powerCheck.Checked) { progressBar.Maximum += 1; }
            else { progressBar.Maximum += 2; }
            if (crcCheck.Checked) { progressBar.Maximum += 1; }
            if (anyDeskCheck.Checked) { progressBar.Maximum += 2; }
            if (nanaZipCheck.Checked) { progressBar.Maximum += 2; }
            if (bitDefenderCheck.Checked) { progressBar.Maximum += 2; }
            if (bingWallpapersCheck.Checked) { progressBar.Maximum += 2; }
            if (discordCheck.Checked) { progressBar.Maximum += 2; }
            if (googleChromeCheck.Checked) { progressBar.Maximum += 2; }
            if (libreOfficeCheck.Checked) { progressBar.Maximum += 2; }
            if (nvidiaAppCheck.Checked) { progressBar.Maximum += 2; }
            if (mozillaFirefoxCheck.Checked) { progressBar.Maximum += 2; }
            if (mozillaThunderbirdCheck.Checked) { progressBar.Maximum += 2; }
            if (steamCheck.Checked) { progressBar.Maximum += 2; }
            if (hpEliteBook == "1") { progressBar.Maximum += 4; }
            if (taskbarCheck.Checked) { progressBar.Maximum += 1;  }
            if (nvidiaAppCheck.Checked & nvidia == "1")
            {
                AppendLine("🎮 Nvidia GPU has been detected and selected, Nvidia App will be installed.");
                AppendLine("✅ You can uncheck this if you want.");
            }
            if (powerCheck.Checked)
            {
                AppendLine("📌 Disable sleep on AC power is selected.");
                AppendLine("🔄 Disabling sleep and screen timeout while on AC power...");
                Process.Start("powercfg", "/change monitor-timeout-ac 0");
                Process.Start("powercfg", "/change standby-timeout-ac 0");
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }
            else
            {
                AppendLine("🔄 Disabling sleep and screen timeout while on AC power temporarily during install...");
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }
            if (crcCheck.Checked)
            {
                AppendLine("📌 Computer Repair Centre OEM information is selected.");
                if (romsey == "1")
                {
                    AppendLine("📦 Installing Romsey Computer Repair Centre OEM information...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(crcOEMURL, crcOEMFilename);
                    }
                    const string oemRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation";
                    const string logoReg = "Logo";
                    const string logoRegData = @"C:\ProgramData\Computer Repair Centre\OEM\computerRepairCentreOEM.bmp";
                    const string manufacturerReg = "Manufacturer";
                    const string manufacturerRegData = "Computer Repair Centre";
                    const string supportHoursReg = "SupportHours";
                    const string supportHoursData = "Mon-Fri 9:15am-5:00pm - Sat 9:15am-4:00pm";
                    const string supportPhoneReg = "SupportPhone";
                    const string supportPhoneData = "01794 517142";
                    const string supportURLReg = "SupportURL";
                    const string supportURLRegData = "https://www.thecomputerrepaircentre.co.uk/romsey";
                    using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(oemRegPath, writable: true))
                    {
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(logoReg, logoRegData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{logoReg}' to {logoRegData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(manufacturerReg, manufacturerRegData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{manufacturerReg}' to {manufacturerRegData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(supportHoursReg, supportHoursData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{supportHoursReg}' to {supportHoursData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(supportPhoneReg, supportPhoneData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{supportPhoneReg}' to {supportPhoneData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(supportURLReg, supportURLRegData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{supportURLReg}' to {supportURLRegData} in '{oemRegPath}'.");
                    }
                }
                if (chandlersFord == "1")
                {
                    AppendLine("📦 Installing Chandlers Ford Computer Repair Centre OEM information...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(crcOEMURL, crcOEMFilename);
                    }
                    const string oemRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation";
                    const string logoReg = "Logo";
                    const string logoRegData = @"C:\ProgramData\Computer Repair Centre\OEM\computerRepairCentreOEM.bmp";
                    const string manufacturerReg = "Manufacturer";
                    const string manufacturerRegData = "Computer Repair Centre";
                    const string supportHoursReg = "SupportHours";
                    const string supportHoursData = "Mon-Fri 9:00am-5:30pm - Sat 9:00am-2:00pm";
                    const string supportPhoneReg = "SupportPhone";
                    const string supportPhoneData = "02380 270271";
                    const string supportURLReg = "SupportURL";
                    const string supportURLRegData = "https://www.thecomputerrepaircentre.co.uk/chandlers-ford";
                    using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(oemRegPath, writable: true))
                    {
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(logoReg, logoRegData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{logoReg}' to {logoRegData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(manufacturerReg, manufacturerRegData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{manufacturerReg}' to {manufacturerRegData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(supportHoursReg, supportHoursData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{supportHoursReg}' to {supportHoursData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(supportPhoneReg, supportPhoneData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{supportPhoneReg}' to {supportPhoneData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(supportURLReg, supportURLRegData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{supportURLReg}' to {supportURLRegData} in '{oemRegPath}'.");
                    }
                }
                if (highcliffe == "1")
                {
                    AppendLine("📦 Installing Highcliffe Computer Repair Centre OEM information...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(crcOEMURL, crcOEMFilename);
                    }
                    const string oemRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation";
                    const string logoReg = "Logo";
                    const string logoRegData = @"C:\ProgramData\Computer Repair Centre\OEM\computerRepairCentreOEM.bmp";
                    const string manufacturerReg = "Manufacturer";
                    const string manufacturerRegData = "Computer Repair Centre";
                    const string supportHoursReg = "SupportHours";
                    const string supportHoursData = "Mon-Fri 9:15am-5:00pm - Sat 9:15am-2:00pm";
                    const string supportPhoneReg = "SupportPhone";
                    const string supportPhoneData = "01425 278579";
                    const string supportURLReg = "SupportURL";
                    const string supportURLRegData = "https://www.thecomputerrepaircentre.co.uk/highcliffe";
                    using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(oemRegPath, writable: true))
                    {
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(logoReg, logoRegData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{logoReg}' to {logoRegData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(manufacturerReg, manufacturerRegData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{manufacturerReg}' to {manufacturerRegData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(supportHoursReg, supportHoursData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{supportHoursReg}' to {supportHoursData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(supportPhoneReg, supportPhoneData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{supportPhoneReg}' to {supportPhoneData} in '{oemRegPath}'.");
                    }
                    using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(oemRegPath, writable: true))
                    {
                        registryKey.SetValue(supportURLReg, supportURLRegData, RegistryValueKind.String);
                        Console.WriteLine($"Set '{supportURLReg}' to {supportURLRegData} in '{oemRegPath}'.");
                    }
                }
            }
            if (nanaZipCheck.Checked)
            {
                AppendLine("📌 NanaZip is selected.");
                AppendLine("🔄 Checking if NanaZip is installed...");
                try
                {
                    var files = Directory.GetFiles(windowsAppsPath, nanaZipExe, SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        nanaZipPath = files[0];
                        AppendLine($"✅ NanaZip is already installed.");
                        progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                    }
                    else
                    {
                        AppendLine("❌ NanaZip not found, proceeding with installation.");
                        AppendLine("🔄 Downloading NanaZip...");
                        using (WebClient wc = new WebClient())
                        {
                            await wc.DownloadFileTaskAsync(nanaZipURL, nanaZipFilename);
                        }
                        AppendLine("📦 Installing NanaZip...");
                        Process nanaZipInstallProcess = Process.Start(new ProcessStartInfo
                        {
                            FileName = "powershell",
                            Arguments = $"-Command Add-AppxPackage -Path '{nanaZipFilename}'",
                            UseShellExecute = false,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            CreateNoWindow = true
                        });
                        if (nanaZipInstallProcess != null)
                        {
                            await Task.Run(() => nanaZipInstallProcess.WaitForExit());
                        }
                        AppendLine("✅ Completed installation of NanaZip.");
                        progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    AppendLine("⚠️ Access denied to WindowsApps. Try running as Administrator.");
                }
                catch (Exception ex)
                {
                    AppendLine("❌ Error: " + ex.Message);
                }
            }
            if (taskbarCheck.Checked)
            {
                AppendLine("📌 Move taskbar is selected.");
                AppendLine("✅ Aligning the taskbar to the left...");
                const string taskbarRegPath = @"SOFTWARE\microsoft\windows\currentversion\explorer\advanced";
                const string taskbarReg = "TaskbarAl";
                const int taskbarRegData = 0;
                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(taskbarRegPath, writable: true))
                {
                    registryKey.SetValue(taskbarReg, taskbarRegData, RegistryValueKind.DWord);
                    Console.WriteLine($"Set '{taskbarReg}' to {taskbarRegData} in '{taskbarRegPath}'.");
                }
                AppendLine("✅ Moved taskbar to the left.");
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }
            if (anyDeskCheck.Checked)
            {
                AppendLine("📌 AnyDesk is selected.");
                if (System.IO.File.Exists(@"C:\Program Files (x86)\AnyDeskMSI\AnyDeskMSI.exe"))
                {
                    AppendLine("✅ AnyDesk is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else if (System.IO.File.Exists(@"C:\Program Files (x86)\AnyDesk\AnyDesk.exe"))
                {
                    AppendLine("✅ AnyDesk is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    AppendLine("🔄 Downloading AnyDesk...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(anyDeskURL, anyDeskFilename);
                    }
                    AppendLine("📦 Installing AnyDesk...");
                    await Task.Run(() =>
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "msiexec";
                            process.StartInfo.Arguments = $"/package \"{anyDeskFilename}\" /passive";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.StartInfo.CreateNoWindow = true;
                            try
                            {
                                process.Start();
                                string output = process.StandardOutput.ReadToEnd();
                                string error = process.StandardError.ReadToEnd();
                                process.WaitForExit();
                                Console.WriteLine("Output: " + output);
                                if (!string.IsNullOrEmpty(error))
                                {
                                    Console.WriteLine("Error: " + error);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred: " + ex.Message);
                            }
                        }
                    });
                    AppendLine("✅ Completed installation of AnyDesk.");
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (bingWallpapersCheck.Checked)
            {
                AppendLine("📌 Bing Wallpapers is selected.");
                if (System.IO.File.Exists(bingWallpaperAppPath))
                {
                    AppendLine("✅ Bing Wallpapers is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    AppendLine("🔄 Downloading Bing Wallpapers...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(bingWallpapersURL, bingWallpapersFilename);
                    }
                    AppendLine("📦 Installing Bing Wallpapers...");
                    await Task.Run(() =>
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "msiexec";
                            process.StartInfo.Arguments = $"/package \"{bingWallpapersFilename}\" /passive";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.StartInfo.CreateNoWindow = true;
                            try
                            {
                                process.Start();
                                string output = process.StandardOutput.ReadToEnd();
                                string error = process.StandardError.ReadToEnd();
                                process.WaitForExit();
                                Console.WriteLine("Output: " + output);
                                if (!string.IsNullOrEmpty(error))
                                {
                                    Console.WriteLine("Error: " + error);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred: " + ex.Message);
                            }
                        }
                    });
                    AppendLine("✅ Completed installation of Bing Wallpapers.");
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (bitDefenderCheck.Checked)
            {
                AppendLine("📌 BitDefender is selected.");
                if (System.IO.File.Exists(@"C:\Program Files\Bitdefender\Bitdefender Security App\seccenter.exe"))
                {
                    AppendLine("✅ BitDefender is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    AppendLine("🔄 Downloading BitDefender...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(bitDefenderURL, bitDefenderFilename);
                    }
                    AppendLine("📦 Installing BitDefender...");
                    await Task.Run(() =>
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = bitDefenderFilename,
                            Arguments = "/bdparams /silent",
                            UseShellExecute = true,
                            Verb = "runas"
                        };
                        try
                        {
                            using (Process process = Process.Start(startInfo))
                            {
                                process.WaitForExit();
                                int exitCode = process.ExitCode;
                                if (exitCode == 0)
                                {
                                    Console.WriteLine("Installation successful.");
                                }
                                else
                                {
                                    Console.WriteLine($"Installation exited with code: {exitCode}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    });
                    AppendLine("✅ Completed installation of BitDefender.");
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (discordCheck.Checked)
            {
                AppendLine("📌 Discord is selected.");
                if (System.IO.File.Exists(discordAppPath))
                {
                    AppendLine("✅ Discord is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    AppendLine("🔄 Downloading Discord...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(discordURL, discordFilename);
                    }
                    AppendLine("📦 Installing Discord...");
                    await Task.Run(() =>
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = discordFilename,
                            Arguments = "-s",
                            UseShellExecute = true,
                            Verb = "runas"
                        };
                        try
                        {
                            using (Process process = Process.Start(startInfo))
                            {
                                process.WaitForExit();
                                int exitCode = process.ExitCode;
                                if (exitCode == 0)
                                {
                                    Console.WriteLine("Installation successful.");
                                }
                                else
                                {
                                    Console.WriteLine($"Installation exited with code: {exitCode}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    });
                    AppendLine("✅ Completed installation of Discord.");
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (googleChromeCheck.Checked)
            {
                AppendLine("📌 Google Chrome is selected.");
                if (!File.Exists(googleChromeExePath))
                {
                    AppendLine("🔄 Downloading Google Chrome...");
                    try
                    {
                        using (WebClient wc = new WebClient())
                        {
                            await wc.DownloadFileTaskAsync(googleChromeURL, googleChromeFilename);
                        }
                        AppendLine("✅ Chrome download completed.");
                    }
                    catch (WebException ex)
                    {
                        AppendLine("❌ Failed to download Google Chrome: " + ex.Message);
                    } 
                    AppendLine("📦 Installing Google Chrome...");
                    try
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "msiexec";
                            process.StartInfo.Arguments = $"/package \"{googleChromeFilename}\" /passive";
                            process.StartInfo.UseShellExecute = true;
                            process.Start();
                            process.WaitForExit();
                        }
                        AppendLine("✅ Completed installation of Google Chrome.");
                    }
                    catch (Exception ex)
                    {
                        AppendLine("❌ Chrome installation failed: " + ex.Message);
                    }
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
                else
                {
                    AppendLine("✅ Google Chrome is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
            }
            if (libreOfficeCheck.Checked)
            {
                AppendLine("📌 LibreOffice is selected.");
                if (System.IO.File.Exists(@"C:\Program Files\LibreOffice\program\soffice.exe"))
                {
                    AppendLine("✅ LibreOffice is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    AppendLine("🔄 Downloading LibreOffice...");
                    string libreOfficeVersion = GetLibreOfficeVersion();
                    if (string.IsNullOrEmpty(libreOfficeVersion))
                    {
                        MessageBox.Show("Could not determine the latest LibreOffice version.");
                        return;
                    }
                    string libreOfficeDownloadUrl = $"https://download.documentfoundation.org/libreoffice/stable/{libreOfficeVersion}/win/x86_64/LibreOffice_{libreOfficeVersion}_Win_x86-64.msi";
                    Uri libreOfficeURL = new Uri(libreOfficeDownloadUrl);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(libreOfficeURL, libreOfficeFilename);
                    }
                    if (!File.Exists(libreOfficeFilename))
                    {
                        AppendLine("❌ LibreOffice download failed; falling back to known installer.");
                        libreOfficeURL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/libreOffice.msi");
                        using (WebClient wc = new WebClient())
                        {
                            wc.DownloadFileCompleted += wc_progressBarStep;
                            await wc.DownloadFileTaskAsync(libreOfficeURL, libreOfficeFilename);
                        }
                    }
                    AppendLine("📦 Installing LibreOffice...");
                    await Task.Run(() =>
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "msiexec";
                            process.StartInfo.Arguments = $"/package \"{libreOfficeFilename}\" /passive /norestart";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.StartInfo.CreateNoWindow = true;
                            try
                            {
                                process.Start();
                                string output = process.StandardOutput.ReadToEnd();
                                string error = process.StandardError.ReadToEnd();
                                process.WaitForExit();
                                Console.WriteLine("Output: " + output);
                                if (!string.IsNullOrEmpty(error))
                                {
                                    Console.WriteLine("Error: " + error);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred: " + ex.Message);
                            }
                        }
                    });
                    AppendLine("✅ Completed installation of LibreOffice.");
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (nvidiaAppCheck.Checked)
            {
                AppendLine("📌 Nvidia App is selected.");
                AppendLine("🔄 Searching for latest Nvidia App installer...");
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string edition = "Public";
                        string baseUrl = edition.Equals("Enterprise", StringComparison.OrdinalIgnoreCase)
                            ? "https://www.nvidia.com/en-us/software/nvidia-app-enterprise/"
                            : "https://www.nvidia.com/en-us/software/nvidia-app/";
                        string htmlContent = await client.GetStringAsync(baseUrl);
                        string pattern = @"https:\/\/us\.download\.nvidia\.com\/nvapp\/client\/[\d\.]+\/NVIDIA_app_v[\d\.]+\.exe";
                        Match match = Regex.Match(htmlContent, pattern, RegexOptions.IgnoreCase);
                        if (match.Success)
                        {
                            string downloadUrl = match.Value;
                            AppendLine($"🔗 Found latest Nvidia installer: {downloadUrl}");
                            byte[] fileBytes = await client.GetByteArrayAsync(downloadUrl);
                            File.WriteAllBytes(nvidiaAppFilename, fileBytes);
                        }
                        else
                        {
                            AppendLine("⚠️ Could not find Nvidia App download link.");
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendLine($"⚠️ Error downloading Nvidia App: {ex.Message}");
                    return;
                }
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                AppendLine("📦 Installing Nvidia App silently...");
                await Task.Run(() =>
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = nvidiaAppFilename,
                        Arguments = "/s",
                        UseShellExecute = true,
                        Verb = "runas"
                    };
                    try
                    {
                        using (Process process = Process.Start(startInfo))
                        {
                            process.WaitForExit();
                            int exitCode = process.ExitCode;
                            AppendLine(exitCode == 0
                                ? "✅ Installation successful."
                                : $"⚠️ Installation exited with code: {exitCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLine($"⚠️ Installation failed: {ex.Message}");
                    }
                });
                AppendLine("✅ Completed installation of Nvidia App.");
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }
            if (mozillaFirefoxCheck.Checked)
            {
                AppendLine("📌 Mozilla Firefox is selected.");
                if (File.Exists(mozillaFirefoxExePath))
                {
                    AppendLine("✅ Mozilla Firefox is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    AppendLine("🔄 Downloading Mozilla Firefox...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(mozillaFirefoxURL, mozillaFirefoxFilename);
                    }
                    AppendLine("📦 Installing Mozilla Firefox...");
                    await Task.Run(() =>
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "msiexec";
                            process.StartInfo.Arguments = $"/package \"{mozillaFirefoxFilename}\" /passive";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.StartInfo.CreateNoWindow = true;
                            try
                            {
                                process.Start();
                                string output = process.StandardOutput.ReadToEnd();
                                string error = process.StandardError.ReadToEnd();
                                process.WaitForExit();
                                Console.WriteLine("Output: " + output);
                                if (!string.IsNullOrEmpty(error))
                                    Console.WriteLine("Error: " + error);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred: " + ex.Message);
                            }
                        }
                    });
                    AppendLine("✅ Completed installation of Mozilla Firefox.");
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (mozillaThunderbirdCheck.Checked)
            {
                AppendLine("📌 Mozilla Thunderbird is selected.");
                // Check if Thunderbird is already installed
                if (File.Exists(mozillaThunderbirdExePath))
                {
                    AppendLine("✅ Mozilla Thunderbird is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    // Download Thunderbird
                    AppendLine("🔄 Downloading Mozilla Thunderbird...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(mozillaThunderbirdURL, mozillaThunderbirdFilename);
                    }
                    // Install Thunderbird
                    AppendLine("📦 Installing Mozilla Thunderbird...");
                    await Task.Run(() =>
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "msiexec";
                            process.StartInfo.Arguments = $"/package \"{mozillaThunderbirdFilename}\" /passive";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.StartInfo.CreateNoWindow = true;
                            try
                            {
                                process.Start();
                                string output = process.StandardOutput.ReadToEnd();
                                string error = process.StandardError.ReadToEnd();
                                process.WaitForExit();
                                Console.WriteLine("Output: " + output);
                                if (!string.IsNullOrEmpty(error))
                                    Console.WriteLine("Error: " + error);
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred: " + ex.Message);
                            }
                        }
                    });
                    AppendLine("✅ Completed installation of Mozilla Thunderbird.");
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (steamCheck.Checked)
            {
                AppendLine("📌 Steam is selected.");
                if (System.IO.File.Exists(@"C:\Program Files (x86)\Steam\Steam.exe"))
                {
                    AppendLine("✅ Steam is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    AppendLine("🔄 Downloading Steam...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(steamURL, steamFilename);
                    }
                    AppendLine("📦 Installing Steam...");
                    await Task.Run(() =>
                    {
                        ProcessStartInfo startInfo = new ProcessStartInfo
                        {
                            FileName = steamFilename,
                            Arguments = "/S",
                            UseShellExecute = true,
                            Verb = "runas"
                        };
                        try
                        {
                            using (Process process = Process.Start(startInfo))
                            {
                                process.WaitForExit();
                                int exitCode = process.ExitCode;
                                if (exitCode == 0)
                                {
                                    Console.WriteLine("Installation successful.");
                                }
                                else
                                {
                                    Console.WriteLine($"Installation exited with code: {exitCode}");
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    });
                    AppendLine("✅ Completed installation of Steam.");
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (vlcMediaPlayerCheck.Checked)
            {
                AppendLine("📌 VLC Media Player is selected.");
                if (System.IO.File.Exists(@"C:\Program Files\VideoLAN\VLC\vlc.exe"))
                {
                    AppendLine("✅ VLC Media Player is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    AppendLine("🔄 Downloading VLC Media Player...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(vlcMediaPlayerURL, vlcMediaPlayerFilename);
                    }
                    AppendLine("📦 Installing VLC Media Player...");
                    await Task.Run(() =>
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "msiexec";
                            process.StartInfo.Arguments = $"/package \"{vlcMediaPlayerFilename}\" /passive";
                            process.StartInfo.UseShellExecute = false;
                            process.StartInfo.RedirectStandardOutput = true;
                            process.StartInfo.RedirectStandardError = true;
                            process.StartInfo.CreateNoWindow = true;
                            try
                            {
                                process.Start();
                                string output = process.StandardOutput.ReadToEnd();
                                string error = process.StandardError.ReadToEnd();
                                process.WaitForExit();
                                Console.WriteLine("Output: " + output);
                                if (!string.IsNullOrEmpty(error))
                                {
                                    Console.WriteLine("Error: " + error);
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine("An error occurred: " + ex.Message);
                            }
                        }
                    });
                    AppendLine("✅ Completed installation of VLC Media Player.");
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (hpEliteBook == "1")
            {
                AppendLine("💻 The installer is being run on an HP EliteBook.");
                AppendLine("🔄 Downloading HP Hotkey Support...");
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += wc_progressBarStep;
                    await wc.DownloadFileTaskAsync(hpHotkeySupportURL, hpHotkeySupportFilename);
                }
                AppendLine("🔄 Checking if NanaZip is installed...");                
                try
                {
                    var files = Directory.GetFiles(windowsAppsPath, nanaZipExe, SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        nanaZipPath = files[0];
                        AppendLine($"✅ NanaZip is already installed.");
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    AppendLine("⚠️ Access denied to WindowsApps. Try running as Administrator.");
                }
                if (string.IsNullOrEmpty(nanaZipPath))
                {
                    AppendLine("⚠️ NanaZip is not installed and is required for extraction.");
                    AppendLine("🔄 Downloading NanaZip...");
                    using (WebClient wc = new WebClient())
                    {
                        await wc.DownloadFileTaskAsync(nanaZipURL, nanaZipFilename);
                    }
                    AppendLine("📦 Installing NanaZip...");
                    Process nanaZipInstallProcess = Process.Start(new ProcessStartInfo
                    {
                        FileName = "powershell",
                        Arguments = $"-Command Add-AppxPackage -Path '{nanaZipFilename}'",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    });
                    if (nanaZipInstallProcess != null)
                    {
                        await Task.Run(() => nanaZipInstallProcess.WaitForExit());
                    }
                    AppendLine("✅ Completed installation of NanaZip.");
                    try
                    {
                        var files = Directory.GetFiles(windowsAppsPath, nanaZipExe, SearchOption.AllDirectories);
                        if (files.Length > 0)
                        {
                            nanaZipPath = files[0];
                            AppendLine($"✅ NanaZip is already installed.");
                        }
                        else
                        {
                            AppendLine("❌ Failed to find NanaZip after installation.");
                            return;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        AppendLine("⚠️ Access denied while searching for NanaZip after installation.");
                        return;
                    }
                }
                AppendLine("📂 Extracting HP Hotkey Support...");
                string hpHotkeySupportExtractPath = @"C:\Computer Repair Centre\apps\hpHotkeySupport";
                async Task RunNanaZipExtractionHPAsync()
                {
                    ProcessStartInfo processStartInfo = new ProcessStartInfo
                    {
                        FileName = nanaZipPath,
                        Arguments = $"x \"{hpHotkeySupportFilename}\" -o\"{hpHotkeySupportExtractPath}\" -aoa",
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    };
                    try
                    {
                        using (Process process = new Process { StartInfo = processStartInfo })
                        {
                            process.Start();
                            Task<string> outputTask = process.StandardOutput.ReadToEndAsync();
                            Task<string> errorTask = process.StandardError.ReadToEndAsync();
                            await Task.Run(() => process.WaitForExit());
                            string output = await outputTask;
                            string errors = await errorTask;
                            if (!string.IsNullOrEmpty(output))
                            {
                                AppendLine(output);
                            }
                            if (!string.IsNullOrEmpty(errors))
                            {
                                AppendLine("❌ Errors: " + errors);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLine("❌ Exception: " + ex.Message);
                    }
                }
                if (!Directory.Exists(hpHotkeySupportExtractPath))
                {
                    Directory.CreateDirectory(hpHotkeySupportExtractPath);
                }
                await RunNanaZipExtractionHPAsync();
                AppendLine("✅ Completed extraction of HP Hotkey Support.");
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                AppendLine("📦 Installing HP Hotkey Support...");
                async Task InstallHPHotkeySupport()
                {
                    await StartProcessAsync(@"C:\Computer Repair Centre\apps\hpHotkeySupport\SP103615\src\install.cmd");
                }
                async Task InstallHPFramework()
                {
                    await StartProcessAsync(@"C:\Computer Repair Centre\SP103615\src\install.cmd");
                }
                async Task StartProcessAsync(string filePath)
                {
                    try
                    {
                        ProcessStartInfo processInfo = new ProcessStartInfo
                        {
                            FileName = filePath,
                            UseShellExecute = false
                        };
                        using (Process process = Process.Start(processInfo))
                        {
                            if (process != null)
                            {
                                await Task.Run(() => process.WaitForExit());
                                AppendLine($"✅ Process completed successfully for {filePath}.");
                            }
                            else
                            {
                                AppendLine($"❌ Failed to start the process: {filePath}.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLine("❌ An error occurred: " + ex.Message);
                    }
                }
                await InstallHPHotkeySupport();
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                AppendLine("📦 Installing HP Framework...");
                await InstallHPFramework();
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                AppendLine("✅ Completed installation of HP Hotkey Support.");
            }
            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                if (key != null)
                {
                    string buildNumber = key.GetValue("CurrentBuild")?.ToString();
                    if (int.TryParse(buildNumber, out int build))
                    {
                        if (build >= 22000)
                        {
                            AppendLine("✅ Disabling device encryption...");
                            const string bitLockerRegPath = @"SYSTEM\CurrentControlSet\Control\BitLocker";
                            const string bitLockerReg = "PreventDeviceEncryption";
                            const int bitLockerRegData = 1;
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(bitLockerRegPath, writable: true))
                            {
                                registryKey.SetValue(bitLockerReg, bitLockerRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{bitLockerReg}' to {bitLockerRegData} in '{bitLockerRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            AppendLine("✅ Disabling fastboot mode...");
                            const string hiberbootRegPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Power";
                            const string hiberbootReg = "HiberbootEnabled";
                            const int hiberbootRegData = 0;
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(hiberbootRegPath, writable: true))
                            {
                                registryKey.SetValue(hiberbootReg, hiberbootRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{hiberbootReg}' to {hiberbootRegData} in '{hiberbootRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            AppendLine("✅ Disabling location tracking...");
                            const string locationRegPath1 = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}";
                            const string locationReg1 = "SensorPermissionState";
                            const int locationRegData1 = 0;
                            const string locationRegPath2 = @"SYSTEM\CurrentControlSet\Services\lfsvc\Service\Configuration";
                            const string locationReg2 = "Status";
                            const int locationRegData2 = 0;
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(locationRegPath1, writable: true))
                            {
                                registryKey.SetValue(locationReg1, locationRegData1, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{locationReg1}' to {locationRegData1} in '{locationRegPath1}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(locationRegPath2, writable: true))
                            {
                                registryKey.SetValue(locationReg2, locationRegData2, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{locationReg2}' to {locationRegData2} in '{locationRegPath2}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            AppendLine("✅ Disabling People icon...");
                            const string peopleRegPath1 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People";
                            const string peopleRegPath2 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People";
                            const string peopleReg2 = "PeopleBand";
                            const int peopleRegData2 = 0;
                            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(peopleRegPath1, writable: true))
                            {
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(peopleRegPath2, writable: true))
                            {
                                registryKey.SetValue(peopleReg2, peopleRegData2, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{peopleReg2}' to {peopleRegData2} in '{peopleRegPath2}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            AppendLine("✅ Hiding recently used files and folders in File Explorer...");
                            const string recentRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer";
                            const string recentReg = "ShowRecent";
                            const int recentRegData = 0;
                            const string frequentReg = "ShowFrequent";
                            const int frequentRegData = 0;
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(recentRegPath, writable: true))
                            {
                                registryKey.SetValue(recentReg, recentRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{recentReg}' to {recentRegData} in '{recentRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(recentRegPath, writable: true))
                            {
                                registryKey.SetValue(frequentReg, frequentRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{frequentReg}' to {frequentRegData} in '{recentRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                        }
                        else if (build >= 19041)
                        {
                            AppendLine("✅ Setting explorer to open to This PC...");
                            const string thisPCRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                            const string thisPCReg = "LaunchTo";
                            const int thisPCRegData = 1;
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(thisPCRegPath, writable: true))
                            {
                                registryKey.SetValue(thisPCReg, thisPCRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{thisPCReg}' to {thisPCRegData} in '{thisPCRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            AppendLine("✅ Disabling fastboot mode...");
                            const string hiberbootRegPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Power";
                            const string hiberbootReg = "HiberbootEnabled";
                            const int hiberbootRegData = 0;
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(hiberbootRegPath, writable: true))
                            {
                                registryKey.SetValue(hiberbootReg, hiberbootRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{hiberbootReg}' to {hiberbootRegData} in '{hiberbootRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            AppendLine("✅ Disabling location tracking...");
                            const string locationRegPath1 = @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}";
                            const string locationReg1 = "SensorPermissionState";
                            const int locationRegData1 = 0;
                            const string locationRegPath2 = @"SYSTEM\CurrentControlSet\Services\lfsvc\Service\Configuration";
                            const string locationReg2 = "Status";
                            const int locationRegData2 = 0;
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(locationRegPath1, writable: true))
                            {
                                registryKey.SetValue(locationReg1, locationRegData1, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{locationReg1}' to {locationRegData1} in '{locationRegPath1}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(locationRegPath2, writable: true))
                            {
                                registryKey.SetValue(locationReg2, locationRegData2, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{locationReg2}' to {locationRegData2} in '{locationRegPath2}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            AppendLine("✅ Disabling People icon...");
                            const string peopleRegPath1 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People";
                            const string peopleRegPath2 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People";
                            const string peopleReg2 = "PeopleBand";
                            const int peopleRegData2 = 0;
                            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(peopleRegPath1, writable: true))
                            {
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(peopleRegPath2, writable: true))
                            {
                                registryKey.SetValue(peopleReg2, peopleRegData2, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{peopleReg2}' to {peopleRegData2} in '{peopleRegPath2}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            AppendLine("✅ Hiding recently used files and folders in File Explorer...");
                            const string recentRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer";
                            const string recentReg = "ShowRecent";
                            const int recentRegData = 0;
                            const string frequentReg = "ShowFrequent";
                            const int frequentRegData = 0;
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(recentRegPath, writable: true))
                            {
                                registryKey.SetValue(recentReg, recentRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{recentReg}' to {recentRegData} in '{recentRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(recentRegPath, writable: true))
                            {
                                registryKey.SetValue(frequentReg, frequentRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{frequentReg}' to {frequentRegData} in '{recentRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                        }
                        else
                        {
                            AppendLine("⬆️ This computer is running an old version of Windows, please update it.");
                        }
                    }
                }
            }
            if (powerCheck.Checked) { }
            else
            {
                AppendLine("✅ Re-enabling sleep and screen timeout on AC power...");
                Process.Start("powercfg", "/change monitor-timeout-ac 10");
                Process.Start("powercfg", "/change standby-timeout-ac 20");
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }
            AppendLine("✅ Cleaning up installation files...");
            var deletionHelper = new FileDeletionHelper();
            await deletionHelper.DeleteFilesAndDirectoryAsync(appsDir, launcherPath);
            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            if (recycleBinCheck.Checked)
            {
                AppendLine("✅ Empty Recycle Bin is checked.");
                AppendLine("🗑️ Emptying Recycle Bin...");
                try
                {
                    SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
                    AppendLine("✅ Recycle Bin emptied successfully.");
                }
                catch (Exception ex)
                {
                    AppendLine($"⚠️ Failed to empty Recycle Bin: {ex.Message}");
                }
            }
            var currentEvent = _themeManager.GetCurrentEvent();
            if (currentEvent != null && currentEvent.PlaySound != null)
            {
                currentEvent.PlaySound();
            }
            else
            {
                AudioEffects.PlayCompleteChime();
            }
            if (restartCheck.Checked)
            {
                Process.Start("shutdown", "/r /t 60");
                AppendLine("🔄 System will restart in 60 seconds. If you need to cancel this press the close button.");
            }
            if (shutdownCheck.Checked)
            {
                Process.Start("shutdown", "/s /t 60");
                AppendLine("⏻ System will shutdown in 60 seconds. If you need to cancel this press the close button.");
            }
            progressBar.Value = Math.Min(progressBar.Maximum, progressBar.Value);
            progressBar.Value = progressBar.Maximum;
            AppendLine("✅ The installation has completed.");
        }
        private void wc_progressBarStep(object sender, AsyncCompletedEventArgs e)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum); });
            }
            else
            {
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }
        }
        private void ShutdownCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (shutdownCheck.Checked)
                restartCheck.Checked = false;
        }
        private void RestartCheck_CheckedChanged(object sender, EventArgs e)
        {
            if (restartCheck.Checked)
                shutdownCheck.Checked = false;
        }
        private async void close_Click(object sender, EventArgs e)
        {
            await Task.Delay(325);
            Process.Start("shutdown", "/a");
            this.Close();
        }
        private async void restart_Click(object sender, EventArgs e)
        {
            await Task.Delay(325);
            Process.Start("shutdown","/r /t 1");
        }
        private async void shutdown_Click(object sender, EventArgs e)
        {
            await Task.Delay(325);
            Process.Start("shutdown", "/s /t 1");
        }
        private async void test_Click(object sender, EventArgs e)
        {
            AppendLine("❌ No current tests. You nosey bastard.");
        }
        private void versionLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start(
                new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "https://professorshroom.com/projects/PlutoPoint_Installer/#changelog",
                    UseShellExecute = true
                }
            );
        }
        private static string ComputeSHA256(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(input));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
        private void AppendLine(string text = "")
        {
            installerTextBox.Text += text + Environment.NewLine;
            installerLogPanel.PerformLayout();
            installerLogPanel.AutoScrollPosition = new Point(0, installerTextBox.Bottom);
        }
        private class PasswordForm : Form
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
}