using System.Globalization;

namespace CraftMyFit.Helpers.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is not bool boolValue)
            {
                return false;
            }

            // Se il parametro è "invert", inverti il risultato
            bool invert = parameter?.ToString()?.ToLower() == "invert";

            return invert ? !boolValue : boolValue;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is not bool boolValue)
            {
                return false;
            }

            // Se il parametro è "invert", inverti il risultato
            bool invert = parameter?.ToString()?.ToLower() == "invert";

            return invert ? !boolValue : boolValue;
        }
    }
}