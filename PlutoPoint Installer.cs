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

namespace PlutoPoint_Installer
{
    public partial class installerForm : Form
    {
        public installerForm()
        {
            InitializeComponent();
        }

        Uri crcOEMURL = new Uri("https://github.com/charliehoward/PlutoPoint-Installer/raw/master/Resources/computerRepairCentre/computerRepairCentreOEM.bmp");
        string crcOEMFilename = @"C:\Computer Repair Centre\oem\computerRepairCentreOEM.bmp";
        Uri bingWallpapersURL = new Uri("https://files.crchq.net/installer/bingWallpapers.msi");
        string bingWallpapersFilename = @"C:\Computer Repair Centre\apps\bingWallpapers.msi";
        Uri googleChromeURL = new Uri("https://files.crchq.net/installer/chrome.msi");
        string googleChromeFilename = @"C:\Computer Repair Centre\apps\chrome.msi";
        Uri libreOfficeURL = new Uri("https://files.crchq.net/installer/libreOffice.msi");
        string libreOfficeFilename = @"C:\Computer Repair Centre\apps\libreOffice.msi";
        Uri mozillaFirefoxURL = new Uri("https://files.crchq.net/installer/firefox.msi");
        string mozillaFirefoxFilename = @"C:\Computer Repair Centre\apps\firefox.msi";

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
        private async void install_Click(object sender, EventArgs e)
        {
            string rootDir = @"C:\Computer Repair Centre";
            string oemDir = @"C:\Computer Repair Centre\oem";
            string appsDir = @"C:\Computer Repair Centre\apps";
            string scriptsDir = @"C:\Computer Repair Centre\scripts";
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
            if (!Directory.Exists(scriptsDir))
            {
                Directory.CreateDirectory(scriptsDir);
            }

            string publicIP = await GetPublicIPAddressAsync();
            string romsey = null;
            string chandlersFord = null;
            string highcliffe = null;
            if (publicIP != null)
            {
                if (publicIP == "81.134.32.116")
                {
                    romsey = "1";
                }
                if (publicIP == "86.12.18.92")
                {
                    chandlersFord = "1";
                }
                if (publicIP == "81.130.137.162")
                {
                    highcliffe = "1";
                }
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
                            progressBar.Maximum += 9;
                            if (romsey == "1") { progressBar.Maximum += 1; };
                            if (highcliffe == "1") { progressBar.Maximum += 1; };

                        }
                        else if (build >= 19041)
                        {
                            progressBar.Maximum += 0;
                        }
                    }
                }
            }
            if (crcCheck.Checked) { progressBar.Maximum += 1; }
            if (bingWallpapersCheck.Checked) { progressBar.Maximum += 2; }
            if (googleChromeCheck.Checked) { progressBar.Maximum += 2; }
            if (libreOfficeCheck.Checked) { progressBar.Maximum += 2; }
            if (mozillaFirefoxCheck.Checked) { progressBar.Maximum += 2; }

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
                    const string logoRegData = @"C:\Computer Repair Centre\computerRepairCentreOEM.bmp";
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
            }
            if (bingWallpapersCheck.Checked)
            {
                installerTextBox.AppendText("Bing Wallpapers is selected.");
                installerTextBox.AppendText(Environment.NewLine);
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
                installerTextBox.AppendText("Completed installation of Bing Wallpapers.");
                installerTextBox.AppendText(Environment.NewLine); ;
                progressBar.Value += 1;
            }
            if (googleChromeCheck.Checked)
            {
                installerTextBox.AppendText("Google Chrome is selected.");
                installerTextBox.AppendText(Environment.NewLine);
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
                progressBar.Value += 1;
            }
            if (libreOfficeCheck.Checked)
            {
                installerTextBox.AppendText("LibreOffice is selected.");
                installerTextBox.AppendText(Environment.NewLine);
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
                progressBar.Value += 1;
            }
            if (mozillaFirefoxCheck.Checked)
            {
                installerTextBox.AppendText("Mozilla Firefox is selected.");
                installerTextBox.AppendText(Environment.NewLine);
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
                progressBar.Value += 1;
            }


            if (restartCheck.Checked)
            {
                Process.Start("shutdown", "/r /t 60");
                installerTextBox.AppendText("System will restart in 60 seconds. Please save any data.");
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
                            installerTextBox.AppendText("This computer is running Windows 11.");
                            installerTextBox.AppendText(Environment.NewLine);

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
                                progressBar.Value += 1;
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
                                progressBar.Value += 1;
                            }
                            installerTextBox.AppendText("Enabling end task in the taskbar...");
                            installerTextBox.AppendText(Environment.NewLine);
                            const string endTaskRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings";
                            const string endTaskReg = "TaskbarEndTask";
                            const int endTaskRegData = 1;
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(endTaskRegPath, writable: true))
                            {                             
                            registryKey.SetValue(endTaskReg, endTaskRegData, RegistryValueKind.DWord);
                            Console.WriteLine($"Set '{endTaskReg}' to {endTaskRegData} in '{endTaskRegPath}'.");
                            }
                            progressBar.Value += 1;

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
                            progressBar.Value += 1;

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
                            progressBar.Value += 1;

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
                            progressBar.Value += 1;
                            using (RegistryKey registryKey = Registry.LocalMachine.OpenSubKey(locationRegPath2, writable: true))
                            {
                                registryKey.SetValue(locationReg2, locationRegData2, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{locationReg2}' to {locationRegData2} in '{locationRegPath2}'.");
                            }
                            progressBar.Value += 1;

                            installerTextBox.AppendText("Disabling People icon...");
                            installerTextBox.AppendText(Environment.NewLine);
                            const string peopleRegPath1 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People";
                            const string peopleRegPath2 = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People";
                            const string peopleReg2 = "PeopleBand";
                            const int peopleRegData2 = 0;
                            using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(peopleRegPath1, writable: true))
                            {
                            }
                            progressBar.Value += 1;
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(peopleRegPath2, writable: true))
                            {
                                registryKey.SetValue(peopleReg2, peopleRegData2, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{peopleReg2}' to {peopleRegData2} in '{peopleRegPath2}'.");
                            }
                            progressBar.Value += 1;

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
                            progressBar.Value += 1;
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(recentRegPath, writable: true))
                            {
                                registryKey.SetValue(frequentReg, frequentRegData, RegistryValueKind.DWord);
                                Console.WriteLine($"Set '{frequentReg}' to {frequentRegData} in '{recentRegPath}'.");
                            }
                            progressBar.Value += 1;

                        }
                        else if (build >= 19041)
                        {
                            installerTextBox.AppendText("This computer is running Windows 10.");
                            installerTextBox.AppendText(Environment.NewLine);
                        }
                        else
                        {
                            installerTextBox.AppendText("This computer is running an old version of Windows, please update it.");
                            installerTextBox.AppendText(Environment.NewLine);
                        }
                    }
                }
            }

        }
        private void wc_progressBarStep(object sender, AsyncCompletedEventArgs e)
        {
            if (progressBar.InvokeRequired)
            {
                progressBar.Invoke((MethodInvoker)delegate { progressBar.Value += 1; });
            }
            else
            {
                progressBar.Value += 1;
            }
        }

        private void close_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void restart_Click(object sender, EventArgs e)
        {
            Process.Start("shutdown","/r /t 5");
        }

        private void versionLabel_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            System.Diagnostics.Process.Start("https://github.com/charliehoward/PlutoPoint-Installer/blob/main/README.md");
        }
    }
}