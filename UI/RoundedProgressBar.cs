using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer.UI
{
    internal class RoundedGradientProgressBar : Control
    {
        public Color BackShadeColor { get; set; } = Color.FromArgb(25, Color.Black);
        public Color FillShadeColor { get; set; } = Color.FromArgb(128, Color.Black);
        public Color ShineShadeColor { get; set; } = Color.FromArgb(80, Color.Black);
        public int CornerRadius { get; set; } = 12;
        public int ShineWidth { get; set; } = 16;
        private int _maximum = 100;
        public int Maximum
        {
            get => _maximum;
            set
            {
                if (value <= 0) value = 1;
                _maximum = value;
                if (_value > _maximum) _value = _maximum;
            }
        }
        private int _value = 0;
        public int Value
        {
            get => _value;
            set
            {
                if (InvokeRequired)
                {
                    Invoke((MethodInvoker)(() => Value = value));
                    return;
                }
                _value = Math.Max(0, Math.Min(value, Maximum));
            }
        }
        public int StepSize { get; set; } = 1;
        public void PerformStep() => Value = Math.Min(Value + StepSize, Maximum);
        private double _displayedValue = 0;
        private Timer _timer;
        private float _shineOffset = 0f;
        private float _shineDuration = 2000f;
        public RoundedGradientProgressBar()
        {
            DoubleBuffered = true;
            SetStyle(ControlStyles.SupportsTransparentBackColor |
                     ControlStyles.UserPaint |
                     ControlStyles.AllPaintingInWmPaint |
                     ControlStyles.OptimizedDoubleBuffer, true);
            BackColor = Color.Transparent;
            Size = new Size(200, 24);
            _timer = new Timer();
            _timer.Interval = 15;
            _timer.Tick += (s, e) =>
            {
                if (_displayedValue < _value)
                {
                    _displayedValue += Math.Max(1, (_value - _displayedValue) * 0.2);
                    if (_displayedValue > _value) _displayedValue = _value;
                }
                else if (_displayedValue > _value)
                {
                    _displayedValue -= Math.Max(1, (_displayedValue - _displayedValue) * 0.2);
                    if (_displayedValue < _value) _displayedValue = _value;
                }
                Invalidate();
            };
            _timer.Start();
        }
        protected override void OnPaint(PaintEventArgs e)
        {
            e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
            Rectangle emptyRect = new Rectangle(0, 0, Width, Height);
            using (GraphicsPath emptyPath = RoundedRect(emptyRect, CornerRadius))
            using (SolidBrush emptyBrush = new SolidBrush(BackShadeColor))
            {
                e.Graphics.FillPath(emptyBrush, emptyPath);
            }
            double percent = _displayedValue / Maximum;
            int fillWidth = (int)(Width * percent);
            if (fillWidth > 0)
            {
                Rectangle fillRect = new Rectangle(0, 0, fillWidth, Height);
                using (GraphicsPath fillPath = RoundedRect(fillRect, CornerRadius))
                {
                    using (SolidBrush fillBrush = new SolidBrush(FillShadeColor))
                    {
                        e.Graphics.FillPath(fillBrush, fillPath);
                    }
                    int shineWidth = ShineWidth;
                    float tickTime = _timer.Interval;
                    _shineOffset += (fillWidth + shineWidth) * (tickTime / _shineDuration);
                    _shineOffset %= fillWidth + shineWidth;
                    int shineX = (int)(_shineOffset - shineWidth / 2);
                    Rectangle shineRect = new Rectangle(shineX, 0, shineWidth, Height);
                    using (LinearGradientBrush shineBrush = new LinearGradientBrush(shineRect,
                        Color.FromArgb(0, ShineShadeColor),
                        ShineShadeColor,
                        LinearGradientMode.Horizontal))
                    {
                        ColorBlend blend = new ColorBlend();
                        blend.Colors = new Color[]
                        {
                            Color.FromArgb(0, ShineShadeColor),
                            ShineShadeColor,
                            Color.FromArgb(0, ShineShadeColor)
                        };
                        blend.Positions = new float[] { 0f, 0.5f, 1f };
                        shineBrush.InterpolationColors = blend;
                        using (Region clip = new Region(fillPath))
                        {
                            e.Graphics.Clip = clip;
                            e.Graphics.FillRectangle(shineBrush, shineRect);
                            e.Graphics.ResetClip();
                        }
                    }
                }
            }
        }
        private GraphicsPath RoundedRect(Rectangle rect, int radius)
        {
            GraphicsPath path = new GraphicsPath();
            int diameter = radius * 2;
            Rectangle arc = new Rectangle(rect.Location, new Size(diameter, diameter));
            path.AddArc(arc, 180, 90);
            arc.X = rect.Right - diameter;
            path.AddArc(arc, 270, 90);
            arc.Y = rect.Bottom - diameter;
            path.AddArc(arc, 0, 90);
            arc.X = rect.Left;
            path.AddArc(arc, 90, 90);
            path.CloseFigure();
            return path;
        }
    }
}