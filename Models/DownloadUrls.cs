// Copyright © Charlie Howard 2016-2026 All rights reserved

using System.Collections.Generic;

namespace PlutoPoint_Installer.Models
{
    public class DownloadUrls
    {
        public string crcOEM { get; set; } = null!;
        public string anyDesk { get; set; } = null!;
        public string bingWallpapers { get; set; } = null!;
        public string bitDefender { get; set; } = null!;
        public string discord { get; set; } = null!;
        public string googleChrome { get; set; } = null!;
        public string mozillaFirefox { get; set; } = null!;
        public string mozillaThunderbird { get; set; } = null!;
        public string nanaZip { get; set; } = null!;
        public string steam { get; set; } = null!;
        public string vlcMediaPlayer { get; set; } = null!;
        public string office2024 { get; set; } = null!;
        public Dictionary<string, string>? arm64 { get; set; }
    }
}