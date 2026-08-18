using System.Collections.ObjectModel;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdvancedUnitConverter.UI.ViewModels
{
    public partial class HistoryViewModel : ObservableObject
    {
        private readonly IHistoryRepository _historyRepo;
        private readonly IClipboardService _clipboard;
        private readonly IDialogService _dialog;
        private readonly Action<UnitCategoryType, string, string, double> _onReuseConversion;

        [ObservableProperty]
        private ObservableCollection<ConversionRecord> _historyItems = new();

        [ObservableProperty]
        private ConversionRecord? _selectedRecord;

        [ObservableProperty]
        private string _searchQuery = string.Empty;

        [ObservableProperty]
        private UnitCategoryType? _selectedCategoryFilter;

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isEmpty;

        public IReadOnlyList<UnitCategoryType?> CategoryFilters { get; }

        public HistoryViewModel(
            IHistoryRepository historyRepo,
            IClipboardService clipboard,
            IDialogService dialog,
            Action<UnitCategoryType, string, string, double> onReuseConversion)
        {
            _historyRepo = historyRepo;
            _clipboard = clipboard;
            _dialog = dialog;
            _onReuseConversion = onReuseConversion;

            var filters = new List<UnitCategoryType?> { null };
            filters.AddRange(Enum.GetValues<UnitCategoryType>().Cast<UnitCategoryType?>());
            CategoryFilters = filters;
        }

        public async Task LoadHistoryAsync()
        {
            IsLoading = true;
            try
            {
                IEnumerable<ConversionRecord> records;

                if (!string.IsNullOrWhiteSpace(SearchQuery))
                {
                    records = await _historyRepo.SearchAsync(SearchQuery);
                }
                else if (SelectedCategoryFilter.HasValue)
                {
                    records = await _historyRepo.GetByCategoryAsync(SelectedCategoryFilter.Value);
                }
                else
                {
                    records = await _historyRepo.GetAllAsync();
                }

                HistoryItems = new ObservableCollection<ConversionRecord>(records);
                IsEmpty = HistoryItems.Count == 0;
            }
            finally
            {
                IsLoading = false;
            }
        }

        partial void OnSearchQueryChanged(string value) => _ = LoadHistoryAsync();
        partial void OnSelectedCategoryFilterChanged(UnitCategoryType? value) => _ = LoadHistoryAsync();

        [RelayCommand]
        private async Task CopyResultAsync(ConversionRecord? record)
        {
            if (record == null) return;
            await _clipboard.SetTextAsync(record.FormattedResult);
        }

        [RelayCommand]
        private void ReuseConversion(ConversionRecord? record)
        {
            if (record == null) return;
            _onReuseConversion(record.Category, record.FromUnitId, record.ToUnitId, record.InputValue);
        }

        [RelayCommand]
        private async Task DeleteRecordAsync(ConversionRecord? record)
        {
            if (record == null) return;
            await _historyRepo.DeleteAsync(record.Id);
            HistoryItems.Remove(record);
            IsEmpty = HistoryItems.Count == 0;
        }

        [RelayCommand]
        private async Task ClearAllAsync()
        {
            bool confirm = await _dialog.ShowConfirmationAsync("Clear History", "Are you sure you want to clear all conversion history?");
            if (confirm)
            {
                await _historyRepo.ClearAllAsync();
                HistoryItems.Clear();
                IsEmpty = true;
            }
        }
    }
}
