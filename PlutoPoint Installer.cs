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

namespace PlutoPoint_Installer
{
    public partial class installerForm : Form
    {
        public installerForm()
        {
            InitializeComponent();
        }

        Uri bingWallpapersURL = new Uri("https://files.crchq.net/installer/bingWallpapers.msi");
        string bingWallpapersFilename = @"C:\Computer Repair Centre\apps\bingWallpapers.msi";
        Uri googleChromeURL = new Uri("https://files.crchq.net/installer/chrome.msi");
        string googleChromeFilename = @"C:\Computer Repair Centre\apps\chrome.msi";
        Uri libreOfficeURL = new Uri("https://files.crchq.net/installer/libreOffice.msi");
        string libreOfficeFilename = @"C:\Computer Repair Centre\apps\libreOffice.msi";
        Uri mozillaFirefoxURL = new Uri("https://files.crchq.net/installer/firefox.msi");
        string mozillaFirefoxFilename = @"C:\Computer Repair Centre\apps\firefox.msi";

        private async void install_Click(object sender, EventArgs e)
        {
            string rootDir = @"C:\Computer Repair Centre";
            string appsDir = @"C:\Computer Repair Centre\apps";
            string scriptsDir = @"C:\Computer Repair Centre\scripts";
            if (!Directory.Exists(rootDir))
            {
                Directory.CreateDirectory(rootDir);
            }
            if (!Directory.Exists(appsDir))
            {
                Directory.CreateDirectory(appsDir);
            }
            if (!Directory.Exists(scriptsDir))
            {
                Directory.CreateDirectory(scriptsDir);
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
                            progressBar.Maximum += 2;
                        }
                        else if (build >= 19041)
                        {
                            progressBar.Maximum += 2;
                        }
                    }
                }
            }
            if (bingWallpapersCheck.Checked) { progressBar.Maximum += 2; }
            if (googleChromeCheck.Checked) { progressBar.Maximum += 2; }
            if (libreOfficeCheck.Checked) { progressBar.Maximum += 2; }
            if (mozillaFirefoxCheck.Checked) { progressBar.Maximum += 2; }         

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
                            installerTextBox.AppendText("Enabling end task in the taskbar.");
                            installerTextBox.AppendText(Environment.NewLine);
                            const string path = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\TaskbarDeveloperSettings";
                            const string valueName = "TaskbarEndTask";
                            const int valueData = 1;
                            using (RegistryKey registryKey = Registry.CurrentUser.OpenSubKey(path, writable: true))
                            {                             
                            registryKey.SetValue(valueName, valueData, RegistryValueKind.DWord);
                            Console.WriteLine($"Set '{valueName}' to {valueData} in '{path}'.");
                            }
                        }
                        else if (build >= 19041)
                        {
                            installerTextBox.AppendText("This computer is running Windows 10.");
                            installerTextBox.AppendText(Environment.NewLine); ;
                        }
                        else
                        {
                            installerTextBox.AppendText("This computer is running an old version of Windows, please update it.");
                            installerTextBox.AppendText(Environment.NewLine); ;
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