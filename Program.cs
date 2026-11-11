using Avalonia;
using System;
using System.IO;
using System.Runtime.InteropServices;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer
{
    internal class Program
    {
        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        private static extern int MessageBoxW(IntPtr hWnd, string text, string caption, uint type);

        [STAThread]
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (_, e) => ReportCrash(e.ExceptionObject as Exception);
            try
            {
                BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            }
            catch (Exception ex)
            {
                ReportCrash(ex);
            }
        }

        private static void ReportCrash(Exception ex)
        {
            string text = ex?.ToString() ?? "Unknown error";
            string logPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "installerCrash.log");
            try { File.WriteAllText(logPath, $"{DateTime.Now:o}{Environment.NewLine}{text}"); } catch { }
            MessageBoxW(IntPtr.Zero, $"{text}{Environment.NewLine}{Environment.NewLine}Saved to {logPath}", "Installer crashed", 0x10);
        }

        public static AppBuilder BuildAvaloniaApp() => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
    }
}