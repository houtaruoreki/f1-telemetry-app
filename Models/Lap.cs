namespace F1TelemetryApp.Models;

/// <summary>
/// Represents a single lap with timing information.
/// Maps to OpenF1 API /laps endpoint.
/// </summary>
public class Lap
{
    /// <summary>
    /// Unique session identifier
    /// </summary>
    public int SessionKey { get; set; }

    /// <summary>
    /// Driver's racing number
    /// </summary>
    public int DriverNumber { get; set; }

    /// <summary>
    /// Lap number within the session
    /// </summary>
    public int LapNumber { get; set; }

    /// <summary>
    /// Total lap time in seconds (null if lap not completed)
    /// </summary>
    public double? LapDuration { get; set; }

    /// <summary>
    /// Sector 1 time in seconds
    /// </summary>
    public double? DurationSector1 { get; set; }

    /// <summary>
    /// Sector 2 time in seconds
    /// </summary>
    public double? DurationSector2 { get; set; }

    /// <summary>
    /// Sector 3 time in seconds
    /// </summary>
    public double? DurationSector3 { get; set; }

    /// <summary>
    /// Whether this is a personal best lap for the driver
    /// </summary>
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
