using System;
using System.Drawing;
using System.Drawing.Text;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows.Forms;

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
            return new Font(_ubuntuFamily, size, style);
        }
        private static void EnsureFonts()
        {
            if (_initialized)
                return;
            _fonts = new PrivateFontCollection();
            AddFont(Properties.Resources.Ubuntu_Regular);
            AddFont(Properties.Resources.Ubuntu_Bold);
            AddFont(Properties.Resources.Ubuntu_Italic);
            _ubuntuFamily = _fonts.Families
                .FirstOrDefault(f => f.Name == "Ubuntu");
            if (_ubuntuFamily == null)
                throw new InvalidOperationException("Ubuntu font failed to load.");
            _initialized = true;
        }
        private static void AddFont(byte[] fontData)
        {
            IntPtr ptr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, ptr, fontData.Length);
            _fonts.AddMemoryFont(ptr, fontData.Length);
            Marshal.FreeCoTaskMem(ptr);
        }
    }
}