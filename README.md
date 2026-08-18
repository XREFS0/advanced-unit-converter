# Advanced Unit Converter (C# / .NET 8 / WPF / MVVM)

[![.NET 8.0](https://img.shields.io/badge/.NET-8.0-blue.svg)](https://dotnet.microsoft.com/download/dotnet/8.0)
[![Clean Architecture](https://img.shields.io/badge/Architecture-Clean%20Architecture-brightgreen.svg)](https://github.com/XREFS0/advanced-unit-converter)
[![MVVM](https://img.shields.io/badge/Pattern-MVVM-orange.svg)](https://github.com/XREFS0/advanced-unit-converter)
[![SQLite](https://img.shields.io/badge/Database-SQLite-003B57.svg)](https://github.com/XREFS0/advanced-unit-converter)
[![Build & Tests](https://img.shields.io/badge/Tests-21%20Passed-success.svg)](https://github.com/XREFS0/advanced-unit-converter)

**Advanced Unit Converter** is a desktop application built with C#, .NET 8 (WPF), and Clean Architecture. It features decoupled conversion engines, offline SQLite persistence, customizable precision controls, and a modern Windows Fluent interface.

---

## Application Screenshots

### Converter Screen (Dark & Light Theme)
<p align="center">
  <img src="src/converter_dark.png" alt="Converter Dark Theme" width="48%" />
  <img src="src/converter_light.png" alt="Converter Light Theme" width="48%" />
</p>

### Quick Conversions & Settings
<p align="center">
  <img src="src/quick_conversions.png" alt="Quick Conversions" width="48%" />
  <img src="src/settings_view.png" alt="Settings View" width="48%" />
</p>

---

## Key Features

- **14+ Measurement Categories**:
  - **Length**: Millimeter, Centimeter, Meter, Kilometer, Inch, Foot, Yard, Mile, Nautical Mile, Micrometer, Nanometer.
  - **Weight / Mass**: Milligram, Gram, Kilogram, Metric Ton, Ounce, Pound, Stone, Carat.
  - **Temperature**: Conversion between Celsius (°C), Fahrenheit (°F), Kelvin (K), and Rankine (°R).
  - **Area**: Square Millimeter, Square Centimeter, Square Meter, Square Kilometer, Hectare, Acre, Square Inch, Square Foot, Square Yard, Square Mile.
  - **Volume**: Milliliter, Liter, Cubic Meter, Cubic Centimeter, US Gallon, US Quart, US Pint, US Cup, US Fluid Ounce, Imperial Gallon.
  - **Speed**: Meters per Second, Kilometers per Hour, Miles per Hour, Knots, Feet per Second.
  - **Time**: Millisecond, Second, Minute, Hour, Day, Week, Month, Year.
  - **Data Storage**: Bit, Byte, KB/KiB, MB/MiB, GB/GiB, TB/TiB, PB/PiB (Supports Binary 1024 and Decimal 1000 standards).
  - **Energy**: Joule, Kilojoule, Calorie, Kilocalorie, Watt-hour, Kilowatt-hour, Electronvolt, BTU.
  - **Power**: Watt, Kilowatt, Megawatt, Horsepower, Metric Horsepower.
  - **Pressure**: Pascal, Kilopascal, Bar, Atmosphere, PSI, Torr / mmHg.
  - **Angle**: Degree, Radian, Gradian, Arcminute, Arcsecond.
  - **Frequency**: Hertz, Kilohertz, Megahertz, Gigahertz.
  - **Fuel Consumption**: Reciprocal conversion between L/100km, km/L, MPG (US), and MPG (UK).

- **Instant Live Conversions**: Calculates in real-time as values are entered, with one-click unit swapping.
- **Precision Management**:
  - Auto-trimming trailing zeros.
  - 2, 4, 6, 10 decimal digits or custom precision.
  - Automatic and scientific notation support for very large and small numbers.
- **History System (SQLite + Dapper)**:
  - Persistent local calculation records.
  - Category filtering, real-time search, reuse conversion, single/batch deletion, and clear all.
- **Favorites**: Save frequent unit pairs for fast access.
- **Quick Conversions Hub**: Ready-to-use cards for everyday unit conversions.
- **Global Search (`Ctrl + F`)**: Instant search across all categories, units, and symbols.
- **Modern Fluent Interface**: Dark and Light themes with instant switching.
- **Keyboard Shortcuts**:
  - `Ctrl + C`: Copy result.
  - `Ctrl + R`: Reset values and units.
  - `Ctrl + S`: Toggle Favorite.
  - `Ctrl + F`: Global Search.
  - `Ctrl + H`: History view.
  - `Ctrl + ,`: Settings view.

---

## Architecture Overview

The solution follows Clean Architecture and SOLID principles:

```
AdvancedUnitConverter/
│
├── AdvancedUnitConverter.Core/              # Domain Layer (Enums, Models, Interfaces)
├── AdvancedUnitConverter.Application/       # Application Layer (Converters, Engine, Formatters)
├── AdvancedUnitConverter.Infrastructure/    # Infrastructure Layer (SQLite Database, Repositories, Settings, Logging)
├── AdvancedUnitConverter.UI/                # Presentation Layer (WPF, ViewModels, Views, Themes, Controls)
└── AdvancedUnitConverter.Tests/             # Unit Tests (xUnit test suite)
```

---

## Getting Started

### Prerequisites
- .NET 8.0 SDK or newer.
- Windows 10/11.

### Build and Run
```bash
# Clone the repository
git clone https://github.com/XREFS0/advanced-unit-converter.git
cd advanced-unit-converter

# Restore dependencies
dotnet restore

# Run Unit Tests
dotnet test

# Launch the Application
dotnet run --project AdvancedUnitConverter.UI
```

---

## Unit Tests

Run the full test suite:
```bash
dotnet test
```

Test coverage includes:
- Linear scale factor conversions across physical units.
- Non-linear temperature calculations.
- Reciprocal fuel consumption formulas.
- Data storage binary (1024) vs decimal (1000) conversions.
- Global unit and symbol searching.

---

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

Copyright (c) 2026 XREFS0.
