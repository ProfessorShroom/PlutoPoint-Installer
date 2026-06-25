using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer
{
    internal static class Program
    {
        private static PrivateFontCollection _fonts;
        private static FontFamily _ubuntuFamily;
        private static bool _initialized;
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new installerForm());
        }
        public static Font Ubuntu(float size, FontStyle style = FontStyle.Regular)
        {
            EnsureFonts();

            if (_ubuntuFamily == null)
                throw new InvalidOperationException("Ubuntu font not initialized.");
            return new Font(_ubuntuFamily, size, style);
        }
        private static IntPtr _fontPtr;
        private static void EnsureFonts()
        {
            if (_initialized) return;
            _fonts = new PrivateFontCollection();
            _fontPtr = AddFont(Properties.Resources.Ubuntu_Regular);
            _ubuntuFamily = _fonts.Families.FirstOrDefault(f => f.Name == "Ubuntu");
            if (_ubuntuFamily == null)
                throw new Exception("Ubuntu font NOT loaded correctly. Ensure resource name is correct.");
            _initialized = true;
        }
        private static IntPtr AddFont(byte[] fontData)
        {
            IntPtr ptr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, ptr, fontData.Length);
            _fonts.AddMemoryFont(ptr, fontData.Length);
            return ptr;
        }
    }
}