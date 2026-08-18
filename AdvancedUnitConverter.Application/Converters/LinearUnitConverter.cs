using AdvancedUnitConverter.Application.Formatters;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;

namespace AdvancedUnitConverter.Application.Converters
{
    public class LinearUnitConverter : IUnitConverter
    {
        public UnitCategoryType CategoryType { get; }

        public LinearUnitConverter(UnitCategoryType categoryType)
        {
            CategoryType = categoryType;
        }

        public virtual ConversionResult Convert(double value, UnitDefinition fromUnit, UnitDefinition toUnit, ConversionOptions? options = null)
        {
            if (fromUnit == null || toUnit == null)
                return ConversionResult.Fail("Invalid unit definition.");

            if (fromUnit.Category != CategoryType || toUnit.Category != CategoryType)
                return ConversionResult.Fail($"Unit category mismatch: expected {CategoryType}.");

            try
            {
                // Value in Base Unit = Value * fromUnit.FactorToBase
                double valueInBase = value * fromUnit.FactorToBase;
                // Target Value = Value in Base / toUnit.FactorToBase
                double targetValue = valueInBase / toUnit.FactorToBase;

                var opt = options ?? new ConversionOptions();
                string formatted = PrecisionFormatter.Format(targetValue, opt.PrecisionMode, opt.CustomPrecision);
                string formula = fromUnit.Id == toUnit.Id
                    ? $"{value} {fromUnit.Symbol} = {formatted} {toUnit.Symbol}"
                    : $"1 {fromUnit.Symbol} = {(fromUnit.FactorToBase / toUnit.FactorToBase):G6} {toUnit.Symbol}";

                return ConversionResult.Success(targetValue, formatted, formula);
            }
            catch (Exception ex)
            {
                return ConversionResult.Fail($"Conversion error: {ex.Message}");
            }
        }
    }
}
