namespace F1TelemetryApp.Models;

using System.Collections.ObjectModel;

/// <summary>
/// Represents a group of sessions for a single Grand Prix/Meeting.
/// </summary>
public class SessionGroup : ObservableCollection<Session>
{
    public string MeetingName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string CircuitName { get; set; } = string.Empty;
    public DateTime DateStart { get; set; }
    public bool IsExpanded { get; set; } = true; // Expanded by default

    public SessionGroup(string meetingName, string location, string circuitName, DateTime dateStart)
    {
        MeetingName = meetingName;
        Location = location;
        CircuitName = circuitName;
        DateStart = dateStart;
    }

    public string DisplayHeader => $"🏁 {MeetingName} - {Location}";
    public string SessionCount => $"{Count} session{(Count != 1 ? "s" : "")}";
    public string FormattedDate => DateStart.ToString("MMM dd, yyyy");
}
