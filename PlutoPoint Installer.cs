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
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);
        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;
        public installerForm()
        {
            InitializeComponent();
            this.installerTextBox.Font = Program.Ubuntu(12f, FontStyle.Regular);
            SoundPlayer hoverSound = new SoundPlayer(Properties.Resources.buttonHover);
            SoundPlayer clickSound = new SoundPlayer(Properties.Resources.buttonHover);
            shutdownCheck.CheckedChanged += ShutdownCheck_CheckedChanged;
            restartCheck.CheckedChanged += RestartCheck_CheckedChanged;
            this.DoubleBuffered = true;
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
            shutdown.MouseEnter += (s, e) => PlayHover();
            shutdown.Click += (s, e) => PlayClick();
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
            CheckCharlieBirthday();
            CheckDeanBirthday();
            CheckSteveBirthday();
            CheckHowardBirthday();
            CheckAdamBirthday();
            CheckGeethBirthday();
            OverrideRoundedBoxColours();
            CheckIP();
            CheckEliteBook();
            UpdateOverlayFromFlags();
            // Info checks
            PrintVersion();
            PrintDay();
            CheckWindowsVersion();
            CheckForIntelHardware();
            CheckforAMDHardware();
            CheckForNvidiaGPU();
            GetLibreOfficeVersion();
            AppendLocation();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            this.versionLabel.Text = $"Version {version}";
        }
        // Set strings
        string christmas = null;
        string newyear = null;
        string halloween = null;
        string valentines = null;
        string birthday = null;
        string pancake = null;
        string puffin = null;
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
        private const string charliePasswordHash = "61a8b0026371a90d41b114644694485ecdaf999473977a125d028e39cb6d77b2";
        private const string CRCPasswordHash = "1c98fa014f3400abee047920e535036a74661fa0c88f34d24ebed7866a1fc630";
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
        private void PrintDay()
        {
            if (christmas == "1")
            {
                AppendLine("");
                var rm = Properties.Resources.ResourceManager;
                var set = rm.GetResourceSet(CultureInfo.CurrentUICulture, true, true);
                // Find all resources starting with "song"
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
                        while ((line = reader.ReadLine()) != null)
                            AppendLine(line);
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
                AppendLine("❤️ Happy Valentines day!");
                AppendLine("");
            }
            else if (pancake == "1")
            {
                AppendLine("");
                AppendLine("🥞 It's pancake day!");
                AppendLine("🥞 Don't forget to have some pancakes you fat bastard!");
                AppendLine("");
            }
            else if (puffin == "1")
            {
                AppendLine("");
                AppendLine("🐧 Today is world Puffin day!");
                AppendLine("");
            }
            else if (dachshund == "1")
            {
                AppendLine("");
                AppendLine("🌭 Today is world Dachshund day!");
                AppendLine("");
            }
            else if (hippo == "1")
            {
                AppendLine("");
                AppendLine("🦛 Today is world Hippo day!");
                AppendLine("");
            }
            else if (rhino == "1")
            {
                AppendLine("");
                AppendLine("🦏 Today is world Rhino day!");
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
        private void CheckChristmas()
        {
            if (DateTime.Now.Month == 12)
            {
                christmas = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(130)))), ((int)(((byte)(60)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(0)))), ((int)(((byte)(28)))));
                install.ForeColor = System.Drawing.Color.White;
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(0)))), ((int)(((byte)(28)))));
                close.ForeColor = System.Drawing.Color.White;
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(0)))), ((int)(((byte)(28)))));
                restart.ForeColor = System.Drawing.Color.White;
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(0)))), ((int)(((byte)(28)))));
                shutdown.ForeColor = System.Drawing.Color.White;
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(135)))), ((int)(((byte)(62)))));
                this.Invalidate();
            }
        }
        private void CheckNewYear()
        {
            if (DateTime.Now.Month == 1 && (DateTime.Now.Day == 1 || DateTime.Now.Day == 2 || DateTime.Now.Day == 3 || DateTime.Now.Day == 4 || DateTime.Now.Day == 5 || DateTime.Now.Day == 6 || DateTime.Now.Day == 7))
            {
                newyear = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(183)))), ((int)(((byte)(58)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(237)))), ((int)(((byte)(231)))));
                install.ForeColor = System.Drawing.Color.Black;
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(237)))), ((int)(((byte)(231)))));
                close.ForeColor = System.Drawing.Color.Black;
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(237)))), ((int)(((byte)(231)))));
                restart.ForeColor = System.Drawing.Color.Black;
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(232)))), ((int)(((byte)(237)))), ((int)(((byte)(231)))));
                shutdown.ForeColor = System.Drawing.Color.Black;
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(247)))), ((int)(((byte)(183)))), ((int)(((byte)(58)))));
                this.Invalidate();
            }
        }
        private void CheckHalloween()
        {
            if (DateTime.Now.Month == 10 && (DateTime.Now.Day == 26 || DateTime.Now.Day == 27 || DateTime.Now.Day == 28 || DateTime.Now.Day == 29 || DateTime.Now.Day == 30 || DateTime.Now.Day == 31))
            {
                halloween = "1";
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(76)))), ((int)(((byte)(2)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(76)))), ((int)(((byte)(2)))));
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(76)))), ((int)(((byte)(2)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(76)))), ((int)(((byte)(2)))));
                this.Invalidate();
            }
        }
        private void CheckValentines()
        {
            if (DateTime.Now.Month == 2 && DateTime.Now.Day == 14)
            {
                valentines = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(200)))), ((int)(((byte)(213)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(0)))), ((int)(((byte)(28)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(0)))), ((int)(((byte)(28)))));
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(0)))), ((int)(((byte)(28)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(0)))), ((int)(((byte)(28)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(231)))), ((int)(((byte)(200)))), ((int)(((byte)(213)))));
                installerTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(160)))), ((int)(((byte)(0)))), ((int)(((byte)(28)))));
                this.Invalidate();
            }
        }
        private void CheckPancake()
        {
            if (DateTime.Now.Month == 3 && DateTime.Now.Day == 4 && DateTime.Now.Year == 2025 || DateTime.Now.Month == 2 && DateTime.Now.Day == 17 && DateTime.Now.Year == 2026 || DateTime.Now.Month == 2 && DateTime.Now.Day == 9 && DateTime.Now.Year == 2027 || DateTime.Now.Month == 2 && DateTime.Now.Day == 29 && DateTime.Now.Year == 2028)
            {
                pancake = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(183)))), ((int)(((byte)(139)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(154)))), ((int)(((byte)(108)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(154)))), ((int)(((byte)(108)))));
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(154)))), ((int)(((byte)(108)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(214)))), ((int)(((byte)(154)))), ((int)(((byte)(108)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(183)))), ((int)(((byte)(139)))));
                installerTextBox.ForeColor = Color.Black;
                this.Invalidate();
            }
        }
        private void CheckPuffin()
        {
            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 14)
            {
                puffin = "1";
                install.BackColor = System.Drawing.Color.White;
                install.ForeColor = System.Drawing.Color.Black;
                restart.BackColor = System.Drawing.Color.White;
                restart.ForeColor = System.Drawing.Color.Black;
                shutdown.BackColor = System.Drawing.Color.White;
                shutdown.ForeColor = System.Drawing.Color.Black;
                close.BackColor = System.Drawing.Color.White;
                close.ForeColor = System.Drawing.Color.Black;
                this.Invalidate();
            }
        }
        private void CheckDachshund()
        {
            if (DateTime.Now.Month == 6 && DateTime.Now.Day == 21)
            {
                dachshund = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                install.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                restart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                shutdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));   
                close.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                installerTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                versionLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                this.Invalidate();
            }
        }

        private void CheckPluto()
        {
            if (DateTime.Now.Month == 3 && DateTime.Now.Day == 12)
            {
                pluto = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                install.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                restart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                shutdown.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                close.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                installerTextBox.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                versionLabel.LinkColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                this.Invalidate();
            }
        }

        private void CheckHippo()
        {
            if (DateTime.Now.Month == 2 && DateTime.Now.Day == 15)
            {
                hippo = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                install.ForeColor = System.Drawing.Color.White;
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                restart.ForeColor = System.Drawing.Color.White;
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                shutdown.ForeColor = System.Drawing.Color.White;
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                close.ForeColor = System.Drawing.Color.White;
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
                installerTextBox.ForeColor = System.Drawing.Color.White;
                versionLabel.LinkColor = System.Drawing.Color.White;
                this.Invalidate();
            }
        }
        private void CheckRhino()
        {
            if (DateTime.Now.Month == 9 && DateTime.Now.Day == 22)
            {
                rhino = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                install.ForeColor = System.Drawing.Color.White;
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                restart.ForeColor = System.Drawing.Color.White;
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                shutdown.ForeColor = System.Drawing.Color.White;
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                close.ForeColor = System.Drawing.Color.White;
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(83)))), ((int)(((byte)(83)))), ((int)(((byte)(83)))));
                installerTextBox.ForeColor = System.Drawing.Color.White;
                versionLabel.LinkColor = System.Drawing.Color.White;
                this.Invalidate();
            }
        }
        private void CheckCharlieBirthday()
        {
            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 6)
            {
                birthday = "1";
                birthdayName = "Charlie";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.ForeColor = System.Drawing.Color.Black;
                this.Invalidate();
            }
        }
        private void CheckDeanBirthday()
        {
            if (DateTime.Now.Month == 4 && DateTime.Now.Day == 21)
            {
                birthday = "1";
                birthdayName = "Dean";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.ForeColor = System.Drawing.Color.Black;
                this.Invalidate();
            }
        }
        private void CheckSteveBirthday()
        {
            if (DateTime.Now.Month == 6 && DateTime.Now.Day == 24)
            {
                birthday = "1";
                birthdayName = "Steve";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.ForeColor = System.Drawing.Color.Black;
                this.Invalidate();
            }
        }
        private void CheckHowardBirthday()
        {
            if (DateTime.Now.Month == 5 && DateTime.Now.Day == 16)
            {
                birthday = "1";
                birthdayName = "Howard";
                this.BackColor = System.Drawing.Color.FromArgb(155, 211, 221);
                install.BackColor = System.Drawing.Color.FromArgb(242, 172, 185);
                restart.BackColor = System.Drawing.Color.FromArgb(242, 172, 185);
                shutdown.BackColor = System.Drawing.Color.FromArgb(242, 172, 185);
                close.BackColor = System.Drawing.Color.FromArgb(242, 172, 185);
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(242, 172, 185);
                installerTextBox.ForeColor = System.Drawing.Color.Black;

                this.Invalidate();
            }
        }
        private void CheckAdamBirthday()
        {
            if (DateTime.Now.Month == 6 && DateTime.Now.Day == 9)
            {
                birthday = "1";
                birthdayName = "Adam";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.ForeColor = System.Drawing.Color.Black;
                this.Invalidate();
            }
        }
        private void CheckGeethBirthday()
        {
            if (DateTime.Now.Month == 7 && DateTime.Now.Day == 25)
            {
                birthday = "1";
                birthdayName = "Geeth";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                shutdown.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.ForeColor = System.Drawing.Color.Black;
                this.Invalidate();
            }
        }
        private void UpdateOverlayFromFlags()
        {
            _overlayImage = null;
            _overlayIcon = null;
            _overlayRotationDegrees = 0f;

            _overlayX = 160;
            _overlayY = 320;
            _overlayWidth = 100;
            _overlayHeight = 100;
            if (christmas == "1")
            {
                _overlayImage = Properties.Resources.christmasTree;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.computerRepairCentreIconChristmas;
                _overlayX = 160; _overlayY = 320; _overlayWidth = 100; _overlayHeight = 100;
            }
            else if (newyear == "1")
            {
                _overlayImage = Properties.Resources.newyear;
                _overlayIcon = null;
                _overlayX = 160; _overlayY = 320; _overlayWidth = 100; _overlayHeight = 100;
            }
            else if (halloween == "1")
            {
                _overlayImage = Properties.Resources.pumpkin;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.computerRepairCentreIconHalloween;
                _overlayX = 160; _overlayY = 320; _overlayWidth = 100; _overlayHeight = 100;
            }
            else if (valentines == "1")
            {
                _overlayImage = Properties.Resources.heart;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.computerRepairCentreIconValentines;
                _overlayRotationDegrees = 30f;
                _overlayX = 160; _overlayY = 320; _overlayWidth = 100; _overlayHeight = 100;
            }
            else if (pancake == "1")
            {
                _overlayImage = Properties.Resources.pancake;
                _overlayIcon = null;
                _overlayX = 160; _overlayY = 320; _overlayWidth = 100; _overlayHeight = 100;
            }
            else if (puffin == "1")
            {
                _overlayImage = Properties.Resources.puffin;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.computerRepairCentreIconPuffin;
                _overlayX = 320; _overlayY = 320; _overlayWidth = 100; _overlayHeight = 100;
            }
            else if (dachshund == "1")
            {
                _overlayImage = Properties.Resources.pluto;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.plutoLogo;
                _overlayX = 140; _overlayY = 320; _overlayWidth = 130; _overlayHeight = 100;
            }
            else if (pluto == "1")
            {
                _overlayImage = Properties.Resources.pluto;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.plutoLogo;
                _overlayX = 140; _overlayY = 320; _overlayWidth = 130; _overlayHeight = 100;
            }
            else if (hippo == "1")
            {
                _overlayImage = Properties.Resources.hippo;
                _overlayIcon = null;
                _overlayX = 140; _overlayY = 320; _overlayWidth = 130; _overlayHeight = 100;
            }
            else if (rhino == "1")
            {
                _overlayImage = Properties.Resources.rhino;
                _overlayIcon = null;
                _overlayX = 140; _overlayY = 320; _overlayWidth = 120; _overlayHeight = 100;
            }
            else if (birthday == "1")
            {
                _overlayImage = Properties.Resources.present;
                _overlayIcon = PlutoPoint_Installer.Properties.Resources.computerRepairCentreIconBirthday;
                _overlayX = 160; _overlayY = 320; _overlayWidth = 100; _overlayHeight = 100;
            }
            if (_overlayIcon != null && this.Icon != _overlayIcon)
                this.Icon = _overlayIcon;
            this.Invalidate();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);

            if (_overlayImage == null)
                return;

            e.Graphics.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            e.Graphics.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBilinear;
            e.Graphics.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;

            var dest = new Rectangle(_overlayX, _overlayY, _overlayWidth, _overlayHeight);

            if (_overlayRotationDegrees == 0f)
            {
                e.Graphics.DrawImage(_overlayImage, dest);
                return;
            }
            var state = e.Graphics.Save();
            try
            {
                float cx = _overlayX + (_overlayWidth / 2f);
                float cy = _overlayY + (_overlayHeight / 2f);

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
            roundedGroupBox1.BorderColorOverride = versionLabel.LinkColor;
            roundedGroupBox1.TextColorOverride = versionLabel.LinkColor;
            roundedGroupBox2.BorderColorOverride = versionLabel.LinkColor;
            roundedGroupBox2.TextColorOverride = versionLabel.LinkColor;
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
        public class LocationHashes
        {
            public string romsey { get; set; }
            public string chandlersFord { get; set; }
            public string highcliffe { get; set; }
            public string charlieHome { get; set; }
        }
        private LocationHashes GetLocationHashes()
        {
            try
            {
                string url = "https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/Resources/json/internetProtocolHash.json";

                using (var webClient = new WebClient())
                {
                    string json = webClient.DownloadString(url);
                    var serializer = new JavaScriptSerializer();
                    return serializer.Deserialize<LocationHashes>(json);
                }
            }
            catch
            {
                return null;
            }
        }
        private void CheckIP()
        {
            string publicIP = GetPublicIPAddress();
            if (string.IsNullOrWhiteSpace(publicIP))
                return;

            string publicIPHash = HashIP(publicIP);

            LocationHashes hashes = GetLocationHashes();
            if (hashes == null)
                return;

            safeLocation = "0";

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

            UpdateLocation();
        }
        private void AppendLocation()
        {
            if (romsey == "1") { AppendLine("📍 The installer is being run from the Romsey shop."); }
            else if (chandlersFord == "1") { AppendLine("📍 The installer is being run from the Chandlers Ford shop."); }
            else if (highcliffe == "1") { AppendLine("📍 The installer is being run from the Highcliffe shop."); }
            else if (charlieHome == "1") { AppendLine("📍 The installer is being run from Charlie's house."); }
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
        Uri crcOEMURL = new Uri("https://raw.githubusercontent.com/professorshroom/PlutoPoint-Installer/refs/heads/main/Resources/computerRepairCentre/computerRepairCentreOEM.bmp");
        string crcOEMFilename = @"C:\Computer Repair Centre\oem\computerRepairCentreOEM.bmp";
        Uri anyDeskURL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/anyDesk.msi");
        string anyDeskFilename = @"C:\Computer Repair Centre\apps\anyDesks.msi";
        Uri bingWallpapersURL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/bingWallpapers.msi");
        string bingWallpapersFilename = @"C:\Computer Repair Centre\apps\bingWallpapers.msi";
        Uri bitDefenderURL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/bitDefender.exe");
        string bitDefenderFilename = @"C:\Computer Repair Centre\apps\bitDefender.exe";
        Uri discordURL = new Uri("https://discord.com/api/download?platform=win");
        string discordFilename = @"C:\Computer Repair Centre\apps\discord.exe";
        Uri googleChromeURL = new Uri("https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463c-AFF1-A69D9E530F96%7D&iid=&lang=en&browser=4&usagestats=0&appname=Google%2520Chrome%2520Enterprise&needsadmin=false/edgedl/chrome/install/GoogleChromeStandaloneEnterprise64.msi");
        string googleChromeFilename = @"C:\Computer Repair Centre\apps\googleChrome.msi";
        string libreOfficeFilename = @"C:\Computer Repair Centre\apps\libreOffice.msi";
        Uri microsoftOffice2007URL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/office2007.zip");
        string microsoftOffice2007Filename = @"C:\Computer Repair Centre\apps\office2007.zip";
        Uri mozillaFirefoxURL = new Uri("https://download.mozilla.org/?product=firefox-msi-latest-ssl&os=win64&lang=en-GB");
        string mozillaFirefoxFilename = @"C:\Computer Repair Centre\apps\mozillaFirefox.msi";
        Uri mozillaThunderbirdURL = new Uri("https://download.mozilla.org/?product=thunderbird-msi-latest-ssl&os=win64&lang=en-GB");
        string mozillaThunderbirdFilename = @"C:\Computer Repair Centre\apps\mozillaThunderbird.msi";
        Uri nanaZipURL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/nanaZip.msixbundle");
        string nanaZipFilename = @"C:\Computer Repair Centre\apps\nanaZip.msixbundle";
        Uri steamURL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/steam.exe");
        string steamFilename = @"C:\Computer Repair Centre\apps\steam.exe";
        Uri hpHotkeySupportURL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/HPHotkey.zip");
        string hpHotkeySupportFilename = @"C:\Computer Repair Centre\apps\hpHotkeySupport.zip";
        Uri vlcMediaPlayerURL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/vlcMediaPlayer.msi");
        string sysPinFilename = @"C:\Computer Repair Centre\apps\sysPin.exe";
        Uri sysPinURL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/syspin.exe");
        string vlcMediaPlayerFilename = @"C:\Computer Repair Centre\apps\vlcMediaPlayer.msi";
        string nvidiaAppFilename = @"C:\Computer Repair Centre\apps\nvidiaApp.exe";
        private SoundPlayer hoverSound;
        private SoundPlayer clickSound;

        public bool IsClickPlaying { get; private set; }

        private static async Task<string> GetPublicIPAddressAsync()
        {
            using (HttpClient client = new HttpClient())
            {
                try
                {
                    HttpResponseMessage response = await client.GetAsync("https://api.ipify.org");
                    response.EnsureSuccessStatusCode();
                    return await response.Content.ReadAsStringAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Error fetching public IP: " + ex.Message);
                    return null;
                }
            }
        }
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
                using (PasswordForm pf = new PasswordForm())
                {
                    if (pf.ShowDialog() == DialogResult.OK)
                    {
                        string enteredHash = ComputeSHA256(pf.EnteredPassword);

                        if (enteredHash != charliePasswordHash && enteredHash != CRCPasswordHash)
                        {
                            MessageBox.Show("Incorrect password. Exiting.");
                            this.Close();
                        }
                    }
                    else
                    {
                        MessageBox.Show("Password required. Exiting installer.");
                        Environment.Exit(0);
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
            string googleChromeShortcut = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Google Chrome.lnk";
            string mozillaFirefoxExePath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
            string mozillaFirefoxShortcut = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Firefox.lnk";
            string mozillaThunderbirdExePath = @"C:\Program Files\Mozilla Thunderbird\thunderbird.exe";
            string mozillaThunderbirdShortcut = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Thunderbird.lnk";
            string microsoftEdgeShortcut = @"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\Microsoft Edge.lnk";
            // Installer tools
            string sysPinFilename = System.IO.Path.Combine(appsDir, "sysPin.exe");
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
            if (windows11 == "1") { progressBar.Maximum += 8; }
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
            else if (pancake == "1")
            {
                player = new SoundPlayer(Properties.Resources.win98shutdown);
            }
            else if (puffin == "1")
            {
                player = new SoundPlayer(Properties.Resources.win98shutdown);
            }
            else if (dachshund == "1")
            {
                player = new SoundPlayer(Properties.Resources.win98shutdown);
            }
            else if (hippo == "1")
            {
                player = new SoundPlayer(Properties.Resources.win98shutdown);
            }
            else if (rhino == "1")
            {
                player = new SoundPlayer(Properties.Resources.win98shutdown);
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
            if (googleChromeCheck.Checked || mozillaFirefoxCheck.Checked)
            {
                AppendLine("🔄 Preparing taskbar pinning...");
                AppendLine("🔄 Downloading SysPin...");
                try
                {
                    using (WebClient wc = new WebClient())
                    {
                        await wc.DownloadFileTaskAsync(sysPinURL, sysPinFilename);
                    }
                    AppendLine("✅ SysPin downloaded.");
                }
                catch (Exception ex)
                {
                    AppendLine("❌ Failed to download SysPin: " + ex.Message);
                }
                try
                {
                    Process.Start(sysPinFilename, $"\"{microsoftEdgeShortcut}\" 5387");
                    AppendLine("📌 Microsoft Edge unpinned from taskbar.");
                }
                catch (Exception ex)
                {
                    AppendLine("⚠ Could not unpin Microsoft Edge: " + ex.Message);
                }

                AppendLine("✅ Taskbar pinning prep completed.");
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
                if (File.Exists(googleChromeShortcut))
                {
                    try
                    {
                        Process.Start(sysPinFilename, $"\"{googleChromeShortcut}\" 5386");
                        AppendLine("📌 Google Chrome pinned to taskbar.");
                    }
                    catch (Exception ex)
                    {
                        AppendLine("⚠ Could not pin Chrome: " + ex.Message);
                    }
                }
                else
                {
                    AppendLine("⚠ Chrome shortcut not found, cannot pin to taskbar.");
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
                if (File.Exists(mozillaFirefoxShortcut))
                {
                    try
                    {
                        Process.Start(sysPinFilename, $"\"{mozillaFirefoxShortcut}\" 5386");
                        AppendLine("📌 Mozilla Firefox pinned to taskbar.");
                    }
                    catch (Exception ex)
                    {
                        AppendLine("⚠ Could not pin Firefox: " + ex.Message);
                    }
                }
                else
                {
                    AppendLine("⚠ Firefox shortcut not found, cannot pin to taskbar.");
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

                if (File.Exists(mozillaThunderbirdShortcut))
                {
                    try
                    {
                        Process.Start(sysPinFilename, $"\"{mozillaThunderbirdShortcut}\" 5386");
                        AppendLine("📌 Mozilla Thunderbird pinned to taskbar.");
                    }
                    catch (Exception ex)
                    {
                        AppendLine("⚠ Could not pin Thunderbird: " + ex.Message);
                    }
                }
                else
                {
                    AppendLine("⚠ Thunderbird shortcut not found, cannot pin to taskbar.");
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
                            AppendLine("✅ Aligning the taskbar to the left...");
                            const string taskbarRegPath = @"SOFTWARE\microsoft\windows\currentversion\explorer\advanced";
                            const string taskbarReg = "TaskbarAl";
                            const int taskbarRegData = 0;
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(taskbarRegPath, writable: true))
                            {
                                registryKey.SetValue(taskbarReg, taskbarRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{taskbarReg}' to {taskbarRegData} in '{taskbarRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
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
            AppendLine("❌ No current tests.");
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
            installerTextBox.SelectionStart = installerTextBox.TextLength;
            installerTextBox.SelectionLength = 0;
            installerTextBox.SelectionFont = Program.Ubuntu(12f, FontStyle.Regular);
            installerTextBox.AppendText(text + Environment.NewLine);
            installerTextBox.ScrollToCaret();
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