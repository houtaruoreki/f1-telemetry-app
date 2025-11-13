namespace F1TelemetryApp.Converters;

using System.Globalization;

/// <summary>
/// Converter that returns true if value is not null, false otherwise.
/// Used for conditional visibility in XAML.
/// </summary>
public class IsNotNullConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value != null;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
