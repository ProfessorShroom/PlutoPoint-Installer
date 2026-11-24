using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Win32;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public sealed record WindowsTweak(string Name, string Message, Action Apply, int MinimumBuild = WindowsTweaks.MinimumBuild);

    public static class WindowsTweaks
    {
        public const int MinimumBuild = 19041;
        public const int Windows11Build = 22000;

        private const string ExplorerKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Explorer";

        private static readonly WindowsTweak[] All =
        {
            new("Disable Device Encryption", "✅ Disabling device encryption...",
                () => SetDword(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\BitLocker", "PreventDeviceEncryption", 1), Windows11Build),
            new("Open Explorer to This PC", "✅ Setting explorer to open to This PC...",
                () => SetDword(Registry.CurrentUser, ExplorerKey + @"\Advanced", "LaunchTo", 1)),
            new("Disable Fast Startup", "✅ Disabling fastboot mode...",
                () => SetDword(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Control\Session Manager\Power", "HiberbootEnabled", 0)),
            new("Disable Location Tracking", "✅ Disabling location tracking...", () =>
            {
                SetDword(Registry.LocalMachine, @"SOFTWARE\Microsoft\Windows NT\CurrentVersion\Sensor\Overrides\{BFA794E4-F964-4FDB-90F6-51056BFE4B44}", "SensorPermissionState", 0);
                SetDword(Registry.LocalMachine, @"SYSTEM\CurrentControlSet\Services\lfsvc\Service\Configuration", "Status", 0);
            }),
            new("Disable People Icon", "✅ Disabling People icon...",
                () => SetDword(Registry.CurrentUser, ExplorerKey + @"\Advanced\People", "PeopleBand", 0)),
            new("Hide Recent Files and Folders", "✅ Hiding recently used files and folders in File Explorer...", () =>
            {
                SetDword(Registry.CurrentUser, ExplorerKey, "ShowRecent", 0);
                SetDword(Registry.CurrentUser, ExplorerKey, "ShowFrequent", 0);
            }),
            new("Reset Cursor Size", "✅ Resetting cursor size to default (20)...",
                () => SetDword(Registry.CurrentUser, @"Control Panel\Cursors", "CursorBaseSize", 32))
        };

        public static IReadOnlyList<WindowsTweak> ForBuild(int build) => All.Where(t => build >= t.MinimumBuild).ToList();

        public static int? ReadBuild()
        {
            using RegistryKey? key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            return int.TryParse(key?.GetValue("CurrentBuild")?.ToString(), out int build) ? build : null;
        }

        private static void SetDword(RegistryKey root, string path, string name, int value)
        {
            using RegistryKey key = root.CreateSubKey(path, writable: true);
            key.SetValue(name, value, RegistryValueKind.DWord);
        }
    }
}
