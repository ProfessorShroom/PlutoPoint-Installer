using System;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public sealed class PowerSettings
    {
        private uint? _standbySeconds;
        private uint? _monitorSeconds;

        public bool RestorePending { get; private set; }

        public static void DisableSleep()
        {
            ProcessRunner.RunSilent("powercfg", "/change standby-timeout-ac 0");
            ProcessRunner.RunSilent("powercfg", "/change monitor-timeout-ac 0");
        }

        public static void SetPowerButtonToShutdown()
        {
            ProcessRunner.RunSilent("powercfg", "/setacvalueindex SCHEME_CURRENT 4f971e89-eebd-4455-a8de-9e59040e7347 7648efa3-dd9c-4e3e-b566-50f929386280 3");
            ProcessRunner.RunSilent("powercfg", "/setactive SCHEME_CURRENT");
        }

        public async Task DisableSleepTemporarilyAsync()
        {
            _standbySeconds = await ReadAcSettingAsync("SUB_SLEEP", "STANDBYIDLE");
            _monitorSeconds = await ReadAcSettingAsync("SUB_VIDEO", "VIDEOIDLE");
            RestorePending = true;
            DisableSleep();
        }

        public bool Restore()
        {
            if (!RestorePending)
                return true;
            RestorePending = false;
            RestoreSetting(_standbySeconds, "SUB_SLEEP STANDBYIDLE", "standby-timeout-ac");
            RestoreSetting(_monitorSeconds, "SUB_VIDEO VIDEOIDLE", "monitor-timeout-ac");
            ProcessRunner.RunSilent("powercfg", "/setactive SCHEME_CURRENT");
            return _standbySeconds.HasValue && _monitorSeconds.HasValue;
        }

        private static void RestoreSetting(uint? seconds, string index, string change) =>
            ProcessRunner.RunSilent("powercfg", seconds.HasValue
                ? $"/setacvalueindex SCHEME_CURRENT {index} {seconds.Value}"
                : $"/change {change} 5");

        private static async Task<uint?> ReadAcSettingAsync(string subgroup, string setting)
        {
            var result = await ProcessRunner.CaptureAsync("powercfg", $"/q SCHEME_CURRENT {subgroup} {setting}", TimeSpan.FromSeconds(15));
            return ParseAcSetting(result.Output);
        }

        public static uint? ParseAcSetting(string output)
        {
            MatchCollection matches = Regex.Matches(output, @"0x([0-9a-fA-F]{8})");
            return matches.Count < 2 ? null : Convert.ToUInt32(matches[matches.Count - 2].Groups[1].Value, 16);
        }
    }
}
