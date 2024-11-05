using PlutoPoint_Launcher.Properties;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PlutoPoint_Launcher
{
    public partial class installerForm : Form
    {
        string rootDir = @"C:\Computer Repair Centre";
        Uri computerRepairCentreInstallerURL = new Uri("https://raw.githubusercontent.com/charliehoward/PlutoPoint-Installer/master/bin/x64/Release/Computer%20Repair%20Centre%20Installer.exe");
        string computerRepairCentreInstallerFilename = @"C:\Computer Repair Centre\computerRepairCentreInstaller.exe";

        public installerForm()
        {
            InitializeComponent();
            InitializeLoadingIcon();
            this.Shown += installerForm_Shown;
        }

        private void InitializeLoadingIcon()
        {
        }

        private async void installerForm_Shown(object sender, EventArgs e)
        {
            loadingIcon.Visible = true;
            await InstallAndRunApplicationAsync();
        }

        private async Task InstallAndRunApplicationAsync()
        {
            Directory.CreateDirectory(rootDir);

            using (WebClient wc = new WebClient())
            {
                try
                {
                    await wc.DownloadFileTaskAsync(computerRepairCentreInstallerURL, computerRepairCentreInstallerFilename);
                    Console.WriteLine("Download completed.");
                }
                catch (Exception ex)
                {
                    MessageBox.Show("Error downloading file: " + ex.Message);
                    return;
                }
            }

            Process process = new Process();
            process.StartInfo.FileName = computerRepairCentreInstallerFilename;
            process.StartInfo.WorkingDirectory = rootDir;

            try
            {
                process.Start();
                this.Close();
                Console.WriteLine("Installer run started, form closed.");
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error running installer: " + ex.Message);
            }
        }
    }
}
