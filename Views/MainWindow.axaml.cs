using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.Http;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Threading;
using Microsoft.Win32;
using PlutoPoint_Installer.Models;
using PlutoPoint_Installer.Utilities;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer.Views
{
    public partial class MainWindow : Window
    {
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern uint SHEmptyRecycleBin(IntPtr hwnd, string pszRootPath, uint dwFlags);
        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;

        private bool? _wingetAvailable;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Set strings
        private string safeLocation = null;
        private string location = null;
        private string romsey = null;
        private string chandlersFord = null;
        private string highcliffe = null;
        private string charlieHome = null;
        private string locationLine = null;
        private string windows10 = null;
        private string windows11 = null;
        private string nvidia = null;
        private string intel = null;
        private string amd = null;
        private string rootDir;
        private string programDataDir;
        private DownloadUrls urls;
        private Task _initialiseUrlsTask;
        private Task _checkIpTask;
        private List<(string Name, string Status)> installResults;
        private string _logFilePath;
        private readonly PlutoPoint_Installer.UI.ThemeManager _themeManager = new PlutoPoint_Installer.UI.ThemeManager();
        DateTime buildDate = File.GetLastWriteTime(Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location);

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
        private Uri vlcMediaPlayerURL => new Uri(urls.vlcMediaPlayer);

        public MainWindow()
        {
            InitializeComponent();
            // Shutdown/restart checks
            WireCheckboxMutualExclusion();
            // Hover
            InstallButton.PointerEntered += (s, e) => AudioEffects.PlayHoverPop();
            RestartButton.PointerEntered += (s, e) => AudioEffects.PlayHoverPop();
            CloseButton.PointerEntered += (s, e) => AudioEffects.PlayHoverPop();
            // Click
            InstallButton.Click += (s, e) => AudioEffects.PlayClickChime();
            RestartButton.Click += (s, e) => AudioEffects.PlayClickChime();
            CloseButton.Click += (s, e) => AudioEffects.PlayClickChime();
            // Background tasks only
            _initialiseUrlsTask = InitialiseUrlsAsync();
            _checkIpTask = CheckIPAsync();
            // Info checks
            PrintVersion();
            CheckWindowsVersion();
            CheckForIntelHardware();
            CheckforAMDHardware();
            CheckForNvidiaGPU();
            AppendLine(locationLine);
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            _themeManager.ApplyThemeAndMessages(
                (top, bottom, log) =>
                {
                    if (Background is LinearGradientBrush gradientBrush && gradientBrush.GradientStops.Count >= 2)
                    {
                        gradientBrush.GradientStops[0].Color = top;
                        gradientBrush.GradientStops[1].Color = bottom;
                    }
                    InstallerTextBox.Foreground = new SolidColorBrush(log);
                },
                (msg) => AppendLine(msg)
            );
            _themeManager.ApplyButtonTheme(
                (backColor, foreColor) =>
                {
                    var backBrush = new SolidColorBrush(backColor);
                    var foreBrush = new SolidColorBrush(foreColor);
                    InstallButton.Background = backBrush;
                    InstallButton.Foreground = foreBrush;
                    RestartButton.Background = backBrush;
                    RestartButton.Foreground = foreBrush;
                    CloseButton.Background = backBrush;
                    CloseButton.Foreground = foreBrush;
                }
            );
            _themeManager.UpdateGUIEvent(this, SeasonalOverlayImage);
        }

        public void RunSilentCommand(string fileName, string arguments)
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            };
            using (var process = Process.Start(startInfo))
            {
                process.WaitForExit();
            }
        }

        private void EnsureLogFile()
        {
            if (_logFilePath != null)
                return;
            try
            {
                string logDir = Path.Combine(
                    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                    "ComputerRepairCentre", "Logs");
                Directory.CreateDirectory(logDir);
                _logFilePath = Path.Combine(logDir, $"install-log-{DateTime.Now:yyyyMMdd-HHmmss}.txt");
            }
            catch
            {
                _logFilePath = string.Empty;
            }
        }

        private void AppendLine(string text = "")
        {
            void Write()
            {
                InstallerTextBox.Text += text + Environment.NewLine;
                LogScrollViewer.ScrollToEnd();
            }

            if (Dispatcher.UIThread.CheckAccess())
                Write();
            else
                Dispatcher.UIThread.Post(Write);

            EnsureLogFile();
            if (!string.IsNullOrEmpty(_logFilePath))
            {
                try
                {
                    File.AppendAllText(_logFilePath, $"[{DateTime.Now:HH:mm:ss}] {text}{Environment.NewLine}");
                }
                catch
                {
                }
            }
        }

        private void TrackResult(string name, string status)
        {
            installResults?.Add((name, status));
        }

        private void AppendInstallSummary()
        {
            if (installResults == null || installResults.Count == 0)
                return;
            int total = installResults.Count;
            int succeeded = installResults.Count(r => r.Status == "Installed" || r.Status == "Already Installed" || r.Status == "Applied");
            int skipped = installResults.Count(r => r.Status == "Skipped");
            int failed = installResults.Count(r => r.Status == "Failed");
            AppendLine("");
            AppendLine("📋 Summary:");
            AppendLine($"✅ {succeeded}/{total} succeeded");
            if (skipped > 0) AppendLine($"⏭️ {skipped} skipped");
            if (failed > 0)
            {
                AppendLine($"❌ {failed} failed:");
                foreach (var r in installResults.Where(r => r.Status == "Failed"))
                    AppendLine($"   • {r.Name}");
            }
        }

        private static string ComputeSHA256(string input)
        {
            using (SHA256 sha = SHA256.Create())
            {
                byte[] bytes = sha.ComputeHash(Encoding.UTF8.GetBytes(input));
                var builder = new StringBuilder();
                foreach (byte b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }

        private void PrintVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string versionStr = assembly
                .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
            if (string.IsNullOrWhiteSpace(versionStr))
                versionStr = assembly.GetName().Version?.ToString() ?? "";
            string versionText = $"Version {versionStr}";
            if (versionStr.Replace(".", "").Contains("69"))
            {
                versionText += " - Nice";
            }

            string WithDaySuffix(int day)
            {
                if (day >= 11 && day <= 13) return day + "th";
                switch (day % 10)
                {
                    case 1: return day + "st";
                    case 2: return day + "nd";
                    case 3: return day + "rd";
                    default: return day + "th";
                }
            }

            string formatted = string.Format("{0} of {1} {2}",
                WithDaySuffix(buildDate.Day),
                buildDate.ToString("MMMM"),
                buildDate.Year);

            VersionLabel.Text = versionText;
            AppendLine($"🛠️ {versionText}");
            AppendLine("📅 Last updated on " + formatted + ".");
        }

        private void WireCheckboxMutualExclusion()
        {
            ShutdownCheck.IsCheckedChanged += (_, _) =>
            {
                if (ShutdownCheck.IsChecked == true)
                    RestartCheck.IsChecked = false;
            };
            RestartCheck.IsCheckedChanged += (_, _) =>
            {
                if (RestartCheck.IsChecked == true)
                    ShutdownCheck.IsChecked = false;
            };
        }

        private async void CloseButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await System.Threading.Tasks.Task.Delay(325);
            Process.Start("shutdown", "/a");
            Close();
        }

        private async void RestartButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await System.Threading.Tasks.Task.Delay(325);
            Process.Start("shutdown", "/r /t 1");
        }

        private void TestButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            AppendLine("❌ No current tests. You nosey bastard.");
        }

        private void VersionLabel_PointerPressed(object sender, PointerPressedEventArgs e)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = "https://professorshroom.com/projects/PlutoPoint_Installer/#changelog",
                UseShellExecute = true
            });
        }

        private void CheckWindowsVersion()
        {
            try
            {
                using (var key = Microsoft.Win32.Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion"))
                {
                    if (key != null)
                    {
                        string buildNumber = key.GetValue("CurrentBuild")?.ToString();
                        if (int.TryParse(buildNumber, out int build))
                        {
                            string versionText;
                            string osLabelText;
                            if (build >= 22000)
                            {
                                versionText = "🪟 Windows 11 detected.";
                                osLabelText = "🪟 Windows 11";
                                windows11 = "1";
                            }
                            else if (build >= 10240)
                            {
                                versionText = "🪟 Windows 10 detected. Time to move on grandad.";
                                osLabelText = "🪟 Windows 10";
                                windows10 = "1";
                            }
                            else
                            {
                                versionText = "🪟 Older or unknown Windows version detected.";
                                osLabelText = "🪟 Unknown OS";
                            }
                            AppendLine(versionText);
                            void Write() => OsLabel.Text = osLabelText;
                            if (Dispatcher.UIThread.CheckAccess())
                                Write();
                            else
                                Dispatcher.UIThread.Post(Write);
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
                        NvidiaAppCheck.IsChecked = true;
                        nvidia = "1";
                        AppendLine("🎮 Nvidia GPU detected.");
                        return;
                    }
                }
            }
            NvidiaAppCheck.IsChecked = false;
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

        private static string HashIP(string ip)
        {
            using (var sha256 = SHA256.Create())
            {
                byte[] bytes = Encoding.UTF8.GetBytes(ip.Trim());
                byte[] hash = sha256.ComputeHash(bytes);
                return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            }
        }

        private async Task<PasswordHashes> GetPasswordHashesAsync()
        {
            try
            {
                string url = "https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/Data/Passwords.json";
                using (var client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    return JsonSerializer.Deserialize<PasswordHashes>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                AppendLine($"❌ Failed to load hashes: {ex.Message}");
                return null;
            }
        }

        private async Task<LocationHashes> GetLocationHashesAsync()
        {
            try
            {
                string url = "https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/Data/IPs.json";
                using (var client = new HttpClient())
                {
                    string json = await client.GetStringAsync(url);
                    return JsonSerializer.Deserialize<LocationHashes>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                AppendLine($"❌ Failed to load hashes: {ex.Message}");
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
        }

        private void UpdateLocation()
        {
            if (romsey == "1")
                location = "Romsey";
            else if (chandlersFord == "1")
                location = "Chandler's Ford";
            else if (highcliffe == "1")
                location = "Highcliffe";
            else if (charlieHome == "1")
                location = "Charlie's House";
            else
                location = "Unknown";

            void Write()
            {
                LocationLabel.Text = "Current location: " + location;
                foreach (var item in LocationOverrideComboBox.Items)
                {
                    if (item is ComboBoxItem cbi && (cbi.Content?.ToString() == location))
                    {
                        LocationOverrideComboBox.SelectedItem = item;
                        break;
                    }
                }
            }
            if (Dispatcher.UIThread.CheckAccess())
                Write();
            else
                Dispatcher.UIThread.Post(Write);
        }

        private string SelectedOemLocation =>
            (LocationOverrideComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString();

        private async Task InitialiseUrlsAsync()
        {
            urls = await GetDownloadUrlsAsync();
            if (urls == null)
            {
                AppendLine("❌ Failed to load download URLs.");
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
                    return JsonSerializer.Deserialize<DownloadUrls>(json, _jsonOptions);
                }
            }
            catch (Exception ex)
            {
                AppendLine($"❌ Failed to load URLs: {ex.Message}");
                return null;
            }
        }

        private async Task<bool> PassesPasswordGateAsync()
        {
            if (safeLocation != "0")
                return true;

            var hashes = await GetPasswordHashesAsync();
            if (hashes?.allowedHashes == null || hashes.allowedHashes.Count == 0)
            {
                AppendLine("❌ Unable to load password hashes.");
                return false;
            }

            var dialog = new PasswordDialog();
            bool? result = await dialog.ShowDialog<bool?>(this);
            if (result == true)
            {
                string enteredHash = ComputeSHA256(dialog.EnteredPassword);
                if (!hashes.allowedHashes.Contains(enteredHash))
                {
                    AppendLine("❌ Incorrect password. Exiting.");
                    Close();
                    return false;
                }
                return true;
            }

            AppendLine("❌ Password required. Exiting installer.");
            Environment.Exit(0);
            return false;
        }

        private string ResolveShortcutPath(string shortcutFileName)
        {
            string[] searchDirs =
            {
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonStartMenu), "Programs"),
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.StartMenu), "Programs")
            };
            foreach (var dir in searchDirs)
            {
                try
                {
                    if (!Directory.Exists(dir)) continue;
                    var matches = Directory.GetFiles(dir, shortcutFileName, SearchOption.AllDirectories);
                    if (matches.Length > 0)
                        return matches[0];
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error searching {dir} for {shortcutFileName}: {ex.Message}");
                }
            }
            return null;
        }

        private void CopyShortcutToDesktop(string sourceLnkPath, string label)
        {
            try
            {
                string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                string destPath = Path.Combine(desktopPath, Path.GetFileName(sourceLnkPath));
                File.Copy(sourceLnkPath, destPath, overwrite: true);
                AppendLine($"✅ Added {label} shortcut to Desktop.");
            }
            catch (Exception ex)
            {
                AppendLine($"⚠️ Failed to add {label} shortcut to Desktop: {ex.Message}");
            }
        }

        private bool AddPinEntry(StringBuilder sb, bool wasChecked, string shortcutFileName, string label, bool alsoAddToDesktop = false)
        {
            if (!wasChecked)
                return false;
            string lnkPath = ResolveShortcutPath(shortcutFileName);
            if (lnkPath == null)
            {
                AppendLine($"⚠️ Could not find {label} shortcut to pin, skipping.");
                return false;
            }
            sb.AppendLine($"        <taskbar:DesktopApp DesktopApplicationLinkPath=\"{lnkPath}\" />");
            if (alsoAddToDesktop)
                CopyShortcutToDesktop(lnkPath, label);
            return true;
        }

        private void ScheduleLayoutFileCleanup(string layoutPath)
        {
            string vbsPath = Path.Combine(Path.GetTempPath(), "PlutoPointTaskbarCleanup.vbs");
            string vbsContent =
                "Set objShell = CreateObject(\"WScript.Shell\")\r\n" +
                $"objShell.Run \"cmd.exe /c ping -n 16 127.0.0.1 >nul & del \"\"{layoutPath}\"\"\", 0, True\r\n" +
                "Set objFSO = CreateObject(\"Scripting.FileSystemObject\")\r\n" +
                "On Error Resume Next\r\n" +
                "objFSO.DeleteFile WScript.ScriptFullName, True\r\n";
            File.WriteAllText(vbsPath, vbsContent);
            string cmd = $"wscript.exe //B //Nologo \"{vbsPath}\"";
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\RunOnce", writable: true))
            {
                key?.SetValue("PlutoPointTaskbarLayoutCleanup", cmd, RegistryValueKind.String);
            }
        }

        private void ApplyTaskbarPinLayout()
        {
            var pins = new StringBuilder();
            int pinnedCount = 0;
            if (AddPinEntry(pins, GoogleChromeCheck.IsChecked == true, "Google Chrome.lnk", "Google Chrome")) pinnedCount++;
            if (AddPinEntry(pins, MozillaFirefoxCheck.IsChecked == true, "Firefox.lnk", "Mozilla Firefox")) pinnedCount++;
            if (AddPinEntry(pins, MozillaThunderbirdCheck.IsChecked == true, "Thunderbird.lnk", "Mozilla Thunderbird")) pinnedCount++;
            if (AddPinEntry(pins, LibreOfficeCheck.IsChecked == true, "LibreOffice Writer.lnk", "LibreOffice Writer", alsoAddToDesktop: true)) pinnedCount++;
            if (AddPinEntry(pins, LibreOfficeCheck.IsChecked == true, "LibreOffice Calc.lnk", "LibreOffice Calc", alsoAddToDesktop: true)) pinnedCount++;
            if (AddPinEntry(pins, true, "File Explorer.lnk", "File Explorer")) pinnedCount++;
            if (pinnedCount == 0)
            {
                AppendLine("⚠️ No apps available to pin, skipping taskbar layout.");
                return;
            }
            string xml = $@"<?xml version=""1.0"" encoding=""utf-8""?>
<LayoutModificationTemplate
    xmlns=""http://schemas.microsoft.com/Start/2014/LayoutModification""
    xmlns:defaultlayout=""http://schemas.microsoft.com/Start/2014/FullDefaultLayout""
    xmlns:taskbar=""http://schemas.microsoft.com/Start/2014/TaskbarLayout""
    Version=""1"">
  <CustomTaskbarLayoutCollection PinListPlacement=""Replace"">
    <defaultlayout:TaskbarLayout>
      <taskbar:TaskbarPinList>
{pins}      </taskbar:TaskbarPinList>
    </defaultlayout:TaskbarLayout>
  </CustomTaskbarLayoutCollection>
</LayoutModificationTemplate>";
            string shellDir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                @"Microsoft\Windows\Shell");
            Directory.CreateDirectory(shellDir);
            string layoutPath = Path.Combine(shellDir, "LayoutModification.xml");
            File.WriteAllText(layoutPath, xml);
            using (RegistryKey key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer", writable: true))
            {
                key?.DeleteSubKeyTree("Taskband", throwOnMissingSubKey: false);
            }
            ScheduleLayoutFileCleanup(layoutPath);
            AppendLine($"✅ Taskbar layout built with {pinnedCount} app(s), will apply after next reboot or sign-in.");
        }

        private void SetOemInfo(string hours, string phone, string url)
        {
            const string oemRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation";
            const string logoRegData = @"C:\ProgramData\Computer Repair Centre\OEM\computerRepairCentreOEM.bmp";
            const string manufacturerRegData = "Computer Repair Centre";
            using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(oemRegPath, writable: true))
            {
                registryKey.SetValue("Logo", logoRegData, RegistryValueKind.String);
                registryKey.SetValue("Manufacturer", manufacturerRegData, RegistryValueKind.String);
                registryKey.SetValue("SupportHours", hours, RegistryValueKind.String);
                registryKey.SetValue("SupportPhone", phone, RegistryValueKind.String);
                registryKey.SetValue("SupportURL", url, RegistryValueKind.String);
                Console.WriteLine($"Set OEM info in '{oemRegPath}': hours='{hours}', phone='{phone}', url='{url}'.");
            }
        }

        private void ProgressBarStep(object sender, AsyncCompletedEventArgs e)
        {
            void Step() => InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
            if (Dispatcher.UIThread.CheckAccess())
                Step();
            else
                Dispatcher.UIThread.Post(Step);
        }

        private async Task DownloadWithRetryAsync(Uri url, string destinationPath, int maxAttempts = 3)
        {
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
#pragma warning disable SYSLIB0014 // WebClient is obsolete; kept for behavioral parity with the original
                    using (WebClient wc = new WebClient())
                    {
                        wc.DownloadFileCompleted += ProgressBarStep;
                        await wc.DownloadFileTaskAsync(url, destinationPath);
                    }
#pragma warning restore SYSLIB0014
                    return;
                }
                catch (Exception ex) when (attempt < maxAttempts)
                {
                    AppendLine($"⚠️ Download failed (attempt {attempt}/{maxAttempts}): {ex.Message}. Retrying...");
                    await Task.Delay(1500 * attempt);
                }
            }
        }

        private void KillProcessSafely(Process process)
        {
            try
            {
                if (!process.HasExited)
                    process.Kill();
            }
            catch
            {
            }
        }

        private bool IsWingetAvailable()
        {
            if (_wingetAvailable.HasValue)
                return _wingetAvailable.Value;

            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "winget";
                    process.StartInfo.Arguments = "--version";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = true;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    bool exited = process.WaitForExit(5000);
                    if (!exited)
                    {
                        KillProcessSafely(process);
                        _wingetAvailable = false;
                    }
                    else
                    {
                        _wingetAvailable = process.ExitCode == 0;
                    }
                }
            }
            catch
            {
                _wingetAvailable = false;
            }
            if (_wingetAvailable == true)
            {
                try
                {
                    using (Process warmup = new Process())
                    {
                        warmup.StartInfo.FileName = "winget";
                        warmup.StartInfo.Arguments = "list --accept-source-agreements --accept-package-agreements";
                        warmup.StartInfo.UseShellExecute = false;
                        warmup.StartInfo.RedirectStandardOutput = true;
                        warmup.StartInfo.RedirectStandardError = true;
                        warmup.StartInfo.CreateNoWindow = true;
                        warmup.Start();
                        bool exited = warmup.WaitForExit(60000);
                        if (!exited)
                        {
                            KillProcessSafely(warmup);
                            AppendLine("⚠️ winget did not finish accepting source agreements (first-run initialisation), using direct downloads for everything.");
                            _wingetAvailable = false;
                        }
                    }
                }
                catch
                {
                    _wingetAvailable = false;
                }
            }
            if (_wingetAvailable != true)
            {
                _wingetAvailable = false;
                AppendLine("⚠️ winget is not available on this machine (or not accessible from this context); using direct downloads for everything.");
            }
            return _wingetAvailable.Value;
        }

        private bool IsInstalledViaWinget(string packageId)
        {
            try
            {
                using (Process process = new Process())
                {
                    process.StartInfo.FileName = "winget";
                    process.StartInfo.Arguments = $"list -e --id \"{packageId}\" --accept-source-agreements";
                    process.StartInfo.UseShellExecute = false;
                    process.StartInfo.RedirectStandardOutput = true;
                    process.StartInfo.RedirectStandardError = false;
                    process.StartInfo.CreateNoWindow = true;
                    process.Start();
                    string output = process.StandardOutput.ReadToEnd();
                    bool exited = process.WaitForExit(15000);
                    if (!exited)
                    {
                        KillProcessSafely(process);
                        return false;
                    }
                    return output.IndexOf(packageId, StringComparison.OrdinalIgnoreCase) >= 0;
                }
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> TryWingetInstallAsync(string packageId, string label)
        {
            if (!IsWingetAvailable())
                return false;
            AppendLine($"🔄 Attempting winget install of {label}...");
            bool timedOut = false;
            try
            {
                await Task.Run(() =>
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "winget";
                        process.StartInfo.Arguments =
                            $"install -e --id \"{packageId}\" --silent --disable-interactivity " +
                            "--accept-package-agreements --accept-source-agreements";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.RedirectStandardOutput = false;
                        process.StartInfo.RedirectStandardError = false;
                        process.StartInfo.CreateNoWindow = true;
                        process.Start();
                        bool exited = process.WaitForExit(180000); // 3 minutes
                        if (!exited)
                        {
                            timedOut = true;
                            KillProcessSafely(process);
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                AppendLine($"⚠️ winget install of {label} threw an error ({ex.Message}), falling back to direct download.");
                return false;
            }
            if (timedOut)
            {
                AppendLine($"⚠️ winget install of {label} timed out after 3 minutes and was terminated, falling back to direct download.");
                return false;
            }
            bool confirmed = IsInstalledViaWinget(packageId);
            if (confirmed)
            {
                AppendLine($"✅ {label} installed via winget.");
            }
            else
            {
                AppendLine($"⚠️ winget install of {label} could not be verified, falling back to direct download.");
            }
            return confirmed;
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

        private async void InstallButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            installResults = new List<(string, string)>();
            if (!await PassesPasswordGateAsync())
                return;

            InstallProgressBar.Maximum = 0;

            // Paths
            rootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ComputerRepairCentre");
            programDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string oemDir = Path.Combine(programDataDir, "Computer Repair Centre\\OEM\\");
            string appsDir = Path.Combine(rootDir, "apps");
            string windowsAppsPath = @"C:\Program Files\WindowsApps";

            // Installed apps
            string googleChromeExePath = @"C:\Program Files\Google\Chrome\Application\chrome.exe";
            string mozillaFirefoxExePath = @"C:\Program Files\Mozilla Firefox\firefox.exe";
            string mozillaThunderbirdExePath = @"C:\Program Files\Mozilla Thunderbird\thunderbird.exe";
            string nanaZipExe = "NanaZip.Modern.FileManager.exe";
            string nanaZipPath = null;

            // Downloaded installers
            string crcOEMFilename = Path.Combine(oemDir, "computerRepairCentreOEM.bmp");
            string googleChromeFilename = Path.Combine(appsDir, "googleChrome.msi");
            string mozillaFirefoxFilename = Path.Combine(appsDir, "mozillaFirefox.msi");
            string anyDeskFilename = Path.Combine(appsDir, "anyDesk.msi");
            string bingWallpapersFilename = Path.Combine(appsDir, "bingWallpapers.msi");
            string bitDefenderFilename = Path.Combine(appsDir, "bitDefender.exe");
            string discordFilename = Path.Combine(appsDir, "discord.exe");
            string libreOfficeFilename = Path.Combine(appsDir, "libreOffice.msi");
            string mozillaThunderbirdFilename = Path.Combine(appsDir, "mozillaThunderbird.msi");
            string nanaZipFilename = Path.Combine(appsDir, "nanaZip.msixbundle");
            string steamFilename = Path.Combine(appsDir, "steam.exe");
            string vlcMediaPlayerFilename = Path.Combine(appsDir, "vlcMediaPlayer.msi");
            string nvidiaAppFilename = Path.Combine(appsDir, "nvidiaApp.exe");

            // Other apps
            string bingWallpaperAppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Microsoft\BingWallpaperApp\BingWallpaperApp.exe");
            string discordAppPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), @"Discord\Update.exe");

            // Desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string launcherPath = Path.Combine(desktopPath, @"Computer Repair Centre Installer.exe");

            if (!Directory.Exists(rootDir)) Directory.CreateDirectory(rootDir);
            if (!Directory.Exists(oemDir)) Directory.CreateDirectory(oemDir);
            if (!Directory.Exists(appsDir)) Directory.CreateDirectory(appsDir);

            if (windows10 == "1")
            {
                if (romsey == "1") { InstallProgressBar.Maximum += 1; }
                if (highcliffe == "1") { InstallProgressBar.Maximum += 1; }
            }
            if (windows11 == "1") { InstallProgressBar.Maximum += 6; }
            if (PowerCheck.IsChecked == true) { InstallProgressBar.Maximum += 1; } else { InstallProgressBar.Maximum += 2; }
            if (CrcCheck.IsChecked == true) { InstallProgressBar.Maximum += 1; }
            if (AnyDeskCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (NanaZipCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (BitDefenderCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (BingWallpapersCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (DiscordCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (GoogleChromeCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (LibreOfficeCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (NvidiaAppCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (MozillaFirefoxCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (MozillaThunderbirdCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (SteamCheck.IsChecked == true) { InstallProgressBar.Maximum += 2; }
            if (TaskbarCheck.IsChecked == true) { InstallProgressBar.Maximum += 1; }
            if (PinAppsCheck.IsChecked == true) { InstallProgressBar.Maximum += 1; }

            if (NvidiaAppCheck.IsChecked == true && nvidia == "1")
            {
                AppendLine("🎮 Nvidia GPU has been detected and selected, Nvidia App will be installed.");
                AppendLine("✅ You can uncheck this if you want.");
            }

            if (PowerCheck.IsChecked == true)
            {
                AppendLine("📌 Disable sleep on AC power is selected.");
                AppendLine("🔄 Disabling sleep and screen timeout while on AC power...");
                RunSilentCommand("powercfg", "/change standby-timeout-ac 0");
                RunSilentCommand("powercfg", "/change monitor-timeout-ac 0");
                AppendLine("🔄 Changing power button behavior to shutdown...");
                RunSilentCommand("powercfg", "/setacvalueindex SCHEME_CURRENT 4f971e89-eebd-4455-a8de-9e59040e7347 7648efa3-dd9c-4e3e-b566-50f929386280 3");
                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
            }
            else
            {
                AppendLine("🔄 Disabling sleep and screen timeout while on AC power temporarily during install...");
                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
            }

            if (CrcCheck.IsChecked == true)
            {
                AppendLine("📌 Computer Repair Centre OEM information is selected.");
                string oemLocation = SelectedOemLocation;
                if (oemLocation == "Romsey")
                {
                    AppendLine("📦 Installing Romsey Computer Repair Centre OEM information...");
                    await DownloadWithRetryAsync(crcOEMURL, crcOEMFilename);
                    SetOemInfo(hours: "Mon-Fri 9:15am-5:00pm - Sat 9:15am-4:00pm", phone: "01794 517142", url: "https://www.thecomputerrepaircentre.co.uk/romsey");
                    TrackResult("Computer Repair Centre OEM Info", "Installed");
                }
                else if (oemLocation == "Chandler's Ford")
                {
                    AppendLine("📦 Installing Chandlers Ford Computer Repair Centre OEM information...");
                    await DownloadWithRetryAsync(crcOEMURL, crcOEMFilename);
                    SetOemInfo(hours: "Mon-Fri 9:00am-5:30pm - Sat 9:00am-2:00pm", phone: "02380 270271", url: "https://www.thecomputerrepaircentre.co.uk/chandlers-ford");
                    TrackResult("Computer Repair Centre OEM Info", "Installed");
                }
                else if (oemLocation == "Highcliffe")
                {
                    AppendLine("📦 Installing Highcliffe Computer Repair Centre OEM information...");
                    await DownloadWithRetryAsync(crcOEMURL, crcOEMFilename);
                    SetOemInfo(hours: "Mon-Fri 9:15am-5:00pm - Sat 9:15am-2:00pm", phone: "01425 278579", url: "https://www.thecomputerrepaircentre.co.uk/highcliffe");
                    TrackResult("Computer Repair Centre OEM Info", "Installed");
                }
                else
                {
                    AppendLine("⚠️ No known shop location selected, skipping OEM info.");
                    TrackResult("Computer Repair Centre OEM Info", "Skipped");
                }
            }

            if (NanaZipCheck.IsChecked == true)
            {
                AppendLine("📌 NanaZip is selected.");
                AppendLine("🔄 Checking if NanaZip is installed...");
                try
                {
                    var files = Directory.GetFiles(windowsAppsPath, nanaZipExe, SearchOption.AllDirectories);
                    if (files.Length > 0)
                    {
                        nanaZipPath = files[0];
                        AppendLine("✅ NanaZip is already installed.");
                        TrackResult("NanaZip", "Already Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                    }
                    else
                    {
                        AppendLine("❌ NanaZip not found, proceeding with installation.");
                        AppendLine("🔄 Downloading NanaZip...");
                        await DownloadWithRetryAsync(nanaZipURL, nanaZipFilename);
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
                        TrackResult("NanaZip", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                    }
                }
                catch (UnauthorizedAccessException)
                {
                    AppendLine("⚠️ Access denied to WindowsApps. Try running as Administrator.");
                    TrackResult("NanaZip", "Failed");
                }
                catch (Exception ex)
                {
                    AppendLine("❌ Error: " + ex.Message);
                }
            }

            if (TaskbarCheck.IsChecked == true)
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
                TrackResult("Taskbar Alignment", "Applied");
                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
            }

            if (AnyDeskCheck.IsChecked == true)
            {
                AppendLine("📌 AnyDesk is selected.");
                if (File.Exists(@"C:\Program Files (x86)\AnyDeskMSI\AnyDeskMSI.exe"))
                {
                    AppendLine("✅ AnyDesk is already installed, skipping installation.");
                    TrackResult("AnyDesk", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
                else if (File.Exists(@"C:\Program Files (x86)\AnyDesk\AnyDesk.exe"))
                {
                    AppendLine("✅ AnyDesk is already installed, skipping installation.");
                    TrackResult("AnyDesk", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
                else
                {
                    bool wingetOk = await TryWingetInstallAsync("AnyDeskSoftwareGmbH.AnyDesk", "AnyDesk");
                    if (wingetOk)
                    {
                        TrackResult("AnyDesk", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                    }
                    else
                    {
                        AppendLine("🔄 Downloading AnyDesk...");
                        await DownloadWithRetryAsync(anyDeskURL, anyDeskFilename);
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
                                    if (!string.IsNullOrEmpty(error)) Console.WriteLine("Error: " + error);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("An error occurred: " + ex.Message);
                                }
                            }
                        });
                        AppendLine("✅ Completed installation of AnyDesk.");
                        TrackResult("AnyDesk", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                    }
                }
            }

            if (BingWallpapersCheck.IsChecked == true)
            {
                AppendLine("📌 Bing Wallpapers is selected.");
                if (File.Exists(bingWallpaperAppPath))
                {
                    AppendLine("✅ Bing Wallpapers is already installed, skipping installation.");
                    TrackResult("Bing Wallpapers", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
                else
                {
                    bool wingetOk = await TryWingetInstallAsync("Microsoft.BingWallpaper", "Bing Wallpapers");
                    if (wingetOk)
                    {
                        TrackResult("Bing Wallpapers", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                    }
                    else
                    {
                        AppendLine("🔄 Downloading Bing Wallpapers...");
                        await DownloadWithRetryAsync(bingWallpapersURL, bingWallpapersFilename);
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
                                    if (!string.IsNullOrEmpty(error)) Console.WriteLine("Error: " + error);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("An error occurred: " + ex.Message);
                                }
                            }
                        });
                        AppendLine("✅ Completed installation of Bing Wallpapers.");
                        TrackResult("Bing Wallpapers", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                    }
                }
            }

            if (BitDefenderCheck.IsChecked == true)
            {
                AppendLine("📌 BitDefender is selected.");
                if (File.Exists(@"C:\Program Files\Bitdefender\Bitdefender Security App\seccenter.exe"))
                {
                    AppendLine("✅ BitDefender is already installed, skipping installation.");
                    TrackResult("BitDefender", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
                else
                {
                    AppendLine("🔄 Downloading BitDefender...");
                    await DownloadWithRetryAsync(bitDefenderURL, bitDefenderFilename);
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
                                Console.WriteLine(exitCode == 0 ? "Installation successful." : $"Installation exited with code: {exitCode}");
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"An error occurred: {ex.Message}");
                        }
                    });
                    AppendLine("✅ Completed installation of BitDefender.");
                    TrackResult("BitDefender", "Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                }
            }

            if (DiscordCheck.IsChecked == true)
            {
                AppendLine("📌 Discord is selected.");
                if (File.Exists(discordAppPath))
                {
                    AppendLine("✅ Discord is already installed, skipping installation.");
                    TrackResult("Discord", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
                else
                {
                    bool wingetOk = await TryWingetInstallAsync("Discord.Discord", "Discord");
                    if (wingetOk)
                    {
                        TrackResult("Discord", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                    }
                    else
                    {
                        AppendLine("🔄 Downloading Discord...");
                        await DownloadWithRetryAsync(discordURL, discordFilename);
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
                                    Console.WriteLine(exitCode == 0 ? "Installation successful." : $"Installation exited with code: {exitCode}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"An error occurred: {ex.Message}");
                            }
                        });
                        AppendLine("✅ Completed installation of Discord.");
                        TrackResult("Discord", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                    }
                }
            }

            if (GoogleChromeCheck.IsChecked == true)
            {
                AppendLine("📌 Google Chrome is selected.");
                if (!File.Exists(googleChromeExePath))
                {
                    bool wingetOk = await TryWingetInstallAsync("Google.Chrome", "Google Chrome");
                    if (wingetOk)
                    {
                        TrackResult("Google Chrome", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                    }
                    else
                    {
                        AppendLine("🔄 Downloading Google Chrome...");
                        try
                        {
                            await DownloadWithRetryAsync(googleChromeURL, googleChromeFilename);
                            AppendLine("✅ Chrome download completed.");
                        }
                        catch (WebException ex)
                        {
                            AppendLine("❌ Failed to download Google Chrome: " + ex.Message);
                            TrackResult("Google Chrome", "Failed");
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
                            TrackResult("Google Chrome", "Installed");
                        }
                        catch (Exception ex)
                        {
                            AppendLine("❌ Chrome installation failed: " + ex.Message);
                            TrackResult("Google Chrome", "Failed");
                        }
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                    }
                }
                else
                {
                    AppendLine("✅ Google Chrome is already installed, skipping installation.");
                    TrackResult("Google Chrome", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
            }

            if (LibreOfficeCheck.IsChecked == true)
            {
                AppendLine("📌 LibreOffice is selected.");
                if (File.Exists(@"C:\Program Files\LibreOffice\program\soffice.exe"))
                {
                    AppendLine("✅ LibreOffice is already installed, skipping installation.");
                    TrackResult("LibreOffice", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
                else
                {
                    bool wingetOk = await TryWingetInstallAsync("TheDocumentFoundation.LibreOffice", "LibreOffice");
                    if (wingetOk)
                    {
                        TrackResult("LibreOffice", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                    }
                    else
                    {
                        AppendLine("🔄 Downloading LibreOffice...");
                        string libreOfficeVersion = GetLibreOfficeVersion();
                        if (string.IsNullOrEmpty(libreOfficeVersion))
                        {
                            AppendLine("❌ Could not determine the latest LibreOffice version.");
                            TrackResult("LibreOffice", "Failed");
                            AppendInstallSummary();
                            return;
                        }
                        string libreOfficeDownloadUrl = $"https://download.documentfoundation.org/libreoffice/stable/{libreOfficeVersion}/win/x86_64/LibreOffice_{libreOfficeVersion}_Win_x86-64.msi";
                        Uri libreOfficeURL = new Uri(libreOfficeDownloadUrl);
                        await DownloadWithRetryAsync(libreOfficeURL, libreOfficeFilename);
                        if (!File.Exists(libreOfficeFilename))
                        {
                            AppendLine("❌ LibreOffice download failed; falling back to known installer.");
                            libreOfficeURL = new Uri("https://cloud.howardgb.com/public.php/dav/files/EFyAqCm3tEQ6W25/libreOffice.msi");
                            await DownloadWithRetryAsync(libreOfficeURL, libreOfficeFilename);
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
                                    if (!string.IsNullOrEmpty(error)) Console.WriteLine("Error: " + error);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("An error occurred: " + ex.Message);
                                }
                            }
                        });
                        AppendLine("✅ Completed installation of LibreOffice.");
                        TrackResult("LibreOffice", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                    }
                }
            }

            if (NvidiaAppCheck.IsChecked == true)
            {
                AppendLine("📌 Nvidia App is selected.");
                AppendLine("🔄 Searching for latest Nvidia App installer...");
                try
                {
                    using (HttpClient client = new HttpClient())
                    {
                        string baseUrl = "https://www.nvidia.com/en-us/software/nvidia-app/";
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
                            TrackResult("Nvidia App", "Failed");
                            AppendInstallSummary();
                            return;
                        }
                    }
                }
                catch (Exception ex)
                {
                    AppendLine($"⚠️ Error downloading Nvidia App: {ex.Message}");
                    TrackResult("Nvidia App", "Failed");
                    AppendInstallSummary();
                    return;
                }
                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
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
                            AppendLine(exitCode == 0 ? "✅ Installation successful." : $"⚠️ Installation exited with code: {exitCode}");
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLine($"⚠️ Installation failed: {ex.Message}");
                    }
                });
                AppendLine("✅ Completed installation of Nvidia App.");
                TrackResult("Nvidia App", "Installed");
                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
            }

            if (MozillaFirefoxCheck.IsChecked == true)
            {
                AppendLine("📌 Mozilla Firefox is selected.");
                if (File.Exists(mozillaFirefoxExePath))
                {
                    AppendLine("✅ Mozilla Firefox is already installed, skipping installation.");
                    TrackResult("Mozilla Firefox", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
                else
                {
                    bool wingetOk = await TryWingetInstallAsync("Mozilla.Firefox", "Mozilla Firefox");
                    if (wingetOk)
                    {
                        TrackResult("Mozilla Firefox", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                    }
                    else
                    {
                        AppendLine("🔄 Downloading Mozilla Firefox...");
                        await DownloadWithRetryAsync(mozillaFirefoxURL, mozillaFirefoxFilename);
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
                                    if (!string.IsNullOrEmpty(error)) Console.WriteLine("Error: " + error);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("An error occurred: " + ex.Message);
                                }
                            }
                        });
                        AppendLine("✅ Completed installation of Mozilla Firefox.");
                        TrackResult("Mozilla Firefox", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                    }
                }
            }

            if (MozillaThunderbirdCheck.IsChecked == true)
            {
                AppendLine("📌 Mozilla Thunderbird is selected.");
                // Check if Thunderbird is already installed
                if (File.Exists(mozillaThunderbirdExePath))
                {
                    AppendLine("✅ Mozilla Thunderbird is already installed, skipping installation.");
                    TrackResult("Mozilla Thunderbird", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
                else
                {
                    bool wingetOk = await TryWingetInstallAsync("Mozilla.Thunderbird", "Mozilla Thunderbird");
                    if (wingetOk)
                    {
                        TrackResult("Mozilla Thunderbird", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                    }
                    else
                    {
                        // Download Thunderbird
                        AppendLine("🔄 Downloading Mozilla Thunderbird...");
                        await DownloadWithRetryAsync(mozillaThunderbirdURL, mozillaThunderbirdFilename);
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
                                    if (!string.IsNullOrEmpty(error)) Console.WriteLine("Error: " + error);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("An error occurred: " + ex.Message);
                                }
                            }
                        });
                        AppendLine("✅ Completed installation of Mozilla Thunderbird.");
                        TrackResult("Mozilla Thunderbird", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                    }
                }
            }

            if (SteamCheck.IsChecked == true)
            {
                AppendLine("📌 Steam is selected.");
                if (File.Exists(@"C:\Program Files (x86)\Steam\Steam.exe"))
                {
                    AppendLine("✅ Steam is already installed, skipping installation.");
                    TrackResult("Steam", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
                else
                {
                    bool wingetOk = await TryWingetInstallAsync("Valve.Steam", "Steam");
                    if (wingetOk)
                    {
                        TrackResult("Steam", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                    }
                    else
                    {
                        AppendLine("🔄 Downloading Steam...");
                        await DownloadWithRetryAsync(steamURL, steamFilename);
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
                                    Console.WriteLine(exitCode == 0 ? "Installation successful." : $"Installation exited with code: {exitCode}");
                                }
                            }
                            catch (Exception ex)
                            {
                                Console.WriteLine($"An error occurred: {ex.Message}");
                            }
                        });
                        AppendLine("✅ Completed installation of Steam.");
                        TrackResult("Steam", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                    }
                }
            }

            if (VlcMediaPlayerCheck.IsChecked == true)
            {
                AppendLine("📌 VLC Media Player is selected.");
                if (File.Exists(@"C:\Program Files\VideoLAN\VLC\vlc.exe"))
                {
                    AppendLine("✅ VLC Media Player is already installed, skipping installation.");
                    TrackResult("VLC Media Player", "Already Installed");
                    InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                }
                else
                {
                    bool wingetOk = await TryWingetInstallAsync("VideoLAN.VLC", "VLC Media Player");
                    if (wingetOk)
                    {
                        TrackResult("VLC Media Player", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 2, InstallProgressBar.Maximum);
                    }
                    else
                    {
                        AppendLine("🔄 Downloading VLC Media Player...");
                        await DownloadWithRetryAsync(vlcMediaPlayerURL, vlcMediaPlayerFilename);
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
                                    if (!string.IsNullOrEmpty(error)) Console.WriteLine("Error: " + error);
                                }
                                catch (Exception ex)
                                {
                                    Console.WriteLine("An error occurred: " + ex.Message);
                                }
                            }
                        });
                        AppendLine("✅ Completed installation of VLC Media Player.");
                        TrackResult("VLC Media Player", "Installed");
                        InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                    }
                }
            }

            if (PinAppsCheck.IsChecked == true)
            {
                AppendLine("📌 Pin apps to taskbar is selected.");
                AppendLine("✅ Building taskbar layout from installed apps...");
                try
                {
                    ApplyTaskbarPinLayout();
                    TrackResult("Pin Apps to Taskbar", "Applied");
                }
                catch (Exception ex)
                {
                    AppendLine("❌ Failed to set taskbar layout: " + ex.Message);
                    TrackResult("Pin Apps to Taskbar", "Failed");
                }
                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
            }

            try
            {
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
                                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\BitLocker", writable: true))
                                    registryKey.SetValue("PreventDeviceEncryption", 1, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);

                                AppendLine("✅ Disabling fastboot mode...");
                                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", writable: true))
                                    registryKey.SetValue("HiberbootEnabled", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);

                                AppendLine("✅ Disabling location tracking...");
                                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}", writable: true))
                                    registryKey.SetValue("SensorPermissionState", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\lfsvc\Service\Configuration", writable: true))
                                    registryKey.SetValue("Status", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);

                                AppendLine("✅ Hiding recently used files and folders in File Explorer...");
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", writable: true))
                                    registryKey.SetValue("ShowRecent", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", writable: true))
                                    registryKey.SetValue("ShowFrequent", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                            }
                            else if (build >= 19041)
                            {
                                AppendLine("✅ Setting explorer to open to This PC...");
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", writable: true))
                                    registryKey.SetValue("LaunchTo", 1, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);

                                AppendLine("✅ Disabling fastboot mode...");
                                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", writable: true))
                                    registryKey.SetValue("HiberbootEnabled", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);

                                AppendLine("✅ Disabling location tracking...");
                                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}", writable: true))
                                    registryKey.SetValue("SensorPermissionState", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\lfsvc\Service\Configuration", writable: true))
                                    registryKey.SetValue("Status", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);

                                AppendLine("✅ Disabling People icon...");
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People", writable: true)) { }
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People", writable: true))
                                    registryKey.SetValue("PeopleBand", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);

                                AppendLine("✅ Hiding recently used files and folders in File Explorer...");
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", writable: true))
                                    registryKey.SetValue("ShowRecent", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", writable: true))
                                    registryKey.SetValue("ShowFrequent", 0, RegistryValueKind.DWord);
                                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
                            }
                            else
                            {
                                AppendLine("⬆️ This computer is running an old version of Windows, please update it.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLine($"⚠️ Some Windows tweaks could not be applied: {ex.Message}");
            }

            if (PowerCheck.IsChecked == true) { }
            else
            {
                AppendLine("✅ Re-enabling sleep and screen timeout on AC power...");
                RunSilentCommand("powercfg", "/change monitor-timeout-ac 10");
                RunSilentCommand("powercfg", "/change standby-timeout-ac 20");
                InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);
            }

            AppendLine("✅ Cleaning up installation files...");
            var deletionHelper = new FileDeletionHelper();
            await deletionHelper.DeleteFilesAndDirectoryAsync(appsDir, launcherPath);
            InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + 1, InstallProgressBar.Maximum);

            if (RecycleBinCheck.IsChecked == true)
            {
                AppendLine("✅ Empty Recycle Bin is checked.");
                AppendLine("🗑️ Emptying Recycle Bin...");
                try
                {
                    SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
                    AppendLine("✅ Recycle Bin emptied successfully.");
                    TrackResult("Empty Recycle Bin", "Applied");
                }
                catch (Exception ex)
                {
                    AppendLine($"⚠️ Failed to empty Recycle Bin: {ex.Message}");
                    TrackResult("Empty Recycle Bin", "Failed");
                }
            }

            _themeManager.PlayEventSound();

            if (RestartCheck.IsChecked == true)
            {
                Process.Start("shutdown", "/r /t 60");
                AppendLine("🔄 System will restart in 60 seconds. If you need to cancel this press the close button.");
            }
            if (ShutdownCheck.IsChecked == true)
            {
                Process.Start("shutdown", "/s /t 60");
                AppendLine("⏻ System will shutdown in 60 seconds. If you need to cancel this press the close button.");
            }

            InstallProgressBar.Value = InstallProgressBar.Maximum;
            AppendInstallSummary();
            AppendLine("✅ The installation has completed.");
        }
    }
}
