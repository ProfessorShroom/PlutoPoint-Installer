using System;
using System.IO;
using System.Net;
using System.Windows.Forms;
using AutoUpdaterDotNET;

namespace PlutoPoint_Launcher
{
    public partial class installerForm : Form
    {
        string rootDir;
        string computerRepairCentreInstallerFilename;
        Uri computerRepairCentreInstallerURL = new Uri("http://crcinstaller.professorshroom.com");

        public installerForm()
        {
            InitializeComponent();
            rootDir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "ComputerRepairCentre");
            computerRepairCentreInstallerFilename = Path.Combine(rootDir, "computerRepairCentreInstaller.exe");

            this.Shown += installerForm_Shown;
        }

        private void installerForm_Shown(object sender, EventArgs e)
        {
            AutoUpdater.CheckForUpdateEvent += AutoUpdater_CheckForUpdateEvent;
            AutoUpdater.Start("https://raw.githubusercontent.com/ProfessorShroom/PlutoPoint-Installer/refs/heads/main/update.xml");
        }
        private void AutoUpdater_CheckForUpdateEvent(UpdateInfoEventArgs args)
        {
            if (args != null && args.IsUpdateAvailable)
            {
                AutoUpdater.ShowUpdateForm(args);
            }
            else
            {
                this.BeginInvoke(new Action(() => {
                    RunMainInstallerLogic();
                }));
            }
        }
        private async void RunMainInstallerLogic()
        {
            Directory.CreateDirectory(rootDir);
            using (WebClient wc = new WebClient())
            {
                wc.Headers.Add(HttpRequestHeader.UserAgent, "Mozilla/5.0");
                try
                {
                    await wc.DownloadFileTaskAsync(computerRepairCentreInstallerURL, computerRepairCentreInstallerFilename);
                    System.Diagnostics.Process.Start(computerRepairCentreInstallerFilename);
                    this.Close();
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error: " + ex.Message);
                }
            }
        }
    }
}