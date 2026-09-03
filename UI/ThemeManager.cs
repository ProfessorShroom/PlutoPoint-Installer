using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform;
using PlutoPoint_Installer.Models;
using PlutoPoint_Installer.Utilities;

namespace PlutoPoint_Installer.UI
{
    public class ThemeManager
    {
        private readonly List<InstallerEvent> _events = new List<InstallerEvent>();

        private static Bitmap LoadImage(string name) =>
            new Bitmap(AssetLoader.Open(new Uri($"avares://Computer Repair Centre Installer/Resources/images/{name}.png")));

        private static WindowIcon LoadIcon(string name) =>
            new WindowIcon(AssetLoader.Open(new Uri($"avares://Computer Repair Centre Installer/Resources/icons/{name}.ico")));

        public ThemeManager()
        {
            // Pancake Day
            _events.Add(new InstallerEvent
            {
                Name = "Pancake Day",
                IsActiveToday = (date) => date.Date == GetShroveTuesday(date.Year),
                GradientTop = Color.FromArgb(255, 242, 200, 150),
                GradientBottom = Color.FromArgb(255, 205, 145, 95),
                LogColor = Color.FromArgb(255, 95, 55, 25),
                Messages = new List<string> { "", "🥞 It's Pancake Day!", "Don't forget to have some pancakes you fat bastard!", "" },
                OverlayImage = LoadImage("pancake")
            });

            var fixedEvents = new[]
            {
                new {
                    Name = "New Year",
                    M = 12, D = 31, M2 = 1, D2 = 5,
                    Top = Color.FromArgb(255, 255, 210, 90), Bot = Color.FromArgb(255, 198, 140, 35), Log = Color.FromArgb(255, 90, 65, 15),
                    Sound = (Action)AudioEffects.PlayCompleteNewYearsChime,
                    Msg = new List<string> { "", "🎉 Happy New Year!", "" },
                    Img = "newyear", Icon = (string)null, Rot = 0f
                },
                new {
                    Name = "Christmas",
                    M = 12, D = 1, M2 = 12, D2 = 31,
                    Top = Color.FromArgb(255, 18, 110, 58), Bot = Color.FromArgb(255, 120, 18, 32), Log = Colors.White,
                    Sound = (Action)AudioEffects.PlayCompleteChristmasChime,
                    Msg = new List<string> { "", "🎄 Merry Christmas!", "" },
                    Img = "christmas", Icon = "computerRepairCentreIconChristmas", Rot = 0f
                },
                new {
                    Name = "Halloween",
                    M = 10, D = 25, M2 = 10, D2 = 31,
                    Top = Color.FromArgb(255, 35, 35, 35), Bot = Color.FromArgb(255, 120, 45, 0), Log = Colors.White,
                    Sound = (Action)AudioEffects.PlayCompleteHalloweenChime,
                    Msg = new List<string> { "", "🎃 Boo! Happy Halloween!", "" },
                    Img = "pumpkin", Icon = "computerRepairCentreIconHalloween", Rot = 0f
                },
                new {
                    Name = "Valentines",
                    M = 2, D = 14, M2 = 2, D2 = 14,
                    Top = Color.FromArgb(255, 245, 215, 225), Bot = Color.FromArgb(255, 214, 150, 175), Log = Color.FromArgb(255, 135, 20, 50),
                    Sound = (Action)AudioEffects.PlayCompleteValentinesChime,
                    Msg = new List<string> { "", "❤️ Happy Valentines Day!", "" },
                    Img = "heart", Icon = "computerRepairCentreIconValentines", Rot = 30f
                },
                new {
                    Name = "Puffin Day",
                    M = 4, D = 14, M2 = 4, D2 = 14,
                    Top = Color.FromArgb(255, 35, 45, 70), Bot = Color.FromArgb(255, 90, 125, 160), Log = Colors.White,
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🐧 Today is World Puffin day!", "" },
                    Img = "puffin", Icon = "computerRepairCentreIconPuffin", Rot = 0f
                },
                new {
                    Name = "Duck Day",
                    M = 4, D = 4, M2 = 4, D2 = 4,
                    Top = Color.FromArgb(255, 215, 185, 125), Bot = Color.FromArgb(255, 150, 120, 78), Log = Color.FromArgb(255, 76, 94, 64),
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🦆 Today is National Duck day!", "Did someone say duck?", "" },
                    Img = "duck", Icon = (string)null, Rot = 0f
                },
                new {
                    Name = "Dachshund Day",
                    M = 6, D = 21, M2 = 6, D2 = 21,
                    Top = Color.FromArgb(255, 245, 228, 212), Bot = Color.FromArgb(255, 205, 170, 138), Log = Color.FromArgb(255, 110, 72, 45),
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🌭 Today is National Dachshund day!", "" },
                    Img = "pluto", Icon = "plutoLogo", Rot = 0f
                },
                new {
                    Name = "Pluto Day",
                    M = 3, D = 12, M2 = 3, D2 = 12,
                    Top = Color.FromArgb(255, 242, 225, 210), Bot = Color.FromArgb(255, 182, 132, 92), Log = Color.FromArgb(255, 108, 72, 48),
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🪐🌭🎂 Today is Pluto's Birthday!", "" },
                    Img = "pluto", Icon = "plutoLogo", Rot = 0f
                },
                new {
                    Name = "Hippo Day",
                    M = 2, D = 15, M2 = 2, D2 = 15,
                    Top = Color.FromArgb(255, 98, 98, 105), Bot = Color.FromArgb(255, 58, 58, 64), Log = Colors.White,
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🦛 Today is World Hippo day!", "Don't get too excited Steve", "" },
                    Img = "hippo", Icon = (string)null, Rot = 0f
                },
                new {
                    Name = "Rhino Day",
                    M = 9, D = 22, M2 = 9, D2 = 22,
                    Top = Color.FromArgb(255, 110, 110, 115), Bot = Color.FromArgb(255, 62, 62, 68), Log = Colors.White,
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🦏 Today is World Rhino day!", "Don't get too excited Steve", "" },
                    Img = "rhino", Icon = (string)null, Rot = 0f
                },
                new {
                    Name = "Star Wars Day",
                    M = 5, D = 4, M2 = 5, D2 = 4,
                    Top = Color.FromArgb(255, 34, 34, 34), Bot = Color.FromArgb(255, 40, 40, 40), Log = Colors.White,
                    Sound = (Action)null,
                    Msg = new List<string> { "", "🌌 Today is Star Wars day!", "May the 4th be with you!", "" },
                    Img = "starwars", Icon = (string)null, Rot = 0f
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
                    OverlayImage = LoadImage(e.Img),
                    OverlayIcon = e.Icon != null ? LoadIcon(e.Icon) : null,
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
                GradientTop = Color.FromArgb(255, 175, 220, 228),
                GradientBottom = Color.FromArgb(255, 245, 182, 198),
                LogColor = Color.FromArgb(255, 80, 60, 75),
                PlaySound = (Action)AudioEffects.PlayCompleteBirthdayChime,
                Messages = new List<string> { "", $"🎂 It is {name}'s birthday today!", $"🎉 Happy birthday {name}!", "" },
                OverlayImage = LoadImage("present"),
                OverlayIcon = LoadIcon("computerRepairCentreIconBirthday"),
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

        public void ApplyButtonTheme(Action<Color, Color> applyButtonColors)
        {
            var currentEvent = GetCurrentEvent();
            if (currentEvent != null)
            {
                applyButtonColors(currentEvent.GradientTop, Colors.White);
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

        public void UpdateGUIEvent(Window window, Image overlayImageControl)
        {
            var currentEvent = GetCurrentEvent();
            if (currentEvent == null) return;
            if (currentEvent.OverlayIcon != null)
                window.Icon = currentEvent.OverlayIcon;
            overlayImageControl.Source = currentEvent.OverlayImage;
            overlayImageControl.IsVisible = currentEvent.OverlayImage != null;
            if (overlayImageControl.RenderTransform is RotateTransform rotateTransform)
                rotateTransform.Angle = currentEvent.RotationDegrees;
        }
    }
}
