using CraftMyFit.Helpers.Extensions;
using System.Globalization;

namespace CraftMyFit.Helpers.Converters
{
    public class DateTimeToStringConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if(value is not DateTime dateTime)
            {
                return string.Empty;
            }

            string? format = parameter?.ToString();

            return format switch
            {
                "friendly" => dateTime.ToFriendlyString(),
                "timeago" => dateTime.ToTimeAgo(),
                "date" => dateTime.ToString("dd/MM/yyyy"),
                "time" => dateTime.ToString("HH:mm"),
                "datetime" => dateTime.ToString("dd/MM/yyyy HH:mm"),
                "short" => dateTime.ToString("dd/MM/yy"),
                "long" => dateTime.ToString("dddd, dd MMMM yyyy"),
                "monthyear" => dateTime.ToString("MMMM yyyy"),
                "daymonth" => dateTime.ToString("dd MMM"),
                null or "" => dateTime.ToString("dd/MM/yyyy"),
                _ => dateTime.ToString(format)
            };
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) => value is not string stringValue || string.IsNullOrWhiteSpace(stringValue)
                ? DateTime.MinValue
                : (object)(DateTime.TryParse(stringValue, out DateTime result) ? result : DateTime.MinValue);
    }
}