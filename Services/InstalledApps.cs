using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Microsoft.Win32;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public static class InstalledApps
    {
        public static bool IsOfficeInstalled() =>
            File.Exists(@"C:\Program Files\Microsoft Office\root\Office16\WINWORD.EXE")
            || File.Exists(@"C:\Program Files (x86)\Microsoft Office\root\Office16\WINWORD.EXE");

        public static bool IsListed(Func<string, bool> matches)
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

        public static bool IsNanaZipInstalled()
        {
            string output = ProcessRunner.CaptureAsync("powershell", "-NoProfile -NonInteractive -Command \"Get-AppxPackage -Name '*NanaZip*' | Select-Object -ExpandProperty Name\"", TimeSpan.FromSeconds(30)).GetAwaiter().GetResult().Output;
            if (output.Contains("NanaZip", StringComparison.OrdinalIgnoreCase))
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

        public static List<(string Name, string ProductCode)> FindLibreOfficeProducts()
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

        public static bool IsAmdChipsetInstalled() => IsListed(IsAmdChipsetName);

        public static bool IsAmdAdrenalinInstalled() =>
            File.Exists(@"C:\Program Files\AMD\CNext\CNext\RadeonSoftware.exe") || IsListed(IsAmdAdrenalinName);

        public static bool IsNvidiaAppInstalled() =>
            File.Exists(@"C:\Program Files\NVIDIA Corporation\NVIDIA App\CEF\NVIDIA App.exe") || IsListed(IsNvidiaAppName);

        public static bool IsAmdChipsetName(string name) =>
            name.StartsWith("AMD Chipset Software", StringComparison.OrdinalIgnoreCase);

        public static bool IsAmdAdrenalinName(string name) =>
            name.Equals("AMD Software", StringComparison.OrdinalIgnoreCase) ||
            name.StartsWith("AMD Software: Adrenalin Edition", StringComparison.OrdinalIgnoreCase);

        public static bool IsNvidiaAppName(string name) =>
            Regex.IsMatch(name, @"^NVIDIA App( v?\d[\d.]*)?$", RegexOptions.IgnoreCase);
    }
}
