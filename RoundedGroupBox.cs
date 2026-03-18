using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
namespace PlutoPoint_Installer
{
    public class RoundedGroupBox : GroupBox
    {
        public int CornerRadius { get; set; } = 3;
        public Color BorderColor { get; set; } = Color.Silver;
        public Color? BorderColorOverride { get; set; } = null;
        public Color? TextColorOverride { get; set; } = null;
        public float BorderThickness { get; set; } = 2f;
        public int BorderShadowTop { get; set; } = 26;
        public int BorderShadowBottom { get; set; } = 77;
        public RoundedGroupBox()
        {
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.SupportsTransparentBackColor, true);
            BackColor = Color.Transparent;
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            int topPadding = 7;
            int bottomPadding = 7;
            SizeF textSize = e.Graphics.MeasureString(Text, Font);
            RectangleF textRect = new RectangleF(
                (ClientRectangle.Width - textSize.Width) / 2f - 4,
                0,
                textSize.Width + 8,
                textSize.Height
            );
            Rectangle rect = new Rectangle(
                0,
                topPadding,
                ClientRectangle.Width - 1,
                ClientRectangle.Height - topPadding - bottomPadding
            );
            using (GraphicsPath path = new GraphicsPath())
            {
                int dia = CornerRadius * 2;
                path.AddArc(rect.X, rect.Y, dia, dia, 180, 90);
                path.AddArc(rect.Right - dia, rect.Y, dia, dia, 270, 90);
                path.AddArc(rect.Right - dia, rect.Bottom - dia, dia, dia, 0, 90);
                path.AddArc(rect.X, rect.Bottom - dia, dia, dia, 90, 90);
                path.CloseFigure();
                Region oldClip = e.Graphics.Clip;
                e.Graphics.SetClip(textRect, CombineMode.Exclude);
                using (LinearGradientBrush borderBrush = new LinearGradientBrush(
                    rect,
                    Color.FromArgb(BorderShadowTop, 0, 0, 0),
                    Color.FromArgb(BorderShadowBottom, 0, 0, 0),
                    LinearGradientMode.Vertical))
                {
                    using (Pen pen = new Pen(borderBrush, BorderThickness))
                        e.Graphics.DrawPath(pen, path);
                }
                e.Graphics.Clip = oldClip;
            }
            using (SolidBrush textBrush = new SolidBrush(TextColorOverride ?? ForeColor))
                e.Graphics.DrawString(Text, Font, textBrush, textRect.X + 4, textRect.Y);
        }
    }
}