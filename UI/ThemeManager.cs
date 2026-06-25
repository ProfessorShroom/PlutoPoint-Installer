using PlutoPoint_Installer.Models;
using PlutoPoint_Installer.Utilities;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Windows.Forms;
using PlutoPoint_Installer;

namespace PlutoPoint_Installer.UI
{
    public class ThemeManager
    {
        private readonly List<InstallerEvent> _events = new List<InstallerEvent>();

        public ThemeManager()
        {
            // Pancake Day
            _events.Add(new InstallerEvent
            {
                Name = "Pancake Day",
                IsActiveToday = (date) => date.Date == GetShroveTuesday(date.Year),
                GradientTop = Color.FromArgb(242, 200, 150),
                GradientBottom = Color.FromArgb(205, 145, 95),
                LogColor = Color.FromArgb(95, 55, 25),
                Messages = new List<string> { "", "🥞 It's Pancake Day!", "Don't forget to have some pancakes you fat bastard!", "" },
                OverlayImage = Properties.Resources.pancake
            });

            var fixedEvents = new[]
                        {
                new {
                    Name = "New Year",
                    M = 12, D = 31, M2 = 1, D2 = 5,
                    Top = Color.FromArgb(255, 210, 90), Bot = Color.FromArgb(198, 140, 35), Log = Color.FromArgb(90, 65, 15),
                    Sound = (Action)AudioEffects.PlayCompleteNewYearsChime,
                    Msg = new List<string> { "", "🎉 Happy New Year!", "" },
                    Img = Properties.Resources.newyear, Icon = (Icon)null, Rot = 0f
                },
                new {
                    Name = "Christmas",
                    M = 12, D = 1, M2 = 12, D2 = 31,
                    Top = Color.FromArgb(18, 110, 58), Bot = Color.FromArgb(120, 18, 32), Log = Color.White,
                    Sound = (Action)AudioEffects.PlayCompleteChristmasChime,
                    Msg = new List<string> { "", "🎄 Merry Christmas!", "" },
                    Img = Properties.Resources.christmasTree, Icon = Properties.Resources.computerRepairCentreIconChristmas, Rot = 0f
                },
                new {
                    Name = "Halloween",
                    M = 10, D = 25, M2 = 10, D2 = 31,
                    Top = Color.FromArgb(35, 35, 35), Bot = Color.FromArgb(120, 45, 0), Log = Color.White,
                    Sound = (Action)AudioEffects.PlayCompleteHalloweenChime,
                    Msg = new List<string> { "", "🎃 Boo! Happy Halloween!", "" },
                    Img = Properties.Resources.pumpkin, Icon = Properties.Resources.computerRepairCentreIconHalloween, Rot = 0f
                },
                new {
                    Name = "Valentines",
                    M = 2, D = 14, M2 = 2, D2 = 14,
                    Top = Color.FromArgb(245, 215, 225), Bot = Color.FromArgb(214, 150, 175), Log = Color.FromArgb(135, 20, 50),
                    Sound = (Action)AudioEffects.PlayCompleteValentinesChime,
                    Msg = new List<string> { "", "❤️ Happy Valentines Day!", "" },
                    Img = Properties.Resources.heart, Icon = Properties.Resources.computerRepairCentreIconValentines, Rot = 30f
                },
                new {
                    Name = "Puffin Day",
                    M = 4, D = 14, M2 = 4, D2 = 14,
                    Top = Color.FromArgb(35, 45, 70), Bot = Color.FromArgb(90, 125, 160), Log = Color.White,
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🐧 Today is World Puffin day!", "" },
                    Img = Properties.Resources.puffin, Icon = Properties.Resources.computerRepairCentreIconPuffin, Rot = 0f
                },
                new {
                    Name = "Duck Day",
                    M = 4, D = 4, M2 = 4, D2 = 4,
                    Top = Color.FromArgb(215, 185, 125), Bot = Color.FromArgb(150, 120, 78), Log = Color.FromArgb(76, 94, 64),
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🦆 Today is National Duck day!", "Did someone say duck?", "" },
                    Img = Properties.Resources.duck, Icon = (Icon)null, Rot = 0f
                },
                new {
                    Name = "Dachshund Day",
                    M = 6, D = 21, M2 = 6, D2 = 21,
                    Top = Color.FromArgb(245, 228, 212), Bot = Color.FromArgb(205, 170, 138), Log = Color.FromArgb(110, 72, 45),
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🌭 Today is National Dachshund day!", "" },
                    Img = Properties.Resources.pluto, Icon = Properties.Resources.plutoLogo, Rot = 0f
                },
                new {
                    Name = "Pluto Day",
                    M = 3, D = 12, M2 = 3, D2 = 12,
                    Top = Color.FromArgb(242, 225, 210), Bot = Color.FromArgb(182, 132, 92), Log = Color.FromArgb(108, 72, 48),
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🪐🌭🎂 Today is Pluto's Birthday!", "" },
                    Img = Properties.Resources.pluto, Icon = Properties.Resources.plutoLogo, Rot = 0f
                },
                new {
                    Name = "Hippo Day",
                    M = 2, D = 15, M2 = 2, D2 = 15,
                    Top = Color.FromArgb(98, 98, 105), Bot = Color.FromArgb(58, 58, 64), Log = Color.White,
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🦛 Today is World Hippo day!", "Don't get too excited Steve", "" },
                    Img = Properties.Resources.hippo, Icon = (Icon)null, Rot = 0f
                },
                new {
                    Name = "Rhino Day",
                    M = 9, D = 22, M2 = 9, D2 = 22,
                    Top = Color.FromArgb(110, 110, 115), Bot = Color.FromArgb(62, 62, 68), Log = Color.White,
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🦏 Today is World Rhino day!", "Don't get too excited Steve", "" },
                    Img = Properties.Resources.rhino, Icon = (Icon)null, Rot = 0f
                },
                new {
                    Name = "Star Wars Day",
                    M = 5, D = 4, M2 = 5, D2 = 4,
                    Top = Color.FromArgb(34, 34, 34), Bot = Color.FromArgb(40, 40, 40), Log = Color.White,
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🌌 Today is Star Wars day!", "May the 4th be with you!", "" },
                    Img = Properties.Resources.starwars, Icon = (Icon)null, Rot = 0f
                }
            };

            foreach (var e in fixedEvents)
            {
                _events.Add(new InstallerEvent
                {
                    Name = e.Name,
                    IsActiveToday = (date) =>
                    {
                        DateTime start = new DateTime(date.Year, e.M, e.D);
                        DateTime end = new DateTime(date.Year, e.M2, e.D2);
                        return (start <= end) ? (date >= start && date <= end) : (date >= start || date <= end);
                    },
                    GradientTop = e.Top,
                    GradientBottom = e.Bot,
                    LogColor = e.Log,
                    PlaySound = e.Sound,
                    Messages = e.Msg,
                    OverlayImage = e.Img,
                    OverlayIcon = e.Icon,
                    RotationDegrees = e.Rot
                });
            }

            AddBirthday("Charlie", 4, 6);
            AddBirthday("Dean", 4, 21);
            AddBirthday("Steve", 6, 24);
            AddBirthday("Howard", 5, 16);
            AddBirthday("Adam", 6, 9);
            AddBirthday("Geeth", 7, 25);
        }

