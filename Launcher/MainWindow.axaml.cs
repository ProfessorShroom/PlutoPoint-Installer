using System;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection;
using System.Threading.Tasks;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Shapes;
using Avalonia.Media;
using Avalonia.Threading;
using PlutoPoint_Launcher.Services;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Launcher;

public partial class MainWindow : Window
{
    private static readonly HttpClient Http = new();

    private const double RingDiameter = 64;
    private const double RingStrokeThickness = 6;
    private const double RingPathDiameter = RingDiameter - RingStrokeThickness;
    private const double RingCircumference = Math.PI * RingPathDiameter;

    private const double RingCircumferenceInDashUnits = RingCircumference / RingStrokeThickness;

    private readonly UpdateService _updateService;
    private readonly string _rootDir;
    private readonly string _productInstallerPath;

    private static readonly Uri ProductInstallerUrl = new("http://crcinstaller.professorshroom.com");

    public MainWindow()
    {
        InitializeComponent();

        Http.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0 (Windows NT 10.0; Win64; x64)");
        _updateService = new UpdateService(Http);

        _rootDir = System.IO.Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "ComputerRepairCentre");
        _productInstallerPath = System.IO.Path.Combine(_rootDir, "computerRepairCentreInstaller.exe");

        Opened += async (_, _) => await RunAsync();
    }

    private async Task RunAsync()
    {
        try
        {
            BeginIndeterminate("Checking for updates…");

            var currentVersion = Assembly.GetExecutingAssembly().GetName().Version ?? new Version(0, 0, 0, 0);
            var update = await _updateService.CheckForUpdateAsync(currentVersion);

            if (update.IsAvailable && update.DownloadUrl is not null)
            {
                var updatePath = GetTempDownloadPath(update.DownloadUrl);
                await DownloadAndRun(update.DownloadUrl, updatePath, "Downloading update…");
                return;
            }

            await DownloadAndRun(ProductInstallerUrl, _productInstallerPath, "Downloading latest installer…");
        }
        catch (Exception ex)
        {
            ShowError($"Something went wrong: {ex.Message}");
        }
    }

    private async Task DownloadAndRun(Uri url, string destinationPath, string statusText)
    {
        Directory.CreateDirectory(_rootDir);
        BeginDeterminate(statusText);

        var progress = new Progress<double>(percent =>
            Dispatcher.UIThread.Post(() => SetRingProgress(percent)));

        await FileDownloader.DownloadAsync(Http, url, destinationPath, progress);

        Process.Start(new ProcessStartInfo(destinationPath) { UseShellExecute = true });
        Close();
    }

    private static string GetTempDownloadPath(Uri url)
    {
        var fileName = System.IO.Path.GetFileName(url.LocalPath);
        if (string.IsNullOrWhiteSpace(fileName))
            fileName = "PlutoPointLauncherUpdate.exe";

        return System.IO.Path.Combine(System.IO.Path.GetTempPath(), fileName);
    }

    private void BeginIndeterminate(string statusText)
    {
        StatusText.Text = statusText;
        StatusText.Foreground = Brushes.WhiteSmoke;

        PercentText.IsVisible = false;
        SetSpinning(true);

        ProgressRing.Stroke = new SolidColorBrush(Color.Parse("#4C9AFF"));
        ProgressRing.StrokeDashArray = new AvaloniaList<double>
        {
            RingCircumferenceInDashUnits * 0.28,
            RingCircumferenceInDashUnits * 0.72,
        };
    }

    private void BeginDeterminate(string statusText)
    {
        StatusText.Text = statusText;
        StatusText.Foreground = Brushes.WhiteSmoke;

        SetSpinning(false);
        PercentText.IsVisible = true;
        SetRingProgress(0);
    }

    private void SetRingProgress(double percent)
    {
        percent = Math.Clamp(percent, 0, 100);
        var filled = RingCircumferenceInDashUnits * percent / 100.0;
        var gap = RingCircumferenceInDashUnits - filled;

        ProgressRing.StrokeDashArray = new AvaloniaList<double> { filled, gap };
        PercentText.Text = $"{percent:0}%";
    }

    private void ShowError(string message)
    {
        SetSpinning(false);
        PercentText.IsVisible = false;

        ProgressRing.StrokeDashArray = null;
        ProgressRing.Stroke = Brushes.OrangeRed;

        StatusText.Text = message;
        StatusText.Foreground = Brushes.OrangeRed;
    }

    private void SetSpinning(bool spinning)
    {
        if (spinning)
        {
            if (!ProgressRing.Classes.Contains("spinner"))
                ProgressRing.Classes.Add("spinner");
        }
        else
        {
            ProgressRing.Classes.Remove("spinner");
        }
    }
}
