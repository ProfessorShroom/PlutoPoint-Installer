using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Models
{
    public class FileDeletionHelper
    {
        public async Task DeleteFilesAndDirectoryAsync(string appsDir, string launcherPath, string lock1Path = null, string lock2Path = null)
        {
            if (Directory.Exists(appsDir))
            {
                var deleteFileTasks = new List<Task>();
                foreach (var file in Directory.EnumerateFiles(appsDir))
                {
                    deleteFileTasks.Add(Task.Run(() =>
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting file {file}: {ex.Message}");
                        }
                    }));
                }
                await Task.WhenAll(deleteFileTasks);

                try
                {
                    await Task.Run(() => Directory.Delete(appsDir, true));
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting directory {appsDir}: {ex.Message}");
                }
            }

            if (File.Exists(launcherPath))
            {
                try
                {
                    await Task.Run(() => File.Delete(launcherPath));
                    Console.WriteLine("Launcher file deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting launcher file: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("Launcher file does not exist.");
            }

            var lockPaths = new[] { lock1Path, lock2Path };
            foreach (var lockPath in lockPaths)
            {
                if (!string.IsNullOrEmpty(lockPath) && File.Exists(lockPath))
                {
                    try
                    {
                        await Task.Run(() => File.Delete(lockPath));
                        Console.WriteLine($"Deleted lock file: {lockPath}");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting lock file {lockPath}: {ex.Message}");
                    }
                }
            }
        }
    }
}