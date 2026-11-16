using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using Microsoft.Win32;

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
                    IsInstalled = IsNanaZipInstalled
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
                        IsListedAsInstalled(n => n.StartsWith("AnyDesk", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "Bing Wallpapers",
                    Check = BingWallpapersCheck,
                    InstallerFileName = "bingWallpapers.msi",
                    Url = () => bingWallpapersURL,
                    IsInstalled = () =>
                        File.Exists(Path.Combine(localAppData, @"Microsoft\BingWallpaperApp\BingWallpaperApp.exe")) ||
                        IsListedAsInstalled(n => n.StartsWith("Bing Wallpaper", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "BitDefender",
                    Check = BitDefenderCheck,
                    InstallerFileName = "bitDefender.exe",
                    Url = () => bitDefenderURL,
                    Arguments = "/bdparams /silent",
                    Elevate = true,
                    IsInstalled = () =>
                        File.Exists(@"C:\Program Files\Bitdefender\Bitdefender Security App\seccenter.exe") ||
                        IsListedAsInstalled(n => n.StartsWith("Bitdefender", StringComparison.OrdinalIgnoreCase) &&
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
                        IsListedAsInstalled(n => n.Equals("Discord", StringComparison.OrdinalIgnoreCase))
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
                        IsListedAsInstalled(n => n.Equals("Google Chrome", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "LibreOffice",
                    Check = LibreOfficeCheck,
                    InstallerFileName = "libreOffice.msi",
                    Download = DownloadLibreOfficeAsync,
                    WingetId = "TheDocumentFoundation.LibreOffice",
                    Arguments = "/norestart",
                    SkipReason = () => Office2024Check.IsChecked == true
                        ? "Microsoft Office 2024 is selected, so LibreOffice won't be installed."
                        : null,
                    IsInstalled = () =>
                        File.Exists(@"C:\Program Files\LibreOffice\program\soffice.exe") ||
                        FindLibreOfficeProducts().Any(p => !p.Name.Contains("Pack", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "AMD Chipset Drivers",
                    Check = AmdChipsetCheck,
                    InstallerFileName = "amdChipset.exe",
                    Download = DownloadAmdChipsetAsync,
                    Arguments = "/S",
                    Elevate = true,
                    SkipReason = () => IsArm64 ? "AMD Chipset Drivers are not available for ARM64, skipping." : null,
                    InstallingMessage = "📦 Installing AMD Chipset Drivers silently..."
                },
                new()
                {
                    Name = "AMD Software: Adrenalin Edition",
                    Check = AmdAdrenalinCheck,
                    InstallerFileName = "amdAdrenalin.exe",
                    Download = DownloadAmdAdrenalinAsync,
                    Arguments = "-install",
                    Elevate = true,
                    SkipReason = () => IsArm64 ? "AMD Software: Adrenalin Edition is not available for ARM64, skipping." : null,
                    InstallingMessage = "📦 Installing AMD Software: Adrenalin Edition, this downloads the driver so it can take a while..."
                },
                new()
                {
                    Name = "Nvidia App",
                    Check = NvidiaAppCheck,
                    InstallerFileName = "nvidiaApp.exe",
                    Download = DownloadNvidiaAppAsync,
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
                        IsListedAsInstalled(n => n.StartsWith("Mozilla Firefox", StringComparison.OrdinalIgnoreCase)),
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
                        IsListedAsInstalled(n => n.StartsWith("Mozilla Thunderbird", StringComparison.OrdinalIgnoreCase))
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
                        IsListedAsInstalled(n => n.Equals("Steam", StringComparison.OrdinalIgnoreCase))
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
                        IsListedAsInstalled(n => n.StartsWith("VLC media player", StringComparison.OrdinalIgnoreCase))
                },
                new()
                {
                    Name = "Microsoft Office 2024",
                    Check = Office2024Check,
                    InstallerFileName = "officeSetup.exe",
                    Download = path => DownloadOfficeAsync(path, officeConfigPath),
                    Arguments = $"/configure \"{officeConfigPath}\"",
                    RunInAppsDirectory = true,
                    IsInstalled = IsOfficeInstalled,
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
                TrackResult(app.Name, "Skipped");
                StepProgress(2);
                return;
            }

            if (app.IsInstalled?.Invoke() == true)
            {
                AppendLine($"✅ {app.Name} is already installed, skipping installation.");
                TrackResult(app.Name, "Already Installed");
                StepProgress(2);
                return;
            }

            if (app.WingetId != null && await TryWingetInstallAsync(app.WingetId, app.Name))
            {
                TrackResult(app.Name, "Installed");
                StepProgress(2);
                return;
            }

            string installerPath = Path.Combine(appsDir, app.InstallerFileName);
            bool? downloaded;
            if (app.Download != null)
            {
                downloaded = await app.Download(installerPath);
            }
            else
            {
                AppendLine($"🔄 Downloading {app.Name}...");
                downloaded = await DownloadWithRetryAsync(app.Url!(), installerPath, app.Name);
            }
            StepProgress();

            if (downloaded != true)
            {
                TrackResult(app.Name, downloaded == null ? "Skipped" : "Failed");
                StepProgress();
                return;
            }

            AppendLine(app.InstallingMessage ?? $"📦 Installing {app.Name}...");
            var (fileName, arguments) = GetInstallerCommand(app, installerPath);
            int exitCode = await RunInstallerAsync(fileName, arguments, app.Elevate, app.RunInAppsDirectory ? appsDir : null);
            ReportInstallOutcome(app.Name, app.IsInstalled?.Invoke(), exitCode);
            StepProgress();
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

        private void ReportInstallOutcome(string label, bool? detected, int exitCode)
        {
            bool exitOk = exitCode == 0 || exitCode == 3010;
            if (detected == true || (detected == null && exitOk))
            {
                AppendLine($"✅ Completed installation of {label}.");
                TrackResult(label, "Installed");
            }
            else if (exitOk)
            {
                AppendLine($"⚠️ The {label} installer finished, but {label} couldn't be found afterwards.");
                TrackResult(label, "Unverified");
            }
            else
            {
                AppendLine($"❌ {label} failed to install (exit code {exitCode}).");
                TrackResult(label, "Failed");
            }
        }

        private static bool IsListedAsInstalled(Func<string, bool> matches)
        {
            var locations = new[]
            {
                (RegistryHive.LocalMachine, RegistryView.Registry64),
                (RegistryHive.LocalMachine, RegistryView.Registry32),
                (RegistryHive.CurrentUser, RegistryView.Default)
            };
            foreach (var (hive, view) in locations)
            {
                try
                {
                    using RegistryKey baseKey = RegistryKey.OpenBaseKey(hive, view);
                    using RegistryKey? uninstall = baseKey.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
                    if (uninstall == null)
                        continue;
                    foreach (string keyName in uninstall.GetSubKeyNames())
                    {
                        using RegistryKey? entry = uninstall.OpenSubKey(keyName);
                        if (entry?.GetValue("DisplayName") is string name && matches(name))
                            return true;
                    }
                }
                catch
                {
                }
            }
            return false;
        }

        private static bool IsNanaZipInstalled()
        {
            string? output = RunAndCapture("powershell", "-NoProfile -NonInteractive -Command \"Get-AppxPackage -Name '*NanaZip*' | Select-Object -ExpandProperty Name\"");
            if (output != null && output.Contains("NanaZip", StringComparison.OrdinalIgnoreCase))
                return true;
            try
            {
                return Directory.GetFiles(@"C:\Program Files\WindowsApps", "NanaZip.Modern.FileManager.exe", SearchOption.AllDirectories).Length > 0;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool?> DownloadLibreOfficeAsync(string installerPath)
        {
            AppendLine("🔄 Finding the latest LibreOffice version...");
            string version = await GetLibreOfficeVersionAsync();
            if (string.IsNullOrEmpty(version))
            {
                AppendLine("⚠️ Could not determine the latest LibreOffice version from libreoffice.org.");
            }
            else
            {
                string url = IsArm64
                    ? $"https://download.documentfoundation.org/libreoffice/stable/{version}/win/aarch64/LibreOffice_{version}_Win_aarch64.msi"
                    : $"https://download.documentfoundation.org/libreoffice/stable/{version}/win/x86_64/LibreOffice_{version}_Win_x86-64.msi";
                AppendLine($"🔄 Downloading LibreOffice {version}...");
                if (await DownloadWithRetryAsync(new Uri(url), installerPath, "LibreOffice"))
                    return true;
            }
            AppendLine("🔁 Using the fallback LibreOffice download...");
            return await DownloadWithRetryAsync(libreOfficeFallbackURL, installerPath, "LibreOffice");
        }

        private async Task<bool?> DownloadOfficeAsync(string installerPath, string configPath)
        {
            AppendLine("🔄 Downloading Microsoft Office 2024...");
            if (!await DownloadWithRetryAsync(office2024URL, installerPath, "Microsoft Office 2024"))
                return false;
            await File.WriteAllTextAsync(configPath, Office2024Config);
            return true;
        }

        private async Task<bool?> DownloadFromCandidatesAsync(string label, List<string> candidates, string installerPath)
        {
            foreach (string candidate in candidates)
            {
                AppendLine($"🔗 Trying {label} from: {candidate}");
                try
                {
                    if (await DownloadInstallerAsync(BrowserHttp, candidate, installerPath, AmdReferer))
                        return true;
                }
                catch (Exception ex)
                {
                    AppendLine($"⚠️ Download failed: {ex.Message}");
                }
            }
            AppendLine($"⚠️ Could not download {label}.");
            return false;
        }

        private async Task<bool?> DownloadAmdChipsetAsync(string installerPath)
        {
            AppendLine("🔄 Searching for latest AMD Chipset Drivers...");
            List<string> candidates = await FindAmdChipsetDownloadUrlsAsync(BrowserHttp);
            return await DownloadFromCandidatesAsync("AMD Chipset Drivers", candidates, installerPath);
        }

        private async Task<bool?> DownloadAmdAdrenalinAsync(string installerPath)
        {
            AppendLine("🔄 Searching for latest AMD Software: Adrenalin Edition...");
            List<string> candidates = await FindAmdAdrenalinDownloadUrlsAsync(BrowserHttp);
            if (candidates.Count == 0)
            {
                AppendLine("⏭️ Could not find AMD Software: Adrenalin Edition download link, skipping.");
                return null;
            }
            return await DownloadFromCandidatesAsync("AMD Software: Adrenalin Edition", candidates, installerPath);
        }

        private async Task<bool?> DownloadNvidiaAppAsync(string installerPath)
        {
            AppendLine("🔄 Searching for latest Nvidia App installer...");
            string downloadUrl = await FindNvidiaAppDownloadUrlAsync(BrowserHttp);
            if (downloadUrl == null)
            {
                AppendLine("⚠️ Could not find Nvidia App download link.");
                return false;
            }
            AppendLine($"🔗 Found latest Nvidia installer: {downloadUrl}");
            return await DownloadWithRetryAsync(new Uri(downloadUrl), installerPath, "Nvidia App", BrowserHttp);
        }
    }
}
