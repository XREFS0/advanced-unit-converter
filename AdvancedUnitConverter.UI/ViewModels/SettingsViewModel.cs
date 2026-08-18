using System.Collections.ObjectModel;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;
using AdvancedUnitConverter.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdvancedUnitConverter.UI.ViewModels
{
    public partial class SettingsViewModel : ObservableObject
    {
        private readonly ISettingsService _settingsService;
        private readonly IDialogService _dialog;

        [ObservableProperty]
        private AppTheme _selectedTheme;

        [ObservableProperty]
        private UnitCategoryType _defaultCategory;

        [ObservableProperty]
        private PrecisionMode _defaultPrecision;

        [ObservableProperty]
        private int _customPrecision;

        [ObservableProperty]
        private DataStorageSystem _dataStorageSystem;

        [ObservableProperty]
        private string _decimalSeparator = ".";

        [ObservableProperty]
        private string _thousandsSeparator = ",";

        [ObservableProperty]
        private bool _autoSaveHistory;

        [ObservableProperty]
        private string _selectedLanguage = "en";

        public IReadOnlyList<AppTheme> AvailableThemes { get; } = Enum.GetValues<AppTheme>();
        public IReadOnlyList<UnitCategoryType> AvailableCategories { get; } = Enum.GetValues<UnitCategoryType>();
        public IReadOnlyList<PrecisionMode> AvailablePrecisions { get; } = Enum.GetValues<PrecisionMode>();
        public IReadOnlyList<DataStorageSystem> AvailableStorageSystems { get; } = Enum.GetValues<DataStorageSystem>();
        public IReadOnlyList<string> AvailableLanguages { get; } = new[] { "English", "العربية (Arabic)" };

        public SettingsViewModel(ISettingsService settingsService, IDialogService dialog)
        {
            _settingsService = settingsService;
            _dialog = dialog;

            LoadSettings();
        }

        private void LoadSettings()
        {
            var s = _settingsService.Settings;
            SelectedTheme = s.Theme;
            DefaultCategory = s.DefaultCategory;
            DefaultPrecision = s.PrecisionMode;
            CustomPrecision = s.CustomPrecision;
            DataStorageSystem = s.DataStorageSystem;
            DecimalSeparator = s.DecimalSeparator;
            ThousandsSeparator = s.ThousandsSeparator;
            AutoSaveHistory = s.AutoSaveHistory;
            SelectedLanguage = s.Language == "ar" ? "العربية (Arabic)" : "English";
        }

        partial void OnSelectedThemeChanged(AppTheme value)
        {
            _settingsService.Settings.Theme = value;
            ThemeManager.ApplyTheme(value);
            _ = _settingsService.SaveAsync();
        }

        [RelayCommand]
        private async Task SaveSettingsAsync()
        {
            var s = _settingsService.Settings;
            s.Theme = SelectedTheme;
            s.DefaultCategory = DefaultCategory;
            s.PrecisionMode = DefaultPrecision;
            s.CustomPrecision = CustomPrecision;
            s.DataStorageSystem = DataStorageSystem;
            s.DecimalSeparator = DecimalSeparator;
            s.ThousandsSeparator = ThousandsSeparator;
            s.AutoSaveHistory = AutoSaveHistory;
            s.Language = SelectedLanguage.StartsWith("العربية") ? "ar" : "en";

            await _settingsService.SaveAsync();
            await _dialog.ShowInformationAsync("Settings Saved", "Your settings preferences have been updated successfully.");
        }
    }
}
