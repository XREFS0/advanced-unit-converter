using AdvancedUnitConverter.Application.Formatters;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;

namespace AdvancedUnitConverter.Application.Converters
{
    public class FuelConsumptionConverter : IUnitConverter
    {
        public UnitCategoryType CategoryType => UnitCategoryType.FuelConsumption;

        // Base unit: Liters per 100 km (L/100km)
        public ConversionResult Convert(double value, UnitDefinition fromUnit, UnitDefinition toUnit, ConversionOptions? options = null)
        {
            if (fromUnit == null || toUnit == null)
                return ConversionResult.Fail("Invalid unit definition.");

            if (value <= 0)
            {
                if (value == 0)
                {
                    return ConversionResult.Success(0, "0", "0 fuel consumption");
                }
                return ConversionResult.Fail("Fuel consumption value must be positive.");
            }

            try
            {
                // Convert from source to base: L/100km
                double lp100km = fromUnit.Id.ToLowerInvariant() switch
                {
                    "lp100km" or "l_per_100km" => value,
                    "kml" or "km_per_l" => 100.0 / value,
                    "mpg_us" or "mpg" => 235.214583 / value,
                    "mpg_imp" or "mpg_uk" => 282.481 / value,
                    _ => throw new NotSupportedException($"Unsupported fuel unit: {fromUnit.Id}")
                };

                // Convert from L/100km to target
                double targetValue = toUnit.Id.ToLowerInvariant() switch
                {
                    "lp100km" or "l_per_100km" => lp100km,
                    "kml" or "km_per_l" => 100.0 / lp100km,
                    "mpg_us" or "mpg" => 235.214583 / lp100km,
                    "mpg_imp" or "mpg_uk" => 282.481 / lp100km,
                    _ => throw new NotSupportedException($"Unsupported fuel unit: {toUnit.Id}")
                };

                var opt = options ?? new ConversionOptions();
                string formatted = PrecisionFormatter.Format(targetValue, opt.PrecisionMode, opt.CustomPrecision);
                string formula = $"{fromUnit.Symbol} → {toUnit.Symbol} (Reciprocal Formula)";

                return ConversionResult.Success(targetValue, formatted, formula);
            }
            catch (Exception ex)
            {
                return ConversionResult.Fail($"Fuel consumption conversion error: {ex.Message}");
            }
        }
    }
}
