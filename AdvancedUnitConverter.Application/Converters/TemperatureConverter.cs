using AdvancedUnitConverter.Application.Formatters;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;

namespace AdvancedUnitConverter.Application.Converters
{
    public class TemperatureConverter : IUnitConverter
    {
        public UnitCategoryType CategoryType => UnitCategoryType.Temperature;

        public ConversionResult Convert(double value, UnitDefinition fromUnit, UnitDefinition toUnit, ConversionOptions? options = null)
        {
            if (fromUnit == null || toUnit == null)
                return ConversionResult.Fail("Invalid unit definition.");

            try
            {
                // Convert FromUnit to Celsius (Base)
                double celsius = fromUnit.Id.ToLowerInvariant() switch
                {
                    "celsius" or "c" => value,
                    "fahrenheit" or "f" => (value - 32.0) * 5.0 / 9.0,
                    "kelvin" or "k" => value - 273.15,
                    "rankine" or "r" => (value - 491.67) * 5.0 / 9.0,
                    _ => throw new NotSupportedException($"Unsupported temperature unit: {fromUnit.Id}")
                };

                // Convert Celsius to ToUnit
                double targetValue = toUnit.Id.ToLowerInvariant() switch
                {
                    "celsius" or "c" => celsius,
                    "fahrenheit" or "f" => (celsius * 9.0 / 5.0) + 32.0,
                    "kelvin" or "k" => celsius + 273.15,
                    "rankine" or "r" => (celsius + 273.15) * 9.0 / 5.0,
                    _ => throw new NotSupportedException($"Unsupported temperature unit: {toUnit.Id}")
                };

                var opt = options ?? new ConversionOptions();
                string formatted = PrecisionFormatter.Format(targetValue, opt.PrecisionMode, opt.CustomPrecision);
                string formula = GetFormulaDescription(fromUnit.Id, toUnit.Id);

                return ConversionResult.Success(targetValue, formatted, formula);
            }
            catch (Exception ex)
            {
                return ConversionResult.Fail($"Temperature conversion error: {ex.Message}");
            }
        }

        private static string GetFormulaDescription(string from, string to)
        {
            string f = from.ToLowerInvariant();
            string t = to.ToLowerInvariant();

            if (f == t) return "Same unit";
            if ((f == "celsius" || f == "c") && (t == "fahrenheit" || t == "f")) return "°F = (°C × 9/5) + 32";
            if ((f == "fahrenheit" || f == "f") && (t == "celsius" || t == "c")) return "°C = (°F − 32) × 5/9";
            if ((f == "celsius" || f == "c") && (t == "kelvin" || t == "k")) return "K = °C + 273.15";
            if ((f == "kelvin" || f == "k") && (t == "celsius" || t == "c")) return "°C = K − 273.15";
            if ((f == "fahrenheit" || f == "f") && (t == "kelvin" || t == "k")) return "K = (°F − 32) × 5/9 + 273.15";
            if ((f == "kelvin" || f == "k") && (t == "fahrenheit" || t == "f")) return "°F = (K − 273.15) × 9/5 + 32";

            return $"{from} → {to}";
        }
    }
}
