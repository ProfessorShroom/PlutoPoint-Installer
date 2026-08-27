using AutoUpdaterDotNET;
using PlutoPoint_Launcher;
using System;
using System.Windows.Forms;
using System.IO;

namespace PlutoPoint_Installer
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            AutoUpdater.HttpUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64)";
            Application.Run(new installerForm());
        }
    }
}