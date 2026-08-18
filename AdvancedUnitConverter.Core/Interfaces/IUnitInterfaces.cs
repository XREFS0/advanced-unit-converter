using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Models;

namespace AdvancedUnitConverter.Core.Interfaces
{
    public interface IUnitConverter
    {
        UnitCategoryType CategoryType { get; }
        ConversionResult Convert(double value, UnitDefinition fromUnit, UnitDefinition toUnit, ConversionOptions? options = null);
    }

    public class ConversionOptions
    {
        public DataStorageSystem DataStorageSystem { get; set; } = DataStorageSystem.Decimal;
        public PrecisionMode PrecisionMode { get; set; } = PrecisionMode.Auto;
        public int CustomPrecision { get; set; } = 4;
    }

    public interface IConversionEngine
    {
        IReadOnlyList<ConversionCategory> GetAllCategories();
        ConversionCategory? GetCategory(UnitCategoryType type);
        ConversionResult Convert(UnitCategoryType category, string fromUnitId, string toUnitId, double value, ConversionOptions? options = null);
        void RegisterConverter(IUnitConverter converter);
        void RegisterCategory(ConversionCategory category);
        IEnumerable<UnitDefinition> SearchUnits(string query);
    }

    public interface IHistoryRepository
    {
        Task<IEnumerable<ConversionRecord>> GetAllAsync();
        Task<IEnumerable<ConversionRecord>> GetByCategoryAsync(UnitCategoryType category);
        Task<IEnumerable<ConversionRecord>> SearchAsync(string query);
        Task<long> AddAsync(ConversionRecord record);
        Task<bool> DeleteAsync(long id);
        Task<bool> DeleteBatchAsync(IEnumerable<long> ids);
        Task<bool> ClearAllAsync();
    }

    public interface IFavoritesRepository
    {
        Task<IEnumerable<FavoriteConversion>> GetAllAsync();
        Task<long> AddAsync(FavoriteConversion favorite);
        Task<bool> DeleteAsync(long id);
        Task<bool> ExistsAsync(UnitCategoryType category, string fromUnitId, string toUnitId);
    }

    public interface ISettingsService
    {
        AppSettings Settings { get; }
        Task LoadAsync();
        Task SaveAsync();
        event Action? SettingsChanged;
    }

    public interface IClipboardService
    {
        Task SetTextAsync(string text);
        Task<string?> GetTextAsync();
    }

    public interface IDialogService
    {
        Task ShowInformationAsync(string title, string message);
        Task ShowErrorAsync(string title, string message);
        Task<bool> ShowConfirmationAsync(string title, string message);
    }
}
