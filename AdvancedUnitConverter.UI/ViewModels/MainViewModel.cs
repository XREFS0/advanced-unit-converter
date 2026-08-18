using System.Collections.ObjectModel;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;
using AdvancedUnitConverter.UI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdvancedUnitConverter.UI.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        private readonly IConversionEngine _engine;
        private readonly IHistoryRepository _historyRepo;
        private readonly IFavoritesRepository _favoritesRepo;
        private readonly ISettingsService _settingsService;
        private readonly IClipboardService _clipboard;
        private readonly IDialogService _dialog;

        [ObservableProperty]
        private string _activeView = "Converter"; // Converter, Quick, History, Favorites, Settings, About

        [ObservableProperty]
        private string _globalSearchQuery = string.Empty;

        [ObservableProperty]
        private bool _isSearchOpen;

        [ObservableProperty]
        private ObservableCollection<UnitDefinition> _searchResults = new();

        [ObservableProperty]
        private ConverterViewModel _converterVM;

        [ObservableProperty]
        private QuickConversionViewModel _quickVM;

        [ObservableProperty]
        private HistoryViewModel _historyVM;

        [ObservableProperty]
        private FavoritesViewModel _favoritesVM;

        [ObservableProperty]
        private SettingsViewModel _settingsVM;

        public IReadOnlyList<ConversionCategory> Categories => _engine.GetAllCategories();

        public MainViewModel(
            IConversionEngine engine,
            IHistoryRepository historyRepo,
            IFavoritesRepository favoritesRepo,
            ISettingsService settingsService,
            IClipboardService clipboard,
            IDialogService dialog)
        {
            _engine = engine;
            _historyRepo = historyRepo;
            _favoritesRepo = favoritesRepo;
            _settingsService = settingsService;
            _clipboard = clipboard;
            _dialog = dialog;

            ConverterVM = new ConverterViewModel(_engine, _historyRepo, _favoritesRepo, _settingsService, _clipboard, _dialog);
            QuickVM = new QuickConversionViewModel(_engine, NavigateToConversion);
            HistoryVM = new HistoryViewModel(_historyRepo, _clipboard, _dialog, (cat, from, to, val) => NavigateToConversion(cat, from, to, val));
            FavoritesVM = new FavoritesViewModel(_favoritesRepo, NavigateToConversion);
            SettingsVM = new SettingsViewModel(_settingsService, _dialog);

            // Default category initialization
            ConverterVM.SetCategory(_settingsService.Settings.DefaultCategory);
        }

        public void NavigateToConversion(UnitCategoryType category, string fromUnitId, string toUnitId, double? val)
        {
            ConverterVM.SetCategory(category, fromUnitId, toUnitId, val);
            ActiveView = "Converter";
            IsSearchOpen = false;
        }

        [RelayCommand]
        private void SelectCategory(UnitCategoryType category)
        {
            ConverterVM.SetCategory(category);
            ActiveView = "Converter";
        }

        [RelayCommand]
        private void Navigate(string viewName)
        {
            ActiveView = viewName;
            if (viewName == "History") _ = HistoryVM.LoadHistoryAsync();
            if (viewName == "Favorites") _ = FavoritesVM.LoadFavoritesAsync();
        }

        [RelayCommand]
        private void ToggleSearch()
        {
            IsSearchOpen = !IsSearchOpen;
            if (IsSearchOpen)
            {
                GlobalSearchQuery = string.Empty;
                SearchResults.Clear();
            }
        }

        partial void OnGlobalSearchQueryChanged(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                SearchResults.Clear();
                return;
            }

            var results = _engine.SearchUnits(value);
            SearchResults = new ObservableCollection<UnitDefinition>(results);
        }

        [RelayCommand]
        private void SelectSearchResult(UnitDefinition? unit)
        {
            if (unit == null) return;
            ConverterVM.SetCategory(unit.Category, unit.Id);
            ActiveView = "Converter";
            IsSearchOpen = false;
        }

        [RelayCommand]
        private void ToggleTheme()
        {
            var next = _settingsService.Settings.Theme == AppTheme.Dark ? AppTheme.Light : AppTheme.Dark;
            _settingsService.Settings.Theme = next;
            ThemeManager.ApplyTheme(next);
            _ = _settingsService.SaveAsync();
        }
    }
}
