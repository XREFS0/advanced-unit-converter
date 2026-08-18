using System.Windows;
using AdvancedUnitConverter.Application.Engine;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Infrastructure.Database;
using AdvancedUnitConverter.Infrastructure.Repositories;
using AdvancedUnitConverter.Infrastructure.Settings;
using AdvancedUnitConverter.UI.Services;
using AdvancedUnitConverter.UI.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AdvancedUnitConverter.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            // Dependency Injection Setup
            var services = new ServiceCollection();

            // Core & Engine
            services.AddSingleton<IConversionEngine, ConversionEngine>();

            // Infrastructure
            services.AddSingleton<DatabaseContext>();
            services.AddSingleton<IHistoryRepository, HistoryRepository>();
            services.AddSingleton<IFavoritesRepository, FavoritesRepository>();
            services.AddSingleton<ISettingsService, SettingsService>();

            // UI Services
            services.AddSingleton<IClipboardService, WpfClipboardService>();
            services.AddSingleton<IDialogService, WpfDialogService>();

            // ViewModels
            services.AddSingleton<MainViewModel>();

            var serviceProvider = services.BuildServiceProvider();

            // Load settings & apply initial theme
            var settingsService = serviceProvider.GetRequiredService<ISettingsService>();
            _ = settingsService.LoadAsync().ContinueWith(_ =>
            {
                Dispatcher.Invoke(() =>
                {
                    ThemeManager.ApplyTheme(settingsService.Settings.Theme);
                });
            });

            var mainVm = serviceProvider.GetRequiredService<MainViewModel>();
            DataContext = mainVm;

            Loaded += (s, e) =>
            {
                ScreenshotGenerator.CaptureScreenshots(this);
            };
        }
    }
}