using System.Globalization;
using System.Windows.Data;

namespace AdvancedUnitConverter.UI.Converters
{
    public class CategoryMatchConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values == null || values.Length < 3) return false;
            string? activeView = values[0] as string;
            object? currentCat = values[1];
            object? itemCat = values[2];

            if (activeView != "Converter") return false;
            if (currentCat == null || itemCat == null) return false;

            return currentCat.Equals(itemCat);
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
