using System;
using System.Threading;
using System.Threading.Tasks;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public sealed class WingetService
    {
        private readonly Action<string> _log;
        private bool? _available;

        public WingetService(Action<string> log) => _log = log;

        public async Task<bool> IsAvailableAsync()
        {
            if (_available.HasValue)
                return _available.Value;

            var version = await ProcessRunner.CaptureAsync("winget", "--version", TimeSpan.FromSeconds(5));
            _available = version.ExitCode == 0;
            if (_available == true)
            {
                var warmup = await ProcessRunner.CaptureAsync("winget", "list --accept-source-agreements --accept-package-agreements", TimeSpan.FromSeconds(60));
                if (warmup.ExitCode == null)
                {
                    _log("⚠️ winget did not finish accepting source agreements (first-run initialisation), using direct downloads for everything.");
                    _available = false;
                }
            }
            if (_available != true)
                _log("⚠️ winget is not available on this machine (or not accessible from this context); using direct downloads for everything.");
            return _available.Value;
        }

        public async Task<bool> IsInstalledAsync(string packageId)
        {
            var result = await ProcessRunner.CaptureAsync("winget", $"list -e --id \"{packageId}\" --accept-source-agreements", TimeSpan.FromSeconds(15));
            return result.ExitCode != null && result.Output.Contains(packageId, StringComparison.OrdinalIgnoreCase);
        }

        public async Task<bool> TryInstallAsync(string packageId, string label, CancellationToken kill)
        {
            if (!await IsAvailableAsync())
                return false;
            _log($"🔄 Attempting winget install of {label}...");
            InstallerResult result = await ProcessRunner.RunInstallerAsync("winget",
                $"install -e --id \"{packageId}\" --silent --disable-interactivity --accept-package-agreements --accept-source-agreements",
                TimeSpan.FromMinutes(3), _log, kill: kill);
            kill.ThrowIfCancellationRequested();
            if (result.End == InstallerEnd.TimedOut)
            {
                _log($"⚠️ winget install of {label} timed out after 3 minutes and was terminated, falling back to direct download.");
                return false;
            }
            bool confirmed = await IsInstalledAsync(packageId);
            _log(confirmed
                ? $"✅ {label} installed via winget."
                : $"⚠️ winget install of {label} could not be verified, falling back to direct download.");
            return confirmed;
        }
    }
}
