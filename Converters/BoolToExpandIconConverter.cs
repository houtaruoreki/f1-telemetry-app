namespace F1TelemetryApp.Converters;

using System.Globalization;

/// <summary>
/// Converts a boolean value to an expand/collapse icon.
/// Parameter format: "expandedIcon,collapsedIcon" (e.g., "▼,▶")
/// </summary>
public class BoolToExpandIconConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not bool isExpanded)
            return "▶";

        var icons = (parameter as string)?.Split(',') ?? new[] { "▼", "▶" };
        return isExpanded ? icons[0] : icons[1];
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}
