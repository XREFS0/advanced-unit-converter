using System.Globalization;
using AdvancedUnitConverter.Core.Enums;

namespace AdvancedUnitConverter.Application.Formatters
{
    public static class PrecisionFormatter
    {
        public static string Format(double value, PrecisionMode mode, int customPrecision = 4, string decimalSep = ".", string thousandsSep = ",")
        {
            if (double.IsNaN(value)) return "NaN";
            if (double.IsPositiveInfinity(value)) return "Infinity";
            if (double.IsNegativeInfinity(value)) return "-Infinity";

            // If value is 0
            if (Math.Abs(value) < 1e-15) return "0";

            // Check if scientific notation is explicitly required or suitable for very large / very small
            double absVal = Math.Abs(value);
            bool useScientific = mode == PrecisionMode.Scientific || (mode == PrecisionMode.Auto && (absVal >= 1e15 || absVal < 1e-7));

            if (useScientific)
            {
                int decimals = mode == PrecisionMode.Custom ? Math.Clamp(customPrecision, 0, 15) : 6;
                string sciStr = value.ToString($"E{decimals}", CultureInfo.InvariantCulture);
                return ApplySeparators(sciStr, decimalSep, thousandsSep);
            }

            int precisionDigits = mode switch
            {
                PrecisionMode.TwoDecimals => 2,
                PrecisionMode.FourDecimals => 4,
                PrecisionMode.SixDecimals => 6,
                PrecisionMode.TenDecimals => 10,
                PrecisionMode.Custom => Math.Clamp(customPrecision, 0, 15),
                _ => 8 // Auto fallback
            };

            // Format with maximum precision, then trim trailing zeroes if Auto or standard clean display
            string formatted = value.ToString($"N{precisionDigits}", CultureInfo.InvariantCulture);

            if (mode == PrecisionMode.Auto)
            {
                // Auto mode trims redundant zeroes
                if (formatted.Contains('.'))
                {
                    formatted = formatted.TrimEnd('0').TrimEnd('.');
                }
            }

            return ApplySeparators(formatted, decimalSep, thousandsSep);
        }

        private static string ApplySeparators(string input, string decimalSep, string thousandsSep)
        {
            if (decimalSep == "." && (thousandsSep == "," || string.IsNullOrEmpty(thousandsSep)))
            {
                return input;
            }

            // Replace standard dot and comma with configured separators
            string placeholder = "___DEC___";
            string res = input.Replace(".", placeholder);
            res = res.Replace(",", thousandsSep);
            res = res.Replace(placeholder, decimalSep);
            return res;
        }
    }
}