        private void AddBirthday(string name, int month, int day)
        {
            _events.Add(new InstallerEvent
            {
                Name = $"{name}'s Birthday",
                IsActiveToday = (date) => date.Month == month && date.Day == day,
                GradientTop = Color.FromArgb(175, 220, 228),
                GradientBottom = Color.FromArgb(245, 182, 198),
                LogColor = Color.FromArgb(80, 60, 75),
                PlaySound = (Action)AudioEffects.PlayCompleteBirthdayChime,
                Messages = new List<string> { "", $"🎂 It is {name}'s birthday today!", $"🎉 Happy birthday {name}!", "" },
                OverlayImage = Properties.Resources.present,
                OverlayIcon = Properties.Resources.computerRepairCentreIconBirthday,
                RotationDegrees = 0f
            });
        }

        // Logic for Shrove Tuesday
        private DateTime GetShroveTuesday(int year)
        {
            int a = year % 19;
            int b = year / 100;
            int c = year % 100;
            int d = b / 4;
            int e = b % 4;
            int f = (b + 8) / 25;
            int g = (b - f + 1) / 3;
            int h = (19 * a + b - d - g + 15) % 30;
            int i = c / 4;
            int k = c % 4;
            int l = (32 + 2 * e + 2 * i - h - k) % 7;
            int m = (a + 11 * h + 22 * l) / 451;
            int month = (h + l - 7 * m + 114) / 31;
            int day = ((h + l - 7 * m + 114) % 31) + 1;

            return new DateTime(year, month, day).AddDays(-47);
        }

        public InstallerEvent GetCurrentEvent()
        {
            var today = DateTime.Today;
            return _events.FirstOrDefault(e => e.IsActiveToday(today));
        }

        public void ApplyThemeAndMessages(Action<Color, Color, Color> applyGradient, Action<string> appendLine)
        {
            var currentEvent = GetCurrentEvent();
            if (currentEvent != null)
            {
                applyGradient(currentEvent.GradientTop, currentEvent.GradientBottom, currentEvent.LogColor);
                foreach (var msg in currentEvent.Messages) appendLine(msg);
            }
        }
        public void PlayEventSound()
        {
            var currentEvent = GetCurrentEvent();
            if (currentEvent != null && currentEvent.PlaySound != null)
            {
                currentEvent.PlaySound();
            }
            else
            {
                AudioEffects.PlayCompleteChime();
            }
        }
        public void UpdateGUIEvent(installerForm form)
        {
            var currentEvent = GetCurrentEvent();
            if (currentEvent == null) return;
            form.Icon = currentEvent.OverlayIcon ?? form.Icon;
            form.OverlayImage = currentEvent.OverlayImage;
            form.OverlayRotationDegrees = currentEvent.RotationDegrees;
            form.AdjustInstallerTextBoxSizeForOverlay();
            form.Invalidate();
        }

        public static Rectangle GetScaledRect(Image img, int x, int y, int maxW, int maxH)
        {
            float ratio = Math.Min((float)maxW / img.Width, (float)maxH / img.Height);
            ratio = Math.Min(ratio, 1f);
            return new Rectangle(x, y, (int)(img.Width * ratio), (int)(img.Height * ratio));
        }

    }
}