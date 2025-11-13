namespace F1TelemetryApp.Models;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a single lap with timing information.
/// Maps to OpenF1 API /laps endpoint.
/// </summary>
public class Lap
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
    /// Lap number within the session
    /// </summary>
    [JsonPropertyName("lap_number")]
    public int LapNumber { get; set; }

    /// <summary>
    /// Total lap time in seconds (null if lap not completed)
    /// </summary>
    [JsonPropertyName("lap_duration")]
    public double? LapDuration { get; set; }

    /// <summary>
    /// Sector 1 time in seconds
    /// </summary>
    [JsonPropertyName("duration_sector_1")]
    public double? DurationSector1 { get; set; }

    /// <summary>
    /// Sector 2 time in seconds
    /// </summary>
    [JsonPropertyName("duration_sector_2")]
    public double? DurationSector2 { get; set; }

    /// <summary>
    /// Sector 3 time in seconds
    /// </summary>
    [JsonPropertyName("duration_sector_3")]
    public double? DurationSector3 { get; set; }

    /// <summary>
    /// Whether this is a personal best lap for the driver
    /// </summary>
    [JsonPropertyName("is_pit_out_lap")]
    public bool IsPersonalBest { get; set; }

    /// <summary>
    /// Segments timing (detailed micro-sectors)
    /// </summary>
    public List<int>? SegmentsSector1 { get; set; }
    public List<int>? SegmentsSector2 { get; set; }
    public List<int>? SegmentsSector3 { get; set; }

    /// <summary>
    /// Timestamp when lap was completed
    /// </summary>
    [JsonPropertyName("date_start")]
    public DateTime? DateStart { get; set; }

    /// <summary>
    /// Gets formatted lap time (MM:SS.mmm)
    /// </summary>
    public string FormattedLapTime
    {
        get
        {
            if (!LapDuration.HasValue)
                return "--:--.---";

            var timeSpan = TimeSpan.FromSeconds(LapDuration.Value);
            return $"{(int)timeSpan.TotalMinutes}:{timeSpan.Seconds:D2}.{timeSpan.Milliseconds:D3}";
        }
    }

    /// <summary>
    /// Gets formatted sector 1 time
    /// </summary>
    public string FormattedSector1
    {
        get
        {
            if (!DurationSector1.HasValue)
                return "--.-";

            return DurationSector1.Value.ToString("F3");
        }
    }

    /// <summary>
    /// Gets formatted sector 2 time
    /// </summary>
    public string FormattedSector2
    {
        get
        {
            if (!DurationSector2.HasValue)
                return "--.-";

            return DurationSector2.Value.ToString("F3");
        }
    }

    /// <summary>
    /// Gets formatted sector 3 time
    /// </summary>
    public string FormattedSector3
    {
        get
        {
            if (!DurationSector3.HasValue)
                return "--.-";

            return DurationSector3.Value.ToString("F3");
        }
    }
}
