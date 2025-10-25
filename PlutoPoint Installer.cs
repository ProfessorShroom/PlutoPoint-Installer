using Microsoft.Win32;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.IO;
using System.Management;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Security.Policy;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using System.Xml.Linq;
using static System.Net.WebRequestMethods;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;

// Copyright © Charlie Howard 2025 All rights reserved.

namespace PlutoPoint_Installer
{

    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Drawing.Text;
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
            SoundPlayer hoverSound = new SoundPlayer(Properties.Resources.buttonHover);
            SoundPlayer clickSound = new SoundPlayer(Properties.Resources.buttonHover);
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
            // Date checks
            CheckChristmas();
            CheckHalloween();
            CheckValentines();
            CheckPancake();
            CheckPuffin();
            CheckDachshund();
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
            // Info checks
            PrintVersion();
            PrintDay();
            CheckWindowsVersion();
            CheckForIntelHardware();
            CheckforAMDHardware();
            CheckForNvidiaGPU();
            GetLibreOfficeVersion();
            Version version = Assembly.GetExecutingAssembly().GetName().Version;
            this.versionLabel.Text = $"Version {version}";
        }
        // Set strings
        string christmas = null;
        string halloween = null;
        string valentines = null;
        string birthday = null;
        string pancake = null;
        string puffin = null;
        string dachshund = null;
        string hippo = null;
        string rhino = null;
        string charlieBirthday = null;
        string deanBirthday = null;
        string steveBirthday = null;
        string howardBirthday = null;
        string adamBirthday = null;
        string geethBirthday = null;
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
        private const string charliePasswordHash = "61a8b0026371a90d41b114644694485ecdaf999473977a125d028e39cb6d77b2";
        private const string CRCPasswordHash = "1c98fa014f3400abee047920e535036a74661fa0c88f34d24ebed7866a1fc630";
        string romseyHash = "aebeec856af3585448c3d5cc72dc93f29d56fa7191027a35c345eba670c533b3";
        string chandlersFordHash = "668cc649b9638504fe7d36a29637e740d44bd8ec2d8839e156c22b8f7a155b43";
        string highcliffeHash = "a9c9ca550056bb3e3062acf0327f99f0e2959ad2421a5745687a49140aa9c4bc";
        string charlieHomeHash = "67177374995543edf423de86cd086b9c6fad3ec80cdbb6d18de5a8c72e048199";
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
            installerTextBox.AppendText($"Version {version}");
            installerTextBox.AppendText(Environment.NewLine);
            installerTextBox.AppendText("Last updated on " + formatted + ".");
            installerTextBox.AppendText(Environment.NewLine);
        }
        private void PrintDay()
        {
            if (christmas == "1")
            {
                installerTextBox.AppendText("Merry Christmas!");
                installerTextBox.AppendText(Environment.NewLine);
            }
            else if (halloween == "1")
            {
                installerTextBox.AppendText("Boo! Happy Halloween!");
                installerTextBox.AppendText(Environment.NewLine);
            }
            else if (valentines == "1")
            {
                installerTextBox.AppendText("Happy Valentines day!");
                installerTextBox.AppendText(Environment.NewLine);
            }
            else if (pancake == "1")
            {
                installerTextBox.AppendText("It's pancake day!");
                installerTextBox.AppendText(Environment.NewLine);
                installerTextBox.AppendText("Don't forget to eat pancakes you fat bastard!");
                installerTextBox.AppendText(Environment.NewLine);
            }
            else if (puffin == "1")
            {
                installerTextBox.AppendText("Today is world Puffin day!");
                installerTextBox.AppendText(Environment.NewLine);
            }
            else if (dachshund == "1")
            {
                installerTextBox.AppendText("Today is world Dachshund day!");
                installerTextBox.AppendText(Environment.NewLine);
            }
            else if (hippo == "1")
            {
                installerTextBox.AppendText("Today is world Hippo day!");
                installerTextBox.AppendText(Environment.NewLine);
            }
            else if (rhino == "1")
            {
                installerTextBox.AppendText("Today is world Rhino day!");
                installerTextBox.AppendText(Environment.NewLine);
            }
            else if (birthday == "1")
            {
                if (charlieBirthday == "1")
                {
                    installerTextBox.AppendText("It is Charlie's birthday today!");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("Happy birthday Charlie!");
                    installerTextBox.AppendText(Environment.NewLine);
                }
                else if (deanBirthday == "1")
                {
                    installerTextBox.AppendText("It is Dean's birthday today!");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("Happy birthday Dean!");
                    installerTextBox.AppendText(Environment.NewLine);
                }
                else if (steveBirthday == "1")
                {
                    installerTextBox.AppendText("It is Steve's birthday today!");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("Happy birthday Steve!");
                    installerTextBox.AppendText(Environment.NewLine);
                }
                else if (howardBirthday == "1")
                {
                    installerTextBox.AppendText("It is Howard's birthday today!");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("Happy birthday Howard!");
                    installerTextBox.AppendText(Environment.NewLine);
                }
                else if (adamBirthday == "1")
                {
                    installerTextBox.AppendText("It is Adam's birthday today!");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("Happy birthday Adam!");
                    installerTextBox.AppendText(Environment.NewLine);
                }
                else if (geethBirthday == "1")
                {
                    installerTextBox.AppendText("It is Geeth's birthday today!");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("Happy birthday Geeth!");
                    installerTextBox.AppendText(Environment.NewLine);
                }
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
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(0)))), ((int)(((byte)(135)))), ((int)(((byte)(62)))));
                this.Invalidate();
            }
        }
        private void CheckHalloween()
        {
            if (DateTime.Now.Month == 10 && (DateTime.Now.Day == 28 || DateTime.Now.Day == 29 || DateTime.Now.Day == 30 || DateTime.Now.Day == 31))
            {
                halloween = "1";
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(76)))), ((int)(((byte)(2)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(252)))), ((int)(((byte)(76)))), ((int)(((byte)(2)))));
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
                restart.BackColor = System.Drawing.Color.White;
                close.BackColor = System.Drawing.Color.White;
                install.ForeColor = System.Drawing.Color.Black;
                restart.ForeColor = System.Drawing.Color.Black;
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
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(167)))), ((int)(((byte)(121)))), ((int)(((byte)(87)))));
                install.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
                restart.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(245)))), ((int)(((byte)(224)))), ((int)(((byte)(205)))));
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
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                install.ForeColor = System.Drawing.Color.White;
                restart.ForeColor = System.Drawing.Color.White;
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
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(66)))), ((int)(((byte)(66)))), ((int)(((byte)(66)))));
                install.ForeColor = System.Drawing.Color.White;
                restart.ForeColor = System.Drawing.Color.White;
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
                charlieBirthday = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
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
                deanBirthday = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
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
                steveBirthday = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
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
                howardBirthday = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.ForeColor = System.Drawing.Color.Black;
                this.Invalidate();
            }
        }
        private void CheckAdamBirthday()
        {
            if (DateTime.Now.Month == 6 && DateTime.Now.Day == 9)
            {
                birthday = "1";
                adamBirthday = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
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
                geethBirthday = "1";
                this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(155)))), ((int)(((byte)(211)))), ((int)(((byte)(221)))));
                install.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                restart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                close.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(242)))), ((int)(((byte)(172)))), ((int)(((byte)(185)))));
                installerTextBox.ForeColor = System.Drawing.Color.Black;
                this.Invalidate();
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            base.OnPaint(e);
            if (christmas == "1")
            {
                try
                {
                    Image heartImage = Properties.Resources.christmasTree;
                    int newWidth = 100;
                    int newHeight = 100;
                    int x = 160;
                    int y = 320;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.TranslateTransform(x + newWidth / 2, y + newHeight / 2);
                    e.Graphics.TranslateTransform(-(x + newWidth / 2), -(y + newHeight / 2));
                    e.Graphics.DrawImage(heartImage, new Rectangle(x, y, newWidth, newHeight));
                    e.Graphics.Restore(state);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
            if (halloween == "1")
            {
                try
                {
                    Image heartImage = Properties.Resources.pumpkin;
                    int newWidth = 100;
                    int newHeight = 100;
                    int x = 160;
                    int y = 320;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.TranslateTransform(x + newWidth / 2, y + newHeight / 2);
                    e.Graphics.TranslateTransform(-(x + newWidth / 2), -(y + newHeight / 2));
                    e.Graphics.DrawImage(heartImage, new Rectangle(x, y, newWidth, newHeight));
                    e.Graphics.Restore(state);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
            if (valentines == "1")
            {
                try
                {
                    Image heartImage = Properties.Resources.heart;
                    int newWidth = 100;
                    int newHeight = 100;
                    int x = 160;
                    int y = 320;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.TranslateTransform(x + newWidth / 2, y + newHeight / 2);
                    e.Graphics.RotateTransform(30);
                    e.Graphics.TranslateTransform(-(x + newWidth / 2), -(y + newHeight / 2));
                    e.Graphics.DrawImage(heartImage, new Rectangle(x, y, newWidth, newHeight));
                    e.Graphics.Restore(state);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
            if (pancake == "1")
            {
                try
                {
                    Image heartImage = Properties.Resources.pancake;
                    int newWidth = 100;
                    int newHeight = 100;
                    int x = 160;
                    int y = 320;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.TranslateTransform(x + newWidth / 2, y + newHeight / 2);
                    e.Graphics.TranslateTransform(-(x + newWidth / 2), -(y + newHeight / 2));
                    e.Graphics.DrawImage(heartImage, new Rectangle(x, y, newWidth, newHeight));
                    e.Graphics.Restore(state);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
            if (puffin == "1")
            {
                try
                {
                    Image heartImage = Properties.Resources.puffin;
                    int newWidth = 100;
                    int newHeight = 100;
                    int x = 320;
                    int y = 320;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.TranslateTransform(x + newWidth / 2, y + newHeight / 2);
                    e.Graphics.TranslateTransform(-(x + newWidth / 2), -(y + newHeight / 2));
                    e.Graphics.DrawImage(heartImage, new Rectangle(x, y, newWidth, newHeight));
                    e.Graphics.Restore(state);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
            if (dachshund == "1")
            {
                try
                {
                    Image heartImage = Properties.Resources.pluto;
                    int newWidth = 130;
                    int newHeight = 100;
                    int x = 140;
                    int y = 320;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.TranslateTransform(x + newWidth / 2, y + newHeight / 2);
                    e.Graphics.TranslateTransform(-(x + newWidth / 2), -(y + newHeight / 2));
                    e.Graphics.DrawImage(heartImage, new Rectangle(x, y, newWidth, newHeight));
                    e.Graphics.Restore(state);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
            if (hippo == "1")
            {
                try
                {
                    Image heartImage = Properties.Resources.hippo;
                    int newWidth = 130;
                    int newHeight = 100;
                    int x = 140;
                    int y = 320;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.TranslateTransform(x + newWidth / 2, y + newHeight / 2);
                    e.Graphics.TranslateTransform(-(x + newWidth / 2), -(y + newHeight / 2));
                    e.Graphics.DrawImage(heartImage, new Rectangle(x, y, newWidth, newHeight));
                    e.Graphics.Restore(state);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
            if (rhino == "1")
            {
                try
                {
                    Image heartImage = Properties.Resources.rhino;
                    int newWidth = 120;
                    int newHeight = 100;
                    int x = 140;
                    int y = 320;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.TranslateTransform(x + newWidth / 2, y + newHeight / 2);
                    e.Graphics.TranslateTransform(-(x + newWidth / 2), -(y + newHeight / 2));
                    e.Graphics.DrawImage(heartImage, new Rectangle(x, y, newWidth, newHeight));
                    e.Graphics.Restore(state);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
            }
            if (birthday == "1")
            {
                try
                {
                    Image heartImage = Properties.Resources.present;
                    int newWidth = 100;
                    int newHeight = 100;
                    int x = 160;
                    int y = 320;
                    e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                    e.Graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                    e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
                    GraphicsState state = e.Graphics.Save();
                    e.Graphics.TranslateTransform(x + newWidth / 2, y + newHeight / 2);
                    e.Graphics.TranslateTransform(-(x + newWidth / 2), -(y + newHeight / 2));
                    e.Graphics.DrawImage(heartImage, new Rectangle(x, y, newWidth, newHeight));
                    e.Graphics.Restore(state);
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error loading image: " + ex.Message);
                }
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
        private async void CheckIP()
        {
            string publicIP = await GetPublicIPAddressAsync();
            if (publicIP != null)
            {
                string publicIPHash = HashIP(publicIP);

                if (publicIPHash == romseyHash)
                {
                    romsey = "1";
                    safeLocation = "1";
                }
                else if (publicIPHash == chandlersFordHash)
                {
                    chandlersFord = "1";
                    safeLocation = "1";
                    microsoftOffice2007Check.Checked = true;
                }
                else if (publicIPHash == highcliffeHash)
                {
                    highcliffe = "1";
                    safeLocation = "1";
                }
                else if (publicIPHash == charlieHomeHash)
                {
                    charlieHome = "1";
                    safeLocation = "1";
                }
                else
                {
                    safeLocation = "0";
                }
            }
            UpdateLocation();
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
        Uri anyDeskURL = new Uri("https://files.crchq.net/installer/anyDesk.msi");
        string anyDeskFilename = @"C:\Computer Repair Centre\apps\anyDesks.msi";
        Uri bingWallpapersURL = new Uri("https://files.crchq.net/installer/bingWallpapers.msi");
        string bingWallpapersFilename = @"C:\Computer Repair Centre\apps\bingWallpapers.msi";
        Uri bitDefenderURL = new Uri("https://files.crchq.net/installer/bitDefender.exe");
        string bitDefenderFilename = @"C:\Computer Repair Centre\apps\bitDefender.exe";
        Uri discordURL = new Uri("https://discord.com/api/download?platform=win");
        string discordFilename = @"C:\Computer Repair Centre\apps\discord.exe";
        Uri googleChromeURL = new Uri("https://dl.google.com/tag/s/appguid%3D%7B8A69D345-D564-463c-AFF1-A69D9E530F96%7D&iid=&lang=en&browser=4&usagestats=0&appname=Google%2520Chrome%2520Enterprise&needsadmin=false/edgedl/chrome/install/GoogleChromeStandaloneEnterprise64.msi");
        string googleChromeFilename = @"C:\Computer Repair Centre\apps\googleChrome.msi";
        string libreOfficeFilename = @"C:\Computer Repair Centre\apps\libreOffice.msi";
        Uri microsoftOffice2007URL = new Uri("https://files.crchq.net/installer/office2007.zip");
        string microsoftOffice2007Filename = @"C:\Computer Repair Centre\apps\office2007.zip";
        Uri mozillaFirefoxURL = new Uri("https://download.mozilla.org/?product=firefox-msi-latest-ssl&os=win64&lang=en-GB");
        string mozillaFirefoxFilename = @"C:\Computer Repair Centre\apps\mozillaFirefox.msi";
        Uri mozillaThunderbirdURL = new Uri("https://download.mozilla.org/?product=thunderbird-msi-latest-ssl&os=win64&lang=en-GB");
        string mozillaThunderbirdFilename = @"C:\Computer Repair Centre\apps\mozillaThunderbird.msi";
        Uri nanaZipURL = new Uri("https://files.crchq.net/installer/nanaZip.msixbundle");
        string nanaZipFilename = @"C:\Computer Repair Centre\apps\nanaZip.msixbundle";
        Uri steamURL = new Uri("https://files.crchq.net/installer/steam.exe");
        string steamFilename = @"C:\Computer Repair Centre\apps\steam.exe";
        Uri hpHotkeySupportURL = new Uri("https://files.crchq.net/installer/HPHotkey.zip");
        string hpHotkeySupportFilename = @"C:\Computer Repair Centre\apps\hpHotkeySupport.zip";
        Uri vlcMediaPlayerURL = new Uri("https://files.crchq.net/installer/vlcMediaPlayer.msi");
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
                            installerTextBox.AppendText(versionText + Environment.NewLine);
                            return;
                        }
                    }
                    installerTextBox.AppendText("⚠️ Unable to determine Windows version." + Environment.NewLine);
                }
            }
            catch (Exception ex)
            {
                installerTextBox.AppendText("❌ Error checking Windows version: " + ex.Message + Environment.NewLine);
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
                //intelCheck.Checked = true;
                intel = "1";
                if (hasIntelGpu && hasIntelCpu)
                    installerTextBox.AppendText("🧠 + 🎮 Intel CPU and GPU detected." + Environment.NewLine);
                else if (hasIntelGpu)
                    installerTextBox.AppendText("🎮 Intel GPU detected." + Environment.NewLine);
                else
                    installerTextBox.AppendText("🧠 Intel CPU detected." + Environment.NewLine);
            }
            else
            {
//                intelCheck.Checked = false;
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
                        installerTextBox.AppendText("🎮 Nvidia GPU detected." + Environment.NewLine);
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
                //amdCheck.Checked = true;
                amd = "1";
                if (hasAmdGpu && hasAmdCpu)
                    installerTextBox.AppendText("🧠 + 🎮 AMD CPU and GPU detected." + Environment.NewLine);
                else if (hasAmdGpu)
                    installerTextBox.AppendText("🎮 AMD GPU detected." + Environment.NewLine);
                else
                    installerTextBox.AppendText("🧠 AMD CPU detected." + Environment.NewLine);
            }
            else
            {
                //amdCheck.Checked = false;
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
            string rootDir = @"C:\Computer Repair Centre";
            string oemDir = @"C:\Computer Repair Centre\oem";
            string appsDir = @"C:\Computer Repair Centre\apps";
            string bingWallpaperAppPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\BingWallpaperApp\BingWallpaperApp.exe");
            string discordAppPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Discord\Update.exe");
            string desktopPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            string launcherPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"Computer Repair Centre Installer Launcher.exe");
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
            if (hpEliteBook == "1") { progressBar.Maximum += 4; }
            if (christmas == "1")
            {
                player = new SoundPlayer(Properties.Resources.christmas);
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
                installerTextBox.AppendText("Nvidia GPU has been detected and selected, Nvidia App will be installed.");
                installerTextBox.AppendText(Environment.NewLine);
                installerTextBox.AppendText("You can uncheck this if you want.");
                installerTextBox.AppendText(Environment.NewLine);
            }
            if (powerCheck.Checked)
            {
                installerTextBox.AppendText("📌 Disable sleep on AC power is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                installerTextBox.AppendText("Disabling sleep and screen timeout while on AC power...");
                installerTextBox.AppendText(Environment.NewLine);
                Process.Start("powercfg", "/change monitor-timeout-ac 0");
                Process.Start("powercfg", "/change standby-timeout-ac 0");
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }
            else
            {
                installerTextBox.AppendText("Disabling sleep and screen timeout while on AC power temporarily during install...");
                installerTextBox.AppendText(Environment.NewLine);
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }

            if (crcCheck.Checked)
            {
                installerTextBox.AppendText("📌 Computer Repair Centre OEM information is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (romsey == "1")
                {
                    installerTextBox.AppendText("The installer is being run from the Romsey shop.");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("📦 Installing Romsey Computer Repair Centre OEM information...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                    installerTextBox.AppendText("The installer is being run from the Chandlers Ford shop.");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("📦 Installing Chandlers Ford Computer Repair Centre OEM information...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                    installerTextBox.AppendText("The installer is being run from the Romsey shop.");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("📦 Installing Romsey Computer Repair Centre OEM information...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                installerTextBox.AppendText("📌 NanaZip is selected.");
                installerTextBox.AppendText(Environment.NewLine);

                string nanaZipExe = "NanaZip.Windows.exe";
                string windowsAppsPath = @"C:\Program Files\WindowsApps";
                string nanaZipPath = null;

                installerTextBox.AppendText("Checking if NanaZip is installed...");
                installerTextBox.AppendText(Environment.NewLine);

                try
                {
                    var files = Directory.GetFiles(windowsAppsPath, nanaZipExe, SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        nanaZipPath = files[0];
                        installerTextBox.AppendText($"✅ NanaZip is already installed.");
                        installerTextBox.AppendText(Environment.NewLine);
                        progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                    }
                    else
                    {
                        installerTextBox.AppendText("❌ NanaZip not found, proceeding with installation.");
                        installerTextBox.AppendText(Environment.NewLine);

                        installerTextBox.AppendText("🔄 Downloading NanaZip...");
                        installerTextBox.AppendText(Environment.NewLine);

                        using (WebClient wc = new WebClient())
                        {
                            await wc.DownloadFileTaskAsync(nanaZipURL, nanaZipFilename);
                        }

                        installerTextBox.AppendText("📦 Installing NanaZip...");
                        installerTextBox.AppendText(Environment.NewLine);

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

                        installerTextBox.AppendText("✅ Completed installation of NanaZip.");
                        installerTextBox.AppendText(Environment.NewLine);
                        progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    installerTextBox.AppendText("⚠️ Access denied to WindowsApps. Try running as Administrator.");
                    installerTextBox.AppendText(Environment.NewLine);
                }
                catch (Exception ex)
                {
                    installerTextBox.AppendText("❌ Error: " + ex.Message);
                    installerTextBox.AppendText(Environment.NewLine);
                }
            }
            if (anyDeskCheck.Checked)
            {
                installerTextBox.AppendText("📌 AnyDesk is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files (x86)\AnyDeskMSI\AnyDeskMSI.exe"))
                {
                    installerTextBox.AppendText("✅ AnyDesk is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else if (System.IO.File.Exists(@"C:\Program Files (x86)\AnyDesk\AnyDesk.exe"))
                {
                    installerTextBox.AppendText("AnyDesk is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading AnyDesk...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(anyDeskURL, anyDeskFilename);
                    }
                    installerTextBox.AppendText("📦 Installing AnyDesk...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                    installerTextBox.AppendText("✅ Completed installation of AnyDesk.");
                    installerTextBox.AppendText(Environment.NewLine); ;
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (bingWallpapersCheck.Checked)
            {
                installerTextBox.AppendText("📌 Bing Wallpapers is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(bingWallpaperAppPath))
                {
                    installerTextBox.AppendText("✅ Bing Wallpapers is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading Bing Wallpapers...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(bingWallpapersURL, bingWallpapersFilename);
                    }
                    installerTextBox.AppendText("📦 Installing Bing Wallpapers...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                    installerTextBox.AppendText("✅ Completed installation of Bing Wallpapers.");
                    installerTextBox.AppendText(Environment.NewLine); ;
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (bitDefenderCheck.Checked)
            {
                installerTextBox.AppendText("📌 BitDefender is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\Bitdefender\Bitdefender Security App\seccenter.exe"))
                {
                    installerTextBox.AppendText("✅ BitDefender is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading BitDefender...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(bitDefenderURL, bitDefenderFilename);
                    }
                    installerTextBox.AppendText("📦 Installing BitDefender...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                    installerTextBox.AppendText("✅ Completed installation of BitDefender.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (discordCheck.Checked)
            {
                installerTextBox.AppendText("📌 Discord is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(discordAppPath))
                {
                    installerTextBox.AppendText("✅ Discord is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading Discord...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(discordURL, discordFilename);
                    }
                    installerTextBox.AppendText("📦 Installing Discord...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                    installerTextBox.AppendText("✅ Completed installation of Discord.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (googleChromeCheck.Checked)
            {
                installerTextBox.AppendText("📌 Google Chrome is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\Google\Chrome\Application\chrome.exe"))
                {
                    installerTextBox.AppendText("✅ Google Chrome is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading Google Chrome...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(googleChromeURL, googleChromeFilename);
                    }
                    installerTextBox.AppendText("📦 Installing Google Chrome...");
                    installerTextBox.AppendText(Environment.NewLine);
                    await Task.Run(() =>
                    {
                        using (Process process = new Process())
                        {
                            process.StartInfo.FileName = "msiexec";
                            process.StartInfo.Arguments = $"/package \"{googleChromeFilename}\" /passive";
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
                    installerTextBox.AppendText("✅ Completed installation of Google Chrome.");
                    installerTextBox.AppendText(Environment.NewLine); ;
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (libreOfficeCheck.Checked)
            {
                installerTextBox.AppendText("📌 LibreOffice is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\LibreOffice\program\soffice.exe"))
                {
                    installerTextBox.AppendText("✅ LibreOffice is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading LibreOffice...");
                    installerTextBox.AppendText(Environment.NewLine);
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

                    installerTextBox.AppendText("📦 Installing LibreOffice...");
                    installerTextBox.AppendText(Environment.NewLine);

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

                    installerTextBox.AppendText("✅ Completed installation of LibreOffice.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (microsoftOffice2007Check.Checked)
            {
                installerTextBox.AppendText("📌 Microsoft Office 2007 is selected.");
                installerTextBox.AppendText(Environment.NewLine);

                string officePath = @"C:\Program Files (x86)\Microsoft Office\Office12\WINWORD.EXE";
                string windowsAppsPath = @"C:\Program Files\WindowsApps";
                string nanaZipExe = "NanaZip.Windows.exe";
                string nanaZipPath = null;
                if (File.Exists(officePath))
                {
                    installerTextBox.AppendText("✅ Microsoft Office 2007 is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading Microsoft Office 2007...");
                    installerTextBox.AppendText(Environment.NewLine);

                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(microsoftOffice2007URL, microsoftOffice2007Filename);
                    }
                    installerTextBox.AppendText("🔎 Checking if NanaZip is installed...");
                    installerTextBox.AppendText(Environment.NewLine);
                    try
                    {
                        var files = Directory.GetFiles(windowsAppsPath, nanaZipExe, SearchOption.AllDirectories);
                        if (files.Length > 0)
                        {
                            nanaZipPath = files[0];
                            installerTextBox.AppendText($"✅ NanaZip is already installed.");
                            installerTextBox.AppendText(Environment.NewLine);
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        installerTextBox.AppendText("⚠️ Access denied to WindowsApps. Try running as Administrator.");
                        installerTextBox.AppendText(Environment.NewLine);
                    }
                    if (string.IsNullOrEmpty(nanaZipPath))
                    {
                        installerTextBox.AppendText("🚀 NanaZip is not installed and is required for extraction.");
                        installerTextBox.AppendText(Environment.NewLine);
                        installerTextBox.AppendText("📥 Downloading NanaZip...");
                        installerTextBox.AppendText(Environment.NewLine);
                        using (WebClient wc = new WebClient())
                        {
                            await wc.DownloadFileTaskAsync(nanaZipURL, nanaZipFilename);
                        }
                        installerTextBox.AppendText("📦 📦 Installing NanaZip...");
                        installerTextBox.AppendText(Environment.NewLine);
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
                        installerTextBox.AppendText("✅ NanaZip installation completed.");
                        installerTextBox.AppendText(Environment.NewLine);
                        try
                        {
                            var files = Directory.GetFiles(windowsAppsPath, nanaZipExe, SearchOption.AllDirectories);
                            if (files.Length > 0)
                            {
                                nanaZipPath = files[0];
                                installerTextBox.AppendText($"✅ NanaZip is already installed.");
                                installerTextBox.AppendText(Environment.NewLine);
                            }
                            else
                            {
                                installerTextBox.AppendText("❌ Failed to find NanaZip after installation.");
                                installerTextBox.AppendText(Environment.NewLine);
                                return;
                            }
                        }
                        catch (UnauthorizedAccessException)
                        {
                            installerTextBox.AppendText("⚠️ Access denied while searching for NanaZip after installation.");
                            installerTextBox.AppendText(Environment.NewLine);
                            return;
                        }
                    }
                    string microsoftOffice2007ExtractPath = Path.Combine(desktopPath, "Microsoft Office 2007");
                    if (!Directory.Exists(microsoftOffice2007ExtractPath))
                    {
                        Directory.CreateDirectory(microsoftOffice2007ExtractPath);
                    }
                    installerTextBox.AppendText("📂 Extracting Microsoft Office 2007 to Desktop...");
                    installerTextBox.AppendText(Environment.NewLine);

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
                                    installerTextBox.AppendText(output);
                                    installerTextBox.AppendText(Environment.NewLine);
                                }

                                if (!string.IsNullOrEmpty(errors))
                                {
                                    installerTextBox.AppendText("⚠️ Errors: " + errors);
                                    installerTextBox.AppendText(Environment.NewLine);
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            installerTextBox.AppendText("❌ Exception: " + ex.Message);
                            installerTextBox.AppendText(Environment.NewLine);
                        }
                    }
                    await RunNanaZipExtractionOfficeAsync();
                    installerTextBox.AppendText("✅ Completed extraction of Microsoft Office 2007.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (nvidiaAppCheck.Checked)
            {
                installerTextBox.AppendText("📌 Nvidia App is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                installerTextBox.AppendText("🔄 Searching for latest Nvidia App installer...");
                installerTextBox.AppendText(Environment.NewLine);
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
                            installerTextBox.AppendText($"🔗 Found latest Nvidia installer: {downloadUrl}");
                            installerTextBox.AppendText(Environment.NewLine);

                            byte[] fileBytes = await client.GetByteArrayAsync(downloadUrl);
                            File.WriteAllBytes(nvidiaAppFilename, fileBytes);
                        }
                        else
                        {
                            installerTextBox.AppendText("⚠️ Could not find Nvidia App download link.");
                            installerTextBox.AppendText(Environment.NewLine);
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    installerTextBox.AppendText($"⚠️ Error downloading Nvidia App: {ex.Message}");
                    installerTextBox.AppendText(Environment.NewLine);
                    return;
                }
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                installerTextBox.AppendText("📦 Installing Nvidia App silently...");
                installerTextBox.AppendText(Environment.NewLine);

                await Task.Run(() =>
                {
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = nvidiaAppFilename,
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
                            installerTextBox.AppendText(exitCode == 0
                                ? "✅ Installation successful."
                                : $"⚠️ Installation exited with code: {exitCode}");
                            installerTextBox.AppendText(Environment.NewLine);
                        }
                    }
                    catch (Exception ex)
                    {
                        installerTextBox.AppendText($"⚠️ Installation failed: {ex.Message}");
                        installerTextBox.AppendText(Environment.NewLine);
                    }
                });
                installerTextBox.AppendText("✅ Completed installation of Nvidia App.");
                installerTextBox.AppendText(Environment.NewLine);
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }
            if (mozillaFirefoxCheck.Checked)
            {
                installerTextBox.AppendText("📌 Mozilla Firefox is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\Mozilla Firefox\firefox.exe"))
                {
                    installerTextBox.AppendText("✅ Mozilla Firefox is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading Mozilla Firefox...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(mozillaFirefoxURL, mozillaFirefoxFilename);
                    }
                    installerTextBox.AppendText("📦 Installing Mozilla Firefox...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                    installerTextBox.AppendText("✅ Completed installation of Mozilla Firefox.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (mozillaThunderbirdCheck.Checked)
            {
                installerTextBox.AppendText("📌 Mozilla Thunderbird is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\Mozilla Thunderbird\thunderbird.exe"))
                {
                    installerTextBox.AppendText("✅ Mozilla Thunderbird is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading Mozilla Thunderbird...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(mozillaThunderbirdURL, mozillaThunderbirdFilename);
                    }
                    installerTextBox.AppendText("📦 Installing Mozilla Thunderbird...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                    installerTextBox.AppendText("✅ Completed installation of Mozilla Thunderbird.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (steamCheck.Checked)
            {
                installerTextBox.AppendText("📌 Steam is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files (x86)\Steam\Steam.exe"))
                {
                    installerTextBox.AppendText("✅ Steam is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading Steam...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(steamURL, steamFilename);
                    }
                    installerTextBox.AppendText("📦 Installing Steam...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                    installerTextBox.AppendText("✅ Completed installation of Steam.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (vlcMediaPlayerCheck.Checked)
            {
                installerTextBox.AppendText("📌 VLC Media Player is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\VideoLAN\VLC\vlc.exe"))
                {
                    installerTextBox.AppendText("✅ VLC Media Player is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("🔄 Downloading VLC Media Player...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(vlcMediaPlayerURL, vlcMediaPlayerFilename);
                    }
                    installerTextBox.AppendText("📦 Installing VLC Media Player...");
                    installerTextBox.AppendText(Environment.NewLine);
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
                    installerTextBox.AppendText("✅ Completed installation of VLC Media Player.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (hpEliteBook == "1")
            {
                installerTextBox.AppendText("The installer is being run on an HP EliteBook.");
                installerTextBox.AppendText(Environment.NewLine);

                installerTextBox.AppendText("🔄 Downloading HP Hotkey Support...");
                installerTextBox.AppendText(Environment.NewLine);

                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += wc_progressBarStep;
                    await wc.DownloadFileTaskAsync(hpHotkeySupportURL, hpHotkeySupportFilename);
                }

                installerTextBox.AppendText("Checking if NanaZip is installed...");
                installerTextBox.AppendText(Environment.NewLine);

                string windowsAppsPath = @"C:\Program Files\WindowsApps";
                string nanaZipExe = "NanaZip.Windows.exe";
                string nanaZipPath = null;

                try
                {
                    var files = Directory.GetFiles(windowsAppsPath, nanaZipExe, SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        nanaZipPath = files[0];
                        installerTextBox.AppendText($"✅ NanaZip is already installed.");
                        installerTextBox.AppendText(Environment.NewLine);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    installerTextBox.AppendText("⚠️ Access denied to WindowsApps. Try running as Administrator.");
                    installerTextBox.AppendText(Environment.NewLine);
                }

                if (string.IsNullOrEmpty(nanaZipPath))
                {
                    installerTextBox.AppendText("NanaZip is not installed and is required for extraction.");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("🔄 Downloading NanaZip...");
                    installerTextBox.AppendText(Environment.NewLine);

                    using (WebClient wc = new WebClient())
                    {
                        await wc.DownloadFileTaskAsync(nanaZipURL, nanaZipFilename);
                    }

                    installerTextBox.AppendText("📦 Installing NanaZip...");
                    installerTextBox.AppendText(Environment.NewLine);

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

                    installerTextBox.AppendText("✅ Completed installation of NanaZip.");
                    installerTextBox.AppendText(Environment.NewLine);

                    try
                    {
                        var files = Directory.GetFiles(windowsAppsPath, nanaZipExe, SearchOption.AllDirectories);
                        if (files.Length > 0)
                        {
                            nanaZipPath = files[0];
                            installerTextBox.AppendText($"✅ NanaZip is already installed.");
                            installerTextBox.AppendText(Environment.NewLine);
                        }
                        else
                        {
                            installerTextBox.AppendText("❌ Failed to find NanaZip after installation.");
                            installerTextBox.AppendText(Environment.NewLine);
                            return;
                        }
                    }
                    catch (UnauthorizedAccessException)
                    {
                        installerTextBox.AppendText("⚠️ Access denied while searching for NanaZip after installation.");
                        installerTextBox.AppendText(Environment.NewLine);
                        return;
                    }
                }

                installerTextBox.AppendText("📂 Extracting HP Hotkey Support...");
                installerTextBox.AppendText(Environment.NewLine);

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
                                installerTextBox.AppendText(output);
                                installerTextBox.AppendText(Environment.NewLine);
                            }

                            if (!string.IsNullOrEmpty(errors))
                            {
                                installerTextBox.AppendText("Errors: " + errors);
                                installerTextBox.AppendText(Environment.NewLine);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        installerTextBox.AppendText("Exception: " + ex.Message);
                        installerTextBox.AppendText(Environment.NewLine);
                    }
                }

                if (!Directory.Exists(hpHotkeySupportExtractPath))
                {
                    Directory.CreateDirectory(hpHotkeySupportExtractPath);
                }

                await RunNanaZipExtractionHPAsync();

                installerTextBox.AppendText("✅ Completed extraction of HP Hotkey Support.");
                installerTextBox.AppendText(Environment.NewLine);
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);

                installerTextBox.AppendText("📦 Installing HP Hotkey Support...");
                installerTextBox.AppendText(Environment.NewLine);

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
                                installerTextBox.AppendText($"Process completed successfully for {filePath}.");
                                installerTextBox.AppendText(Environment.NewLine);
                            }
                            else
                            {
                                installerTextBox.AppendText($"❌ Failed to start the process: {filePath}.");
                                installerTextBox.AppendText(Environment.NewLine);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        installerTextBox.AppendText("An error occurred: " + ex.Message);
                        installerTextBox.AppendText(Environment.NewLine);
                    }
                }

                await InstallHPHotkeySupport();
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);

                installerTextBox.AppendText("📦 Installing HP Framework...");
                installerTextBox.AppendText(Environment.NewLine);

                await InstallHPFramework();
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);

                installerTextBox.AppendText("✅ Completed installation of HP Hotkey Support.");
                installerTextBox.AppendText(Environment.NewLine);
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
                            if (romsey == "1") 
                            {
                                installerTextBox.AppendText("✅ Aligning the taskbar to the left...");
                                installerTextBox.AppendText(Environment.NewLine);
                                const string taskbarRegPath = @"SOFTWARE\microsoft\windows\currentversion\explorer\advanced";
                                const string taskbarReg = "TaskbarAl";
                                const int taskbarRegData = 0;
                                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(taskbarRegPath, writable: true))
                                {
                                    registryKey.SetValue(taskbarReg, taskbarRegData, RegistryValueKind.DWord);
                                    Console.WriteLine($"Set '{taskbarReg}' to {taskbarRegData} in '{taskbarRegPath}'.");
                                }
                                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            }
                            else if (chandlersFord == "1")
                            {
                            }
                            else if (highcliffe == "1")
                            {
                                installerTextBox.AppendText("✅ Aligning the taskbar to the left...");
                                installerTextBox.AppendText(Environment.NewLine);
                                const string taskbarRegPath = @"SOFTWARE\microsoft\windows\currentversion\explorer\advanced";
                                const string taskbarReg = "TaskbarAl";
                                const int taskbarRegData = 0;
                                using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(taskbarRegPath, writable: true))
                                {
                                    registryKey.SetValue(taskbarReg, taskbarRegData, RegistryValueKind.DWord);
                                    Console.WriteLine($"Set '{taskbarReg}' to {taskbarRegData} in '{taskbarRegPath}'.");
                                }
                                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                            }
                            installerTextBox.AppendText("✅ Disabling device encryption...");
                            installerTextBox.AppendText(Environment.NewLine);
                            const string bitLockerRegPath = @"SYSTEM\CurrentControlSet\Control\BitLocker";
                            const string bitLockerReg = "PreventDeviceEncryption";
                            const int bitLockerRegData = 1;
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(bitLockerRegPath, writable: true))
                            {
                                registryKey.SetValue(bitLockerReg, bitLockerRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{bitLockerReg}' to {bitLockerRegData} in '{bitLockerRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);

                            installerTextBox.AppendText("✅ Disabling fastboot mode...");
                            installerTextBox.AppendText(Environment.NewLine);
                            const string hiberbootRegPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Power";
                            const string hiberbootReg = "HiberbootEnabled";
                            const int hiberbootRegData = 0;
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(hiberbootRegPath, writable: true))
                            {
                                registryKey.SetValue(hiberbootReg, hiberbootRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{hiberbootReg}' to {hiberbootRegData} in '{hiberbootRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);

                            installerTextBox.AppendText("✅ Disabling location tracking...");
                            installerTextBox.AppendText(Environment.NewLine);
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

                            installerTextBox.AppendText("✅ Disabling People icon...");
                            installerTextBox.AppendText(Environment.NewLine);
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

                            installerTextBox.AppendText("✅ Hiding recently used files and folders in File Explorer...");
                            installerTextBox.AppendText(Environment.NewLine);
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
                            installerTextBox.AppendText("✅ Setting explorer to open to This PC...");
                            installerTextBox.AppendText(Environment.NewLine);
                            const string thisPCRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced";
                            const string thisPCReg = "LaunchTo";
                            const int thisPCRegData = 1;
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(thisPCRegPath, writable: true))
                            {
                                registryKey.SetValue(thisPCReg, thisPCRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{thisPCReg}' to {thisPCRegData} in '{thisPCRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);

                            installerTextBox.AppendText("✅ Disabling fastboot mode...");
                            installerTextBox.AppendText(Environment.NewLine);
                            const string hiberbootRegPath = @"SYSTEM\CurrentControlSet\Control\Session Manager\Power";
                            const string hiberbootReg = "HiberbootEnabled";
                            const int hiberbootRegData = 0;
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(hiberbootRegPath, writable: true))
                            {
                                registryKey.SetValue(hiberbootReg, hiberbootRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{hiberbootReg}' to {hiberbootRegData} in '{hiberbootRegPath}'.");
                            }
                            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);

                            installerTextBox.AppendText("✅ Disabling location tracking...");
                            installerTextBox.AppendText(Environment.NewLine);
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

                            installerTextBox.AppendText("✅ Disabling People icon...");
                            installerTextBox.AppendText(Environment.NewLine);
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

                            installerTextBox.AppendText("✅ Hiding recently used files and folders in File Explorer...");
                            installerTextBox.AppendText(Environment.NewLine);
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
                            installerTextBox.AppendText("This computer is running an old version of Windows, please update it.");
                            installerTextBox.AppendText(Environment.NewLine);
                        }
                    }
                }
            }

            if (powerCheck.Checked) { }
            else
            {
                installerTextBox.AppendText("✅ Re-enabling sleep and screen timeout on AC power...");
                installerTextBox.AppendText(Environment.NewLine);
                Process.Start("powercfg", "/change monitor-timeout-ac 10");
                Process.Start("powercfg", "/change standby-timeout-ac 20");
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }

            installerTextBox.AppendText("✅ Cleaning up installation files...");
            installerTextBox.AppendText(Environment.NewLine);
            var deletionHelper = new FileDeletionHelper();
            await deletionHelper.DeleteFilesAndDirectoryAsync(appsDir, launcherPath);
            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);

            if (recycleBinCheck.Checked)
            {
                installerTextBox.AppendText("✅ Empty Recycle Bin is checked.");
                installerTextBox.AppendText(Environment.NewLine);
                installerTextBox.AppendText("🗑️ Emptying Recycle Bin...");
                installerTextBox.AppendText(Environment.NewLine);

                try
                {
                    SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
                    installerTextBox.AppendText("✅ Recycle Bin emptied successfully.");
                }
                catch (Exception ex)
                {
                    installerTextBox.AppendText($"⚠️ Failed to empty Recycle Bin: {ex.Message}");
                }

                installerTextBox.AppendText(Environment.NewLine);
            }


            player.Play();

            if (restartCheck.Checked)
            {
                Process.Start("shutdown", "/r /t 60");
                installerTextBox.AppendText("System will restart in 60 seconds. If you need to cancel this press the close button.");
                installerTextBox.AppendText(Environment.NewLine);
            }

            installerTextBox.AppendText("✅ The installation has completed.");
            installerTextBox.AppendText(Environment.NewLine);

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

        private async void close_Click(object sender, EventArgs e)
        {
            await Task.Delay(325);
            Process.Start("shutdown", "/a");
            this.Close();
        }

        private void restart_Click(object sender, EventArgs e)
        {
            Process.Start("shutdown","/r /t 1");
        }

        private void versionLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/professorshroom/PlutoPoint-Installer/blob/main/README.md");
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