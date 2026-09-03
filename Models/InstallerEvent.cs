using System;
using System.Collections.Generic;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;

namespace PlutoPoint_Installer.Models
{
    public class InstallerEvent
    {
        public string Name { get; set; }
        public Func<DateTime, bool> IsActiveToday { get; set; }
        public Color GradientTop { get; set; }
        public Color GradientBottom { get; set; }
        public Color LogColor { get; set; }
        public List<string> Messages { get; set; }
        public Action PlaySound { get; set; }
        public Bitmap OverlayImage { get; set; }
        public WindowIcon OverlayIcon { get; set; }
        public float RotationDegrees { get; set; }
    }
}
