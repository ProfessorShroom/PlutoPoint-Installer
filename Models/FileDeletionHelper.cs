using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer.Models
{
    public class FileDeletionHelper
    {
        public async Task DeleteFilesAndDirectoryAsync(string appsDir, string launcherPath)
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
            if (File.Exists(launcherPath))
            {
                try
                {
                    await Task.Run(() => File.Delete(launcherPath));
                    Console.WriteLine("File deleted successfully.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error deleting file: {ex.Message}");
                }
            }
            else
            {
                Console.WriteLine("File does not exist.");
            }
        }
    }
}
