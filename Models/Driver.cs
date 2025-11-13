namespace F1TelemetryApp.Models;

/// <summary>
/// Represents a Formula 1 driver in a specific session.
/// Maps to OpenF1 API /drivers endpoint.
/// </summary>
public class Driver
{
    /// <summary>
    /// Unique session identifier
    /// </summary>
    public int SessionKey { get; set; }

    /// <summary>
    /// Driver's racing number (e.g., 1, 44, 33)
    /// </summary>
    public int DriverNumber { get; set; }

    /// <summary>
    /// Broadcast name (e.g., "L HAMILTON", "M VERSTAPPEN")
    /// </summary>
    public string BroadcastName { get; set; } = string.Empty;

    /// <summary>
    /// Full driver name (e.g., "Lewis Hamilton")
    /// </summary>
    public string FullName { get; set; } = string.Empty;

    /// <summary>
    /// Three-letter driver code (e.g., "HAM", "VER", "LEC")
    /// </summary>
    public string NameAcronym { get; set; } = string.Empty;

    /// <summary>
    /// Team/constructor name (e.g., "Mercedes", "Red Bull Racing")
    /// </summary>
    public string TeamName { get; set; } = string.Empty;

    /// <summary>
    /// Team color in hexadecimal (e.g., "00D2BE" for Mercedes)
    /// </summary>
    public string TeamColour { get; set; } = string.Empty;

    /// <summary>
    /// Driver's country code (e.g., "GBR", "NED", "MON")
    /// </summary>
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Headshot image URL (if available)
    /// </summary>
    public string? HeadshotUrl { get; set; }

    /// <summary>
    /// Gets the team color as a MAUI Color object
    /// </summary>
    public Color GetTeamColor()
    {
        if (string.IsNullOrEmpty(TeamColour))
            return Colors.Gray;

        try
        {
            return Color.FromArgb($"#{TeamColour}");
        }
        catch
        {
            return Colors.Gray;
        }
    }
}
