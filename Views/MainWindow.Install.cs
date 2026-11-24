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
        [DllImport("Shell32.dll", CharSet = CharSet.Unicode)]
        private static extern int SHEmptyRecycleBin(IntPtr hwnd, string? pszRootPath, uint dwFlags);
        private const uint SHERB_NOCONFIRMATION = 0x00000001;
        private const uint SHERB_NOPROGRESSUI = 0x00000002;
        private const uint SHERB_NOSOUND = 0x00000004;

        private enum AfterStop
        {
            None,
            Close,
            Restart
        }

        private List<(string Name, InstallStatus Status)>? installResults;
        private bool _installRunning;
        private bool _installerActive;
        private AfterStop _afterStop = AfterStop.None;
        private string? _appsDir;
        private CancellationTokenSource _stop = new CancellationTokenSource();
        private CancellationTokenSource _kill = new CancellationTokenSource();

        protected override void OnClosing(WindowClosingEventArgs e)
        {
            base.OnClosing(e);
            if (!_installRunning)
                return;
            if (e.CloseReason == WindowCloseReason.OSShutdown)
            {
                _power.Restore();
                return;
            }
            e.Cancel = true;
            StopThen(AfterStop.Close);
        }

        private void StopThen(AfterStop action)
        {
            _afterStop = action;
            AppendLine(action == AfterStop.Restart
                ? "🔄 The computer will restart once the install has stopped."
                : "🚪 The installer will close once the install has stopped.");
            RequestStop();
        }

        private void RequestStop()
        {
            if (_stop.IsCancellationRequested)
                return;
            _stop.Cancel();
            AppendLine("🛑 Cancel requested. The install will stop after the current step finishes...");
            UpdateCancelButton();
        }

        private void UpdateCancelButton()
        {
            bool canForce = _installerActive && !_kill.IsCancellationRequested;
            CancelInstallButton.Content = !_stop.IsCancellationRequested ? "Cancel" : canForce ? "Force Stop" : "Cancelling...";
            CancelInstallButton.IsEnabled = !_stop.IsCancellationRequested || canForce;
        }

        private void SetInstallerActive(bool active)
        {
            _installerActive = active;
            UpdateCancelButton();
        }

        private void CancelInstallButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (!_installRunning)
                return;
            if (!_stop.IsCancellationRequested)
            {
                RequestStop();
            }
            else if (_installerActive && !_kill.IsCancellationRequested)
            {
                AppendLine("🛑 Force stopping the installer that's running. That app may be left part installed.");
                _kill.Cancel();
                UpdateCancelButton();
            }
        }

        private bool StopIfCancelled(IEnumerable<AppDefinition>? remainingApps = null)
        {
            if (!_stop.IsCancellationRequested)
                return false;
            foreach (AppDefinition app in remainingApps ?? Enumerable.Empty<AppDefinition>())
                TrackResult(app.Name, InstallStatus.Skipped);
            AppendLine("🛑 The installation was cancelled.");
            AppendInstallSummary();
            return true;
        }

        private async void InstallButton_Click(object sender, Avalonia.Interactivity.RoutedEventArgs e)
        {
            if (_installRunning)
                return;
            _installRunning = true;
            _stop = new CancellationTokenSource();
            _kill = new CancellationTokenSource();
            _afterStop = AfterStop.None;
            _installerActive = false;
            InstallButton.IsEnabled = false;
            InstallButton.IsVisible = false;
            UpdateCancelButton();
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
                RestoreSleepSettings();
                await DeleteDownloadsAsync();
                CancelInstallButton.IsVisible = false;
                InstallButton.IsVisible = true;
                InstallButton.IsEnabled = true;
                _installRunning = false;
            }
            if (_afterStop == AfterStop.Restart)
                RestartNow();
            if (_afterStop != AfterStop.None)
                Close();
        }

        private void RestoreSleepSettings()
        {
            if (!_power.RestorePending)
                return;
            AppendLine("✅ Restoring the original sleep and screen timeout settings on AC power...");
            try
            {
                if (!_power.Restore())
                    AppendLine("⚠️ Couldn't read the original sleep settings, so they were set to 5 minutes.");
            }
            catch (Exception ex)
            {
                AppendLine($"❌ Couldn't restore the sleep settings: {ex.Message}");
            }
        }

        private async Task DeleteDownloadsAsync()
        {
            if (_appsDir != null)
                await new FileDeletionHelper().DeleteFilesAndDirectoryAsync(_appsDir, Array.Empty<string>());
        }

        private void TrackResult(string name, InstallStatus status) => installResults?.Add((name, status));

        private void AppendInstallSummary()
        {
            if (installResults == null || installResults.Count == 0)
                return;
            List<string> Named(InstallStatus status) => installResults.Where(r => r.Status == status).Select(r => r.Name).ToList();
            List<string> unverified = Named(InstallStatus.Unverified);
            List<string> failed = Named(InstallStatus.Failed);
            int skipped = Named(InstallStatus.Skipped).Count;
            int succeeded = installResults.Count - unverified.Count - failed.Count - skipped;
            AppendLine("");
            AppendLine("📋 Summary:");
            AppendLine($"✅ {succeeded}/{installResults.Count} succeeded");
            if (skipped > 0) AppendLine($"⏭️ {skipped} skipped");
            if (unverified.Count > 0) AppendLine($"⚠️ {unverified.Count} finished but couldn't be confirmed:");
            foreach (string name in unverified) AppendLine($"   • {name}");
            if (failed.Count > 0) AppendLine($"❌ {failed.Count} failed:");
            foreach (string name in failed) AppendLine($"   • {name}");
        }

        private void StepProgress(int steps = 1) =>
            InstallProgressBar.Value = Math.Min(InstallProgressBar.Value + steps, InstallProgressBar.Maximum);

        private static void SetOemInfo(ShopDetails shop)
        {
            const string oemRegPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\OEMInformation";
            const string logoRegData = @"C:\ProgramData\Computer Repair Centre\OEM\computerRepairCentreOEM.bmp";
            const string manufacturerRegData = "Computer Repair Centre";
            using (RegistryKey registryKey = Registry.LocalMachine.CreateSubKey(oemRegPath, writable: true))
            {
                registryKey.SetValue("Logo", logoRegData, RegistryValueKind.String);
                registryKey.SetValue("Manufacturer", manufacturerRegData, RegistryValueKind.String);
                registryKey.SetValue("SupportHours", shop.Hours, RegistryValueKind.String);
                registryKey.SetValue("SupportPhone", shop.Phone, RegistryValueKind.String);
                registryKey.SetValue("SupportURL", shop.Url, RegistryValueKind.String);
            }
        }

        private async Task RunInstallAsync()
        {
            installResults = new List<(string, InstallStatus)>();
            if (!await PassesPasswordGateAsync())
                return;

            if (!_initialiseUrlsTask.IsCompleted)
                AppendLine("⏳ Waiting for the download links to load...");
            await _initialiseUrlsTask;
            if (urls == null)
            {
                AppendLine("🔄 Trying to load the download links again...");
                await InitialiseUrlsAsync();
            }
            if (urls == null)
            {
                AppendLine("❌ Couldn't load the download links, so nothing can be installed. Check the internet connection and try again.");
                return;
            }

            if (StopIfCancelled())
                return;

            string rootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ComputerRepairCentre");
            string oemDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.CommonApplicationData), "Computer Repair Centre", "OEM");
            string appsDir = Path.Combine(rootDir, "apps");
            _appsDir = appsDir;
            Directory.CreateDirectory(oemDir);
            Directory.CreateDirectory(appsDir);

            List<AppDefinition> selectedApps = BuildAppList(appsDir).Where(app => app.Check.IsChecked == true).ToList();
            IReadOnlyList<WindowsTweak> tweaks = WindowsTweaks.ForBuild(_windowsBuild ?? 0);

            InstallProgressBar.Value = 0;
            InstallProgressBar.Maximum = selectedApps.Count * 2 + tweaks.Count
                + (PowerCheck.IsChecked == true ? 1 : 2)
                + new[] { CrcCheck, TaskbarCheck, PinAppsCheck }.Count(c => c.IsChecked == true)
                + 1;

            LogDetectedSelections();
            await ApplyPowerOptionsAsync();

            if (StopIfCancelled(selectedApps))
                return;

            if (CrcCheck.IsChecked == true && !await InstallOemInfoAsync(Path.Combine(oemDir, "computerRepairCentreOEM.bmp")))
            {
                StopIfCancelled(selectedApps);
                return;
            }

            ApplyTaskbarAlignment();

            for (int i = 0; i < selectedApps.Count; i++)
            {
                if (StopIfCancelled(selectedApps.Skip(i)))
                    return;
                AppDefinition app = selectedApps[i];
                try
                {
                    await InstallAppAsync(app, appsDir);
                }
                catch (OperationCanceledException) when (_stop.IsCancellationRequested)
                {
                    StopIfCancelled(selectedApps.Skip(i));
                    return;
                }
                catch (Exception ex)
                {
                    AppendLine($"❌ {app.Name} error: {ex.Message}");
                    TrackResult(app.Name, InstallStatus.Failed);
                }
                app.AfterInstall?.Invoke();
            }

            if (StopIfCancelled())
                return;

            if (Office2024Check.IsChecked == true && InstalledApps.IsOfficeInstalled())
            {
                await RemoveLibreOfficeAsync();
                if (StopIfCancelled())
                    return;
            }

            PinApps();
            ApplyWindowsTweaks(tweaks);

            if (_power.RestorePending)
            {
                RestoreSleepSettings();
                StepProgress();
            }

            await CleanUpAsync(appsDir);
            EmptyRecycleBin();

            if (StopIfCancelled())
                return;

            _themeManager.PlayEventSound();

            if (RestartCheck.IsChecked == true)
            {
                ScheduleShutdown("/r");
                AppendLine("🔄 System will restart in 60 seconds. If you need to cancel this press the close button.");
            }
            if (ShutdownCheck.IsChecked == true)
            {
                ScheduleShutdown("/s");
                AppendLine("⏻ System will shutdown in 60 seconds. If you need to cancel this press the close button.");
            }

            InstallProgressBar.Value = InstallProgressBar.Maximum;
            AppendInstallSummary();
            AppendLine("✅ The installation has completed.");
        }

        private void ScheduleShutdown(string mode)
        {
            ProcessRunner.RunSilent("shutdown", mode + " /t 60");
            _shutdownScheduled = true;
        }

        private void LogDetectedSelections()
        {
            void Log(CheckBox check, bool detected, string icon, string hardware, string app)
            {
                if (check.IsChecked != true || !detected)
                    return;
                AppendLine($"{icon} {hardware} has been detected and selected, {app} will be installed.");
                AppendLine("✅ You can uncheck this if you want.");
            }
            Log(AmdChipsetCheck, _hardware.HasAmdCpu, "🧠", "AMD CPU", "AMD Chipset Drivers");
            Log(AmdAdrenalinCheck, _hardware.HasAmdGpu, "🎮", "AMD GPU", "AMD Software: Adrenalin Edition");
            Log(NvidiaAppCheck, _hardware.HasNvidiaGpu, "🎮", "Nvidia GPU", "Nvidia App");
        }

        private async Task ApplyPowerOptionsAsync()
        {
            if (PowerCheck.IsChecked == true)
            {
                AppendLine("📌 Disable sleep on AC power is selected.");
                AppendLine("🔄 Disabling sleep and screen timeout while on AC power...");
                PowerSettings.DisableSleep();
                AppendLine("🔄 Changing power button behavior to shutdown...");
                PowerSettings.SetPowerButtonToShutdown();
            }
            else
            {
                AppendLine("🔄 Disabling sleep and screen timeout while on AC power temporarily during install...");
                await _power.DisableSleepTemporarilyAsync();
            }
            StepProgress();
        }

        private async Task<bool> InstallOemInfoAsync(string logoPath)
        {
            const string name = "Computer Repair Centre OEM Info";
            AppendLine("📌 Computer Repair Centre OEM information is selected.");
            ShopDetails? shop = SelectedOemShop;
            if (shop == null)
            {
                AppendLine("⚠️ No known shop location selected, skipping OEM info.");
                TrackResult(name, InstallStatus.Skipped);
                StepProgress();
                return true;
            }
            AppendLine($"📦 Installing {shop.LogName} Computer Repair Centre OEM information...");
            try
            {
                bool logoDownloaded = await _downloader.DownloadWithRetryAsync(crcOEMURL, logoPath, "the OEM logo", _stop.Token);
                SetOemInfo(shop);
                if (!logoDownloaded)
                    AppendLine("⚠️ OEM information was set, but the logo couldn't be downloaded.");
                TrackResult(name, logoDownloaded ? InstallStatus.Installed : InstallStatus.Failed);
            }
            catch (OperationCanceledException) when (_stop.IsCancellationRequested)
            {
                TrackResult(name, InstallStatus.Skipped);
                return false;
            }
            catch (Exception ex)
            {
                AppendLine($"❌ Failed to set OEM information: {ex.Message}");
                TrackResult(name, InstallStatus.Failed);
            }
            StepProgress();
            return true;
        }

        private void ApplyTaskbarAlignment()
        {
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
                    }
                    AppendLine("✅ Moved taskbar to the left.");
                    TrackResult("Taskbar Alignment", InstallStatus.Applied);
                }
                catch (Exception ex)
                {
                    AppendLine($"❌ Failed to move the taskbar: {ex.Message}");
                    TrackResult("Taskbar Alignment", InstallStatus.Failed);
                }
                StepProgress();
            }
        }

        private void PinApps()
        {
            if (PinAppsCheck.IsChecked != true)
                return;
            AppendLine("📌 Pin apps to taskbar is selected.");
            AppendLine("✅ Building taskbar layout from installed apps...");
            var pins = new List<TaskbarPin>();
            void Add(CheckBox check, params TaskbarPin[] wanted)
            {
                if (check.IsChecked == true)
                    pins.AddRange(wanted);
            }
            Add(GoogleChromeCheck, new TaskbarPin("Google Chrome.lnk", "Google Chrome"));
            Add(MozillaFirefoxCheck, new TaskbarPin("Firefox.lnk", "Mozilla Firefox"));
            Add(MozillaThunderbirdCheck, new TaskbarPin("Thunderbird.lnk", "Mozilla Thunderbird"));
            Add(SteamCheck, new TaskbarPin("Steam.lnk", "Steam"));
            Add(DiscordCheck, new TaskbarPin("Discord.lnk", "Discord"));
            Add(Office2024Check,
                new TaskbarPin("Word.lnk", "Microsoft Word", true),
                new TaskbarPin("Excel.lnk", "Microsoft Excel", true),
                new TaskbarPin("Outlook (classic).lnk", "Microsoft Outlook", true));
            Add(LibreOfficeCheck,
                new TaskbarPin("LibreOffice Writer.lnk", "LibreOffice Writer", true),
                new TaskbarPin("LibreOffice Calc.lnk", "LibreOffice Calc", true));
            pins.Add(new TaskbarPin("File Explorer.lnk", "File Explorer"));
            try
            {
                new TaskbarLayout(AppendLine).Apply(pins);
                TrackResult("Pin Apps to Taskbar", InstallStatus.Applied);
            }
            catch (Exception ex)
            {
                AppendLine("❌ Failed to set taskbar layout: " + ex.Message);
                TrackResult("Pin Apps to Taskbar", InstallStatus.Failed);
            }
            StepProgress();
        }

        private void ApplyWindowsTweaks(IReadOnlyList<WindowsTweak> tweaks)
        {
            if (_windowsBuild < WindowsTweaks.MinimumBuild)
                AppendLine("⬆️ This computer is running an old version of Windows, please update it.");
            foreach (WindowsTweak tweak in tweaks)
            {
                AppendLine(tweak.Message);
                try
                {
                    tweak.Apply();
                }
                catch (Exception ex)
                {
                    AppendLine($"⚠️ {tweak.Name} could not be applied: {ex.Message}");
                    TrackResult(tweak.Name, InstallStatus.Failed);
                }
                StepProgress();
            }
        }

        private async Task CleanUpAsync(string appsDir)
        {
            AppendLine("✅ Cleaning up installation files...");
            string desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string[] launcherPatterns =
            {
                "Computer Repair Centre Installer*.exe",
                "Computer Repair Centre Launcher*.exe",
                "Computer.Repair.Centre.Launcher*.exe"
            };
            var launcherPaths = new List<string>();
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
            await new FileDeletionHelper().DeleteFilesAndDirectoryAsync(appsDir, launcherPaths,
                Path.Combine(desktopPath, "service.conf.lock"), Path.Combine(desktopPath, "system.conf.lock"));
            StepProgress();
        }

        private void EmptyRecycleBin()
        {
            if (RecycleBinCheck.IsChecked != true)
                return;
            AppendLine("✅ Empty Recycle Bin is checked.");
            AppendLine("🗑️ Emptying Recycle Bin...");
            try
            {
                const int E_UNEXPECTED = unchecked((int)0x8000FFFF);
                int result = SHEmptyRecycleBin(IntPtr.Zero, null, SHERB_NOCONFIRMATION | SHERB_NOPROGRESSUI | SHERB_NOSOUND);
                if (result != E_UNEXPECTED)
                    Marshal.ThrowExceptionForHR(result);
                AppendLine(result == E_UNEXPECTED ? "✅ Recycle Bin was already empty." : "✅ Recycle Bin emptied successfully.");
                TrackResult("Empty Recycle Bin", InstallStatus.Applied);
            }
            catch (Exception ex)
            {
                AppendLine($"⚠️ Failed to empty Recycle Bin: {ex.Message}");
                TrackResult("Empty Recycle Bin", InstallStatus.Failed);
            }
        }
    }
}
