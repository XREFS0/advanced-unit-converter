using System.Collections.ObjectModel;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdvancedUnitConverter.UI.ViewModels
{
    public partial class FavoritesViewModel : ObservableObject
    {
        private readonly IFavoritesRepository _favoritesRepo;
        private readonly Action<UnitCategoryType, string, string, double?> _onOpenConversion;

        [ObservableProperty]
        private ObservableCollection<FavoriteConversion> _favoriteItems = new();

        [ObservableProperty]
        private bool _isLoading;

        [ObservableProperty]
        private bool _isEmpty;

        public FavoritesViewModel(
            IFavoritesRepository favoritesRepo,
            Action<UnitCategoryType, string, string, double?> onOpenConversion)
        {
            _favoritesRepo = favoritesRepo;
            _onOpenConversion = onOpenConversion;
        }

        public async Task LoadFavoritesAsync()
        {
            IsLoading = true;
            try
            {
                var items = await _favoritesRepo.GetAllAsync();
                FavoriteItems = new ObservableCollection<FavoriteConversion>(items);
                IsEmpty = FavoriteItems.Count == 0;
            }
            finally
            {
                IsLoading = false;
            }
        }

        [RelayCommand]
        private void OpenConversion(FavoriteConversion? favorite)
        {
            if (favorite == null) return;
            _onOpenConversion(favorite.Category, favorite.FromUnitId, favorite.ToUnitId, null);
        }

        [RelayCommand]
        private async Task DeleteFavoriteAsync(FavoriteConversion? favorite)
        {
            if (favorite == null) return;
            await _favoritesRepo.DeleteAsync(favorite.Id);
            FavoriteItems.Remove(favorite);
            IsEmpty = FavoriteItems.Count == 0;
        }
    }
}
