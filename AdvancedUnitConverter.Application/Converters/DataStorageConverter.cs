using AdvancedUnitConverter.Application.Formatters;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;

namespace AdvancedUnitConverter.Application.Converters
{
    public class DataStorageConverter : IUnitConverter
    {
        public UnitCategoryType CategoryType => UnitCategoryType.DataStorage;

        // Base unit: Byte (1 byte = 8 bits)
        public ConversionResult Convert(double value, UnitDefinition fromUnit, UnitDefinition toUnit, ConversionOptions? options = null)
        {
            if (fromUnit == null || toUnit == null)
                return ConversionResult.Fail("Invalid unit definition.");

            var opt = options ?? new ConversionOptions();
            double multiplier = opt.DataStorageSystem == DataStorageSystem.Binary ? 1024.0 : 1000.0;

            try
            {
                // Convert fromUnit to Bytes
                double bytes = ToBytes(value, fromUnit.Id, multiplier);
                // Convert Bytes to toUnit
                double targetValue = FromBytes(bytes, toUnit.Id, multiplier);

                string formatted = PrecisionFormatter.Format(targetValue, opt.PrecisionMode, opt.CustomPrecision);
                string standard = opt.DataStorageSystem == DataStorageSystem.Binary ? "Binary (1024)" : "Decimal (1000)";
                string formula = $"{fromUnit.Symbol} → {toUnit.Symbol} [{standard}]";

                return ConversionResult.Success(targetValue, formatted, formula);
            }
            catch (Exception ex)
            {
                return ConversionResult.Fail($"Data storage conversion error: {ex.Message}");
            }
        }

        private static double ToBytes(double value, string unitId, double multiplier)
        {
            string u = unitId.ToLowerInvariant();
            return u switch
            {
                "bit" or "b_bit" => value / 8.0,
                "byte" or "b" => value,
                "kb" or "kib" or "kilobyte" => value * multiplier,
                "mb" or "mib" or "megabyte" => value * Math.Pow(multiplier, 2),
                "gb" or "gib" or "gigabyte" => value * Math.Pow(multiplier, 3),
                "tb" or "tib" or "terabyte" => value * Math.Pow(multiplier, 4),
                "pb" or "pib" or "petabyte" => value * Math.Pow(multiplier, 5),
                _ => value
            };
        }

        private static double FromBytes(double bytes, string unitId, double multiplier)
        {
            string u = unitId.ToLowerInvariant();
            return u switch
            {
                "bit" or "b_bit" => bytes * 8.0,
                "byte" or "b" => bytes,
                "kb" or "kib" or "kilobyte" => bytes / multiplier,
                "mb" or "mib" or "megabyte" => bytes / Math.Pow(multiplier, 2),
                "gb" or "gib" or "gigabyte" => bytes / Math.Pow(multiplier, 3),
                "tb" or "tib" or "terabyte" => bytes / Math.Pow(multiplier, 4),
                "pb" or "pib" or "petabyte" => bytes / Math.Pow(multiplier, 5),
                _ => bytes
            };
        }
    }
}
