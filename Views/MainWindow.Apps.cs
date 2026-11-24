using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Win32;
using PlutoPoint_Installer.Models;
using PlutoPoint_Installer.Services;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Views
{
    public partial class MainWindow
    {
        private sealed class AppDefinition
        {
            public required string Name { get; init; }
            public required CheckBox Check { get; init; }
            public required string InstallerFileName { get; init; }
            public Func<Uri>? Url { get; init; }
            public Func<string, Task<bool?>>? Download { get; init; }
            public string? WingetId { get; init; }
            public string Arguments { get; init; } = "";
            public bool Elevate { get; init; }
            public bool RunInAppsDirectory { get; init; }
            public Func<bool>? IsInstalled { get; init; }
            public Func<string?>? SkipReason { get; init; }
            public string? InstallingMessage { get; init; }
            public Action? AfterInstall { get; init; }
            public TimeSpan Timeout { get; init; } = TimeSpan.FromMinutes(10);
        }

        private List<AppDefinition> BuildAppList(string appsDir)
        {
            string localAppData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
            bool firefoxIsExe = ArmOverride("mozillaFirefox") != null;
            string officeConfigPath = Path.Combine(appsDir, "office2024.xml");

            return new List<AppDefinition>
            {
                new()
                {
                    Name = "NanaZip",
                    Check = NanaZipCheck,
                    InstallerFileName = "nanaZip.msixbundle",
                    Url = () => nanaZipURL,
                    IsInstalled = InstalledApps.IsNanaZipInstalled
                },
                new()
                {
                    Name = "AnyDesk",
                    Check = AnyDeskCheck,
                    InstallerFileName = "anyDesk.msi",
                    Url = () => anyDeskURL,
                    WingetId = "AnyDesk.AnyDesk",
                    IsInstalled = () =>
                        File.Exists(@"C:\Program Files (x86)\AnyDeskMSI\AnyDeskMSI.exe") ||
                        File.Exists(@"C:\Program Files (x86)\AnyDesk\AnyDesk.exe") ||
                        InstalledApps.IsListed(n => n.StartsWith("AnyDesk", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "Bing Wallpapers",
                    Check = BingWallpapersCheck,
                    InstallerFileName = "bingWallpapers.msi",
                    Url = () => bingWallpapersURL,
                    IsInstalled = () =>
                        File.Exists(Path.Combine(localAppData, @"Microsoft\BingWallpaperApp\BingWallpaperApp.exe")) ||
                        InstalledApps.IsListed(n => n.StartsWith("Bing Wallpaper", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "BitDefender",
                    Check = BitDefenderCheck,
                    InstallerFileName = "bitDefender.exe",
                    Url = () => bitDefenderURL,
                    Arguments = "/bdparams /silent",
                    Timeout = TimeSpan.FromMinutes(20),
                    Elevate = true,
                    IsInstalled = () =>
                        File.Exists(@"C:\Program Files\Bitdefender\Bitdefender Security App\seccenter.exe") ||
                        InstalledApps.IsListed(n => n.StartsWith("Bitdefender", StringComparison.OrdinalIgnoreCase) &&
                            (n.Contains("Security", StringComparison.OrdinalIgnoreCase) || n.Contains("Antivirus", StringComparison.OrdinalIgnoreCase)))
                },
                new()
                {
                    Name = "Discord",
                    Check = DiscordCheck,
                    InstallerFileName = "discord.exe",
                    Url = () => discordURL,
                    WingetId = "Discord.Discord",
                    Arguments = "-s",
                    Elevate = true,
                    IsInstalled = () =>
                        File.Exists(Path.Combine(localAppData, @"Discord\Update.exe")) ||
                        InstalledApps.IsListed(n => n.Equals("Discord", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "Google Chrome",
                    Check = GoogleChromeCheck,
                    InstallerFileName = "googleChrome.msi",
                    Url = () => googleChromeURL,
                    WingetId = "Google.Chrome",
                    IsInstalled = () =>
                        File.Exists(@"C:\Program Files\Google\Chrome\Application\chrome.exe") ||
                        InstalledApps.IsListed(n => n.Equals("Google Chrome", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "LibreOffice",
                    Check = LibreOfficeCheck,
                    InstallerFileName = "libreOffice.msi",
                    Download = path => _vendorDownloads.LibreOfficeAsync(path, libreOfficeFallbackURL, _stop.Token),
                    WingetId = "TheDocumentFoundation.LibreOffice",
                    Arguments = "/norestart",
                    SkipReason = () => Office2024Check.IsChecked == true
                        ? "Microsoft Office 2024 is selected, so LibreOffice won't be installed."
                        : null,
                    IsInstalled = () =>
                        File.Exists(@"C:\Program Files\LibreOffice\program\soffice.exe") ||
                        InstalledApps.FindLibreOfficeProducts().Any(p => !p.Name.Contains("Pack", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "AMD Chipset Drivers",
                    Check = AmdChipsetCheck,
                    InstallerFileName = "amdChipset.exe",
                    Download = path => _vendorDownloads.AmdChipsetAsync(path, urls, _stop.Token),
                    IsInstalled = InstalledApps.IsAmdChipsetInstalled,
                    Arguments = "/S",
                    Elevate = true,
                    SkipReason = () => HardwareInfo.IsArm64 ? "AMD Chipset Drivers are not available for ARM64, skipping." : null,
                    InstallingMessage = "📦 Installing AMD Chipset Drivers silently..."
                },
                new()
                {
                    Name = "AMD Software: Adrenalin Edition",
                    Check = AmdAdrenalinCheck,
                    InstallerFileName = "amdAdrenalin.exe",
                    Download = path => _vendorDownloads.AmdAdrenalinAsync(path, _stop.Token),
                    IsInstalled = InstalledApps.IsAmdAdrenalinInstalled,
                    Timeout = TimeSpan.FromMinutes(20),
                    Arguments = "-install",
                    Elevate = true,
                    SkipReason = () => HardwareInfo.IsArm64 ? "AMD Software: Adrenalin Edition is not available for ARM64, skipping." : null,
                    InstallingMessage = "📦 Installing AMD Software: Adrenalin Edition, this downloads the driver so it can take a while..."
                },
                new()
                {
                    Name = "Nvidia App",
                    Check = NvidiaAppCheck,
                    InstallerFileName = "nvidiaApp.exe",
                    Download = path => _vendorDownloads.NvidiaAppAsync(path, urls, _stop.Token),
                    IsInstalled = InstalledApps.IsNvidiaAppInstalled,
                    Arguments = "/s",
                    Elevate = true,
                    InstallingMessage = "📦 Installing Nvidia App silently..."
                },
                new()
                {
                    Name = "Mozilla Firefox",
                    Check = MozillaFirefoxCheck,
                    InstallerFileName = firefoxIsExe ? "mozillaFirefox.exe" : "mozillaFirefox.msi",
                    Url = () => mozillaFirefoxURL,
                    WingetId = "Mozilla.Firefox",
                    Arguments = firefoxIsExe ? "/S" : "",
                    IsInstalled = () =>
                        File.Exists(@"C:\Program Files\Mozilla Firefox\firefox.exe") ||
                        InstalledApps.IsListed(n => n.StartsWith("Mozilla Firefox", StringComparison.OrdinalIgnoreCase)),
                    AfterInstall = RemoveFirefoxAutoLaunchEntry
                },
                new()
                {
                    Name = "Mozilla Thunderbird",
                    Check = MozillaThunderbirdCheck,
                    InstallerFileName = "mozillaThunderbird.msi",
                    Url = () => mozillaThunderbirdURL,
                    WingetId = "Mozilla.Thunderbird",
                    IsInstalled = () =>
                        File.Exists(@"C:\Program Files\Mozilla Thunderbird\thunderbird.exe") ||
                        InstalledApps.IsListed(n => n.StartsWith("Mozilla Thunderbird", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "Steam",
                    Check = SteamCheck,
                    InstallerFileName = "steam.exe",
                    Url = () => steamURL,
                    WingetId = "Valve.Steam",
                    Arguments = "/S",
                    Elevate = true,
                    IsInstalled = () =>
                        File.Exists(@"C:\Program Files (x86)\Steam\Steam.exe") ||
                        InstalledApps.IsListed(n => n.Equals("Steam", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "VLC Media Player",
                    Check = VlcMediaPlayerCheck,
                    InstallerFileName = "vlcMediaPlayer.msi",
                    Url = () => vlcMediaPlayerURL,
                    WingetId = "VideoLAN.VLC",
                    IsInstalled = () =>
                        File.Exists(@"C:\Program Files\VideoLAN\VLC\vlc.exe") ||
                        InstalledApps.IsListed(n => n.StartsWith("VLC media player", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "Microsoft Office 2024",
                    Check = Office2024Check,
                    InstallerFileName = "officeSetup.exe",
                    Download = path => DownloadOfficeAsync(path, officeConfigPath),
                    Arguments = $"/configure \"{officeConfigPath}\"",
                    RunInAppsDirectory = true,
                    Timeout = TimeSpan.FromMinutes(20),
                    IsInstalled = InstalledApps.IsOfficeInstalled,
                    InstallingMessage = "📦 Installing Microsoft Office 2024, this can take a while..."
                }
            };
        }

        private async Task InstallAppAsync(AppDefinition app, string appsDir)
        {
            AppendLine($"📌 {app.Name} is selected.");

            string? skipReason = app.SkipReason?.Invoke();
            if (skipReason != null)
            {
                AppendLine($"⏭️ {skipReason}");
                TrackResult(app.Name, InstallStatus.Skipped);
                StepProgress(2);
                return;
            }

            if (await CheckInstalledAsync(app) == true)
            {
                AppendLine($"✅ {app.Name} is already installed, skipping installation.");
                TrackResult(app.Name, InstallStatus.AlreadyInstalled);
                StepProgress(2);
                return;
            }

            if (app.WingetId != null && await TryWingetAsync(app))
            {
                TrackResult(app.Name, InstallStatus.Installed);
                StepProgress(2);
                return;
            }

            _stop.Token.ThrowIfCancellationRequested();

            string installerPath = Path.Combine(appsDir, app.InstallerFileName);
            bool? downloaded;
            if (app.Download != null)
            {
                downloaded = await app.Download(installerPath);
            }
            else
            {
                AppendLine($"🔄 Downloading {app.Name}...");
                downloaded = await _downloader.DownloadWithRetryAsync(app.Url!(), installerPath, app.Name, _stop.Token);
            }
            StepProgress();

            if (downloaded != true)
            {
                TrackResult(app.Name, downloaded == null ? InstallStatus.Skipped : InstallStatus.Failed);
                StepProgress();
                return;
            }

            AppendLine(app.InstallingMessage ?? $"📦 Installing {app.Name}...");
            var (fileName, arguments) = GetInstallerCommand(app, installerPath);
            InstallerResult result;
            SetInstallerActive(true);
            try
            {
                result = await ProcessRunner.RunInstallerAsync(fileName, arguments, app.Timeout, AppendLine, app.Elevate,
                    app.RunInAppsDirectory ? appsDir : null, _kill.Token);
            }
            finally
            {
                SetInstallerActive(false);
            }
            ReportInstallOutcome(app, await ConfirmInstalledAsync(app, result), result);
            StepProgress();
        }

        private async Task<bool> TryWingetAsync(AppDefinition app)
        {
            SetInstallerActive(true);
            try
            {
                return await _winget.TryInstallAsync(app.WingetId!, app.Name, _kill.Token);
            }
            finally
            {
                SetInstallerActive(false);
            }
        }

        private static Task<bool?> CheckInstalledAsync(AppDefinition app) => Task.Run(() => app.IsInstalled?.Invoke());

        private static async Task<bool?> ConfirmInstalledAsync(AppDefinition app, InstallerResult result)
        {
            bool? installed = await CheckInstalledAsync(app);
            for (int i = 0; i < 5 && installed == false && result.Succeeded; i++)
            {
                await Task.Delay(2000);
                installed = await CheckInstalledAsync(app);
            }
            return installed;
        }

        private static (string FileName, string Arguments) GetInstallerCommand(AppDefinition app, string installerPath)
        {
            switch (Path.GetExtension(installerPath).ToLowerInvariant())
            {
                case ".msi":
                    return ("msiexec", $"/package \"{installerPath}\" /passive {app.Arguments}".TrimEnd());
                case ".msixbundle":
                    return ("powershell", $"-NoProfile -NonInteractive -Command Add-AppxPackage -Path '{installerPath}'");
                default:
                    return (installerPath, app.Arguments);
            }
        }

        private void ReportInstallOutcome(AppDefinition app, bool? detected, InstallerResult result)
        {
            if (result.End == InstallerEnd.Stopped)
            {
                AppendLine($"❌ {app.Name} was force stopped.");
                TrackResult(app.Name, InstallStatus.Failed);
            }
            else if (result.End == InstallerEnd.TimedOut)
            {
                AppendLine($"❌ {app.Name} was still running after {app.Timeout.TotalMinutes:0} minutes, so it was stopped.");
                TrackResult(app.Name, detected == true ? InstallStatus.Unverified : InstallStatus.Failed);
            }
            else if (detected == true || (detected == null && result.Succeeded))
            {
                AppendLine($"✅ Completed installation of {app.Name}.");
                TrackResult(app.Name, InstallStatus.Installed);
            }
            else if (result.Succeeded)
            {
                AppendLine($"⚠️ The {app.Name} installer finished, but {app.Name} couldn't be found afterwards.");
                TrackResult(app.Name, InstallStatus.Unverified);
            }
            else
            {
                AppendLine($"❌ {app.Name} failed to install (exit code {result.ExitCode}).");
                TrackResult(app.Name, InstallStatus.Failed);
            }
        }

        private async Task<bool?> DownloadOfficeAsync(string installerPath, string configPath)
        {
            AppendLine("🔄 Downloading Microsoft Office 2024...");
            if (!await _downloader.DownloadWithRetryAsync(office2024URL, installerPath, "Microsoft Office 2024", _stop.Token))
                return false;
            await File.WriteAllTextAsync(configPath, Office2024Config);
            return true;
        }

        private async Task RemoveLibreOfficeAsync()
        {
            var products = InstalledApps.FindLibreOfficeProducts();
            if (products.Count == 0)
                return;

            AppendLine("🧹 Microsoft Office 2024 is installed, removing LibreOffice...");
            SetInstallerActive(true);
            try
            {
                foreach (var (name, productCode) in products)
                {
                    AppendLine($"🗑️ Uninstalling {name}...");
                    await ProcessRunner.RunInstallerAsync("msiexec", $"/x {productCode} /qn /norestart", TimeSpan.FromMinutes(10), AppendLine, kill: _kill.Token);
                }
            }
            finally
            {
                SetInstallerActive(false);
            }

            if (InstalledApps.FindLibreOfficeProducts().Count == 0)
            {
                foreach (string desktop in new[] { Environment.GetFolderPath(Environment.SpecialFolder.Desktop), Environment.GetFolderPath(Environment.SpecialFolder.CommonDesktopDirectory) })
                    foreach (string shortcut in new[] { "LibreOffice Writer.lnk", "LibreOffice Calc.lnk" })
                        try { File.Delete(Path.Combine(desktop, shortcut)); } catch { }
                AppendLine("✅ LibreOffice has been removed.");
                TrackResult("LibreOffice", InstallStatus.Removed);
            }
            else
            {
                AppendLine("❌ LibreOffice couldn't be fully removed.");
                TrackResult("LibreOffice", InstallStatus.Failed);
            }
        }

        private void RemoveFirefoxAutoLaunchEntry()
        {
            try
            {
                using (RegistryKey? runKey = Registry.CurrentUser.OpenSubKey(
                    @"Software\Microsoft\Windows\CurrentVersion\Run", writable: true))
                {
                    if (runKey == null) return;
                    foreach (string valueName in runKey.GetValueNames())
                    {
                        string? data = runKey.GetValue(valueName) as string;
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
    }
}
