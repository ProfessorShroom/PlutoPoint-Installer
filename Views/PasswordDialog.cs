using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

// Copyright © Charlie Howard 2026 All rights reserved.

namespace PlutoPoint_Installer.Views
{
    public class PasswordDialog : Window
    {
        public string EnteredPassword { get; private set; }

        private readonly TextBox _txtPassword;

        public PasswordDialog()
        {
            Title = "Password Required.";
            Width = 320;
            Height = 170;
            CanResize = false;
            WindowStartupLocation = WindowStartupLocation.CenterOwner;

            var passwordText = new TextBlock
            {
                Text = "The installer is not being run from a known location, please enter password to continue.",
                Foreground = Brushes.Red,
                TextWrapping = Avalonia.Media.TextWrapping.Wrap,
                Margin = new Avalonia.Thickness(10, 10, 10, 0)
            };

            var lbl = new TextBlock { Text = "Password:", Margin = new Avalonia.Thickness(10, 8, 0, 0) };
            _txtPassword = new TextBox { PasswordChar = '*', Width = 180, Margin = new Avalonia.Thickness(4, 0, 10, 0) };

            var passwordRow = new StackPanel
            {
                Orientation = Orientation.Horizontal,
                Margin = new Avalonia.Thickness(0, 12, 0, 0)
            };
            passwordRow.Children.Add(lbl);
            passwordRow.Children.Add(_txtPassword);

            var btnOK = new Button { Content = "OK", Width = 80, HorizontalAlignment = HorizontalAlignment.Right, Margin = new Avalonia.Thickness(0, 16, 10, 0) };
            btnOK.Click += (_, _) =>
            {
                EnteredPassword = _txtPassword.Text;
                Close(true);
            };

            var root = new StackPanel();
            root.Children.Add(passwordText);
            root.Children.Add(passwordRow);
            root.Children.Add(btnOK);

            Content = root;
        }
    }
}
