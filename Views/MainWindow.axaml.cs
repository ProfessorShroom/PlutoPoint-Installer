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
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using Microsoft.Win32;
using PlutoPoint_Installer.Models;
using PlutoPoint_Installer.Utilities;

// Copyright © Charlie Howard 2016-2026 All rights reserved

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

        private static readonly bool IsArm64 = RuntimeInformation.OSArchitecture == Architecture.Arm64;

        private static readonly JsonSerializerOptions _jsonOptions = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        // Set strings
        private string safeLocation = null;
        private string location = null;
        private string romsey = null;
        private string chandlersFord = null;
        private string highcliffe = null;
        private string locationLine = null;
        private string windows10 = null;
        private string windows11 = null;
        private string nvidia = null;
        private string intel = null;
        private string amd = null;
        private string amdCpu = null;
        private string amdGpu = null;
        private string rootDir;
        private string programDataDir;
        private DownloadUrls urls;
        private Task _initialiseUrlsTask;
        private Task _checkIpTask;
        private List<(string Name, string Status)> installResults;
        private string _logFilePath;
        private readonly PlutoPoint_Installer.UI.ThemeManager _themeManager = new PlutoPoint_Installer.UI.ThemeManager();
        private bool _isBetaBuild = false;
        DateTime buildDate = DateTime.Parse(
    Assembly.GetExecutingAssembly()
        .GetCustomAttribute<PlutoPoint_Installer.Attributes.AssemblyUpdateDateAttribute>()
        ?.Date ?? "1970-01-01T00:00:00Z",
    System.Globalization.CultureInfo.InvariantCulture,
    System.Globalization.DateTimeStyles.RoundtripKind).ToLocalTime();

        private string? ArmOverride(string key) =>
            IsArm64 && urls.arm64 != null && urls.arm64.TryGetValue(key, out var armUrl) && !string.IsNullOrWhiteSpace(armUrl)
                ? armUrl
                : null;

        private string ResolveUrl(string key, string defaultUrl) => ArmOverride(key) ?? defaultUrl;

        private Uri crcOEMURL => new Uri(ResolveUrl("crcOEM", urls.crcOEM));
        private Uri anyDeskURL => new Uri(ResolveUrl("anyDesk", urls.anyDesk));
        private Uri bingWallpapersURL => new Uri(ResolveUrl("bingWallpapers", urls.bingWallpapers));
        private Uri bitDefenderURL => new Uri(ResolveUrl("bitDefender", urls.bitDefender));
        private Uri discordURL => new Uri(ResolveUrl("discord", urls.discord));
        private Uri googleChromeURL => new Uri(ResolveUrl("googleChrome", urls.googleChrome));
        private Uri mozillaFirefoxURL => new Uri(ResolveUrl("mozillaFirefox", urls.mozillaFirefox));
        private Uri mozillaThunderbirdURL => new Uri(ResolveUrl("mozillaThunderbird", urls.mozillaThunderbird));
        private Uri nanaZipURL => new Uri(ResolveUrl("nanaZip", urls.nanaZip));
        private Uri steamURL => new Uri(ResolveUrl("steam", urls.steam));
        private Uri vlcMediaPlayerURL => new Uri(ResolveUrl("vlcMediaPlayer", urls.vlcMediaPlayer));
        private Uri office2024URL => new Uri(ResolveUrl("office2024", urls.office2024));
        private Uri libreOfficeFallbackURL => new Uri(ResolveUrl("libreOfficeFallback",
            string.IsNullOrWhiteSpace(urls.libreOfficeFallback) ? "https://libreoffice.professorshroom.com" : urls.libreOfficeFallback));

        private static readonly HttpClient Http = new HttpClient { Timeout = TimeSpan.FromMinutes(30) };
        private static readonly HttpClient BrowserHttp = CreateBrowserClient();

        private uint? _savedStandbyAcSeconds;
        private uint? _savedMonitorAcSeconds;
        private bool _sleepRestorePending;

        private static bool IsOfficeInstalled() =>
            File.Exists(@"C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE")
            || File.Exists(@"C:\Program Files (x86)\Microsoft Office\root\Office16\WINWORD.EXE");

        private const string Office2024Config = @"<Configuration>
  <Add OfficeClientEdition=""64"" Channel=""PerpetualVL2024"">
    <Product ID=""ProPlus2024Volume"">
      <Language ID=""en-us"" />
    </Product>
  </Add>
  <Display Level=""None"" AcceptEULA=""TRUE"" />
