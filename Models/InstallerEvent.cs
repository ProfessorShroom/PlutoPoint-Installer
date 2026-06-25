using System;
using System.Collections.Generic;
using PlutoPoint_Installer;

using System.Drawing;

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
        public Image OverlayImage { get; set; }
        public Icon OverlayIcon { get; set; }
        public float RotationDegrees { get; set; }
    }
}