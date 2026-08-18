using AdvancedUnitConverter.Application.Converters;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;

namespace AdvancedUnitConverter.Application.Engine
{
    public class ConversionEngine : IConversionEngine
    {
        private readonly Dictionary<UnitCategoryType, IUnitConverter> _converters = new();
        private readonly Dictionary<UnitCategoryType, ConversionCategory> _categories = new();

        public ConversionEngine()
        {
            InitializeDefaultConverters();
            InitializeCategories();
        }

        public IReadOnlyList<ConversionCategory> GetAllCategories() => _categories.Values.ToList();

        public ConversionCategory? GetCategory(UnitCategoryType type) =>
            _categories.TryGetValue(type, out var cat) ? cat : null;

        public void RegisterConverter(IUnitConverter converter)
        {
            _converters[converter.CategoryType] = converter;
        }

        public void RegisterCategory(ConversionCategory category)
        {
            _categories[category.Type] = category;
        }

        public ConversionResult Convert(UnitCategoryType category, string fromUnitId, string toUnitId, double value, ConversionOptions? options = null)
        {
            if (!_categories.TryGetValue(category, out var cat))
                return ConversionResult.Fail($"Category {category} not found.");

            var fromUnit = cat.Units.FirstOrDefault(u => string.Equals(u.Id, fromUnitId, StringComparison.OrdinalIgnoreCase));
            var toUnit = cat.Units.FirstOrDefault(u => string.Equals(u.Id, toUnitId, StringComparison.OrdinalIgnoreCase));

            if (fromUnit == null)
                return ConversionResult.Fail($"From-unit '{fromUnitId}' not found in category {category}.");
            if (toUnit == null)
                return ConversionResult.Fail($"To-unit '{toUnitId}' not found in category {category}.");

            if (!_converters.TryGetValue(category, out var converter))
                return ConversionResult.Fail($"No converter registered for category {category}.");

            return converter.Convert(value, fromUnit, toUnit, options);
        }

        public IEnumerable<UnitDefinition> SearchUnits(string query)
        {
            if (string.IsNullOrWhiteSpace(query))
                return Enumerable.Empty<UnitDefinition>();

            string q = query.Trim().ToLowerInvariant();
            var results = new List<UnitDefinition>();

            foreach (var cat in _categories.Values)
            {
                foreach (var unit in cat.Units)
                {
                    if (unit.Name.ToLowerInvariant().Contains(q) ||
                        unit.Symbol.ToLowerInvariant().Contains(q) ||
                        unit.Id.ToLowerInvariant().Contains(q) ||
                        cat.Name.ToLowerInvariant().Contains(q))
                    {
                        results.Add(unit);
                    }
                }
            }

            return results;
        }