</Configuration>
";

        public MainWindow()
        {
            InitializeComponent();
            ApplyBetaBranding();
            // Shutdown/restart checks
            WireCheckboxMutualExclusion();
            // Hover
            InstallButton.PointerEntered += (s, e) => AudioEffects.PlayHoverPop();
            CancelInstallButton.PointerEntered += (s, e) => AudioEffects.PlayHoverPop();
            RestartButton.PointerEntered += (s, e) => AudioEffects.PlayHoverPop();
            CloseButton.PointerEntered += (s, e) => AudioEffects.PlayHoverPop();
            // Click
            InstallButton.Click += (s, e) => AudioEffects.PlayClickChime();
            CancelInstallButton.Click += (s, e) => AudioEffects.PlayClickChime();
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
        }

        protected override void OnOpened(EventArgs e)
        {
            base.OnOpened(e);
            if (_isBetaBuild)
            {
                var betaBrush = new SolidColorBrush(Color.Parse("#ED8D28"));
                InstallButton.Background = betaBrush;
                InstallButton.Foreground = new SolidColorBrush(Colors.White);
                CancelInstallButton.Background = betaBrush;
                CancelInstallButton.Foreground = new SolidColorBrush(Colors.White);
                RestartButton.Background = betaBrush;
                RestartButton.Foreground = new SolidColorBrush(Colors.White);
                CloseButton.Background = betaBrush;
                CloseButton.Foreground = new SolidColorBrush(Colors.White);
                return;
            }
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
                    CancelInstallButton.Background = backBrush;
                    CancelInstallButton.Foreground = foreBrush;
                    RestartButton.Background = backBrush;
                    RestartButton.Foreground = foreBrush;
                    CloseButton.Background = backBrush;
                    CloseButton.Foreground = foreBrush;
                }
            );
            _themeManager.ApplyTitle(
                (titleSuffix) =>
                {
                    Title += $" - {titleSuffix}";
                    TitleTextBlock.Text += $" - {titleSuffix}";
                }
            );
            _themeManager.ApplyTextColor(
                (textColor) =>
                {
                    TitleTextBlock.Foreground = new SolidColorBrush(textColor);
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
            int succeeded = installResults.Count(r => r.Status == "Installed" || r.Status == "Already Installed" || r.Status == "Applied" || r.Status == "Removed");
            int skipped = installResults.Count(r => r.Status == "Skipped");
            int unverified = installResults.Count(r => r.Status == "Unverified");
            int failed = installResults.Count(r => r.Status == "Failed");
            AppendLine("");
            AppendLine("📋 Summary:");
            AppendLine($"✅ {succeeded}/{total} succeeded");
            if (skipped > 0) AppendLine($"⏭️ {skipped} skipped");
            if (unverified > 0)
            {
                AppendLine($"⚠️ {unverified} finished but couldn't be confirmed:");
                foreach (var r in installResults.Where(r => r.Status == "Unverified"))
                    AppendLine($"   • {r.Name}");
            }
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

        private void ApplyBetaBranding()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string versionStr = assembly
                .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
            if (string.IsNullOrWhiteSpace(versionStr) || !versionStr.Contains("b"))
                return;

            _isBetaBuild = true;

            Title += " Beta";
            TitleTextBlock.Text += " Beta";
            try
            {
                Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://Computer Repair Centre Installer/Resources/icons/computerRepairCentreIconBeta.ico")));
            }
            catch
            {
            }

            if (Background is LinearGradientBrush gradientBrush && gradientBrush.GradientStops.Count >= 2)
            {
                gradientBrush.GradientStops[0].Color = Color.Parse("#ED8D28");
                gradientBrush.GradientStops[1].Color = Color.Parse("#E25E04");
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
                            if (IsArm64)
                            {
                                osLabelText += " ARM";
                                AppendLine("ARM processor detected.");
                            }
                            else
                            {
                                osLabelText += " x64";
                            }
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

        private HashSet<string> _gpuVendorIds;

        private HashSet<string> GpuVendorIds => _gpuVendorIds ??= GetGpuVendorIds();

        private static HashSet<string> GetGpuVendorIds()
        {
            var vendors = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT PNPDeviceID, CompatibleID FROM Win32_PnPEntity WHERE PNPDeviceID LIKE 'PCI%'");
                foreach (ManagementObject device in searcher.Get())
                {
                    if (device["CompatibleID"] is not string[] compatibleIds) continue;
                    if (!compatibleIds.Any(id => id != null && id.IndexOf("CC_03", StringComparison.OrdinalIgnoreCase) >= 0)) continue;
                    if (device["PNPDeviceID"] is string pnpId)
                    {
                        Match vendor = Regex.Match(pnpId, @"VEN_([0-9A-F]{4})", RegexOptions.IgnoreCase);
                        if (vendor.Success) vendors.Add(vendor.Groups[1].Value);
                    }
                }
            }
            catch
            {
            }
            try
            {
                using var searcher = new ManagementObjectSearcher("SELECT Caption, AdapterCompatibility, PNPDeviceID FROM Win32_VideoController");
                foreach (ManagementObject controller in searcher.Get())
                {
                    if (controller["PNPDeviceID"] is string pnpId)
                    {
                        Match vendor = Regex.Match(pnpId, @"VEN_([0-9A-F]{4})", RegexOptions.IgnoreCase);
                        if (vendor.Success) vendors.Add(vendor.Groups[1].Value);
                    }
                    string text = $"{controller["Caption"]} {controller["AdapterCompatibility"]}";
                    if (text.IndexOf("NVIDIA", StringComparison.OrdinalIgnoreCase) >= 0) vendors.Add("10DE");
                    if (text.IndexOf("Intel", StringComparison.OrdinalIgnoreCase) >= 0) vendors.Add("8086");
                    if (text.IndexOf("AMD", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        text.IndexOf("Radeon", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        text.IndexOf("Advanced Micro Devices", StringComparison.OrdinalIgnoreCase) >= 0) vendors.Add("1002");
                }
            }
            catch
            {
            }
            return vendors;
        }

        private void CheckForIntelHardware()
        {
            bool hasIntelGpu = GpuVendorIds.Contains("8086");
            bool hasIntelCpu = false;
            try
            {
                using var cpuSearcher = new ManagementObjectSearcher("SELECT Name, Manufacturer FROM Win32_Processor");
                foreach (ManagementObject queryObj in cpuSearcher.Get())
                {
                    string text = $"{queryObj["Name"]} {queryObj["Manufacturer"]}";
                    if (text.IndexOf("Intel", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        hasIntelCpu = true;
                        break;
                    }
                }
            }
            catch
            {
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
            if (GpuVendorIds.Contains("10DE"))
            {
                NvidiaAppCheck.IsChecked = true;
                nvidia = "1";
                AppendLine("🎮 Nvidia GPU detected.");
                return;
            }
            NvidiaAppCheck.IsChecked = false;
        }

        private void CheckforAMDHardware()
        {
            bool hasAmdGpu = GpuVendorIds.Contains("1002");
            bool hasAmdCpu = false;
            try
            {
                using var cpuSearcher = new ManagementObjectSearcher("SELECT Name, Manufacturer FROM Win32_Processor");
                foreach (ManagementObject queryObj in cpuSearcher.Get())
                {
                    string text = $"{queryObj["Name"]} {queryObj["Manufacturer"]}";
                    if (text.IndexOf("AMD", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        text.IndexOf("Ryzen", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        text.IndexOf("Threadripper", StringComparison.OrdinalIgnoreCase) >= 0)
                    {
                        hasAmdCpu = true;
                        break;
                    }
                }
            }
            catch
            {
            }
            if (hasAmdGpu && !IsArm64)
            {
                amdGpu = "1";
                AmdAdrenalinCheck.IsChecked = true;
            }
            else
            {
                AmdAdrenalinCheck.IsChecked = false;
            }
            if (hasAmdCpu && !IsArm64)
            {
                amdCpu = "1";
                AmdChipsetCheck.IsChecked = true;
            }
            else
            {
                AmdChipsetCheck.IsChecked = false;
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

        private static readonly string[] NvidiaAppPages =
        {
            "https://www.nvidia.com/en-us/software/nvidia-app/",
            "https://www.nvidia.com/en-gb/software/nvidia-app/",
            "https://www.nvidia.com/en-us/software/nvidia-app.md",
            "https://www.nvidia.com/en-zz/software/nvidia-app.md"
        };

        private static readonly Regex NvidiaAppLinkRegex = new Regex(
            @"(?:https?:)?(?:\\?/){2}[a-z0-9.-]*download\.nvidia\.com(?:\\?/)nvapp(?:\\?/)client(?:\\?/)[\d.]+(?:\\?/)NVIDIA_app_(?:beta_)?v[\d.]+\.exe",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static HttpClient CreateBrowserClient()
        {
            var handler = new HttpClientHandler
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate | DecompressionMethods.Brotli,
                AllowAutoRedirect = true
            };
            var client = new HttpClient(handler) { Timeout = TimeSpan.FromMinutes(15) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36");
            client.DefaultRequestHeaders.Accept.ParseAdd("text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8");
            client.DefaultRequestHeaders.AcceptLanguage.ParseAdd("en-GB,en;q=0.9");
            return client;
        }

        private const string AmdReferer = "https://www.amd.com/en/support/download/drivers.html";

        private const string AmdChipsetStableUrl = "https://drivers.amd.com/drivers/installer/chipset/AMD_Chipset_Software.exe";

        private static readonly string[] AmdChipsetPages =
        {
            "https://www.amd.com/en/support/downloads/drivers.html/chipsets/laptop-chipsets/amd-ryzen-and-athlon-mobile-chipset.html",
            "https://www.amd.com/en/support/downloads/drivers.html/chipsets/am5/x870e.html",
            "https://www.amd.com/en/support/downloads/drivers.html/chipsets/am4/x570.html"
        };

        private static readonly Regex AmdChipsetLinkRegex = new Regex(
            @"(?:https?:)?(?:\\?/){2}drivers\.amd\.com(?:\\?/)drivers(?:\\?/)amd_chipset_software_([\d.]+)\.exe",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private async Task<List<string>> FindAmdChipsetDownloadUrlsAsync(HttpClient client)
        {
            var candidates = new List<string>();
            foreach (string page in AmdChipsetPages)
            {
                try
                {
                    string content = await client.GetStringAsync(page);
                    string best = null;
                    Version bestVersion = null;
                    foreach (Match match in AmdChipsetLinkRegex.Matches(content))
                    {
                        if (!Version.TryParse(match.Groups[1].Value, out Version version)) continue;
                        if (bestVersion != null && version <= bestVersion) continue;
                        bestVersion = version;
                        best = match.Value;
                    }
                    if (best == null) continue;
                    best = best.Replace("\\/", "/");
                    if (best.StartsWith("//")) best = "https:" + best;
                    if (best.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) best = "https://" + best.Substring(7);
                    candidates.Add(best);
                    break;
                }
                catch (Exception ex)
                {
                    AppendLine($"⚠️ AMD page {page} failed: {ex.Message}");
                }
            }
            candidates.Add(AmdChipsetStableUrl);
            if (!string.IsNullOrWhiteSpace(urls?.amdChipsetFallback)) candidates.Add(urls.amdChipsetFallback);
            return candidates;
        }

        private static readonly string[] AmdAdrenalinPages =
        {
            "https://www.amd.com/en/support/download/drivers.html",
            "https://www.amd.com/en/support/downloads/drivers.html/graphics/radeon-rx/radeon-rx-9000-series/amd-radeon-rx-9070-xt.html"
        };

        private static readonly Regex AmdAdrenalinLinkRegex = new Regex(
            @"(?:https?:)?(?:\\?/){2}drivers\.amd\.com(?:\\?/)drivers(?:\\?/)installer(?:\\?/)[\w.\\/-]*?amd-software-adrenalin-edition-([\d.]+)-minimalsetup-\d+_web\.exe",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private async Task<List<string>> FindAmdAdrenalinDownloadUrlsAsync(HttpClient client)
        {
            var candidates = new List<string>();
            foreach (string page in AmdAdrenalinPages)
            {
                try
                {
                    string content = await client.GetStringAsync(page);
                    string best = null;
                    Version bestVersion = null;
                    foreach (Match match in AmdAdrenalinLinkRegex.Matches(content))
                    {
                        if (!Version.TryParse(match.Groups[1].Value, out Version version)) continue;
                        if (bestVersion != null && version <= bestVersion) continue;
                        bestVersion = version;
                        best = match.Value;
                    }
                    if (best == null) continue;
                    best = best.Replace("\\/", "/");
                    if (best.StartsWith("//")) best = "https:" + best;
                    if (best.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) best = "https://" + best.Substring(7);
                    candidates.Add(best);
                    break;
                }
                catch (Exception ex)
                {
                    AppendLine($"⚠️ AMD page {page} failed: {ex.Message}");
                }
            }
            return candidates;
        }

        private async Task<bool> DownloadInstallerAsync(HttpClient client, string url, string path, string referer)
        {
            using var request = new HttpRequestMessage(HttpMethod.Get, url);
            if (referer != null) request.Headers.Referrer = new Uri(referer);
            request.Headers.Accept.Clear();
            request.Headers.Accept.ParseAdd("*/*");
            using HttpResponseMessage response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead);
            if (!response.IsSuccessStatusCode)
            {
                AppendLine($"⚠️ {url} returned {(int)response.StatusCode}.");
                return false;
            }
            string mediaType = response.Content.Headers.ContentType?.MediaType ?? "";
            if (mediaType.StartsWith("text/", StringComparison.OrdinalIgnoreCase))
            {
                AppendLine($"⚠️ {url} returned a web page instead of an installer.");
                return false;
            }
            await using (Stream source = await response.Content.ReadAsStreamAsync())
            await using (FileStream target = new FileStream(path, FileMode.Create, FileAccess.Write, FileShare.None))
            {
                await source.CopyToAsync(target);
            }
            if (new FileInfo(path).Length < 1024 * 1024)
            {
                AppendLine($"⚠️ {url} downloaded a file that is too small to be the installer.");
                return false;
            }
            return true;
        }

        private async Task<string> FindNvidiaAppDownloadUrlAsync(HttpClient client)
        {
            foreach (string page in NvidiaAppPages)
            {
                try
                {
                    string content = await client.GetStringAsync(page);
                    Match match = NvidiaAppLinkRegex.Match(content);
                    if (!match.Success) continue;
                    string link = match.Value.Replace("\\/", "/");
                    if (link.StartsWith("//")) link = "https:" + link;
                    if (link.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) link = "https://" + link.Substring(7);
                    return link;
                }
                catch (Exception ex)
                {
                    AppendLine($"⚠️ Nvidia page {page} failed: {ex.Message}");
                }
            }
            if (!string.IsNullOrWhiteSpace(urls?.nvidiaAppFallback))
            {
                AppendLine("🔁 Using fallback Nvidia App link from Downloads.json.");
                return urls.nvidiaAppFallback;
            }
            return null;
        }

        private async Task CheckIPAsync()
        {
            safeLocation = "0";
            romsey = "0";
            chandlersFord = "0";
            highcliffe = "0";
            string publicIP = await GetPublicIPAddressAsync();
            LocationHashes hashes = null;
            if (string.IsNullOrWhiteSpace(publicIP))
                AppendLine("⚠️ Couldn't get this computer's public IP address.");
            else
                hashes = await GetLocationHashesAsync();
            string publicIPHash = hashes == null ? null : HashIP(publicIP);
            if (publicIPHash == null)
            {
            }
            else if (publicIPHash == hashes.romsey)
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
            AppendLocation();
            AppendLine(locationLine);
            UpdateLocation();
        }

        private async Task<string> GetPublicIPAddressAsync()
        {
            string[] services =
            {
                "https://api.ipify.org",
                "https://ipv4.icanhazip.com",
                "https://ifconfig.me/ip",
                "https://checkip.amazonaws.com"
            };
            foreach (string service in services)
            {
                try
                {
                    using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(10));
                    string ip = (await Http.GetStringAsync(service, timeout.Token))?.Trim();
                    if (System.Net.IPAddress.TryParse(ip, out _))
                        return ip;
                }
                catch
                {
                }
            }
            return null;
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
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                string json = await Http.GetStringAsync(url, timeout.Token);
                return JsonSerializer.Deserialize<PasswordHashes>(json, _jsonOptions);
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
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                string json = await Http.GetStringAsync(url, timeout.Token);
                return JsonSerializer.Deserialize<LocationHashes>(json, _jsonOptions);
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
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                string json = await Http.GetStringAsync(url, timeout.Token);
                return JsonSerializer.Deserialize<DownloadUrls>(json, _jsonOptions);
            }
            catch (Exception ex)
            {
                AppendLine($"❌ Failed to load URLs: {ex.Message}");
                return null;
            }
        }

        private async Task<bool> PassesPasswordGateAsync()
        {
            if (!_checkIpTask.IsCompleted)
                AppendLine("⏳ Waiting for the location check to finish...");
            try
            {
                await _checkIpTask;
            }
            catch (Exception ex)
            {
                AppendLine($"⚠️ Location check failed: {ex.Message}");
            }
            if (safeLocation == "1")
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
            if (AddPinEntry(pins, SteamCheck.IsChecked == true, "Steam.lnk", "Steam")) pinnedCount++;
            if (AddPinEntry(pins, DiscordCheck.IsChecked == true, "Discord.lnk", "Discord")) pinnedCount++;
            if (AddPinEntry(pins, Office2024Check.IsChecked == true, "Word.lnk", "Microsoft Word", alsoAddToDesktop: true)) pinnedCount++;
            if (AddPinEntry(pins, Office2024Check.IsChecked == true, "Excel.lnk", "Microsoft Excel", alsoAddToDesktop: true)) pinnedCount++;
            if (AddPinEntry(pins, Office2024Check.IsChecked == true, "Outlook (classic).lnk", "Microsoft Outlook", alsoAddToDesktop: true)) pinnedCount++;
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

        private void StepProgress(int steps = 1)
        {
            void Step() => InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + steps, InstallProgressBar.Maximum);
            if (Dispatcher.UIThread.CheckAccess())
                Step();
            else
                Dispatcher.UIThread.Post(Step);
        }

        private async Task<bool> DownloadWithRetryAsync(Uri url, string destinationPath, string label, HttpClient client = null, int maxAttempts = 3)
        {
            client ??= Http;
            for (int attempt = 1; attempt <= maxAttempts; attempt++)
            {
                try
                {
                    using (HttpResponseMessage response = await client.GetAsync(url, HttpCompletionOption.ResponseHeadersRead))
                    {
                        response.EnsureSuccessStatusCode();
                        await using Stream source = await response.Content.ReadAsStreamAsync();
                        await using FileStream target = new FileStream(destinationPath, FileMode.Create, FileAccess.Write, FileShare.None);
                        await source.CopyToAsync(target);
                    }
                    return true;
                }
                catch (Exception ex)
                {
                    try { File.Delete(destinationPath); } catch { }
                    if (attempt < maxAttempts)
                    {
                        AppendLine($"⚠️ Download failed (attempt {attempt}/{maxAttempts}): {ex.Message}. Retrying...");
                        await Task.Delay(1500 * attempt);
                    }
                    else
                    {
                        AppendLine($"❌ Failed to download {label}: {ex.Message}");
                    }
                }
            }
            return false;
        }

        private Task<int> RunInstallerAsync(string fileName, string arguments, bool elevate = false, string workingDirectory = null)
        {
            return Task.Run(() =>
            {
                var startInfo = new ProcessStartInfo { FileName = fileName, Arguments = arguments };
                if (elevate)
                {
                    startInfo.UseShellExecute = true;
                    startInfo.Verb = "runas";
                }
                else
                {
                    startInfo.UseShellExecute = false;
                    startInfo.CreateNoWindow = true;
                }
                if (workingDirectory != null)
                    startInfo.WorkingDirectory = workingDirectory;
                try
                {
                    using Process process = Process.Start(startInfo);
                    if (process == null)
                        return -1;
                    process.WaitForExit();
                    return process.ExitCode;
                }
                catch (Exception ex)
                {
                    AppendLine($"⚠️ Couldn't run {Path.GetFileName(fileName)}: {ex.Message}");
                    return -1;
                }
            });
        }

        private static string RunAndCapture(string fileName, string arguments)
        {
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        CreateNoWindow = true
                    }
                };
                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
            catch
            {
                return null;
            }
        }

        private static uint? ReadAcPowerSetting(string subgroup, string setting)
        {
            string output = RunAndCapture("powercfg", $"/q SCHEME_CURRENT {subgroup} {setting}");
            if (string.IsNullOrEmpty(output))
                return null;
            MatchCollection matches = Regex.Matches(output, @"0x([0-9a-fA-F]{8})");
            if (matches.Count < 2)
                return null;
            return Convert.ToUInt32(matches[matches.Count - 2].Groups[1].Value, 16);
        }

        private void DisableSleepTemporarily()
        {
            _savedStandbyAcSeconds = ReadAcPowerSetting("SUB_SLEEP", "STANDBYIDLE");
            _savedMonitorAcSeconds = ReadAcPowerSetting("SUB_VIDEO", "VIDEOIDLE");
            RunSilentCommand("powercfg", "/change standby-timeout-ac 0");
            RunSilentCommand("powercfg", "/change monitor-timeout-ac 0");
            _sleepRestorePending = true;
        }

        private void RestoreSleepSettings()
        {
            if (!_sleepRestorePending)
                return;
            _sleepRestorePending = false;
            if (_savedStandbyAcSeconds.HasValue)
                RunSilentCommand("powercfg", $"/setacvalueindex SCHEME_CURRENT SUB_SLEEP STANDBYIDLE {_savedStandbyAcSeconds.Value}");
            else
                RunSilentCommand("powercfg", "/change standby-timeout-ac 5");
            if (_savedMonitorAcSeconds.HasValue)
                RunSilentCommand("powercfg", $"/setacvalueindex SCHEME_CURRENT SUB_VIDEO VIDEOIDLE {_savedMonitorAcSeconds.Value}");
            else
                RunSilentCommand("powercfg", "/change monitor-timeout-ac 5");
            RunSilentCommand("powercfg", "/setactive SCHEME_CURRENT");
            if (!_savedStandbyAcSeconds.HasValue || !_savedMonitorAcSeconds.HasValue)
                AppendLine("⚠️ Couldn't read the original sleep settings, so they were set to 5 minutes.");
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

        private void RemoveFirefoxAutoLaunchEntry()
        {
            try
            {
                using (RegistryKey runKey = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true))
                {
                    if (runKey == null) return;
                    foreach (string valueName in runKey.GetValueNames())
                    {
                        string data = runKey.GetValue(valueName) as string;
                        if (!string.IsNullOrEmpty(data) &&
                            data.IndexOf("firefox.exe", StringComparison.OrdinalIgnoreCase) >= 0)
                        {
                            runKey.DeleteValue(valueName, throwOnMissingValue: false);
                            AppendLine($"✅ Removed Firefox's \"open automatically at startup\" entry ({valueName}).");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                AppendLine($"⚠️ Could not check for a Firefox startup entry: {ex.Message}");
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

        private static List<(string Name, string ProductCode)> FindLibreOfficeProducts()
        {
            var found = new List<(string Name, string ProductCode)>();
            foreach (RegistryView view in new[] { RegistryView.Registry64, RegistryView.Registry32 })
            {
                using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, view))
                using (RegistryKey? uninstall = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall"))
                {
                    if (uninstall == null) continue;
                    foreach (string keyName in uninstall.GetSubKeyNames())
                    {
                        if (!keyName.StartsWith("{")) continue;
                        using (RegistryKey? app = uninstall.OpenSubKey(keyName))
                        {
                            string? name = app?.GetValue("DisplayName") as string;
                            if (name != null
                                && name.StartsWith("LibreOffice", StringComparison.OrdinalIgnoreCase)
                                && !found.Any(f => f.ProductCode == keyName))
                                found.Add((name, keyName));
                        }
                    }
                }
            }
            return found.OrderBy(f => f.Name.Contains("Pack", StringComparison.OrdinalIgnoreCase) ? 0 : 1).ToList();
        }

        private async Task RemoveLibreOfficeAsync()
        {
            var products = FindLibreOfficeProducts();
            if (products.Count == 0)
                return;

            AppendLine("🧹 Microsoft Office 2024 is installed, removing LibreOffice...");
            foreach (var (name, productCode) in products)
            {
                AppendLine($"🗑️ Uninstalling {name}...");
                await Task.Run(() =>
                {
                    using (Process process = new Process())
                    {
                        process.StartInfo.FileName = "msiexec";
                        process.StartInfo.Arguments = $"/x {productCode} /qn /norestart";
                        process.StartInfo.UseShellExecute = false;
                        process.StartInfo.CreateNoWindow = true;
                        try
                        {
                            process.Start();
                            process.WaitForExit();
                            Console.WriteLine($"msiexec exit code for {name}: {process.ExitCode}");
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine("An error occurred: " + ex.Message);
                        }
                    }
                });
            }

            if (FindLibreOfficeProducts().Count == 0)
            {
                foreach (string desktop in new[] { Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) })
                    foreach (string shortcut in new[] { "LibreOffice Writer.lnk", "LibreOffice Calc.lnk" })
                        try { File.Delete(Path.Combine(desktop, shortcut)); } catch { }
                AppendLine("✅ LibreOffice has been removed.");
                TrackResult("LibreOffice", "Removed");
            }
            else
            {
                AppendLine("❌ LibreOffice couldn't be fully removed.");
                TrackResult("LibreOffice", "Failed");
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

        private async Task<string> GetLibreOfficeVersionAsync()
        {
            string url = "https://www.libreoffice.org/download/";
            try
            {
                using var timeout = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                string html = await BrowserHttp.GetStringAsync(url, timeout.Token);
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
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
            }
            return null;
        }

        private bool _installRunning;
        private bool _cancelRequested;

        private void CancelInstallButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!_installRunning || _cancelRequested)
                return;
            _cancelRequested = true;
            CancelInstallButton.IsEnabled = false;
            CancelInstallButton.Content = "Cancelling...";
            AppendLine("🛑 Cancel requested. The install will stop after the current step finishes...");
        }

        private bool StopIfCancelled(IEnumerable<AppDefinition> remainingApps = null)
        {
            if (!_cancelRequested)
                return false;
            if (remainingApps != null)
            {
                foreach (AppDefinition app in remainingApps)
                    TrackResult(app.Name, "Skipped");
            }
            AppendLine("🛑 The installation was cancelled.");
            AppendInstallSummary();
            return true;
        }

        private async void InstallButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_installRunning)
                return;
            _installRunning = true;
            _cancelRequested = false;
            InstallButton.IsEnabled = false;
            InstallButton.IsVisible = false;
            CancelInstallButton.Content = "Cancel";
            CancelInstallButton.IsEnabled = true;
            CancelInstallButton.IsVisible = true;
            try
            {
                await RunInstallAsync();
            }
            catch (Exception ex)
            {
                AppendLine($"❌ The installation stopped because of an error: {ex.Message}");
                AppendInstallSummary();
            }
            finally
            {
                if (_sleepRestorePending)
                {
                    AppendLine("✅ Restoring the original sleep and screen timeout settings on AC power...");
                    RestoreSleepSettings();
                }
                CancelInstallButton.IsVisible = false;
                InstallButton.IsVisible = true;
                InstallButton.IsEnabled = true;
                _installRunning = false;
                _cancelRequested = false;
            }
        }

        private async Task RunInstallAsync()
        {
            installResults = new List<(string, string)>();
            if (!await PassesPasswordGateAsync())
                return;

            if (!_initialiseUrlsTask.IsCompleted)
                AppendLine("⏳ Waiting for the download links to load...");
            await _initialiseUrlsTask;
            if (urls == null)
            {
                AppendLine("🔄 Trying to load the download links again...");
                urls = await GetDownloadUrlsAsync();
            }
            if (urls == null)
            {
                AppendLine("❌ Couldn't load the download links, so nothing can be installed. Check the internet connection and try again.");
                return;
            }

            if (StopIfCancelled())
                return;

            InstallProgressBar.Value = 0;
            InstallProgressBar.Maximum = 0;

            // Paths
            rootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ComputerRepairCentre");
            programDataDir = Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData);
            string oemDir = Path.Combine(programDataDir, "Computer Repair Centre\\OEM\\");
            string appsDir = Path.Combine(rootDir, "apps");
            string crcOEMFilename = Path.Combine(oemDir, "computerRepairCentreOEM.bmp");

            // Desktop
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string[] launcherPatterns =
            {
                "Computer Repair Centre Installer*.exe",
                "Computer Repair Centre Launcher*.exe",
                "Computer.Repair.Centre.Launcher*.exe"
            };
            string lock1 = Path.Combine(desktopPath, @"service.conf.lock");
            string lock2 = Path.Combine(desktopPath, @"system.conf.lock");


            if (!Directory.Exists(rootDir)) Directory.CreateDirectory(rootDir);
            if (!Directory.Exists(oemDir)) Directory.CreateDirectory(oemDir);
            if (!Directory.Exists(appsDir)) Directory.CreateDirectory(appsDir);

            List<AppDefinition> selectedApps = BuildAppList(appsDir).Where(app => app.Check.IsChecked == true).ToList();

            if (windows10 == "1") { InstallProgressBar.Maximum += 9; }
            if (windows11 == "1") { InstallProgressBar.Maximum += 10; }
            if (PowerCheck.IsChecked == true) { InstallProgressBar.Maximum += 1; } else { InstallProgressBar.Maximum += 2; }
            if (CrcCheck.IsChecked == true) { InstallProgressBar.Maximum += 1; }
            if (TaskbarCheck.IsChecked == true) { InstallProgressBar.Maximum += 1; }
            if (PinAppsCheck.IsChecked == true) { InstallProgressBar.Maximum += 1; }
            InstallProgressBar.Maximum += selectedApps.Count * 2;

            if (AmdChipsetCheck.IsChecked == true && amdCpu == "1")
            {
                AppendLine("🧠 AMD CPU has been detected and selected, AMD Chipset Drivers will be installed.");
                AppendLine("✅ You can uncheck this if you want.");
            }

            if (AmdAdrenalinCheck.IsChecked == true && amdGpu == "1")
            {
                AppendLine("🎮 AMD GPU has been detected and selected, AMD Software: Adrenalin Edition will be installed.");
                AppendLine("✅ You can uncheck this if you want.");
            }

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
                RunSilentCommand("powercfg", "/setactive SCHEME_CURRENT");
                StepProgress();
            }
            else
            {
                AppendLine("🔄 Disabling sleep and screen timeout while on AC power temporarily during install...");
                DisableSleepTemporarily();
                StepProgress();
            }

            if (StopIfCancelled(selectedApps))
                return;

            if (CrcCheck.IsChecked == true)
            {
                AppendLine("📌 Computer Repair Centre OEM information is selected.");
                (string Name, string Hours, string Phone, string Url)? oem = SelectedOemLocation switch
                {
                    "Romsey" => ("Romsey", "Mon-Fri 9:15am-5:00pm - Sat 9:15am-2:00pm", "01794 517142", "https://www.thecomputerrepaircentre.co.uk/romsey"),
                    "Chandler's Ford" => ("Chandlers Ford", "Mon-Fri 9:00am-5:00pm - Sat 9:00am-3:00pm", "02380 270271", "https://www.thecomputerrepaircentre.co.uk/chandlers-ford"),
                    "Highcliffe" => ("Highcliffe", "Mon-Fri 9:15am-5:00pm - Sat 9:15am-2:00pm", "01425 278579", "https://www.thecomputerrepaircentre.co.uk/highcliffe"),
                    _ => null
                };
                if (oem == null)
                {
                    AppendLine("⚠️ No known shop location selected, skipping OEM info.");
                    TrackResult("Computer Repair Centre OEM Info", "Skipped");
                }
                else
                {
                    AppendLine($"📦 Installing {oem.Value.Name} Computer Repair Centre OEM information...");
                    bool logoDownloaded = await DownloadWithRetryAsync(crcOEMURL, crcOEMFilename, "the OEM logo");
                    try
                    {
                        SetOemInfo(hours: oem.Value.Hours, phone: oem.Value.Phone, url: oem.Value.Url);
                        if (logoDownloaded)
                        {
                            TrackResult("Computer Repair Centre OEM Info", "Installed");
                        }
                        else
                        {
                            AppendLine("⚠️ OEM information was set, but the logo couldn't be downloaded.");
                            TrackResult("Computer Repair Centre OEM Info", "Failed");
                        }
                    }
                    catch (Exception ex)
                    {
                        AppendLine($"❌ Failed to set OEM information: {ex.Message}");
                        TrackResult("Computer Repair Centre OEM Info", "Failed");
                    }
                }
                StepProgress();
            }

            if (TaskbarCheck.IsChecked == true)
            {
                AppendLine("📌 Move taskbar is selected.");
                AppendLine("✅ Aligning the taskbar to the left...");
                const string taskbarRegPath = @"SOFTWARE\microsoft\windows\currentversion\explorer\advanced";
                const string taskbarReg = "TaskbarAl";
                const int taskbarRegData = 0;
                try
                {
                    using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(taskbarRegPath, writable: true))
                    {
                        registryKey.SetValue(taskbarReg, taskbarRegData, RegistryValueKind.DWord);
                        Console.WriteLine($"Set '{taskbarReg}' to {taskbarRegData} in '{taskbarRegPath}'.");
                    }
                    AppendLine("✅ Moved taskbar to the left.");
                    TrackResult("Taskbar Alignment", "Applied");
                }
                catch (Exception ex)
                {
                    AppendLine($"❌ Failed to move the taskbar: {ex.Message}");
                    TrackResult("Taskbar Alignment", "Failed");
                }
                StepProgress();
            }

            for (int i = 0; i < selectedApps.Count; i++)
            {
                if (StopIfCancelled(selectedApps.Skip(i)))
                    return;
                AppDefinition app = selectedApps[i];
                try
                {
                    await InstallAppAsync(app, appsDir);
                }
                catch (Exception ex)
                {
                    AppendLine($"❌ {app.Name} error: {ex.Message}");
                    TrackResult(app.Name, "Failed");
                }
                app.AfterInstall?.Invoke();
            }

            if (StopIfCancelled())
                return;

            if (Office2024Check.IsChecked == true && IsOfficeInstalled())
            {
                await RemoveLibreOfficeAsync();
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
                StepProgress();
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
                            if (build >= 19041)
                            {
                                if (build >= 22000)
                                {
                                    AppendLine("✅ Disabling device encryption...");
                                    using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\BitLocker", writable: true))
                                        registryKey.SetValue("PreventDeviceEncryption", 1, RegistryValueKind.DWord);
                                    StepProgress();
                                }

                                AppendLine("✅ Setting explorer to open to This PC...");
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced", writable: true))
                                    registryKey.SetValue("LaunchTo", 1, RegistryValueKind.DWord);
                                StepProgress();

                                AppendLine("✅ Disabling fastboot mode...");
                                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Control\Session Manager\Power", writable: true))
                                    registryKey.SetValue("HiberbootEnabled", 0, RegistryValueKind.DWord);
                                StepProgress();

                                AppendLine("✅ Disabling location tracking...");
                                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}", writable: true))
                                    registryKey.SetValue("SensorPermissionState", 0, RegistryValueKind.DWord);
                                StepProgress();
                                using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(@"SYSTEM\CurrentControlSet\Services\lfsvc\Service\Configuration", writable: true))
                                    registryKey.SetValue("Status", 0, RegistryValueKind.DWord);
                                StepProgress();

                                AppendLine("✅ Disabling People icon...");
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People", writable: true)) { }
                                StepProgress();
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer\Advanced\People", writable: true))
                                    registryKey.SetValue("PeopleBand", 0, RegistryValueKind.DWord);
                                StepProgress();

                                AppendLine("✅ Hiding recently used files and folders in File Explorer...");
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", writable: true))
                                    registryKey.SetValue("ShowRecent", 0, RegistryValueKind.DWord);
                                StepProgress();
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer", writable: true))
                                    registryKey.SetValue("ShowFrequent", 0, RegistryValueKind.DWord);
                                StepProgress();

                                AppendLine("✅ Resetting cursor size to default (20)...");
                                using (RegistryKey registryKey = Registry.CurrentUser.CreateSubKey(@"Control Panel\Cursors", writable: true))
                                    registryKey.SetValue("CursorBaseSize", 32, RegistryValueKind.DWord);
                                StepProgress();
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

            if (_sleepRestorePending)
            {
                AppendLine("✅ Restoring the original sleep and screen timeout settings on AC power...");
                RestoreSleepSettings();
                StepProgress();
            }

            AppendLine("✅ Cleaning up installation files...");
            var deletionHelper = new FileDeletionHelper();
            var launcherPaths = new List<string>();
            if (Directory.Exists(desktopPath))
            {
                foreach (string pattern in launcherPatterns)
                {
                    try
                    {
                        launcherPaths.AddRange(Directory.EnumerateFiles(desktopPath, pattern));
                    }
                    catch
                    {
                    }
                }
            }
            await deletionHelper.DeleteFilesAndDirectoryAsync(appsDir, launcherPaths, lock1, lock2);
            StepProgress();

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

            if (_isBetaBuild)
            {
                AudioEffects.PlayCompleteChime();
            }
            else
            {
                _themeManager.PlayEventSound();
            }

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
