using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Platform;
using Avalonia.Threading;
using Microsoft.Win32;
using PlutoPoint_Installer.Models;
using PlutoPoint_Installer.Services;
using PlutoPoint_Installer.Utilities;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Views
{
    public partial class MainWindow : Window
    {
        private Shop _shop = Shop.Unknown;
        private int? _windowsBuild;
        private HardwareInfo _hardware = new HardwareInfo();
        private DownloadUrls urls = null!;
        private readonly Task _initialiseUrlsTask;
        private readonly Task _checkShopTask;
        private string? _logFilePath;
        private readonly PlutoPoint_Installer.UI.ThemeManager _themeManager = new PlutoPoint_Installer.UI.ThemeManager();
        private readonly Downloader _downloader;
        private readonly VendorDownloads _vendorDownloads;
        private readonly WingetService _winget;
        private readonly PowerSettings _power = new PowerSettings();
        private bool _shutdownScheduled;
        private readonly DateTime buildDate = DateTime.Parse(
    Assembly.GetExecutingAssembly()
        .GetCustomAttribute<PlutoPoint_Installer.Attributes.AssemblyUpdateDateAttribute>()
        ?.Date ?? "1970-01-01T00:00:00Z",
    System.Globalization.CultureInfo.InvariantCulture,
    System.Globalization.DateTimeStyles.RoundtripKind).ToLocalTime();

        private string? ArmOverride(string key) =>
            HardwareInfo.IsArm64 && urls.arm64 != null && urls.arm64.TryGetValue(key, out var armUrl) && !string.IsNullOrWhiteSpace(armUrl)
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
            _downloader = new Downloader(AppendLine);
            _vendorDownloads = new VendorDownloads(_downloader, AppendLine);
            _winget = new WingetService(AppendLine);
            ApplyBetaBranding();
            WireCheckboxMutualExclusion();
            foreach (Button button in new[] { InstallButton, CancelInstallButton, RestartButton, CloseButton })
            {
                button.PointerEntered += (s, e) => AudioEffects.PlayHoverPop();
                button.Click += (s, e) => AudioEffects.PlayClickChime();
            }
            // Background tasks only
            _initialiseUrlsTask = InitialiseUrlsAsync();
            _checkShopTask = CheckShopAsync();
            // Info checks
            PrintVersion();
            CheckWindowsVersion();
            CheckHardware();
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
                    TitleSuffixRun.Text += $" - {titleSuffix}";
                }
            );
            _themeManager.ApplyTextColor(
                (textColor) =>
                {
                    TitleTextBlock.Foreground = new SolidColorBrush(textColor);
                    SoftwareGroup.Foreground = new SolidColorBrush(textColor);
                    UtilitiesGroup.Foreground = new SolidColorBrush(textColor);
                }
            );
            _themeManager.ApplyTitleAccent(
                (start, end) =>
                {
                    TitleAccentRun.Foreground = new LinearGradientBrush
                    {
                        StartPoint = new RelativePoint(0, 0, RelativeUnit.Relative),
                        EndPoint = new RelativePoint(1, 1, RelativeUnit.Relative),
                        GradientStops = { new GradientStop(start, 0), new GradientStop(end, 1) }
                    };
                }
            );
            _themeManager.UpdateGUIEvent(this, SeasonalOverlayImage);
            ApplyProgressBarColour();
        }

        private void ApplyProgressBarColour()
        {
            if (Background is not LinearGradientBrush gradientBrush || gradientBrush.GradientStops.Count < 2)
                return;

            double position = 512.0 / 600.0;
            var barCentre = InstallProgressBar.TranslatePoint(new Point(0, InstallProgressBar.Bounds.Height / 2), this);
            if (barCentre.HasValue && Bounds.Height > 0)
                position = Math.Clamp(barCentre.Value.Y / Bounds.Height, 0, 1);

            Color top = gradientBrush.GradientStops[0].Color;
            Color bottom = gradientBrush.GradientStops[1].Color;
            var behindBar = Color.FromRgb(
                (byte)Math.Round(top.R + (bottom.R - top.R) * position),
                (byte)Math.Round(top.G + (bottom.G - top.G) * position),
                (byte)Math.Round(top.B + (bottom.B - top.B) * position));

            HslColor hsl = behindBar.ToHsl();
            double hue = (hsl.H - 4 + 360) % 360;
            double lightness = hsl.L * 0.68;
            if (hsl.L - lightness < 0.12)
                lightness = hsl.L + 0.18;
            InstallProgressBar.Foreground = new SolidColorBrush(
                HslColor.ToRgb(hue, Math.Clamp(hsl.S * 0.81, 0, 1), Math.Clamp(lightness, 0, 1)));
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

        private void AppendLine(string text)
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

        private void ApplyBetaBranding()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string? versionStr = assembly
                .GetCustomAttribute<System.Reflection.AssemblyInformationalVersionAttribute>()
                ?.InformationalVersion;
            if (string.IsNullOrWhiteSpace(versionStr) || !versionStr.Contains("b"))
                return;

            Title += " Beta";
            TitleSuffixRun.Text += " Beta";
            BetaBadge.IsVisible = true;
            try
            {
                Icon = new WindowIcon(AssetLoader.Open(new Uri("avares://Computer Repair Centre Installer/Resources/icons/computerRepairCentreIconBeta.ico")));
            }
            catch
            {
            }
        }

        private void PrintVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            string? versionStr = assembly
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
            await Task.Delay(325);
            if (_shutdownScheduled)
            {
                _shutdownScheduled = false;
                ProcessRunner.RunSilent("shutdown", "/a");
            }
            Close();
        }

        private async void RestartButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            await Task.Delay(325);
            if (_installRunning)
                StopThen(AfterStop.Restart);
            else
                RestartNow();
        }

        private static void RestartNow() => ProcessRunner.RunSilent("shutdown", "/r /t 1");

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
                _windowsBuild = WindowsTweaks.ReadBuild();
                if (_windowsBuild == null)
                {
                    AppendLine("⚠️ Unable to determine Windows version.");
                    return;
                }
                string osLabelText;
                if (_windowsBuild >= WindowsTweaks.Windows11Build)
                {
                    AppendLine("🪟 Windows 11 detected.");
                    osLabelText = "🪟 Windows 11";
                }
                else if (_windowsBuild >= 10240)
                {
                    AppendLine("🪟 Windows 10 detected. Time to move on grandad.");
                    osLabelText = "🪟 Windows 10";
                }
                else
                {
                    AppendLine("🪟 Older or unknown Windows version detected.");
                    osLabelText = "🪟 Unknown OS";
                }
                if (HardwareInfo.IsArm64)
                    AppendLine("ARM processor detected.");
                OsLabel.Text = osLabelText + (HardwareInfo.IsArm64 ? " ARM" : " x64");
            }
            catch (Exception ex)
            {
                AppendLine("❌ Error checking Windows version: " + ex.Message);
            }
        }

        private void CheckHardware()
        {
            _hardware = HardwareInfo.Detect();
            LogVendor("Intel", _hardware.HasIntelCpu, _hardware.HasIntelGpu);
            LogVendor("AMD", _hardware.HasAmdCpu, _hardware.HasAmdGpu);
            LogVendor("Nvidia", false, _hardware.HasNvidiaGpu);
            AmdChipsetCheck.IsChecked = _hardware.HasAmdCpu && !HardwareInfo.IsArm64;
            AmdAdrenalinCheck.IsChecked = _hardware.HasAmdGpu && !HardwareInfo.IsArm64;
            NvidiaAppCheck.IsChecked = _hardware.HasNvidiaGpu;
        }

        private void LogVendor(string vendor, bool cpu, bool gpu)
        {
            if (cpu && gpu)
                AppendLine($"🧠 + 🎮 {vendor} CPU and GPU detected.");
            else if (gpu)
                AppendLine($"🎮 {vendor} GPU detected.");
            else if (cpu)
                AppendLine($"🧠 {vendor} CPU detected.");
        }

        private async Task CheckShopAsync()
        {
            _shop = await RemoteData.DetectShopAsync(AppendLine);
            ShopDetails? details = ShopDetails.For(_shop);
            AppendLine(details == null
                ? "📍 Location unknown."
                : $"📍 The installer is being run from the {details.LogName} shop.");
            Dispatcher.UIThread.Post(() =>
            {
                LocationLabel.Text = "Current location: " + (details?.DisplayName ?? "Unknown");
                foreach (var item in LocationOverrideComboBox.Items)
                {
                    if (details != null && item is ComboBoxItem cbi && cbi.Content?.ToString() == details.DisplayName)
                    {
                        LocationOverrideComboBox.SelectedItem = item;
                        break;
                    }
                }
            });
        }

        private ShopDetails? SelectedOemShop =>
            ShopDetails.ForDisplayName((LocationOverrideComboBox.SelectedItem as ComboBoxItem)?.Content?.ToString());

        private async Task InitialiseUrlsAsync()
        {
            DownloadUrls? loaded = await RemoteData.GetDownloadUrlsAsync(AppendLine);
            if (loaded == null)
                AppendLine("❌ Failed to load download URLs.");
            else
                urls = loaded;
        }

        private async Task<bool> PassesPasswordGateAsync()
        {
            if (!_checkShopTask.IsCompleted)
                AppendLine("⏳ Waiting for the location check to finish...");
            try
            {
                await _checkShopTask;
            }
            catch (Exception ex)
            {
                AppendLine($"⚠️ Location check failed: {ex.Message}");
            }
            if (_shop != Shop.Unknown)
                return true;

            var hashes = await RemoteData.GetPasswordHashesAsync(AppendLine);
            if (hashes?.allowedHashes == null || hashes.allowedHashes.Count == 0)
            {
                AppendLine("❌ Unable to load password hashes.");
                return false;
            }

            var dialog = new PasswordDialog();
            bool? result = await dialog.ShowDialog<bool?>(this);
            if (result == true)
            {
                string enteredHash = RemoteData.Sha256(dialog.EnteredPassword ?? "");
                if (!hashes.allowedHashes.Contains(enteredHash))
                {
                    AppendLine("❌ Incorrect password. Exiting.");
                    _afterStop = AfterStop.Close;
                    return false;
                }
                return true;
            }

            AppendLine("❌ Password required. Exiting installer.");
            Environment.Exit(0);
            return false;
        }
    }
}
