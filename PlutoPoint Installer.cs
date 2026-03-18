using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using Shell32;
using System.Drawing.Drawing2D;
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
        private bool isClickPlaying = false;
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
        public installerForm()
        {
            InitializeComponent();
            this.Resize += (s, e) =>
            {
                this.Invalidate(true);
            };
            this.AutoScaleMode = AutoScaleMode.Dpi;
            // Font
            this.installerTextBox.Font = Program.Ubuntu(12f, FontStyle.Regular);
            // Sounds
            SoundPlayer hoverSound = new SoundPlayer(Properties.Resources.buttonHover);
            SoundPlayer clickSound = new SoundPlayer(Properties.Resources.buttonHover);
            // Shutdown/restart checks
            shutdownCheck.CheckedChanged += ShutdownCheck_CheckedChanged;
            restartCheck.CheckedChanged += RestartCheck_CheckedChanged;
            // Gradient/transparency
            this.SetStyle(ControlStyles.AllPaintingInWmPaint |
                          ControlStyles.UserPaint |
                          ControlStyles.OptimizedDoubleBuffer, true);
            this.DoubleBuffered = true;
            // Button sounds
            void PlayHover()
            {
                if (isClickPlaying) return;
                hoverSound.Stop();
                hoverSound.Play();
            }
            void PlayClick()
            {
                hoverSound.Stop();
                clickSound.Stop();
                clickSound.Play();
                isClickPlaying = true;
                var t = new System.Timers.Timer(150);
                t.AutoReset = false;
                t.Elapsed += (s, e) =>
                {
                    isClickPlaying = false;
                    t.Dispose();
                };
                t.Start();
            }
            install.MouseEnter += (s, e) => PlayHover();
            install.Click += (s, e) => PlayClick();
            restart.MouseEnter += (s, e) => PlayHover();
            restart.Click += (s, e) => PlayClick();
            close.MouseEnter += (s, e) => PlayHover();
            close.Click += (s, e) => PlayClick();
            test.MouseEnter += (s, e) => PlayHover();
            test.Click += (s, e) => PlayClick();
            // Date checks
            CheckChristmas();
            CheckNewYear();
            CheckHalloween();
            CheckValentines();
            CheckPancake();
            CheckPuffin();
            CheckDachshund();
            CheckPluto();
            CheckRhino();
            CheckHippo();
            CheckDuck();
            CheckCharlieBirthday();
            CheckDeanBirthday();
            CheckSteveBirthday();
            CheckHowardBirthday();
            CheckAdamBirthday();
            CheckGeethBirthday();
            OverrideRoundedBoxColours();
            // Background tasks only
            _initialiseUrlsTask = InitialiseUrlsAsync();
            _checkIpTask = CheckIPAsync();
            CheckEliteBook();
            UpdateGUIEvent();
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
            this.versionLabel.Text = $"Version {version}";
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
                microsoftOffice2007Check.Checked = true;
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
        string christmas = null;
        string newyear = null;
        string halloween = null;
        string valentines = null;
        string birthday = null;
        string pancake = null;
        string puffin = null;
        string duck = null;
        string dachshund = null;
        string pluto = null;
        string hippo = null;
        string rhino = null;
        string birthdayName = null;
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
        private Image _overlayImage;
        private Icon _overlayIcon;
        private int _overlayX;
        private int _overlayY;
        private int _overlayWidth;
        private int _overlayHeight;
        private float _overlayRotationDegrees;
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
            if (christmas == "1")
            {
                AppendLine("");
                var rm = Properties.Resources.ResourceManager;
                var set = rm.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                var songKeys = set.Cast<DictionaryEntry>()
                                  .Where(e => e.Key.ToString().StartsWith("song"))
                                  .Select(e => e.Key.ToString())
                                  .ToList();
                if (songKeys.Count > 0)
                {
                    var rnd = new Random();
                    string chosenKey = songKeys[rnd.Next(songKeys.Count)];
                    var bytes = (byte[])rm.GetObject(chosenKey);
                    using (var reader = new StreamReader(new MemoryStream(bytes)))
                    {
                        string line;
                        while ((line = await reader.ReadLineAsync()) != null)
                        {
                            AppendLine(line);
                            await Task.Delay(500);
                        }
                    }
                }
                AppendLine("");
                AppendLine("🎄 Merry Christmas!");
                AppendLine("");
            }
            else if (newyear == "1")
            {
                AppendLine("");
                AppendLine("🎉 Happy New Year!");
                AppendLine("");
            }
            else if (halloween == "1")
            {
                AppendLine("");
                AppendLine("🎃 Boo! Happy Halloween!");
                AppendLine("");
            }
            else if (valentines == "1")
            {
                AppendLine("");
                AppendLine("❤️ Happy Valentines Day!");
                AppendLine("");
            }
            else if (pancake == "1")
            {
                AppendLine("");
                AppendLine("🥞 It's Pancake Day!");
                AppendLine("Don't forget to have some pancakes you fat bastard!");
                AppendLine("");
            }
            else if (puffin == "1")
            {
                AppendLine("");
                AppendLine("🐧 Today is World Puffin day!");
                AppendLine("");
            }
            else if (duck == "1")
            {
                AppendLine("");
                AppendLine("🦆 Today is National Duck day!");
                AppendLine("Did someone say duck?");
                AppendLine("");
            }
            else if (dachshund == "1")
            {
                AppendLine("");
                AppendLine("🌭 Today is National Dachshund day!");
                AppendLine("");
            }
            else if (hippo == "1")
            {
                AppendLine("");
                AppendLine("🦛 Today is World Hippo day!");
                AppendLine("");
            }
            else if (rhino == "1")
            {
                AppendLine("");
                AppendLine("🦏 Today is World Rhino day!");
                AppendLine("");
            }
            else if (birthday == "1" && !string.IsNullOrEmpty(birthdayName))
            {
                AppendLine("");
                AppendLine($"🎂 It is {birthdayName}'s birthday today!");
                AppendLine($"🎉 Happy birthday {birthdayName}!");
                AppendLine("");
            }
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
        private void CheckChristmas()
        {
            if (DateTime.Now.Month == 12)
            {
                christmas = "1";
                ApplyGradientTheme(
                    Color.FromArgb(18, 110, 58),
                    Color.FromArgb(120, 18, 32));
                ApplyButtonTheme(
                    Color.FromArgb(220, 235, 225),
                    Color.FromArgb(120, 18, 32));
                ApplyLogTheme(Color.White);
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void CheckNewYear()
        {
            if (DateTime.Now.Month == 1 &&
                (DateTime.Now.Day == 1 || DateTime.Now.Day == 2 || DateTime.Now.Day == 3 ||
                 DateTime.Now.Day == 4 || DateTime.Now.Day == 5 || DateTime.Now.Day == 6 ||
                 DateTime.Now.Day == 7))
            {
                newyear = "1";
                ApplyGradientTheme(
                    Color.FromArgb(255, 210, 90),
                    Color.FromArgb(198, 140, 35));
                ApplyButtonTheme(
                    Color.FromArgb(245, 245, 240),
                    Color.FromArgb(110, 80, 20));
                ApplyLogTheme(Color.FromArgb(90, 65, 15));
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void CheckHalloween()
        {
            if (DateTime.Now.Month == 10 &&
                (DateTime.Now.Day == 26 || DateTime.Now.Day == 27 || DateTime.Now.Day == 28 ||
                 DateTime.Now.Day == 29 || DateTime.Now.Day == 30 || DateTime.Now.Day == 31))
            {
                halloween = "1";
                ApplyGradientTheme(
                    Color.FromArgb(35, 35, 35),
                    Color.FromArgb(120, 45, 0));
                ApplyButtonTheme(
                    Color.FromArgb(252, 104, 18),
                    Color.White);
                ApplyLogTheme(Color.White);
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void CheckValentines()
        {
            if (DateTime.Now.Month == 2 && DateTime.Now.Day == 14)
            {
                valentines = "1";
                ApplyGradientTheme(
                    Color.FromArgb(245, 215, 225),
                    Color.FromArgb(214, 150, 175));
                ApplyButtonTheme(
                    Color.FromArgb(160, 24, 60),
                    Color.White);
                ApplyLogTheme(Color.FromArgb(135, 20, 50));
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void CheckPancake()
        {
            if ((DateTime.Now.Month == 2 && DateTime.Now.Day == 17 && DateTime.Now.Year == 2026) ||
                (DateTime.Now.Month == 2 && DateTime.Now.Day == 9 && DateTime.Now.Year == 2027) ||
                (DateTime.Now.Month == 2 && DateTime.Now.Day == 29 && DateTime.Now.Year == 2028))
            {
                pancake = "1";
                ApplyGradientTheme(
                    Color.FromArgb(242, 200, 150),
                    Color.FromArgb(205, 145, 95));
                ApplyButtonTheme(
                    Color.FromArgb(176, 116, 72),
                    Color.FromArgb(255, 245, 225));
                ApplyLogTheme(Color.FromArgb(95, 55, 25));
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void CheckPuffin()
        {
            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 14)
            {
                puffin = "1";
                ApplyGradientTheme(
                    Color.FromArgb(35, 45, 70),
                    Color.FromArgb(90, 125, 160));
                ApplyButtonTheme(
                    Color.FromArgb(245, 245, 245),
                    Color.FromArgb(35, 45, 70));
                ApplyLogTheme(Color.White);
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void CheckDuck()
        {
            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 4)
            {
                duck = "1";
                ApplyGradientTheme(
                    Color.FromArgb(215, 185, 125),
                    Color.FromArgb(150, 120, 78));
                ApplyButtonTheme(
                    Color.FromArgb(76, 94, 64),
                    Color.White);
                ApplyLogTheme(Color.FromArgb(76, 94, 64));
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void CheckDachshund()
        {
            if (DateTime.Now.Month == 6 && DateTime.Now.Day == 21)
            {
                dachshund = "1";
                ApplyGradientTheme(
                    Color.FromArgb(245, 228, 212),
                    Color.FromArgb(205, 170, 138));
                ApplyButtonTheme(
                    Color.FromArgb(145, 95, 62),
                    Color.FromArgb(250, 240, 225));
                ApplyLogTheme(Color.FromArgb(110, 72, 45));
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void CheckPluto()
        {
            if (DateTime.Now.Month == 3 && DateTime.Now.Day == 12)
            {
                pluto = "1";
                ApplyGradientTheme(
                    Color.FromArgb(242, 225, 210),
                    Color.FromArgb(182, 132, 92));
                ApplyButtonTheme(
                    Color.FromArgb(132, 88, 58),
                    Color.FromArgb(248, 238, 228));
                ApplyLogTheme(Color.FromArgb(108, 72, 48));
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void CheckHippo()
        {
            if (DateTime.Now.Month == 2 && DateTime.Now.Day == 15)
            {
                hippo = "1";
                ApplyGradientTheme(
                    Color.FromArgb(98, 98, 105),
                    Color.FromArgb(58, 58, 64));
                ApplyButtonTheme(
                    Color.FromArgb(72, 72, 78),
                    Color.White);
                ApplyLogTheme(Color.White);
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void CheckRhino()
        {
            if (DateTime.Now.Month == 9 && DateTime.Now.Day == 22)
            {
                rhino = "1";
                ApplyGradientTheme(
                    Color.FromArgb(110, 110, 115),
                    Color.FromArgb(62, 62, 68));
                ApplyButtonTheme(
                    Color.FromArgb(74, 74, 82),
                    Color.White);
                ApplyLogTheme(Color.White);
                SyncLabelsWithInstall();
                this.Invalidate();
            }
        }
        private void ApplyBirthdayTheme(string name)
        {
            birthday = "1";
            birthdayName = name;
            ApplyGradientTheme(
                Color.FromArgb(175, 220, 228),
                Color.FromArgb(245, 182, 198));
            ApplyButtonTheme(
                Color.FromArgb(255, 245, 235),
                Color.FromArgb(95, 70, 85));
            ApplyLogTheme(Color.FromArgb(80, 60, 75));
            SyncLabelsWithInstall();
            this.Invalidate();
        }
        private void CheckCharlieBirthday()
        {
            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 6)
                ApplyBirthdayTheme("Charlie");
        }
        private void CheckDeanBirthday()
        {
            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 21)
                ApplyBirthdayTheme("Dean");
        }
        private void CheckSteveBirthday()
        {
            if (DateTime.Now.Month == 6 && DateTime.Now.Day == 24)
                ApplyBirthdayTheme("Steve");
        }
        private void CheckHowardBirthday()
        {
            if (DateTime.Now.Month == 5 && DateTime.Now.Day == 16)
                ApplyBirthdayTheme("Howard");
        }
        private void CheckAdamBirthday()
        {
            if (DateTime.Now.Month == 6 && DateTime.Now.Day == 9)
                ApplyBirthdayTheme("Adam");
        }
        private void CheckGeethBirthday()
        {
            if (DateTime.Now.Month == 7 && DateTime.Now.Day == 25)
                ApplyBirthdayTheme("Geeth");
        }
        private void AdjustInstallerTextBoxSizeForOverlay()
        {
            bool hasOverlayImage = (_overlayImage != null);
            this.installerLogPanel.Size = hasOverlayImage
                ? new System.Drawing.Size(517, 258)
                : new System.Drawing.Size(517, 355);
            installerTextBox.MaximumSize = new Size(installerLogPanel.ClientSize.Width - 10, 0);
        }
        private void UpdateGUIEvent()
        {
            _overlayImage = null;
            _overlayIcon = null;
            _overlayRotationDegrees = 0f;
            _overlayX = 670;
            _overlayY = 320;
            if (christmas == "1")
            {
                _overlayImage = Properties.Resources.christmasTree;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.computerRepairCentreIconChristmas;
            }
            else if (newyear == "1")
            {
                _overlayImage = Properties.Resources.newyear;
                _overlayIcon = null;
            }
            else if (halloween == "1")
            {
                _overlayImage = Properties.Resources.pumpkin;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.computerRepairCentreIconHalloween;
            }
            else if (valentines == "1")
            {
                _overlayImage = Properties.Resources.heart;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.computerRepairCentreIconValentines;
                _overlayRotationDegrees = 30f;
            }
            else if (pancake == "1")
            {
                _overlayImage = Properties.Resources.pancake;
                _overlayIcon = null;
            }
            else if (puffin == "1")
            {
                _overlayImage = Properties.Resources.puffin;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.computerRepairCentreIconPuffin;
            }
            else if (duck == "1")
            {
                _overlayImage = Properties.Resources.duck;
            }
            else if (dachshund == "1")
            {
                _overlayImage = Properties.Resources.pluto;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.plutoLogo;
            }
            else if (pluto == "1")
            {
                _overlayImage = Properties.Resources.pluto;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.plutoLogo;
            }
            else if (hippo == "1")
            {
                _overlayImage = Properties.Resources.hippo;
                _overlayIcon = null;
            }
            else if (rhino == "1")
            {
                _overlayImage = Properties.Resources.rhino;
                _overlayIcon = null;
            }
            else if (birthday == "1")
            {
                _overlayImage = Properties.Resources.present;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.computerRepairCentreIconBirthday;
            }
            if (_overlayIcon != null && this.Icon != _overlayIcon)
                this.Icon = _overlayIcon;
            AdjustInstallerTextBoxSizeForOverlay();
            this.Invalidate();
        }
        private static Rectangle GetScaledRect(Image img, int x, int y, int maxW, int maxH)
        {
            float ratioX = (float)maxW / img.Width;
            float ratioY = (float)maxH / img.Height;
            float ratio = Math.Min(ratioX, ratioY);
            ratio = Math.Min(ratio, 1f);
            int w = (int)Math.Round(img.Width * ratio);
            int h = (int)Math.Round(img.Height * ratio);
            return new Rectangle(x, y, w, h);
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (_overlayImage == null)
                return;
            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            var dest = GetScaledRect(_overlayImage, _overlayX, _overlayY, 100, 100);
            if (_overlayRotationDegrees == 0f)
            {
                e.Graphics.DrawImage(_overlayImage, dest);
                return;
            }
            var state = e.Graphics.Save();
            try
            {
                float cx = dest.X + (dest.Width / 2f);
                float cy = dest.Y + (dest.Height / 2f);
                e.Graphics.TranslateTransform(cx, cy);
                e.Graphics.RotateTransform(_overlayRotationDegrees);
                e.Graphics.TranslateTransform(-cx, -cy);
                e.Graphics.DrawImage(_overlayImage, dest);
            }
            finally
            {
                e.Graphics.Restore(state);
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
                string url = "https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/Resources/json/passwordHash.json";
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
                string url = "https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/Resources/json/internetProtocolHash.json";
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
            public string microsoftOffice2007 { get; set; }
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
                string url = "https://raw.githubusercontent.com/professorshroom/PlutoPoint-Installer/main/Resources/json/downloads.json";
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
        private Uri microsoftOffice2007URL => new Uri(urls.microsoftOffice2007);
        private Uri mozillaFirefoxURL => new Uri(urls.mozillaFirefox);
        private Uri mozillaThunderbirdURL => new Uri(urls.mozillaThunderbird);
        private Uri nanaZipURL => new Uri(urls.nanaZip);
        private Uri steamURL => new Uri(urls.steam);
        private Uri hpHotkeySupportURL => new Uri(urls.hpHotkeySupport);
        private Uri vlcMediaPlayerURL => new Uri(urls.vlcMediaPlayer);
        string crcOEMFilename = @"C:\Computer Repair Centre\oem\computerRepairCentreOEM.bmp";
        string anyDeskFilename = @"C:\Computer Repair Centre\apps\anyDesks.msi";
        string bingWallpapersFilename = @"C:\Computer Repair Centre\apps\bingWallpapers.msi";
        string bitDefenderFilename = @"C:\Computer Repair Centre\apps\bitDefender.exe";
        string discordFilename = @"C:\Computer Repair Centre\apps\discord.exe";
        string googleChromeFilename = @"C:\Computer Repair Centre\apps\googleChrome.msi";
        string libreOfficeFilename = @"C:\Computer Repair Centre\apps\libreOffice.msi";
        string microsoftOffice2007Filename = @"C:\Computer Repair Centre\apps\office2007.zip";
        string mozillaFirefoxFilename = @"C:\Computer Repair Centre\apps\mozillaFirefox.msi";
        string mozillaThunderbirdFilename = @"C:\Computer Repair Centre\apps\mozillaThunderbird.msi";
        string nanaZipFilename = @"C:\Computer Repair Centre\apps\nanaZip.msixbundle";
        string steamFilename = @"C:\Computer Repair Centre\apps\steam.exe";
        string hpHotkeySupportFilename = @"C:\Computer Repair Centre\apps\hpHotkeySupport.zip";
        string vlcMediaPlayerFilename = @"C:\Computer Repair Centre\apps\vlcMediaPlayer.msi";
        string nvidiaAppFilename = @"C:\Computer Repair Centre\apps\nvidiaApp.exe";
        // Sounds unchanged
        private SoundPlayer hoverSound;
        private SoundPlayer clickSound;
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
            string url = "https://www.libreoffice.org/download/download-libreoffice/";
            try
            {
                var request = WebRequest.Create(url);
                using (var response = request.GetResponse())
                using (var stream = response.GetResponseStream())
                using (var reader = new StreamReader(stream))
                {
                    string html = reader.ReadToEnd();
                    int index = html.IndexOf("Our latest stable release", StringComparison.OrdinalIgnoreCase);
                    if (index != -1)
                    {
                        string snippet = html.Substring(index, Math.Min(1000, html.Length - index));
                        var versionMatch = Regex.Match(snippet, @"\b\d+\.\d+\.\d+\b");
                        if (versionMatch.Success)
                        {
                            return versionMatch.Value;
                        }
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
            string rootDir = @"C:\Computer Repair Centre";
            string oemDir = System.IO.Path.Combine(rootDir, "oem");
            string appsDir = System.IO.Path.Combine(rootDir, "apps");
            // Installed apps
            string googleChromeExePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            string mozillaFirefoxExePath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
            string mozillaThunderbirdExePath = @"C:\Program Files\Mozilla Thunderbird\thunderbird.exe";
            // Downloaded installers
            string googleChromeFilename = System.IO.Path.Combine(appsDir, "googleChrome.msi");
            string mozillaFirefoxFilename = System.IO.Path.Combine(appsDir, "mozillaFirefox.msi");
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
            SoundPlayer player;
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
            if (microsoftOffice2007Check.Checked) { progressBar.Maximum += 2; }
            if (mozillaFirefoxCheck.Checked) { progressBar.Maximum += 2; }
            if (mozillaThunderbirdCheck.Checked) { progressBar.Maximum += 2; }
            if (steamCheck.Checked) { progressBar.Maximum += 2; }
            if (aiCheck.Checked) { progressBar.Maximum += 1; }
            if (hpEliteBook == "1") { progressBar.Maximum += 4; }
            if (taskbarCheck.Checked) { progressBar.Maximum += 1;  }
            if (christmas == "1")
            {
                player = new SoundPlayer(Properties.Resources.christmas);
            }
            else if (newyear == "1")
            {
                player = new SoundPlayer(Properties.Resources.newYearFireworks);
            }
            else if (halloween == "1")
            {
                player = new SoundPlayer(Properties.Resources.halloween);
            }
            else if (valentines == "1")
            {
                player = new SoundPlayer(Properties.Resources.valentines);
            }
            else if (birthday == "1")
            {
                player = new SoundPlayer(Properties.Resources.birthday);
            }
            else
            {
                player = new SoundPlayer(Properties.Resources.win98shutdown);
            }
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
                    const string logoRegData = @"C:\Computer Repair Centre\oem\computerRepairCentreOEM.bmp";
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
                    const string logoRegData = @"C:\Computer Repair Centre\oem\computerRepairCentreOEM.bmp";
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
                    const string logoRegData = @"C:\Computer Repair Centre\oem\computerRepairCentreOEM.bmp";
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
                string nanaZipExe = "NanaZip.Windows.exe";
                string windowsAppsPath = @"C:\Program Files\WindowsApps";
                string nanaZipPath = null;
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
            if (aiCheck.Checked)
            {
                if (windows11 == "1")
                {
                    AppendLine("📌 Remove Windows AI is selected.");
                    AppendLine("🔄 Removing Windows AI... (this can take a few minutes)");
                    await Task.Run(() =>
                    {
                        var psiAI = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            Arguments =
                                "-NoLogo -NoProfile -WindowStyle Hidden -NonInteractive " +
                                "& ([scriptblock]::Create((irm \"https://raw.githubusercontent.com/zoicware/RemoveWindowsAI/main/RemoveWindowsAi.ps1\"))) " +
                                "-nonInteractive -Options DisableRegKeys,PreventAIPackageReinstall,DisableCopilotPolicies,RemoveRecallFeature,RemoveCBSPackages,HideAIComponents,DisableRewrite,RemoveRecallTasks",
                            RedirectStandardOutput = false,
                            RedirectStandardError = false,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        AppendLine("🔄 Removing Copilot");
                        using (var proc = Process.Start(psiAI))
                        {
                            proc.WaitForExit();
                        }
                        var psiCopilot = new ProcessStartInfo
                        {
                            FileName = "powershell.exe",
                            Arguments =
                                "-NoLogo -NoProfile -WindowStyle Hidden -NonInteractive " +
                                "Get-AppxPackage -AllUsers *Copilot* | Remove-AppxPackage -ErrorAction SilentlyContinue",
                            RedirectStandardOutput = false,
                            RedirectStandardError = false,
                            UseShellExecute = false,
                            CreateNoWindow = true
                        };
                        using (var proc = Process.Start(psiCopilot))
                        {
                            proc.WaitForExit();
                        }
                    });
                    AppendLine("✅ Completed removal of Windows AI and Copilot.");
                }
                else
                {
                    AppendLine("❌ Not running on Windows 11; skipping removal of Windows AI.");
                }
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
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
                            process.StartInfo.Arguments = $"/package \"{libreOfficeFilename}\" /passive";
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
            if (microsoftOffice2007Check.Checked)
            {
                AppendLine("📌 Microsoft Office 2007 is selected.");
                string officePath = @"C:\Program Files (x86)\Microsoft Office\Office12\WINWORD.EXE";
                string windowsAppsPath = @"C:\Program Files\WindowsApps";
                string nanaZipExe = "NanaZip.Windows.exe";
                string nanaZipPath = null;
                if (File.Exists(officePath))
                {
                    AppendLine("✅ Microsoft Office 2007 is already installed, skipping installation.");
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    AppendLine("🔄 Downloading Microsoft Office 2007...");
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(microsoftOffice2007URL, microsoftOffice2007Filename);
                    }
                    AppendLine("🔎 Checking if NanaZip is installed...");
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
                        AppendLine("🚀 NanaZip is not installed and is required for extraction.");
                        AppendLine("📥 Downloading NanaZip...");
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
                        AppendLine("✅ NanaZip installation completed.");
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
                    string microsoftOffice2007ExtractPath = Path.Combine(desktopPath, "Microsoft Office 2007");
                    if (!Directory.Exists(microsoftOffice2007ExtractPath))
                    {
                        Directory.CreateDirectory(microsoftOffice2007ExtractPath);
                    }
                    AppendLine("📂 Extracting Microsoft Office 2007 to Desktop...");
                    async Task RunNanaZipExtractionOfficeAsync()
                    {
                        ProcessStartInfo processStartInfo = new ProcessStartInfo
                        {
                            FileName = nanaZipPath,
                            Arguments = $"x \"{microsoftOffice2007Filename}\" -o\"{microsoftOffice2007ExtractPath}\" -aoa",
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
                                    AppendLine("⚠️ Errors: " + errors);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            AppendLine("❌ Exception: " + ex.Message);
                        }
                    }
                    await RunNanaZipExtractionOfficeAsync();
                    AppendLine("✅ Completed extraction of Microsoft Office 2007.");
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
                string windowsAppsPath = @"C:\Program Files\WindowsApps";
                string nanaZipExe = "NanaZip.Windows.exe";
                string nanaZipPath = null;
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
            player.Play();
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
            AppendLine("Setting safe location to 0.");
            safeLocation = "0";
            //AppendLine("❌ No current tests. You nosey bastard.");
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
                    Text = "The installer is not being run from a safe location, please enter password to continue.",
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