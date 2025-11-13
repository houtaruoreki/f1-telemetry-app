namespace F1TelemetryApp.Models;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a driver's position/standing during a session.
/// Maps to OpenF1 API /position endpoint.
/// </summary>
public class Position
{
    /// <summary>
    /// Unique session identifier
    /// </summary>
    [JsonPropertyName("session_key")]
    public int SessionKey { get; set; }

    /// <summary>
    /// Driver's racing number
    /// </summary>
    [JsonPropertyName("driver_number")]
    public int DriverNumber { get; set; }

    /// <summary>
    /// Timestamp of position update
    /// </summary>
    [JsonPropertyName("date")]
    public DateTime Date { get; set; }

    /// <summary>
    /// Current position (1 = first place, 20 = last)
    /// </summary>
    [JsonPropertyName("position")]
    public int PositionValue { get; set; }

    /// <summary>
    /// Gets ordinal position string (1st, 2nd, 3rd, etc.)
    /// </summary>
    public string OrdinalPosition
    {
        get
        {
            return PositionValue switch
            {
                1 => "1st",
                2 => "2nd",
                3 => "3rd",
                _ => $"{PositionValue}th"
            };
        }
    }

    /// <summary>
    /// Checks if driver is on podium (top 3)
    /// </summary>
    public bool IsOnPodium => PositionValue <= 3;

    /// <summary>
    /// Checks if driver is in points-scoring position (top 10)
    /// </summary>
    public bool IsInPoints => PositionValue <= 10;
}
