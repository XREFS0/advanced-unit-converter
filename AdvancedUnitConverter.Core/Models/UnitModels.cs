using AdvancedUnitConverter.Core.Enums;

namespace AdvancedUnitConverter.Core.Models
{
    public class UnitDefinition
    {
        public string Id { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Symbol { get; set; } = string.Empty;
        public UnitCategoryType Category { get; set; }
        public double FactorToBase { get; set; } = 1.0;
        public bool IsBaseUnit { get; set; }
        public string Description { get; set; } = string.Empty;

        public override string ToString() => $"{Name} ({Symbol})";
    }

    public class ConversionCategory
    {
        public UnitCategoryType Type { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public string IconKey { get; set; } = string.Empty;
        public string BaseUnitId { get; set; } = string.Empty;
        public List<UnitDefinition> Units { get; set; } = new();
    }

    public class ConversionResult
    {
        public bool IsSuccess { get; set; }
        public double Value { get; set; }
        public string FormattedValue { get; set; } = string.Empty;
        public string FormulaApplied { get; set; } = string.Empty;
        public string ErrorMessage { get; set; } = string.Empty;

        public static ConversionResult Success(double value, string formattedValue, string formula)
        {
            return new ConversionResult
            {
                IsSuccess = true,
                Value = value,
                FormattedValue = formattedValue,
                FormulaApplied = formula
            };
        }

        public static ConversionResult Fail(string error)
        {
            return new ConversionResult
            {
                IsSuccess = false,
                ErrorMessage = error
            };
        }
    }

    public class ConversionRecord
    {
        public long Id { get; set; }
        public UnitCategoryType Category { get; set; }
        public string FromUnitId { get; set; } = string.Empty;
        public string FromUnitName { get; set; } = string.Empty;
        public string FromUnitSymbol { get; set; } = string.Empty;
        public string ToUnitId { get; set; } = string.Empty;
        public string ToUnitName { get; set; } = string.Empty;
        public string ToUnitSymbol { get; set; } = string.Empty;
        public double InputValue { get; set; }
        public double ResultValue { get; set; }
        public string FormattedResult { get; set; } = string.Empty;
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
    }

    public class FavoriteConversion
    {
        public long Id { get; set; }
        public UnitCategoryType Category { get; set; }
        public string FromUnitId { get; set; } = string.Empty;
        public string FromUnitName { get; set; } = string.Empty;
        public string FromUnitSymbol { get; set; } = string.Empty;
        public string ToUnitId { get; set; } = string.Empty;
        public string ToUnitName { get; set; } = string.Empty;
        public string ToUnitSymbol { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }

    public class AppSettings
    {
        public AppTheme Theme { get; set; } = AppTheme.Dark;
        public UnitCategoryType DefaultCategory { get; set; } = UnitCategoryType.Length;
        public PrecisionMode PrecisionMode { get; set; } = PrecisionMode.Auto;
        public int CustomPrecision { get; set; } = 4;
        public DataStorageSystem DataStorageSystem { get; set; } = DataStorageSystem.Decimal;
        public string DecimalSeparator { get; set; } = ".";
        public string ThousandsSeparator { get; set; } = ",";
        public bool AutoSaveHistory { get; set; } = true;
        public string Language { get; set; } = "en";
    }
}
