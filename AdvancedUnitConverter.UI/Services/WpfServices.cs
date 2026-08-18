using System.Windows;
using AdvancedUnitConverter.Core.Interfaces;

namespace AdvancedUnitConverter.UI.Services
{
    public class WpfClipboardService : IClipboardService
    {
        public Task SetTextAsync(string text)
        {
            if (System.Windows.Application.Current?.Dispatcher != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        Clipboard.SetDataObject(text, true);
                    }
                    catch
                    {
                        // Fallback or ignore clipboard lock
                    }
                });
            }
            return Task.CompletedTask;
        }

        public Task<string?> GetTextAsync()
        {
            string? text = null;
            if (System.Windows.Application.Current?.Dispatcher != null)
            {
                System.Windows.Application.Current.Dispatcher.Invoke(() =>
                {
                    try
                    {
                        if (Clipboard.ContainsText())
                        {
                            text = Clipboard.GetText();
                        }
                    }
                    catch
                    {
                        text = null;
                    }
                });
            }
            return Task.FromResult(text);
        }
    }

    public class WpfDialogService : IDialogService
    {
        public Task ShowInformationAsync(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Information);
            return Task.CompletedTask;
        }

        public Task ShowErrorAsync(string title, string message)
        {
            MessageBox.Show(message, title, MessageBoxButton.OK, MessageBoxImage.Error);
            return Task.CompletedTask;
        }

        public Task<bool> ShowConfirmationAsync(string title, string message)
        {
            var res = MessageBox.Show(message, title, MessageBoxButton.YesNo, MessageBoxImage.Question);
            return Task.FromResult(res == MessageBoxResult.Yes);
        }
    }

    public class ThemeManager
    {
        public static void ApplyTheme(Core.Enums.AppTheme theme)
        {
            var app = System.Windows.Application.Current;
            if (app == null) return;

            string themeFile = theme switch
            {
                Core.Enums.AppTheme.Light => "Resources/LightTheme.xaml",
                _ => "Resources/DarkTheme.xaml"
            };

            var uri = new Uri(themeFile, UriKind.Relative);
            var resourceDict = new ResourceDictionary { Source = uri };

            // Replace existing theme dictionary (assuming index 0 is theme)
            var merged = app.Resources.MergedDictionaries;
            var oldTheme = merged.FirstOrDefault(d => d.Source != null && (d.Source.OriginalString.Contains("DarkTheme") || d.Source.OriginalString.Contains("LightTheme")));

            if (oldTheme != null)
            {
                merged.Remove(oldTheme);
            }
            merged.Add(resourceDict);
        }
    }
}
