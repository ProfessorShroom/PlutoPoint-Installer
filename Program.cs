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
        public static PrivateFontCollection UbuntuFonts;
        public static FontFamily UbuntuFamily;

        [STAThread]
        static void Main()
        {
            UbuntuFonts = new PrivateFontCollection();
            AddFont(Properties.Resources.Ubuntu_Regular);
            AddFont(Properties.Resources.Ubuntu_Bold);
            AddFont(Properties.Resources.Ubuntu_Italic);

            UbuntuFamily = UbuntuFonts.Families
                .First(f => f.Name == "Ubuntu");

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new installerForm());
        }

        private static void AddFont(byte[] fontData)
        {
            IntPtr fontPtr = Marshal.AllocCoTaskMem(fontData.Length);
            Marshal.Copy(fontData, 0, fontPtr, fontData.Length);

            UbuntuFonts.AddMemoryFont(fontPtr, fontData.Length);
        }

        public static Font Ubuntu(float size, FontStyle style = FontStyle.Regular)
        {
            return new Font(UbuntuFamily, size, style);
        }
    }
}
