using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Collections;
using Microsoft.Win32;
using System.IO;
using System.Xml.Linq;
using System.Diagnostics.Eventing.Reader;
using System.Security.Policy;
using static System.Windows.Forms.VisualStyles.VisualStyleElement;
using System.Net.Http;
using System.Collections.Generic;
using System.Threading;
using System.Media;
using static System.Net.WebRequestMethods;
using System.Management;

// Copyright © Charlie Howard 2025 All rights reserved.

namespace PlutoPoint_Installer
{

    using System.Drawing;
    using System.Drawing.Drawing2D;
    using System.Management;
    using System.Windows.Forms;
    using File = System.IO.File;

    public partial class installerForm : Form
    {

        string updateDate = "13th of February 2025";

        public installerForm()
        {
            InitializeComponent();
            CheckChristmas();
            CheckHalloween();
            CheckValentines();
            CheckCharlieBirthday();
            CheckDeanBirthday();
            CheckSteveBirthday();
            CheckHowardBirthday();
            CheckAdamBirthday();
            CheckGeethBirthday();
            CheckMicrosoftOffice2007Async();
            CheckEliteBook();
        }

        string christmas = null;
        string halloween = null;
        string valentines = null;
        string birthday = null;
        string charlieBirthday = null;
        string deanBirthday = null;
        string steveBirthday = null;
        string howardBirthday = null;
        string adamBirthday = null;
        string geethBirthday = null;
        string hpEliteBook = null;

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
        private void CheckSteveBirthday()
        {
            if (DateTime.Now.Month == 6 && DateTime.Now.Day == 24)
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
        private void CheckHowardBirthday()
        {
            if (DateTime.Now.Month == 5 && DateTime.Now.Day == 16)
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
        private void CheckAdamBirthday()
        {
            if (DateTime.Now.Month == 6 && DateTime.Now.Day == 9)
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
        private void CheckGeethBirthday()
        {
            if (DateTime.Now.Month == 7 && DateTime.Now.Day == 25)
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
                    int x = 165;
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
                    int x = 165;
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
                    int x = 165;
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
            if (birthday == "1")
            {
                try
                {
                    Image heartImage = Properties.Resources.present;
                    int newWidth = 100;
                    int newHeight = 100;
                    int x = 165;
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

        string romsey = null;
        string chandlersFord = null;
        string highcliffe = null;
        private async void CheckMicrosoftOffice2007Async()
        {
            string publicIP = await GetPublicIPAddressAsync();
            if (publicIP != null)
            {
                switch (publicIP)
                {
                    case "81.134.32.116":
                        //Romsey
                        romsey = "1";
                        break;
                    case "86.12.18.92":
                        //Chandlers Ford
                        chandlersFord = "1";
                        microsoftOffice2007Check.Checked = true;
                        break;
                    case "81.130.137.162":
                        //Highcliffe
                        highcliffe = "1";
                        break;
                }
            }
        }

        Uri crcOEMURL = new Uri("https://raw.githubusercontent.com/charliehoward/PlutoPoint-Installer/refs/heads/main/Resources/computerRepairCentre/computerRepairCentreOEM.bmp");
        string crcOEMFilename = @"C:\Computer Repair Centre\oem\computerRepairCentreOEM.bmp";
        Uri anyDeskURL = new Uri("https://files.crchq.net/installer/anyDesk.msi");
        string anyDeskFilename = @"C:\Computer Repair Centre\apps\anyDesks.msi";
        Uri bingWallpapersURL = new Uri("https://files.crchq.net/installer/bingWallpapers.msi");
        string bingWallpapersFilename = @"C:\Computer Repair Centre\apps\bingWallpapers.msi";
        Uri bitDefenderURL = new Uri("https://files.crchq.net/installer/bitDefender.exe");
        string bitDefenderFilename = @"C:\Computer Repair Centre\apps\bitDefender.exe";
        Uri discordURL = new Uri("https://files.crchq.net/installer/discord.exe");
        string discordFilename = @"C:\Computer Repair Centre\apps\discord.exe";
        Uri googleChromeURL = new Uri("https://files.crchq.net/installer/chrome.msi");
        string googleChromeFilename = @"C:\Computer Repair Centre\apps\chrome.msi";
        Uri libreOfficeURL = new Uri("https://files.crchq.net/installer/libreOffice.msi");
        string libreOfficeFilename = @"C:\Computer Repair Centre\apps\libreOffice.msi";
        Uri microsoftOffice2007URL = new Uri("https://files.crchq.net/installer/office2007.zip");
        string microsoftOffice2007Filename = @"C:\Computer Repair Centre\apps\office2007.zip";
        Uri mozillaFirefoxURL = new Uri("https://files.crchq.net/installer/mozillaFirefox.msi");
        string mozillaFirefoxFilename = @"C:\Computer Repair Centre\apps\mozillaFirefox.msi";
        Uri mozillaThunderbirdURL = new Uri("https://files.crchq.net/installer/mozillaThunderbird.msi");
        string mozillaThunderbirdFilename = @"C:\Computer Repair Centre\apps\mozillaThunderbird.msi";
        Uri nanaZipURL = new Uri("https://files.crchq.net/installer/NanaZip_3.1.1080.0.msixbundle");
        string nanaZipFilename = @"C:\Computer Repair Centre\apps\NanaZip_3.1.1080.0.msixbundle";
        Uri steamURL = new Uri("https://files.crchq.net/installer/steam.exe");
        string steamFilename = @"C:\Computer Repair Centre\apps\steam.exe";
        Uri hpHotkeySupportURL = new Uri("https://files.crchq.net/installer/HPHotkey.zip");
        string hpHotkeySupportFilename = @"C:\Computer Repair Centre\apps\hpHotkeySupport.zip";

        string nanaZipPath = @"C:\Program Files\WindowsApps\40174MouriNaruto.NanaZip_5.0.1252.0_x64__gnj4mf6z9tkrc\NanaZip.Windows.exe";

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

        private async void install_Click(object sender, EventArgs e)
        {
            progressBar.Maximum = 0;
            string rootDir = @"C:\Computer Repair Centre";
            string oemDir = @"C:\Computer Repair Centre\oem";
            string appsDir = @"C:\Computer Repair Centre\apps";
            string bingWallpaperAppPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\BingWallpaperApp\BingWallpaperApp.exe");
            string discordAppPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Discord\Update.exe");
            string desktopPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop));
            string launcherPath = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), @"Computer Repair Centre Installer Launcher.exe");

            installerTextBox.AppendText($"Last updated on {updateDate}.");
            installerTextBox.AppendText(Environment.NewLine);


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

            using (RegistryKey key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
            {
                if (key != null)
                {
                    string buildNumber = key.GetValue("CurrentBuild")?.ToString();
                    if (int.TryParse(buildNumber, out int build))
                    {
                        if (build >= 22000)
                        {
                            progressBar.Maximum += 8;
                            if (romsey == "1") { progressBar.Maximum += 1; };
                            if (highcliffe == "1") { progressBar.Maximum += 1; };
                            installerTextBox.AppendText("This computer is running Windows 11.");
                            installerTextBox.AppendText(Environment.NewLine);
                        }
                        else if (build >= 19041)
                        {
                            progressBar.Maximum += 8;
                            installerTextBox.AppendText("This computer is running Windows 10.");
                            installerTextBox.AppendText(Environment.NewLine);
                        }
                    }
                }
            }
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
            if (microsoftOffice2007Check.Checked) { progressBar.Maximum += 2; }
            if (mozillaFirefoxCheck.Checked) { progressBar.Maximum += 2; }
            if (mozillaThunderbirdCheck.Checked) { progressBar.Maximum += 2; }
            if (steamCheck.Checked) { progressBar.Maximum += 2; }
            if (hpEliteBook == "1") { progressBar.Maximum += 4; }

            if (christmas == "1")
            {
                installerTextBox.AppendText("Merry Christmas!");
                installerTextBox.AppendText(Environment.NewLine);
                player = new SoundPlayer(Properties.Resources.christmas);
            }
            else if (halloween == "1")
            {
                installerTextBox.AppendText("Boo! Happy Halloween!");
                installerTextBox.AppendText(Environment.NewLine);
                player = new SoundPlayer(Properties.Resources.halloween);
            }
            else if (valentines == "1")
            { 
                installerTextBox.AppendText("Happy Valentines day!");
                installerTextBox.AppendText(Environment.NewLine);
                player = new SoundPlayer(Properties.Resources.valentines);
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
                player = new SoundPlayer(Properties.Resources.birthday);
            }
            else
            {
                player = new SoundPlayer(Properties.Resources.win98shutdown);
            }

            if (powerCheck.Checked)
            {
                installerTextBox.AppendText("Disable sleep on AC power is selected.");
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
                installerTextBox.AppendText("Computer Repair Centre OEM information is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (romsey == "1")
                {
                    installerTextBox.AppendText("The installer is being run from the Romsey shop.");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("Installing Romsey Computer Repair Centre OEM information...");
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
                    installerTextBox.AppendText("Installing Chandlers Ford Computer Repair Centre OEM information...");
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
                    const string supportHoursData = "Mon-Fri 9:00am-5:00pm - Sat 9:00am-4:00pm";
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
                    installerTextBox.AppendText("Installing Romsey Computer Repair Centre OEM information...");
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
                installerTextBox.AppendText("NanaZip is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\WindowsApps\40174MouriNaruto.NanaZip_5.0.1252.0_x64__gnj4mf6z9tkrc\NanaZip.Windows.exe"))
                {
                    installerTextBox.AppendText("NanaZip is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    using (WebClient wc = new WebClient())
                    {
                        await wc.DownloadFileTaskAsync(nanaZipURL, nanaZipFilename);
                    }
                    installerTextBox.AppendText("Installing NanaZip...");
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
                    installerTextBox.AppendText("Completed installation of NanaZip.");
                    installerTextBox.AppendText(Environment.NewLine); ;
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (anyDeskCheck.Checked)
            {
                installerTextBox.AppendText("AnyDesk is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files (x86)\AnyDeskMSI\AnyDeskMSI.exe"))
                {
                    installerTextBox.AppendText("AnyDesk is already installed, skipping installation.");
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
                    installerTextBox.AppendText("Downloading AnyDesk...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(anyDeskURL, anyDeskFilename);
                    }
                    installerTextBox.AppendText("Installing AnyDesk...");
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
                    installerTextBox.AppendText("Completed installation of AnyDesk.");
                    installerTextBox.AppendText(Environment.NewLine); ;
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (bingWallpapersCheck.Checked)
            {
                installerTextBox.AppendText("Bing Wallpapers is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(bingWallpaperAppPath))
                {
                    installerTextBox.AppendText("Bing Wallpapers is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("Downloading Bing Wallpapers...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(bingWallpapersURL, bingWallpapersFilename);
                    }
                    installerTextBox.AppendText("Installing Bing Wallpapers...");
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
                    installerTextBox.AppendText("Completed installation of Bing Wallpapers.");
                    installerTextBox.AppendText(Environment.NewLine); ;
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (bitDefenderCheck.Checked)
            {
                installerTextBox.AppendText("BitDefender is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\Bitdefender\Bitdefender Security App\seccenter.exe"))
                {
                    installerTextBox.AppendText("BitDefender is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("Downloading BitDefender...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(bitDefenderURL, bitDefenderFilename);
                    }
                    installerTextBox.AppendText("Installing BitDefender...");
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
                    installerTextBox.AppendText("Completed installation of BitDefender.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (discordCheck.Checked)
            {
                installerTextBox.AppendText("Discord is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(discordAppPath))
                {
                    installerTextBox.AppendText("Discord is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("Downloading Discord...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(discordURL, discordFilename);
                    }
                    installerTextBox.AppendText("Installing Discord...");
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
                    installerTextBox.AppendText("Completed installation of Discord.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (googleChromeCheck.Checked)
            {
                installerTextBox.AppendText("Google Chrome is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\Google\Chrome\Application\chrome.exe"))
                {
                    installerTextBox.AppendText("Google Chrome is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("Downloading Google Chrome...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(googleChromeURL, googleChromeFilename);
                    }
                    installerTextBox.AppendText("Installing Google Chrome...");
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
                    installerTextBox.AppendText("Completed installation of Google Chrome.");
                    installerTextBox.AppendText(Environment.NewLine); ;
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (libreOfficeCheck.Checked)
            {
                installerTextBox.AppendText("LibreOffice is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\LibreOffice\program\soffice.exe"))
                {
                    installerTextBox.AppendText("LibreOffice is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("Downloading LibreOffice...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(libreOfficeURL, libreOfficeFilename);
                    }
                    installerTextBox.AppendText("Installing LibreOffice...");
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
                    installerTextBox.AppendText("Completed installation of LibreOffice.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (microsoftOffice2007Check.Checked)
            {
                installerTextBox.AppendText("Microsoft Office 2007 is selected.");
                installerTextBox.AppendText(Environment.NewLine);

                if (System.IO.File.Exists(@"C:\Program Files (x86)\Microsoft Office\Office12\WINWORD.EXE"))
                {
                    installerTextBox.AppendText("Microsoft Office 2007 is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("Downloading Microsoft Office 2007...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(microsoftOffice2007URL, microsoftOffice2007Filename);
                    }
                    installerTextBox.AppendText("Checking if NanaZip is installed...");
                    installerTextBox.AppendText(Environment.NewLine);
                    if (!System.IO.File.Exists(@"C:\Program Files\WindowsApps\40174MouriNaruto.NanaZip_5.0.1252.0_x64__gnj4mf6z9tkrc\NanaZip.Windows.exe"))
                    {
                        installerTextBox.AppendText("NanaZip is already installed, proceeding with extraction.");
                        installerTextBox.AppendText(Environment.NewLine);
                    }
                    else
                    {
                        installerTextBox.AppendText("NanaZip is not installed and is required for extraction.");
                        installerTextBox.AppendText(Environment.NewLine);
                        installerTextBox.AppendText("Downloading NanaZip...");
                        installerTextBox.AppendText(Environment.NewLine);
                        using (WebClient wc = new WebClient())
                        {
                            await wc.DownloadFileTaskAsync(nanaZipURL, nanaZipFilename);
                        }                        installerTextBox.AppendText("Installing NanaZip...");
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
                        installerTextBox.AppendText("Completed installation of NanaZip.");
                        installerTextBox.AppendText(Environment.NewLine);
                    }
                    installerTextBox.AppendText("Extracting Microsoft Office 2007 to the Desktop...");
                    installerTextBox.AppendText(Environment.NewLine);
                    string microsoftOffice2007ExtractPath = Path.Combine(desktopPath, "Microsoft Office 2007");
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
                                    installerTextBox.AppendText("Errors: ");
                                    installerTextBox.AppendText(errors);
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
                    if (!Directory.Exists(microsoftOffice2007ExtractPath))
                    {
                        Directory.CreateDirectory(microsoftOffice2007ExtractPath);
                    }
                    await RunNanaZipExtractionOfficeAsync();
                    installerTextBox.AppendText("Completed extraction of Microsoft Office 2007.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }

            }
            if (mozillaFirefoxCheck.Checked)
            {
                installerTextBox.AppendText("Mozilla Firefox is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\Mozilla Firefox\firefox.exe"))
                {
                    installerTextBox.AppendText("Mozilla Firefox is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("Downloading Mozilla Firefox...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(mozillaFirefoxURL, mozillaFirefoxFilename);
                    }
                    installerTextBox.AppendText("Installing Mozilla Firefox...");
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
                    installerTextBox.AppendText("Completed installation of Mozilla Firefox.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (mozillaThunderbirdCheck.Checked)
            {
                installerTextBox.AppendText("Mozilla Thunderbird is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files\Mozilla Thunderbird\thunderbird.exe"))
                {
                    installerTextBox.AppendText("Mozilla Thunderbird is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("Downloading Mozilla Thunderbird...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(mozillaThunderbirdURL, mozillaThunderbirdFilename);
                    }
                    installerTextBox.AppendText("Installing Mozilla Thunderbird...");
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
                    installerTextBox.AppendText("Completed installation of Mozilla Thunderbird.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (steamCheck.Checked)
            {
                installerTextBox.AppendText("Steam is selected.");
                installerTextBox.AppendText(Environment.NewLine);
                if (System.IO.File.Exists(@"C:\Program Files (x86)\Steam\Steam.exe"))
                {
                    installerTextBox.AppendText("Steam is already installed, skipping installation.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 2, progressBar.Maximum);
                }
                else
                {
                    installerTextBox.AppendText("Downloading Steam...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += wc_progressBarStep;
                        await wc.DownloadFileTaskAsync(steamURL, steamFilename);
                    }
                    installerTextBox.AppendText("Installing Steam...");
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
                    installerTextBox.AppendText("Completed installation of Steam.");
                    installerTextBox.AppendText(Environment.NewLine);
                    progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                }
            }
            if (hpEliteBook == "1")
            {
                installerTextBox.AppendText("The installer is being run on a HP EliteBook.");
                installerTextBox.AppendText(Environment.NewLine);
                installerTextBox.AppendText("Downloading HP Hotkey Support...");
                installerTextBox.AppendText(Environment.NewLine);
                using (WebClient wc = new WebClient())
                {
                    wc.DownloadFileCompleted += wc_progressBarStep;
                    await wc.DownloadFileTaskAsync(hpHotkeySupportURL, hpHotkeySupportFilename);
                }
                installerTextBox.AppendText("Checking if NanaZip is installed...");
                installerTextBox.AppendText(Environment.NewLine);
                if (!System.IO.File.Exists(@"C:\Program Files\WindowsApps\40174MouriNaruto.NanaZip_5.0.1252.0_x64__gnj4mf6z9tkrc\NanaZip.Windows.exe"))
                {
                    installerTextBox.AppendText("NanaZip is already installed, proceeding with extraction.");
                    installerTextBox.AppendText(Environment.NewLine);
                }
                else
                {
                    installerTextBox.AppendText("NanaZip is not installed and is required for extraction.");
                    installerTextBox.AppendText(Environment.NewLine);
                    installerTextBox.AppendText("Downloading NanaZip...");
                    installerTextBox.AppendText(Environment.NewLine);
                    using (WebClient wc = new WebClient())
                    {
                        await wc.DownloadFileTaskAsync(nanaZipURL, nanaZipFilename);
                    }
                    installerTextBox.AppendText("Installing NanaZip...");
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
                    installerTextBox.AppendText("Completed installation of NanaZip.");
                    installerTextBox.AppendText(Environment.NewLine);
                }
                installerTextBox.AppendText("Extracing HP Hotkey Support...");
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
                                installerTextBox.AppendText("Errors: ");
                                installerTextBox.AppendText(errors);
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
                installerTextBox.AppendText("Completed extraction of HP Hotkey Support.");
                installerTextBox.AppendText(Environment.NewLine);
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                installerTextBox.AppendText("Installing HP Hotkey Support...");
                installerTextBox.AppendText(Environment.NewLine);
                async Task InstallHPHotkeySupport()
                {
                    try
                    {
                        ProcessStartInfo processInfo = new ProcessStartInfo
                        {
                            FileName = @"C:\Computer Repair Centre\apps\hpHotkeySupport\SP103615\src\install.cmd",
                            UseShellExecute = false
                        };
                        using (Process process = Process.Start(processInfo))
                        {
                            if (process != null)
                            {
                                await Task.Run(() => process.WaitForExit());
                                Console.WriteLine("Process completed successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to start the process.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
                async Task InstallHPramework()
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
                                Console.WriteLine("Process completed successfully.");
                            }
                            else
                            {
                                Console.WriteLine("Failed to start the process.");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine("An error occurred: " + ex.Message);
                    }
                }
                await InstallHPHotkeySupport();
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                installerTextBox.AppendText("Installing HP Framework...");
                installerTextBox.AppendText(Environment.NewLine);
                await InstallHPramework();
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
                installerTextBox.AppendText("Completed installation of HP Hotkey Support.");
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
                                installerTextBox.AppendText("Aligning the taskbar to the left...");
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
                                installerTextBox.AppendText("Aligning the taskbar to the left...");
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

                            //installerTextBox.AppendText("Enabling end task in the taskbar...");
                            //installerTextBox.AppendText(Environment.NewLine);
                            //const string endTaskRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings";
                            //const string endTaskReg = "TaskbarEndTask";
                            //const int endTaskRegData = 1;
                            //using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(endTaskRegPath, writable: true))
                            //{                             
                            //registryKey.SetValue(endTaskReg, endTaskRegData, RegistryValueKind.DWord);
                            //Console.WriteLine($"Set '{endTaskReg}' to {endTaskRegData} in '{endTaskRegPath}'.");
                            //}
                            //progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);

                            installerTextBox.AppendText("Disabling device encryption...");
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

                            installerTextBox.AppendText("Disabling fastboot mode...");
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

                            installerTextBox.AppendText("Disabling location tracking...");
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

                            installerTextBox.AppendText("Disabling People icon...");
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

                            installerTextBox.AppendText("Hiding recently used files and folders in File Explorer...");
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
                            installerTextBox.AppendText("Setting explorer to open to This PC...");
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

                            installerTextBox.AppendText("Disabling fastboot mode...");
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

                            installerTextBox.AppendText("Disabling location tracking...");
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

                            installerTextBox.AppendText("Disabling People icon...");
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

                            installerTextBox.AppendText("Hiding recently used files and folders in File Explorer...");
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
                installerTextBox.AppendText("Re-enabling sleep and screen timeout on AC power...");
                installerTextBox.AppendText(Environment.NewLine);
                Process.Start("powercfg", "/change monitor-timeout-ac 10");
                Process.Start("powercfg", "/change standby-timeout-ac 20");
                progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);
            }

            installerTextBox.AppendText("Cleaning up installation files...");
            installerTextBox.AppendText(Environment.NewLine);
            var deletionHelper = new FileDeletionHelper();
            await deletionHelper.DeleteFilesAndDirectoryAsync(appsDir, launcherPath);
            progressBar.Value = Math.Min(progressBar.Value + 1, progressBar.Maximum);

            player.Play();

            if (restartCheck.Checked)
            {
                Process.Start("shutdown", "/r /t 60");
                installerTextBox.AppendText("System will restart in 60 seconds. If you need to cancel this press the close button.");
                installerTextBox.AppendText(Environment.NewLine);
            }

            installerTextBox.AppendText("The installation has completed.");
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

        private void close_Click(object sender, EventArgs e)
        {
            Process.Start("shutdown", "/a");
            this.Close();
        }

        private void restart_Click(object sender, EventArgs e)
        {
            Process.Start("shutdown","/r /t 1");
        }

        private void versionLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/charliehoward/PlutoPoint-Installer/blob/main/README.md");
        }
    }
}