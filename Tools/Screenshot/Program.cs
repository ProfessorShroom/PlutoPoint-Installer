using System;
using System.IO;
using System.Threading;
using Avalonia;
using Avalonia.Headless;
using Avalonia.Threading;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Tools.Screenshot
{
    internal static class Program
    {
        [STAThread]
        public static int Main(string[] args)
        {
            string output = Path.GetFullPath(args.Length > 0 ? args[0] : "images/installer.png");
            Directory.CreateDirectory(Path.GetDirectoryName(output)!);

            AppBuilder.Configure<App>()
                .UseSkia()
                .UseHeadless(new AvaloniaHeadlessPlatformOptions { UseHeadlessDrawing = false })
                .WithInterFont()
                .SetupWithoutStarting();

            var window = new Views.MainWindow();
            window.Show();

            for (int i = 0; i < 30; i++)
            {
                Dispatcher.UIThread.RunJobs();
                AvaloniaHeadlessPlatform.ForceRenderTimerTick(6);
                Dispatcher.UIThread.RunJobs();
                Thread.Sleep(100);
            }

            var frame = window.CaptureRenderedFrame()
                ?? throw new InvalidOperationException("No frame was rendered.");
            frame.Save(output);
            Console.WriteLine($"Saved {output}");

            Environment.Exit(0);
            return 0;
        }
    }
}
