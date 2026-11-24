using System;
using System.Collections.Generic;
using Avalonia.Media;

// Copyright © Charlie Howard 2016-2026 All rights reserved

namespace PlutoPoint_Installer.Models
{
    public class InstallerEvent
    {
        public string Name { get; set; } = null!;
        public Func<DateTime, bool> IsActiveToday { get; set; } = null!;
        public Color GradientTop { get; set; }
        public Color GradientBottom { get; set; }
        public Color LogColor { get; set; }
        public List<string> Messages { get; set; } = [];
        public Action? PlaySound { get; set; }
        public string? OverlayImage { get; set; }
        public string? OverlayIcon { get; set; }
        public float RotationDegrees { get; set; }
        public string? TitleSuffix { get; set; }
        public Color TextColor { get; set; } = Colors.White;
        public Color AccentStart { get; set; } = Color.FromRgb(10, 116, 214);
        public Color AccentEnd { get; set; } = Color.FromRgb(0, 64, 200);
    }
}