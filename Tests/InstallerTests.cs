using PlutoPoint_Installer.Models;
using PlutoPoint_Installer.Services;
using PlutoPoint_Installer.UI;
using PlutoPoint_Launcher.Services;
using Xunit;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Tests
{
    public class ReleaseOrderTests
    {
        private static InstallerRelease Release(string version, int beta = 0) =>
            new("v" + version, Version.Parse(version), beta, new Uri("https://example.com"));

        [Theory]
        [InlineData("8.1.0.0", 0, "8.0.0.0", 0, true)]
        [InlineData("8.0.0.0", 0, "8.1.0.0", 0, false)]
        [InlineData("8.10.0.0", 0, "8.9.0.0", 0, true)]
        [InlineData("8.1.0.0", 0, "8.1.0.0", 3, true)]
        [InlineData("8.1.0.0", 3, "8.1.0.0", 0, false)]
        [InlineData("8.1.0.0", 10, "8.1.0.0", 9, true)]
        [InlineData("8.2.0.0", 1, "8.1.0.0", 0, true)]
        [InlineData("8.1.0.0", 0, "8.1.0.0", 0, false)]
        public void IsNewer(string a, int aBeta, string b, int bBeta, bool expected) =>
            Assert.Equal(expected, ReleaseService.IsNewer(Release(a, aBeta), Release(b, bBeta)));
    }

    public class EventTests
    {
        private static readonly ThemeManager Themes = new();

        [Theory]
        [InlineData("2026-12-25", "Christmas")]
        [InlineData("2026-12-01", "Christmas")]
        [InlineData("2026-12-31", "New Year")]
        [InlineData("2027-01-05", "New Year")]
        [InlineData("2027-01-06", null)]
        [InlineData("2026-10-31", "Halloween")]
        [InlineData("2026-11-01", null)]
        [InlineData("2024-02-13", "Pancake Day")]
        [InlineData("2025-03-04", "Pancake Day")]
        [InlineData("2026-02-17", "Pancake Day")]
        [InlineData("2026-04-06", "Charlie's Birthday")]
        public void EventOnDate(string date, string? expected) =>
            Assert.Equal(expected, Themes.GetEvent(DateTime.Parse(date))?.Name);
    }

    public class LinkTests
    {
        [Fact]
        public void NewestAmdChipsetWins() => Assert.Equal(
            "https://drivers.amd.com/drivers/amd_chipset_software_8.10.1.1.exe",
            VendorDownloads.FindNewestLink(
                "https://drivers.amd.com/drivers/amd_chipset_software_8.9.1.1.exe \"//drivers.amd.com/drivers/amd_chipset_software_8.10.1.1.exe\"",
                VendorDownloads.AmdChipsetLinkRegex));

        [Fact]
        public void AmdAdrenalin() => Assert.Equal(
            "https://drivers.amd.com/drivers/installer/25.10/whql/amd-software-adrenalin-edition-25.9.1-minimalsetup-250901_web.exe",
            VendorDownloads.FindNewestLink(
                "<a href=\"https://drivers.amd.com/drivers/installer/25.10/whql/amd-software-adrenalin-edition-25.9.1-minimalsetup-250901_web.exe\">",
                VendorDownloads.AmdAdrenalinLinkRegex));

        [Theory]
        [InlineData("https://uk.download.nvidia.com/nvapp/client/11.0.5.266/NVIDIA_app_v11.0.5.266.exe")]
        [InlineData(@"https:\/\/uk.download.nvidia.com\/nvapp\/client\/11.0.5.266\/NVIDIA_app_v11.0.5.266.exe")]
        [InlineData("http://uk.download.nvidia.com/nvapp/client/11.0.5.266/NVIDIA_app_v11.0.5.266.exe")]
        public void Nvidia(string link) => Assert.Equal(
            "https://uk.download.nvidia.com/nvapp/client/11.0.5.266/NVIDIA_app_v11.0.5.266.exe",
            VendorDownloads.FindNvidiaLink($"x \"{link}\" y"));

        [Fact]
        public void NoLink()
        {
            Assert.Null(VendorDownloads.FindNvidiaLink("nothing here"));
            Assert.Null(VendorDownloads.FindNewestLink("nothing here", VendorDownloads.AmdChipsetLinkRegex));
            Assert.Null(VendorDownloads.FindLibreOfficeVersion("nothing here"));
        }

        [Fact]
        public void LibreOffice() =>
            Assert.Equal("25.8.3", VendorDownloads.FindLibreOfficeVersion("LibreOffice 25.8.3 for Windows (x86-64)"));
    }

    public class DetectionTests
    {
        [Theory]
        [InlineData("NVIDIA App 11.0.5.266", true)]
        [InlineData("NVIDIA app", true)]
        [InlineData("NVIDIA Graphics Driver 581.29", false)]
        [InlineData("NVIDIA Application Framework", false)]
        public void NvidiaApp(string name, bool expected) => Assert.Equal(expected, InstalledApps.IsNvidiaAppName(name));

        [Theory]
        [InlineData("AMD Software", true)]
        [InlineData("AMD Chipset Software", false)]
        public void Adrenalin(string name, bool expected) => Assert.Equal(expected, InstalledApps.IsAmdAdrenalinName(name));

        [Fact]
        public void ShopFromHash()
        {
            var hashes = new LocationHashes
            {
                romsey = "pbkdf2-sha512$600000$1b62684f85ae9a70a695e77d02f1a61e$c45d854525e7355b60c8833f75e455d85cf25dd553ec73c13b29c93c055bcca4",
                chandlersFord = "pbkdf2-sha512$600000$69c2946a9b94d548ee230b75443c0d23$1273fb14751477e6fe510860445c8abc27097c4eaa09dc92de7af7088c939a07",
                highcliffe = "pbkdf2-sha512$600000$ad764178c380842f923873660b2ba097$c443b4406c5c08f8791af11aae1aae5ad7b1c01f702e2ec0a7a03b46f05f36ce"
            };
            Assert.Equal(Shop.ChandlersFord, RemoteData.MatchShop("192.0.2.2", hashes));
            Assert.Equal(Shop.Unknown, RemoteData.MatchShop("192.0.2.4", hashes));
        }

        private const string SecretHash = "pbkdf2-sha512$600000$ff60c17873d158a657517ceb38d3a6de$3000f2723e291c5a23327957d1290dad0ed5271674473bf12bbef719bbbaf539";

        [Theory]
        [InlineData("test-sécret", SecretHash, true)]
        [InlineData("wrong", SecretHash, false)]
        [InlineData("", SecretHash, false)]
        [InlineData("x", null, false)]
        [InlineData("x", "", false)]
        [InlineData("x", "pbkdf2-sha512$600000$zz$zz", false)]
        [InlineData("x", "pbkdf2-sha512$1$$", false)]
        [InlineData("x", "2d711642b726b04401627ca9fbac32f5c8530fb1903cc4db02258717921a4881", false)]
        public void SecretHashing(string secret, string? hash, bool expected) =>
            Assert.Equal(expected, SecretHasher.Verify(secret, hash));

        [Fact]
        public void PowerSettingReadsAcValue() => Assert.Equal(1800u, PowerSettings.ParseAcSetting(
            "Minimum: 0x00000000\nMaximum: 0xffffffff\nCurrent AC Power Setting Index: 0x00000708\nCurrent DC Power Setting Index: 0x00000384"));

        [Fact]
        public void DeviceEncryptionIsOnlyOnWindows11()
        {
            Assert.Empty(WindowsTweaks.ForBuild(18363));
            Assert.Equal(WindowsTweaks.ForBuild(19045).Count + 1, WindowsTweaks.ForBuild(22631).Count);
        }
    }

    public class ProcessTests
    {
        private static readonly (string File, string Args) Sleep = OperatingSystem.IsWindows() ? ("ping", "-n 30 127.0.0.1") : ("sleep", "30");
        private static readonly (string File, string Args) Noisy = OperatingSystem.IsWindows() ? ("cmd", "/c for /L %i in (1,1,20000) do @echo 1234567890") : ("seq", "1 100000");

        [Fact]
        public async Task InstallerTimesOut() => Assert.Equal(InstallerEnd.TimedOut,
            (await ProcessRunner.RunInstallerAsync(Sleep.File, Sleep.Args, TimeSpan.FromMilliseconds(500), _ => { })).End);

        [Fact]
        public async Task InstallerCanBeKilled()
        {
            using var kill = new CancellationTokenSource(500);
            Assert.Equal(InstallerEnd.Stopped,
                (await ProcessRunner.RunInstallerAsync(Sleep.File, Sleep.Args, TimeSpan.FromMinutes(1), _ => { }, kill: kill.Token)).End);
        }

        [Fact]
        public async Task CaptureTimesOut() =>
            Assert.Null((await ProcessRunner.CaptureAsync(Sleep.File, Sleep.Args, TimeSpan.FromMilliseconds(500))).ExitCode);

        [Fact]
        public async Task CaptureDrainsLargeOutput()
        {
            var result = await ProcessRunner.CaptureAsync(Noisy.File, Noisy.Args, TimeSpan.FromSeconds(30));
            Assert.Equal(0, result.ExitCode);
            Assert.True(result.Output.Length > 100000);
        }

        [Fact]
        public async Task MissingProgram()
        {
            Assert.Null((await ProcessRunner.CaptureAsync("no-such-program-x", "", TimeSpan.FromSeconds(5))).ExitCode);
            Assert.Equal(InstallerEnd.FailedToStart,
                (await ProcessRunner.RunInstallerAsync("no-such-program-x", "", TimeSpan.FromSeconds(5), _ => { })).End);
        }
    }
}
