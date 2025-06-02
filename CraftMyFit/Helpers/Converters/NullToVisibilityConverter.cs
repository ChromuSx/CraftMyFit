using System.Globalization;

namespace CraftMyFit.Helpers.Converters
{
    public class NullToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            bool isNull = value == null;

            // Se il parametro è "invert", mostra quando è null invece che quando non è null
            bool invert = parameter?.ToString()?.ToLower() == "invert";

            return invert ? isNull : !isNull;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => throw new NotImplementedException("NullToVisibilityConverter non supporta la conversione inversa");
    }
}