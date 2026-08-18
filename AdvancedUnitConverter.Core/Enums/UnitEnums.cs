namespace AdvancedUnitConverter.Core.Enums
{
    public enum UnitCategoryType
    {
        Length,
        Weight,
        Temperature,
        Area,
        Volume,
        Speed,
        Time,
        DataStorage,
        Energy,
        Power,
        Pressure,
        Angle,
        Frequency,
        FuelConsumption
    }

    public enum DataStorageSystem
    {
        Binary,  // 1024 (KiB, MiB, GiB, TiB, PiB)
        Decimal  // 1000 (KB, MB, GB, TB, PB)
    }

    public enum AppTheme
    {
        Dark,
        Light,
        System
    }

    public enum PrecisionMode
    {
        Auto,
        TwoDecimals,
        FourDecimals,
        SixDecimals,
        TenDecimals,
        Custom,
        Scientific
    }
}
