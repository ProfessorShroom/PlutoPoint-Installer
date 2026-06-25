using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer.UI
{
    internal class RoundedButton : Control
    {
        private bool _hovered;
        private bool _pressed;
        private Color _buttonColor = Color.FromArgb(80, 80, 255);
        private int _cornerRadius = 7;
        public override Color BackColor
        {
            get => _buttonColor;
            set
            {
                _buttonColor = value;
                Invalidate();
            }
        }
        public int CornerRadius
        {
            get => _cornerRadius;
            set
            {
                _cornerRadius = Math.Max(1, value);
                Invalidate();
            }
        }
        public Color HoverShadeColor { get; set; } = Color.FromArgb(25, Color.White);
        public Color PressedShadeColor { get; set; } = Color.FromArgb(50, Color.Black);
        public RoundedButton()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer |
                     ControlStyles.ResizeRedraw |
                     ControlStyles.SupportsTransparentBackColor, true);
            base.BackColor = Color.Transparent;
            ForeColor = Color.White;
            Size = new Size(105, 32);
            Cursor = Cursors.Hand;
        }
        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            _hovered = true;
            Invalidate();
        }
        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            _hovered = false;
            _pressed = false;
            Invalidate();
        }
        protected override void OnMouseDown(MouseEventArgs e)
        {
            base.OnMouseDown(e);
            if (e.Button == MouseButtons.Left)
            {
                _pressed = true;
                Invalidate();
            }
        }
        protected override void OnMouseUp(MouseEventArgs e)
        {
            base.OnMouseUp(e);
            _pressed = false;
            Invalidate();
        }
        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (Parent == null)
            {
                base.OnPaintBackground(pevent);
                return;
            }
            Graphics g = pevent.Graphics;
            GraphicsState state = g.Save();
            try
            {
                g.TranslateTransform(-Left, -Top);
                PaintEventArgs pea = new PaintEventArgs(
                    g,
                    new Rectangle(Left, Top, Width, Height));
                InvokePaintBackground(Parent, pea);
                InvokePaint(Parent, pea);
            }
            finally
            {
                g.Restore(state);
            }
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            e.Graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;
            e.Graphics.CompositingQuality = CompositingQuality.HighQuality;
            Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
            using (GraphicsPath path = RoundedRect(rect, CornerRadius))
            {
                using (SolidBrush backBrush = new SolidBrush(_buttonColor))
                {
                    e.Graphics.FillPath(backBrush, path);
                }
                if (_hovered && !_pressed)
                {
                    using (SolidBrush hoverBrush = new SolidBrush(HoverShadeColor))
                    {
                        e.Graphics.FillPath(hoverBrush, path);
                    }
                }
                if (_pressed)
                {
                    using (SolidBrush pressedBrush = new SolidBrush(PressedShadeColor))
                    {
                        e.Graphics.FillPath(pressedBrush, path);
                    }
                }
            }
            var flags = TextFormatFlags.SingleLine |
                        TextFormatFlags.NoPadding;
            Size textSize = TextRenderer.MeasureText(
                e.Graphics,
                Text,
                Font,
                new Size(int.MaxValue, int.MaxValue),
                flags);
            int textX = (Width - textSize.Width) / 2;
            int textY = (int)Math.Round((Height - textSize.Height) / 2f - 0.5f);
            TextRenderer.DrawText(
                e.Graphics,
                Text,
                Font,
                new Point(textX, textY),
                ForeColor,
                flags | TextFormatFlags.EndEllipsis);
        }
        private GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            radius = Math.Max(1, radius);
            int diameter = radius * 2;
            if (diameter > rect.Width)
                diameter = rect.Width;
            if (diameter > rect.Height)
                diameter = rect.Height;
            Rectangle arc = new Rectangle(rect.X, rect.Y, diameter, diameter);
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.X;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}