        private void InitializeDefaultConverters()
        {
            // Linear converters
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Length));
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Weight));
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Area));
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Volume));
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Speed));
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Time));
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Energy));
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Power));
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Pressure));
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Angle));
            RegisterConverter(new LinearUnitConverter(UnitCategoryType.Frequency));

            // Specialized non-linear converters
            RegisterConverter(new TemperatureConverter());
            RegisterConverter(new FuelConsumptionConverter());
            RegisterConverter(new DataStorageConverter());
        }

        private void InitializeCategories()
        {
            // 1. Length (Base: Meter)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Length,
                Name = "Length",
                Description = "Distance and length measurements",
                IconKey = "RulerIcon",
                BaseUnitId = "m",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "mm", Name = "Millimeter", Symbol = "mm", Category = UnitCategoryType.Length, FactorToBase = 0.001 },
                    new() { Id = "cm", Name = "Centimeter", Symbol = "cm", Category = UnitCategoryType.Length, FactorToBase = 0.01 },
                    new() { Id = "m", Name = "Meter", Symbol = "m", Category = UnitCategoryType.Length, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "km", Name = "Kilometer", Symbol = "km", Category = UnitCategoryType.Length, FactorToBase = 1000.0 },
                    new() { Id = "in", Name = "Inch", Symbol = "in", Category = UnitCategoryType.Length, FactorToBase = 0.0254 },
                    new() { Id = "ft", Name = "Foot", Symbol = "ft", Category = UnitCategoryType.Length, FactorToBase = 0.3048 },
                    new() { Id = "yd", Name = "Yard", Symbol = "yd", Category = UnitCategoryType.Length, FactorToBase = 0.9144 },
                    new() { Id = "mi", Name = "Mile", Symbol = "mi", Category = UnitCategoryType.Length, FactorToBase = 1609.344 },
                    new() { Id = "nmi", Name = "Nautical Mile", Symbol = "nmi", Category = UnitCategoryType.Length, FactorToBase = 1852.0 },
                    new() { Id = "um", Name = "Micrometer", Symbol = "µm", Category = UnitCategoryType.Length, FactorToBase = 1e-6 },
                    new() { Id = "nm", Name = "Nanometer", Symbol = "nm", Category = UnitCategoryType.Length, FactorToBase = 1e-9 }
                }
            });

            // 2. Weight / Mass (Base: Kilogram)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Weight,
                Name = "Weight & Mass",
                Description = "Mass and weight measurements",
                IconKey = "ScaleIcon",
                BaseUnitId = "kg",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "mg", Name = "Milligram", Symbol = "mg", Category = UnitCategoryType.Weight, FactorToBase = 1e-6 },
                    new() { Id = "g", Name = "Gram", Symbol = "g", Category = UnitCategoryType.Weight, FactorToBase = 0.001 },
                    new() { Id = "kg", Name = "Kilogram", Symbol = "kg", Category = UnitCategoryType.Weight, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "t", Name = "Metric Ton", Symbol = "t", Category = UnitCategoryType.Weight, FactorToBase = 1000.0 },
                    new() { Id = "oz", Name = "Ounce", Symbol = "oz", Category = UnitCategoryType.Weight, FactorToBase = 0.028349523125 },
                    new() { Id = "lb", Name = "Pound", Symbol = "lb", Category = UnitCategoryType.Weight, FactorToBase = 0.45359237 },
                    new() { Id = "st", Name = "Stone", Symbol = "st", Category = UnitCategoryType.Weight, FactorToBase = 6.35029318 },
                    new() { Id = "carat", Name = "Carat", Symbol = "ct", Category = UnitCategoryType.Weight, FactorToBase = 0.0002 }
                }
            });

            // 3. Temperature (Base: Celsius)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Temperature,
                Name = "Temperature",
                Description = "Thermal and temperature scales",
                IconKey = "ThermometerIcon",
                BaseUnitId = "celsius",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "celsius", Name = "Celsius", Symbol = "°C", Category = UnitCategoryType.Temperature, IsBaseUnit = true },
                    new() { Id = "fahrenheit", Name = "Fahrenheit", Symbol = "°F", Category = UnitCategoryType.Temperature },
                    new() { Id = "kelvin", Name = "Kelvin", Symbol = "K", Category = UnitCategoryType.Temperature },
                    new() { Id = "rankine", Name = "Rankine", Symbol = "°R", Category = UnitCategoryType.Temperature }
                }
            });

            // 4. Area (Base: Square Meter)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Area,
                Name = "Area",
                Description = "Surface and area measurements",
                IconKey = "AreaIcon",
                BaseUnitId = "sq_m",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "sq_mm", Name = "Square Millimeter", Symbol = "mm²", Category = UnitCategoryType.Area, FactorToBase = 1e-6 },
                    new() { Id = "sq_cm", Name = "Square Centimeter", Symbol = "cm²", Category = UnitCategoryType.Area, FactorToBase = 1e-4 },
                    new() { Id = "sq_m", Name = "Square Meter", Symbol = "m²", Category = UnitCategoryType.Area, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "sq_km", Name = "Square Kilometer", Symbol = "km²", Category = UnitCategoryType.Area, FactorToBase = 1e6 },
                    new() { Id = "ha", Name = "Hectare", Symbol = "ha", Category = UnitCategoryType.Area, FactorToBase = 10000.0 },
                    new() { Id = "ac", Name = "Acre", Symbol = "ac", Category = UnitCategoryType.Area, FactorToBase = 4046.8564224 },
                    new() { Id = "sq_in", Name = "Square Inch", Symbol = "in²", Category = UnitCategoryType.Area, FactorToBase = 0.00064516 },
                    new() { Id = "sq_ft", Name = "Square Foot", Symbol = "ft²", Category = UnitCategoryType.Area, FactorToBase = 0.09290304 },
                    new() { Id = "sq_yd", Name = "Square Yard", Symbol = "yd²", Category = UnitCategoryType.Area, FactorToBase = 0.83612736 },
                    new() { Id = "sq_mi", Name = "Square Mile", Symbol = "mi²", Category = UnitCategoryType.Area, FactorToBase = 2589988.110336 }
                }
            });

            // 5. Volume (Base: Liter)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Volume,
                Name = "Volume",
                Description = "Liquid and 3D space measurements",
                IconKey = "CubeIcon",
                BaseUnitId = "l",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "ml", Name = "Milliliter", Symbol = "mL", Category = UnitCategoryType.Volume, FactorToBase = 0.001 },
                    new() { Id = "l", Name = "Liter", Symbol = "L", Category = UnitCategoryType.Volume, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "cu_m", Name = "Cubic Meter", Symbol = "m³", Category = UnitCategoryType.Volume, FactorToBase = 1000.0 },
                    new() { Id = "cu_cm", Name = "Cubic Centimeter", Symbol = "cm³", Category = UnitCategoryType.Volume, FactorToBase = 0.001 },
                    new() { Id = "gal_us", Name = "US Gallon", Symbol = "gal", Category = UnitCategoryType.Volume, FactorToBase = 3.785411784 },
                    new() { Id = "qt_us", Name = "US Quart", Symbol = "qt", Category = UnitCategoryType.Volume, FactorToBase = 0.946352946 },
                    new() { Id = "pt_us", Name = "US Pint", Symbol = "pt", Category = UnitCategoryType.Volume, FactorToBase = 0.473176473 },
                    new() { Id = "cup_us", Name = "US Cup", Symbol = "cup", Category = UnitCategoryType.Volume, FactorToBase = 0.2365882365 },
                    new() { Id = "floz_us", Name = "US Fluid Ounce", Symbol = "fl oz", Category = UnitCategoryType.Volume, FactorToBase = 0.0295735295625 },
                    new() { Id = "gal_uk", Name = "Imperial Gallon", Symbol = "imp gal", Category = UnitCategoryType.Volume, FactorToBase = 4.54609 }
                }
            });

            // 6. Speed (Base: Meter per second)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Speed,
                Name = "Speed",
                Description = "Velocity and speed rates",
                IconKey = "SpeedometerIcon",
                BaseUnitId = "mps",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "mps", Name = "Meters per Second", Symbol = "m/s", Category = UnitCategoryType.Speed, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "kph", Name = "Kilometers per Hour", Symbol = "km/h", Category = UnitCategoryType.Speed, FactorToBase = 1.0 / 3.6 },
                    new() { Id = "mph", Name = "Miles per Hour", Symbol = "mph", Category = UnitCategoryType.Speed, FactorToBase = 0.44704 },
                    new() { Id = "knot", Name = "Knots", Symbol = "kn", Category = UnitCategoryType.Speed, FactorToBase = 0.51444444444 },
                    new() { Id = "fps", Name = "Feet per Second", Symbol = "ft/s", Category = UnitCategoryType.Speed, FactorToBase = 0.3048 }
                }
            });

            // 7. Time (Base: Second)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Time,
                Name = "Time",
                Description = "Temporal duration units",
                IconKey = "ClockIcon",
                BaseUnitId = "s",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "ms", Name = "Millisecond", Symbol = "ms", Category = UnitCategoryType.Time, FactorToBase = 0.001 },
                    new() { Id = "s", Name = "Second", Symbol = "s", Category = UnitCategoryType.Time, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "min", Name = "Minute", Symbol = "min", Category = UnitCategoryType.Time, FactorToBase = 60.0 },
                    new() { Id = "h", Name = "Hour", Symbol = "h", Category = UnitCategoryType.Time, FactorToBase = 3600.0 },
                    new() { Id = "d", Name = "Day", Symbol = "d", Category = UnitCategoryType.Time, FactorToBase = 86400.0 },
                    new() { Id = "wk", Name = "Week", Symbol = "wk", Category = UnitCategoryType.Time, FactorToBase = 604800.0 },
                    new() { Id = "mo", Name = "Month (30d)", Symbol = "mo", Category = UnitCategoryType.Time, FactorToBase = 2592000.0 },
                    new() { Id = "yr", Name = "Year (365.25d)", Symbol = "yr", Category = UnitCategoryType.Time, FactorToBase = 31557600.0 }
                }
            });

            // 8. Data Storage (Base: Byte)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.DataStorage,
                Name = "Data Storage",
                Description = "Digital storage capacity",
                IconKey = "HardDriveIcon",
                BaseUnitId = "byte",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "bit", Name = "Bit", Symbol = "b", Category = UnitCategoryType.DataStorage },
                    new() { Id = "byte", Name = "Byte", Symbol = "B", Category = UnitCategoryType.DataStorage, IsBaseUnit = true },
                    new() { Id = "kb", Name = "Kilobyte", Symbol = "KB", Category = UnitCategoryType.DataStorage },
                    new() { Id = "mb", Name = "Megabyte", Symbol = "MB", Category = UnitCategoryType.DataStorage },
                    new() { Id = "gb", Name = "Gigabyte", Symbol = "GB", Category = UnitCategoryType.DataStorage },
                    new() { Id = "tb", Name = "Terabyte", Symbol = "TB", Category = UnitCategoryType.DataStorage },
                    new() { Id = "pb", Name = "Petabyte", Symbol = "PB", Category = UnitCategoryType.DataStorage }
                }
            });

            // 9. Energy (Base: Joule)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Energy,
                Name = "Energy",
                Description = "Work and energy units",
                IconKey = "LightningIcon",
                BaseUnitId = "j",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "j", Name = "Joule", Symbol = "J", Category = UnitCategoryType.Energy, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "kj", Name = "Kilojoule", Symbol = "kJ", Category = UnitCategoryType.Energy, FactorToBase = 1000.0 },
                    new() { Id = "cal", Name = "Calorie", Symbol = "cal", Category = UnitCategoryType.Energy, FactorToBase = 4.184 },
                    new() { Id = "kcal", Name = "Kilocalorie", Symbol = "kcal", Category = UnitCategoryType.Energy, FactorToBase = 4184.0 },
                    new() { Id = "wh", Name = "Watt Hour", Symbol = "Wh", Category = UnitCategoryType.Energy, FactorToBase = 3600.0 },
                    new() { Id = "kwh", Name = "Kilowatt Hour", Symbol = "kWh", Category = UnitCategoryType.Energy, FactorToBase = 3.6e6 },
                    new() { Id = "ev", Name = "Electronvolt", Symbol = "eV", Category = UnitCategoryType.Energy, FactorToBase = 1.602176634e-19 },
                    new() { Id = "btu", Name = "British Thermal Unit", Symbol = "BTU", Category = UnitCategoryType.Energy, FactorToBase = 1055.06 }
                }
            });

            // 10. Power (Base: Watt)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Power,
                Name = "Power",
                Description = "Power and mechanical output",
                IconKey = "PowerIcon",
                BaseUnitId = "w",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "w", Name = "Watt", Symbol = "W", Category = UnitCategoryType.Power, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "kw", Name = "Kilowatt", Symbol = "kW", Category = UnitCategoryType.Power, FactorToBase = 1000.0 },
                    new() { Id = "mw", Name = "Megawatt", Symbol = "MW", Category = UnitCategoryType.Power, FactorToBase = 1e6 },
                    new() { Id = "hp", Name = "Horsepower (Mechanical)", Symbol = "hp", Category = UnitCategoryType.Power, FactorToBase = 745.699872 },
                    new() { Id = "hp_m", Name = "Metric Horsepower", Symbol = "PS", Category = UnitCategoryType.Power, FactorToBase = 735.49875 }
                }
            });

            // 11. Pressure (Base: Pascal)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Pressure,
                Name = "Pressure",
                Description = "Fluid and atmospheric pressure",
                IconKey = "GaugeIcon",
                BaseUnitId = "pa",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "pa", Name = "Pascal", Symbol = "Pa", Category = UnitCategoryType.Pressure, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "kpa", Name = "Kilopascal", Symbol = "kPa", Category = UnitCategoryType.Pressure, FactorToBase = 1000.0 },
                    new() { Id = "bar", Name = "Bar", Symbol = "bar", Category = UnitCategoryType.Pressure, FactorToBase = 100000.0 },
                    new() { Id = "atm", Name = "Atmosphere", Symbol = "atm", Category = UnitCategoryType.Pressure, FactorToBase = 101325.0 },
                    new() { Id = "psi", Name = "Pounds per Sq Inch", Symbol = "psi", Category = UnitCategoryType.Pressure, FactorToBase = 6894.757293168 },
                    new() { Id = "torr", Name = "Torr / mmHg", Symbol = "Torr", Category = UnitCategoryType.Pressure, FactorToBase = 133.322368421 }
                }
            });

            // 12. Angle (Base: Degree)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Angle,
                Name = "Angle",
                Description = "Geometric angular units",
                IconKey = "CompassIcon",
                BaseUnitId = "deg",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "deg", Name = "Degree", Symbol = "°", Category = UnitCategoryType.Angle, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "rad", Name = "Radian", Symbol = "rad", Category = UnitCategoryType.Angle, FactorToBase = 180.0 / Math.PI },
                    new() { Id = "grad", Name = "Gradian", Symbol = "grad", Category = UnitCategoryType.Angle, FactorToBase = 0.9 },
                    new() { Id = "arcmin", Name = "Arcminute", Symbol = "′", Category = UnitCategoryType.Angle, FactorToBase = 1.0 / 60.0 },
                    new() { Id = "arcsec", Name = "Arcsecond", Symbol = "″", Category = UnitCategoryType.Angle, FactorToBase = 1.0 / 3600.0 }
                }
            });

            // 13. Frequency (Base: Hertz)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.Frequency,
                Name = "Frequency",
                Description = "Periodic cycle frequencies",
                IconKey = "WaveIcon",
                BaseUnitId = "hz",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "hz", Name = "Hertz", Symbol = "Hz", Category = UnitCategoryType.Frequency, FactorToBase = 1.0, IsBaseUnit = true },
                    new() { Id = "khz", Name = "Kilohertz", Symbol = "kHz", Category = UnitCategoryType.Frequency, FactorToBase = 1000.0 },
                    new() { Id = "mhz", Name = "Megahertz", Symbol = "MHz", Category = UnitCategoryType.Frequency, FactorToBase = 1e6 },
                    new() { Id = "ghz", Name = "Gigahertz", Symbol = "GHz", Category = UnitCategoryType.Frequency, FactorToBase = 1e9 }
                }
            });

            // 14. Fuel Consumption (Base: L/100km)
            RegisterCategory(new ConversionCategory
            {
                Type = UnitCategoryType.FuelConsumption,
                Name = "Fuel Consumption",
                Description = "Automotive economy and efficiency",
                IconKey = "FuelIcon",
                BaseUnitId = "lp100km",
                Units = new List<UnitDefinition>
                {
                    new() { Id = "lp100km", Name = "Liters per 100 km", Symbol = "L/100km", Category = UnitCategoryType.FuelConsumption, IsBaseUnit = true },
                    new() { Id = "kml", Name = "Kilometers per Liter", Symbol = "km/L", Category = UnitCategoryType.FuelConsumption },
                    new() { Id = "mpg_us", Name = "Miles per Gallon (US)", Symbol = "MPG (US)", Category = UnitCategoryType.FuelConsumption },
                    new() { Id = "mpg_imp", Name = "Miles per Gallon (UK)", Symbol = "MPG (UK)", Category = UnitCategoryType.FuelConsumption }
                }
            });
        }
    }
}
