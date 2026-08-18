using System.Collections.ObjectModel;
using AdvancedUnitConverter.Core.Enums;
using AdvancedUnitConverter.Core.Interfaces;
using AdvancedUnitConverter.Core.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AdvancedUnitConverter.UI.ViewModels
{
    public class QuickConversionItem
    {
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public UnitCategoryType Category { get; set; }
        public string FromUnitId { get; set; } = string.Empty;
        public string ToUnitId { get; set; } = string.Empty;
        public double SampleValue { get; set; }
        public string SampleResult { get; set; } = string.Empty;
        public string IconKey { get; set; } = "ZapIcon";
    }

    public partial class QuickConversionViewModel : ObservableObject
    {
        private readonly IConversionEngine _engine;
        private readonly Action<UnitCategoryType, string, string, double?> _onOpenConversion;

        [ObservableProperty]
        private ObservableCollection<QuickConversionItem> _quickItems = new();

        public QuickConversionViewModel(
            IConversionEngine engine,
            Action<UnitCategoryType, string, string, double?> onOpenConversion)
        {
            _engine = engine;
            _onOpenConversion = onOpenConversion;
            LoadQuickConversions();
        }

        private void LoadQuickConversions()
        {
            var presets = new List<(string Title, string Desc, UnitCategoryType Cat, string From, string To, double Val, string Icon)>
            {
                ("Celsius to Fahrenheit", "Standard body / weather temperature", UnitCategoryType.Temperature, "celsius", "fahrenheit", 25, "ThermometerIcon"),
                ("Kilometers to Miles", "Road distances & speeds", UnitCategoryType.Length, "km", "mi", 100, "RulerIcon"),
                ("Kilograms to Pounds", "Body weight & groceries", UnitCategoryType.Weight, "kg", "lb", 70, "ScaleIcon"),
                ("Meters to Feet", "Height & room dimensions", UnitCategoryType.Length, "m", "ft", 1.8, "RulerIcon"),
                ("Liters to Gallons (US)", "Fuel & liquid volume", UnitCategoryType.Volume, "l", "gal_us", 50, "CubeIcon"),
                ("km/h to mph", "Driving velocity limit", UnitCategoryType.Speed, "kph", "mph", 120, "SpeedometerIcon"),
                ("Gigabytes to Megabytes", "File storage capacity", UnitCategoryType.DataStorage, "gb", "mb", 16, "HardDriveIcon"),
                ("Kilowatt-hours to Joules", "Electrical energy consumption", UnitCategoryType.Energy, "kwh", "kj", 1, "LightningIcon"),
                ("Bar to PSI", "Tire pressure measurement", UnitCategoryType.Pressure, "bar", "psi", 2.2, "GaugeIcon"),
                ("L/100km to MPG (US)", "Car fuel economy", UnitCategoryType.FuelConsumption, "lp100km", "mpg_us", 7.5, "FuelIcon")
            };

            var list = new List<QuickConversionItem>();
            foreach (var p in presets)
            {
                var res = _engine.Convert(p.Cat, p.From, p.To, p.Val);
                list.Add(new QuickConversionItem
                {
                    Title = p.Title,
                    Description = p.Desc,
                    Category = p.Cat,
                    FromUnitId = p.From,
                    ToUnitId = p.To,
                    SampleValue = p.Val,
                    SampleResult = res.IsSuccess ? $"{p.Val} → {res.FormattedValue}" : string.Empty,
                    IconKey = p.Icon
                });
            }

            QuickItems = new ObservableCollection<QuickConversionItem>(list);
        }

        [RelayCommand]
        private void LaunchConversion(QuickConversionItem? item)
        {
            if (item == null) return;
            _onOpenConversion(item.Category, item.FromUnitId, item.ToUnitId, item.SampleValue);
        }
    }
}
