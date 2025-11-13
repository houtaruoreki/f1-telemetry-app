namespace F1TelemetryApp.Models;

using System.Text.Json.Serialization;

/// <summary>
/// Represents a racing session (Practice, Qualifying, Race, Sprint).
/// Maps to OpenF1 API /sessions endpoint.
/// </summary>
public class Session
{
    /// <summary>
    /// Unique session identifier
    /// </summary>
    [JsonPropertyName("session_key")]
    public int SessionKey { get; set; }

    /// <summary>
    /// Session name (e.g., "Race", "Qualifying", "Practice 1")
    /// </summary>
    [JsonPropertyName("session_name")]
    public string SessionName { get; set; } = string.Empty;

    /// <summary>
    /// Session type (e.g., "Race", "Qualifying", "Practice")
    /// </summary>
    [JsonPropertyName("session_type")]
    public string SessionType { get; set; } = string.Empty;

    /// <summary>
    /// Date/time when session starts
    /// </summary>
    [JsonPropertyName("date_start")]
    public DateTime DateStart { get; set; }

    /// <summary>
    /// Date/time when session ends
    /// </summary>
    [JsonPropertyName("date_end")]
    public DateTime DateEnd { get; set; }

    /// <summary>
    /// Grand Prix/Meeting key this session belongs to
    /// </summary>
    [JsonPropertyName("meeting_key")]
    public int MeetingKey { get; set; }

    /// <summary>
    /// Location (e.g., "Bahrain International Circuit")
    /// </summary>
    [JsonPropertyName("location")]
    public string Location { get; set; } = string.Empty;

    /// <summary>
    /// Country name (e.g., "Bahrain")
    /// </summary>
    [JsonPropertyName("country_name")]
    public string CountryName { get; set; } = string.Empty;

    /// <summary>
    /// Country code (e.g., "BHR")
    /// </summary>
    [JsonPropertyName("country_code")]
    public string CountryCode { get; set; } = string.Empty;

    /// <summary>
    /// Circuit short name (e.g., "Bahrain")
    /// </summary>
    [JsonPropertyName("circuit_short_name")]
    public string CircuitShortName { get; set; } = string.Empty;

    /// <summary>
    /// Grand Prix official name (e.g., "Bahrain Grand Prix")
    /// </summary>
    [JsonPropertyName("gmt_offset")]
    public string GmtOffset { get; set; } = string.Empty;

    /// <summary>
    /// Year of the session
    /// </summary>
    [JsonPropertyName("year")]
    public int Year { get; set; }

    /// <summary>
    /// Gets formatted session date for display
    /// </summary>
    public string FormattedDate => DateStart.ToString("MMM dd, yyyy");

    /// <summary>
    /// Gets formatted session time for display
    /// </summary>
    public string FormattedTime => DateStart.ToString("HH:mm");

    /// <summary>
    /// Checks if session is in the future
    /// </summary>
    public bool IsUpcoming => DateStart > DateTime.UtcNow;

    /// <summary>
    /// Checks if session is currently live
    /// </summary>
    public bool IsLive => DateTime.UtcNow >= DateStart && DateTime.UtcNow <= DateEnd;

    /// <summary>
    /// Checks if session is completed
    /// </summary>
    public bool IsCompleted => DateTime.UtcNow > DateEnd;

    /// <summary>
    /// Gets session status as a string
    /// </summary>
    public string Status
    {
        get
        {
            if (IsLive) return "LIVE";
            if (IsUpcoming) return "UPCOMING";
            return "COMPLETED";
        }
    }
}
