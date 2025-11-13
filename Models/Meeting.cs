namespace F1TelemetryApp.Models;
using System.Text.Json.Serialization;

/// <summary>
/// Represents a Grand Prix weekend or testing event.
/// Maps to OpenF1 API /meetings endpoint.
/// </summary>
public class Meeting
{
    /// <summary>
    /// Unique meeting identifier
    /// </summary>
    [JsonPropertyName("meeting_key")]
    public int MeetingKey { get; set; }

    /// <summary>
    /// Official meeting name (e.g., "Bahrain Grand Prix")
    /// </summary>
    [JsonPropertyName("meeting_name")]
    public string MeetingName { get; set; } = string.Empty;

    /// <summary>
    /// Official full name (e.g., "FORMULA 1 GULF AIR BAHRAIN GRAND PRIX 2024")
    /// </summary>
    [JsonPropertyName("meeting_official_name")]
    public string MeetingOfficialName { get; set; } = string.Empty;

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
    /// Circuit key identifier
    /// </summary>
    [JsonPropertyName("circuit_key")]
    public int CircuitKey { get; set; }

    /// <summary>
    /// Date/time when meeting starts
    /// </summary>
    [JsonPropertyName("date_start")]
    public DateTime DateStart { get; set; }

    /// <summary>
    /// GMT offset (e.g., "+03:00")
    /// </summary>
    [JsonPropertyName("gmt_offset")]
    public string GmtOffset { get; set; } = string.Empty;

    /// <summary>
    /// Year of the meeting
    /// </summary>
    [JsonPropertyName("year")]
    public int Year { get; set; }

    /// <summary>
    /// Gets formatted meeting date for display
    /// </summary>
    public string FormattedDate => DateStart.ToString("MMMM dd, yyyy");

    /// <summary>
    /// Gets short date format
    /// </summary>
    public string ShortDate => DateStart.ToString("MMM dd");

    /// <summary>
    /// Checks if meeting is in the future
    /// </summary>
    public bool IsUpcoming => DateStart > DateTime.UtcNow;
}
