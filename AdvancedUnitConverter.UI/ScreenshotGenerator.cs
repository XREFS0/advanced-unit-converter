using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using AdvancedUnitConverter.UI.ViewModels;

namespace AdvancedUnitConverter.UI
{
    public static class ScreenshotGenerator
    {
        public static void CaptureScreenshots(MainWindow window)
        {
            string baseDir = AppDomain.CurrentDomain.BaseDirectory;
            // Go up to the repository root directory
            string projectRoot = Path.GetFullPath(Path.Combine(baseDir, "..", "..", "..", ".."));
            string srcFolder = Path.Combine(projectRoot, "src");
            Directory.CreateDirectory(srcFolder);

            if (window.DataContext is not MainViewModel vm) return;

            // Make sure window is rendered
            window.UpdateLayout();

            // 1. Capture Converter Dark Mode
            SaveWindowAsPng(window, Path.Combine(srcFolder, "converter_dark.png"));

            // 2. Capture Quick Conversions
            vm.NavigateCommand.Execute("Quick");
            window.UpdateLayout();
            SaveWindowAsPng(window, Path.Combine(srcFolder, "quick_conversions.png"));

            // 3. Capture History
            vm.NavigateCommand.Execute("History");
            window.UpdateLayout();
            SaveWindowAsPng(window, Path.Combine(srcFolder, "history_view.png"));

            // 4. Capture Settings
            vm.NavigateCommand.Execute("Settings");
            window.UpdateLayout();
            SaveWindowAsPng(window, Path.Combine(srcFolder, "settings_view.png"));

            // 5. Capture Converter in Light Mode
            vm.ToggleThemeCommand.Execute(null);
            vm.NavigateCommand.Execute("Converter");
            window.UpdateLayout();
            SaveWindowAsPng(window, Path.Combine(srcFolder, "converter_light.png"));

            // Revert back to Dark Theme
            vm.ToggleThemeCommand.Execute(null);
        }

        private static void SaveWindowAsPng(Window window, string filePath)
        {
            try
            {
                int width = (int)window.ActualWidth;
                int height = (int)window.ActualHeight;

                if (width <= 0) width = 1100;
                if (height <= 0) height = 750;

                var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
                rtb.Render(window);

                var encoder = new PngBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(rtb));

                using var stream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
                encoder.Save(stream);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Failed to capture screenshot: {ex.Message}");
            }
        }
    }
}
