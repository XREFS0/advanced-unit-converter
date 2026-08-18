using System.Collections.ObjectModel;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdvancedUnitConverter.UI.ViewModels
{
    public partial class ConverterViewModel : ObservableObject
    {
        private readonly IConversionEngine _engine;
        private readonly IHistoryRepository _historyRepo;
        private readonly IFavoritesRepository _favoritesRepo;
        private readonly ISettingsService _settingsService;
        private readonly IClipboardService _clipboard;
        private readonly IDialogService _dialog;

        [ObservableProperty]
        private ConversionCategory? _selectedCategory;

        [ObservableProperty]
        private ObservableCollection<UnitDefinition> _availableUnits = new();

        [ObservableProperty]
        private UnitDefinition? _fromUnit;

        [ObservableProperty]
        private UnitDefinition? _toUnit;

        [ObservableProperty]
        private string _inputValue = "1";

        [ObservableProperty]
        private string _outputValue = "0";

        [ObservableProperty]
        private string _formulaApplied = string.Empty;

        [ObservableProperty]
        private string _errorMessage = string.Empty;

        [ObservableProperty]
        private bool _hasError;

        [ObservableProperty]
        private bool _isFavorite;

        [ObservableProperty]
        private PrecisionMode _currentPrecision = PrecisionMode.Auto;

        [ObservableProperty]
        private string _copyNotificationText = string.Empty;

        [ObservableProperty]
        private bool _showCopyNotification;

        public ConverterViewModel(
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

            CurrentPrecision = _settingsService.Settings.PrecisionMode;
            _settingsService.SettingsChanged += OnSettingsChanged;
        }

        private void OnSettingsChanged()
        {
            CurrentPrecision = _settingsService.Settings.PrecisionMode;
            CalculateConversion();
        }

        public void SetCategory(UnitCategoryType categoryType, string? fromUnitId = null, string? toUnitId = null, double? initialValue = null)
        {
            var cat = _engine.GetCategory(categoryType);
            if (cat == null) return;

            SelectedCategory = cat;
            AvailableUnits = new ObservableCollection<UnitDefinition>(cat.Units);

            if (!string.IsNullOrEmpty(fromUnitId))
                FromUnit = AvailableUnits.FirstOrDefault(u => u.Id.Equals(fromUnitId, StringComparison.OrdinalIgnoreCase));
            else
                FromUnit = AvailableUnits.FirstOrDefault(u => u.IsBaseUnit) ?? AvailableUnits.FirstOrDefault();

            if (!string.IsNullOrEmpty(toUnitId))
                ToUnit = AvailableUnits.FirstOrDefault(u => u.Id.Equals(toUnitId, StringComparison.OrdinalIgnoreCase));
            else
                ToUnit = AvailableUnits.FirstOrDefault(u => u != FromUnit) ?? AvailableUnits.LastOrDefault();

            if (initialValue.HasValue)
            {
                InputValue = initialValue.Value.ToString();
            }

            CheckIsFavorite();
            CalculateConversion();
        }

        partial void OnInputValueChanged(string value) => CalculateConversion();
        partial void OnFromUnitChanged(UnitDefinition? value)
        {
            CheckIsFavorite();
            CalculateConversion();
        }

        partial void OnToUnitChanged(UnitDefinition? value)
        {
            CheckIsFavorite();
            CalculateConversion();
        }

        partial void OnCurrentPrecisionChanged(PrecisionMode value) => CalculateConversion();

        public void CalculateConversion()
        {
            if (SelectedCategory == null || FromUnit == null || ToUnit == null)
            {
                OutputValue = "-";
                FormulaApplied = string.Empty;
                HasError = false;
                ErrorMessage = string.Empty;
                return;
            }

            if (string.IsNullOrWhiteSpace(InputValue))
            {
                OutputValue = "0";
                FormulaApplied = string.Empty;
                HasError = false;
                ErrorMessage = string.Empty;
                return;
            }

            // Parse input string
            string sanitized = InputValue.Trim().Replace(',', '.');
            if (!double.TryParse(sanitized, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out double val))
            {
                HasError = true;
                ErrorMessage = "Please enter a valid numeric value";
                OutputValue = "-";
                FormulaApplied = string.Empty;
                return;
            }

            var options = new ConversionOptions
            {
                DataStorageSystem = _settingsService.Settings.DataStorageSystem,
                PrecisionMode = CurrentPrecision,
                CustomPrecision = _settingsService.Settings.CustomPrecision
            };

            var result = _engine.Convert(SelectedCategory.Type, FromUnit.Id, ToUnit.Id, val, options);

            if (result.IsSuccess)
            {
                HasError = false;
                ErrorMessage = string.Empty;
                OutputValue = result.FormattedValue;
                FormulaApplied = result.FormulaApplied;

                if (_settingsService.Settings.AutoSaveHistory && Math.Abs(val) > 0)
                {
                    _ = SaveHistoryAsync(val, result.Value, result.FormattedValue);
                }
            }
            else
            {
                HasError = true;
                ErrorMessage = result.ErrorMessage;
                OutputValue = "-";
                FormulaApplied = string.Empty;
            }
        }

        private async Task SaveHistoryAsync(double inputVal, double resultVal, string formatted)
        {
            try
            {
                if (SelectedCategory == null || FromUnit == null || ToUnit == null) return;
                await _historyRepo.AddAsync(new ConversionRecord
                {
                    Category = SelectedCategory.Type,
                    FromUnitId = FromUnit.Id,
                    FromUnitName = FromUnit.Name,
                    FromUnitSymbol = FromUnit.Symbol,
                    ToUnitId = ToUnit.Id,
                    ToUnitName = ToUnit.Name,
                    ToUnitSymbol = ToUnit.Symbol,
                    InputValue = inputVal,
                    ResultValue = resultVal,
                    FormattedResult = formatted,
                    Timestamp = DateTime.UtcNow
                });
            }
            catch
            {
                // Silently handle history write lock
            }
        }

        private async void CheckIsFavorite()
        {
            if (SelectedCategory == null || FromUnit == null || ToUnit == null)
            {
                IsFavorite = false;
                return;
            }
            IsFavorite = await _favoritesRepo.ExistsAsync(SelectedCategory.Type, FromUnit.Id, ToUnit.Id);
        }

        [RelayCommand]
        private void SwapUnits()
        {
            if (FromUnit == null || ToUnit == null) return;
            var temp = FromUnit;
            FromUnit = ToUnit;
            ToUnit = temp;
        }

        [RelayCommand]
        private async Task ToggleFavoriteAsync()
        {
            if (SelectedCategory == null || FromUnit == null || ToUnit == null) return;

            if (IsFavorite)
            {
                // Remove favorite
                var favs = await _favoritesRepo.GetAllAsync();
                var item = favs.FirstOrDefault(f => f.Category == SelectedCategory.Type && f.FromUnitId == FromUnit.Id && f.ToUnitId == ToUnit.Id);
                if (item != null)
                {
                    await _favoritesRepo.DeleteAsync(item.Id);
                }
                IsFavorite = false;
            }
            else
            {
                // Add favorite
                await _favoritesRepo.AddAsync(new FavoriteConversion
                {
                    Category = SelectedCategory.Type,
                    FromUnitId = FromUnit.Id,
                    FromUnitName = FromUnit.Name,
                    FromUnitSymbol = FromUnit.Symbol,
                    ToUnitId = ToUnit.Id,
                    ToUnitName = ToUnit.Name,
                    ToUnitSymbol = ToUnit.Symbol,
                    CreatedAt = DateTime.UtcNow
                });
                IsFavorite = true;
            }
        }

        [RelayCommand]
        private async Task CopyResultAsync()
        {
            if (!string.IsNullOrEmpty(OutputValue) && OutputValue != "-")
            {
                await _clipboard.SetTextAsync(OutputValue);
                ShowNotification("Copied result to clipboard!");
            }
        }

        [RelayCommand]
        private void Clear()
        {
            InputValue = "0";
        }

        [RelayCommand]
        private void Reset()
        {
            InputValue = "1";
            if (SelectedCategory != null && AvailableUnits.Count > 1)
            {
                FromUnit = AvailableUnits.FirstOrDefault(u => u.IsBaseUnit) ?? AvailableUnits[0];
                ToUnit = AvailableUnits.FirstOrDefault(u => u != FromUnit) ?? AvailableUnits[1];
            }
        }

        private async void ShowNotification(string message)
        {
            CopyNotificationText = message;
            ShowCopyNotification = true;
            await Task.Delay(2000);
            ShowCopyNotification = false;
        }
    }
}
