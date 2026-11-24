using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using PlutoPoint_Installer.Models;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Services
{
    public sealed class VendorDownloads
    {
        private static readonly string[] NvidiaAppPages =
        {
            "https://www.nvidia.com/en-us/software/nvidia-app/",
            "https://www.nvidia.com/en-gb/software/nvidia-app/",
            "https://www.nvidia.com/en-us/software/nvidia-app.md",
            "https://www.nvidia.com/en-zz/software/nvidia-app.md"
        };

        public static readonly Regex NvidiaAppLinkRegex = new Regex(
            @"(?:https?:)?(?:\\?/){2}[a-z0-9.-]*download\.nvidia\.com(?:\\?/)nvapp(?:\\?/)client(?:\\?/)[\d.]+(?:\\?/)NVIDIA_app_(?:beta_)?v[\d.]+\.exe",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private const string AmdReferer = "https://www.amd.com/en/support/download/drivers.html";

        private const string AmdChipsetStableUrl = "https://drivers.amd.com/drivers/installer/chipset/AMD_Chipset_Software.exe";

        private static readonly string[] AmdChipsetPages =
        {
            "https://www.amd.com/en/support/downloads/drivers.html/chipsets/laptop-chipsets/amd-ryzen-and-athlon-mobile-chipset.html",
            "https://www.amd.com/en/support/downloads/drivers.html/chipsets/am5/x870e.html",
            "https://www.amd.com/en/support/downloads/drivers.html/chipsets/am4/x570.html"
        };

        public static readonly Regex AmdChipsetLinkRegex = new Regex(
            @"(?:https?:)?(?:\\?/){2}drivers\.amd\.com(?:\\?/)drivers(?:\\?/)amd_chipset_software_([\d.]+)\.exe",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly string[] AmdAdrenalinPages =
        {
            "https://www.amd.com/en/support/download/drivers.html",
            "https://www.amd.com/en/support/downloads/drivers.html/graphics/radeon-rx/radeon-rx-9000-series/amd-radeon-rx-9070-xt.html"
        };

        public static readonly Regex AmdAdrenalinLinkRegex = new Regex(
            @"(?:https?:)?(?:\\?/){2}drivers\.amd\.com(?:\\?/)drivers(?:\\?/)installer(?:\\?/)[\w.\\/-]*?amd-software-adrenalin-edition-([\d.]+)-minimalsetup-\d+_web\.exe",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private static readonly Regex LibreOfficeVersionRegex = new Regex(
            @"(\d+\.\d+(?:\.\d+)?)[^0-9]{0,100}Windows \(x86-64\)",
            RegexOptions.IgnoreCase | RegexOptions.Compiled);

        private readonly Downloader _downloader;
        private readonly Action<string> _log;

        public VendorDownloads(Downloader downloader, Action<string> log)
        {
            _downloader = downloader;
            _log = log;
        }

        public static string CleanLink(string link)
        {
            link = link.Replace("\\/", "/");
            if (link.StartsWith("//")) link = "https:" + link;
            if (link.StartsWith("http://", StringComparison.OrdinalIgnoreCase)) link = "https://" + link.Substring(7);
            return link;
        }

        public static string? FindNewestLink(string content, Regex regex)
        {
            string? best = null;
            Version? bestVersion = null;
            foreach (Match match in regex.Matches(content))
            {
                if (!Version.TryParse(match.Groups[1].Value, out Version? version)) continue;
                if (bestVersion != null && version <= bestVersion) continue;
                bestVersion = version;
                best = match.Value;
            }
            return best == null ? null : CleanLink(best);
        }

        public static string? FindNvidiaLink(string content)
        {
            Match match = NvidiaAppLinkRegex.Match(content);
            return match.Success ? CleanLink(match.Value) : null;
        }

        public static string? FindLibreOfficeVersion(string html)
        {
            Match match = LibreOfficeVersionRegex.Match(html);
            return match.Success ? match.Groups[1].Value : null;
        }

        private async Task<string?> SearchPagesAsync(string vendor, string[] pages, Func<string, string?> find, CancellationToken stop)
        {
            foreach (string page in pages)
            {
                try
                {
                    string? link = find(await Downloader.BrowserHttp.GetStringAsync(page, stop));
                    if (link != null)
                        return link;
                }
                catch (Exception ex)
                {
                    stop.ThrowIfCancellationRequested();
                    _log($"⚠️ {vendor} page {page} failed: {ex.Message}");
                }
            }
            return null;
        }

        private async Task<bool?> DownloadFromCandidatesAsync(string label, IEnumerable<string> candidates, string installerPath, CancellationToken stop)
        {
            foreach (string candidate in candidates)
            {
                _log($"🔗 Trying {label} from: {candidate}");
                try
                {
                    if (await _downloader.DownloadInstallerAsync(Downloader.BrowserHttp, candidate, installerPath, AmdReferer, stop))
                        return true;
                }
                catch (Exception ex)
                {
                    stop.ThrowIfCancellationRequested();
                    _log($"⚠️ Download failed: {ex.Message}");
                }
            }
            _log($"⚠️ Could not download {label}.");
            return false;
        }

        public async Task<bool?> AmdChipsetAsync(string installerPath, DownloadUrls urls, CancellationToken stop)
        {
            _log("🔄 Searching for latest AMD Chipset Drivers...");
            var candidates = new List<string>();
            string? newest = await SearchPagesAsync("AMD", AmdChipsetPages, c => FindNewestLink(c, AmdChipsetLinkRegex), stop);
            if (newest != null) candidates.Add(newest);
            candidates.Add(AmdChipsetStableUrl);
            if (!string.IsNullOrWhiteSpace(urls.amdChipsetFallback)) candidates.Add(urls.amdChipsetFallback);
            return await DownloadFromCandidatesAsync("AMD Chipset Drivers", candidates, installerPath, stop);
        }

        public async Task<bool?> AmdAdrenalinAsync(string installerPath, CancellationToken stop)
        {
            _log("🔄 Searching for latest AMD Software: Adrenalin Edition...");
            string? newest = await SearchPagesAsync("AMD", AmdAdrenalinPages, c => FindNewestLink(c, AmdAdrenalinLinkRegex), stop);
            if (newest == null)
            {
                _log("⏭️ Could not find AMD Software: Adrenalin Edition download link, skipping.");
                return null;
            }
            return await DownloadFromCandidatesAsync("AMD Software: Adrenalin Edition", new[] { newest }, installerPath, stop);
        }

        public async Task<bool?> NvidiaAppAsync(string installerPath, DownloadUrls urls, CancellationToken stop)
        {
            _log("🔄 Searching for latest Nvidia App installer...");
            string? link = await SearchPagesAsync("Nvidia", NvidiaAppPages, FindNvidiaLink, stop);
            if (link == null && !string.IsNullOrWhiteSpace(urls.nvidiaAppFallback))
            {
                _log("🔁 Using fallback Nvidia App link from Downloads.json.");
                link = urls.nvidiaAppFallback;
            }
            if (link == null)
            {
                _log("⚠️ Could not find Nvidia App download link.");
                return false;
            }
            _log($"🔗 Found latest Nvidia installer: {link}");
            return await _downloader.DownloadWithRetryAsync(new Uri(link), installerPath, "Nvidia App", stop, Downloader.BrowserHttp);
        }

        public async Task<bool?> LibreOfficeAsync(string installerPath, Uri fallback, CancellationToken stop)
        {
            _log("🔄 Finding the latest LibreOffice version...");
            string? version = null;
            try
            {
                using var timeout = CancellationTokenSource.CreateLinkedTokenSource(stop);
                timeout.CancelAfter(TimeSpan.FromSeconds(30));
                version = FindLibreOfficeVersion(await Downloader.BrowserHttp.GetStringAsync("https://www.libreoffice.org/download/", timeout.Token));
            }
            catch
            {
                stop.ThrowIfCancellationRequested();
            }
            if (version == null)
            {
                _log("⚠️ Could not determine the latest LibreOffice version from libreoffice.org.");
            }
            else
            {
                string url = HardwareInfo.IsArm64
                    ? $"https://download.documentfoundation.org/libreoffice/stable/{version}/win/aarch64/LibreOffice_{version}_Win_aarch64.msi"
                    : $"https://download.documentfoundation.org/libreoffice/stable/{version}/win/x86_64/LibreOffice_{version}_Win_x86-64.msi";
                _log($"🔄 Downloading LibreOffice {version}...");
                if (await _downloader.DownloadWithRetryAsync(new Uri(url), installerPath, "LibreOffice", stop))
                    return true;
            }
            _log("🔁 Using the fallback LibreOffice download...");
            return await _downloader.DownloadWithRetryAsync(fallback, installerPath, "LibreOffice", stop);
        }
    }
}
