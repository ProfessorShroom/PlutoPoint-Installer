using AutoUpdaterDotNET;
using PlutoPoint_Launcher;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            AutoUpdater.Start("https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/update.xml");
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new installerForm());
        }
    }
}
