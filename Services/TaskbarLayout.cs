using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Microsoft.Win32;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public sealed record TaskbarPin(string Shortcut, string Label, bool AlsoOnDesktop = false);

    public sealed class TaskbarLayout
    {
        private readonly Action<string> _log;

        public TaskbarLayout(Action<string> log) => _log = log;

        private static string? ResolveShortcutPath(string shortcutFileName)
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
                _log($"✅ Added {label} shortcut to Desktop.");
            }
            catch (Exception ex)
            {
                _log($"⚠️ Failed to add {label} shortcut to Desktop: {ex.Message}");
            }
        }

        private bool AddPinEntry(StringBuilder sb, TaskbarPin pin)
        {
            string? lnkPath = ResolveShortcutPath(pin.Shortcut);
            if (lnkPath == null)
            {
                _log($"⚠️ Could not find {pin.Label} shortcut to pin, skipping.");
                return false;
            }
            sb.AppendLine($"        <taskbar:DesktopApp DesktopApplicationLinkPath=\"{lnkPath}\" />");
            if (pin.AlsoOnDesktop)
                CopyShortcutToDesktop(lnkPath, pin.Label);
            return true;
        }

        private static void ScheduleLayoutFileCleanup(string layoutPath)
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
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\RunOnce", writable: true))
            {
                key?.SetValue("PlutoPointTaskbarLayoutCleanup", cmd, RegistryValueKind.String);
            }
        }

        public void Apply(IEnumerable<TaskbarPin> wanted)
        {
            var pins = new StringBuilder();
            int pinnedCount = 0;
            foreach (TaskbarPin pin in wanted)
                if (AddPinEntry(pins, pin)) pinnedCount++;
            if (pinnedCount == 0)
            {
                _log("⚠️ No apps available to pin, skipping taskbar layout.");
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
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(
                @"Software\Microsoft\Windows\CurrentVersion\Explorer", writable: true))
            {
                key?.DeleteSubKeyTree("Taskband", throwOnMissingSubKey: false);
            }
            ScheduleLayoutFileCleanup(layoutPath);
            _log($"✅ Taskbar layout built with {pinnedCount} app(s), will apply after next reboot or sign-in.");
        }
    }
}
