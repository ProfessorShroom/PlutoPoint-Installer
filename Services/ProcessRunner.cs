using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public enum InstallerEnd
    {
        Exited,
        TimedOut,
        Stopped,
        FailedToStart
    }

    public readonly record struct InstallerResult(InstallerEnd End, int ExitCode)
    {
        public bool Succeeded => End == InstallerEnd.Exited && (ExitCode == 0 || ExitCode == 3010 || ExitCode == 1641);
    }

    public static class ProcessRunner
    {
        public static void RunSilent(string fileName, string arguments)
        {
            using Process? process = Process.Start(new ProcessStartInfo
            {
                FileName = fileName,
                Arguments = arguments,
                CreateNoWindow = true,
                UseShellExecute = false,
                WindowStyle = ProcessWindowStyle.Hidden
            });
            process?.WaitForExit();
        }

        public static void Kill(Process process)
        {
            try
            {
                if (!process.HasExited)
                    process.Kill(entireProcessTree: true);
            }
            catch
            {
            }
        }

        public static async Task<(int? ExitCode, string Output)> CaptureAsync(string fileName, string arguments, TimeSpan timeout, CancellationToken stop = default)
        {
            using var limit = CancellationTokenSource.CreateLinkedTokenSource(stop);
            limit.CancelAfter(timeout);
            try
            {
                using var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments,
                        UseShellExecute = false,
                        RedirectStandardOutput = true,
                        RedirectStandardError = true,
                        CreateNoWindow = true
                    }
                };
                await Task.Run(process.Start).ConfigureAwait(false);
                Task<string> output = process.StandardOutput.ReadToEndAsync(limit.Token);
                Task<string> error = process.StandardError.ReadToEndAsync(limit.Token);
                try
                {
                    await Task.WhenAll(output, error, process.WaitForExitAsync(limit.Token)).ConfigureAwait(false);
                    return (process.ExitCode, await output.ConfigureAwait(false));
                }
                catch (OperationCanceledException)
                {
                    Kill(process);
                    return (null, "");
                }
            }
            catch
            {
                return (null, "");
            }
        }

        public static async Task<InstallerResult> RunInstallerAsync(string fileName, string arguments, TimeSpan timeout, Action<string> log,
            bool elevate = false, string? workingDirectory = null, CancellationToken kill = default)
        {
            var startInfo = new ProcessStartInfo { FileName = fileName, Arguments = arguments };
            if (elevate)
            {
                startInfo.UseShellExecute = true;
                startInfo.Verb = "runas";
            }
            else
            {
                startInfo.UseShellExecute = false;
                startInfo.CreateNoWindow = true;
            }
            if (workingDirectory != null)
                startInfo.WorkingDirectory = workingDirectory;

            using var limit = CancellationTokenSource.CreateLinkedTokenSource(kill);
            limit.CancelAfter(timeout);
            try
            {
                using Process? process = await Task.Run(() => Process.Start(startInfo)).ConfigureAwait(false);
                if (process == null)
                    return new InstallerResult(InstallerEnd.FailedToStart, -1);
                try
                {
                    await process.WaitForExitAsync(limit.Token).ConfigureAwait(false);
                    return new InstallerResult(InstallerEnd.Exited, process.ExitCode);
                }
                catch (OperationCanceledException)
                {
                    Kill(process);
                    return new InstallerResult(kill.IsCancellationRequested ? InstallerEnd.Stopped : InstallerEnd.TimedOut, -1);
                }
            }
            catch (Exception ex)
            {
                log($"⚠️ Couldn't run {Path.GetFileName(fileName)}: {ex.Message}");
                return new InstallerResult(InstallerEnd.FailedToStart, -1);
            }
        }
    }
}
