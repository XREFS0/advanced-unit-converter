using AdvancedUnitConverter.Application.Engine;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using Xunit;

namespace AdvancedUnitConverter.Tests
{
    public class EngineTests
    {
        private readonly IConversionEngine _engine = new ConversionEngine();

        [Theory]
        [InlineData(1000, "m", "km", 1.0)]
        [InlineData(1, "km", "m", 1000.0)]
        [InlineData(1, "in", "cm", 2.54)]
        [InlineData(1, "mi", "m", 1609.344)]
        [InlineData(1, "ft", "in", 12.0)]
        public void LengthConversions_AreAccurate(double val, string from, string to, double expected)
        {
            var res = _engine.Convert(UnitCategoryType.Length, from, to, val);
            Assert.True(res.IsSuccess);
            Assert.Equal(expected, res.Value, 4);
        }

        [Theory]
        [InlineData(1000, "g", "kg", 1.0)]
        [InlineData(1, "kg", "g", 1000.0)]
        [InlineData(1, "t", "kg", 1000.0)]
        [InlineData(1, "lb", "g", 453.59237)]
        [InlineData(1, "oz", "g", 28.349523)]
        public void WeightConversions_AreAccurate(double val, string from, string to, double expected)
        {
            var res = _engine.Convert(UnitCategoryType.Weight, from, to, val);
            Assert.True(res.IsSuccess);
            Assert.Equal(expected, res.Value, 4);
        }

        [Theory]
        [InlineData(0, "celsius", "fahrenheit", 32.0)]
        [InlineData(100, "celsius", "fahrenheit", 212.0)]
        [InlineData(32, "fahrenheit", "celsius", 0.0)]
        [InlineData(212, "fahrenheit", "celsius", 100.0)]
        [InlineData(0, "celsius", "kelvin", 273.15)]
        [InlineData(273.15, "kelvin", "celsius", 0.0)]
        public void TemperatureConversions_AreAccurate(double val, string from, string to, double expected)
        {
            var res = _engine.Convert(UnitCategoryType.Temperature, from, to, val);
            Assert.True(res.IsSuccess);
            Assert.Equal(expected, res.Value, 2);
        }

        [Theory]
        [InlineData(8, "lp100km", "kml", 12.5)]
        [InlineData(10, "kml", "lp100km", 10.0)]
        [InlineData(235.214583, "mpg_us", "lp100km", 1.0)]
        public void FuelConversions_AreAccurate(double val, string from, string to, double expected)
        {
            var res = _engine.Convert(UnitCategoryType.FuelConsumption, from, to, val);
            Assert.True(res.IsSuccess);
            Assert.Equal(expected, res.Value, 2);
        }

        [Fact]
        public void DataStorage_BinaryVsDecimal_WorksCorrectly()
        {
            // Binary (1024)
            var binRes = _engine.Convert(UnitCategoryType.DataStorage, "kb", "byte", 1, new ConversionOptions
            {
                DataStorageSystem = DataStorageSystem.Binary
            });
            Assert.True(binRes.IsSuccess);
            Assert.Equal(1024.0, binRes.Value);

            // Decimal (1000)
            var decRes = _engine.Convert(UnitCategoryType.DataStorage, "kb", "byte", 1, new ConversionOptions
            {
                DataStorageSystem = DataStorageSystem.Decimal
            });
            Assert.True(decRes.IsSuccess);
            Assert.Equal(1000.0, decRes.Value);
        }

        [Fact]
        public void GlobalSearch_FindsUnits()
        {
            var results = _engine.SearchUnits("celsius");
            Assert.NotEmpty(results);
            Assert.Contains(results, u => u.Id == "celsius");

            var kgResults = _engine.SearchUnits("kg");
            Assert.NotEmpty(kgResults);
            Assert.Contains(kgResults, u => u.Symbol == "kg");
        }
    }
}
