namespace F1TelemetryApp.Models;

/// <summary>
/// Represents a single telemetry data point for a car.
/// Maps to OpenF1 API /car_data endpoint.
/// Sampled at approximately 3.7 Hz.
/// </summary>
public class CarData
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
    /// Timestamp of this telemetry sample
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Speed in km/h
    /// </summary>
    public int Speed { get; set; }

    /// <summary>
    /// Engine RPM
    /// </summary>
    public int Rpm { get; set; }

    /// <summary>
    /// Current gear (0-8, where 0 is neutral/reverse)
    /// </summary>
    public int NGear { get; set; }

    /// <summary>
    /// Throttle position (0-100%)
    /// </summary>
    public int Throttle { get; set; }

    /// <summary>
    /// Brake application (true = braking)
    /// </summary>
    public bool Brake { get; set; }

    /// <summary>
    /// DRS (Drag Reduction System) status
    /// 0 = Off, 1 = Available (not activated), 8 = Activated
    /// </summary>
    public int Drs { get; set; }

    /// <summary>
    /// Gets DRS status as a readable string
    /// </summary>
    public string DrsStatus
    {
        get => Drs switch
        {
            0 => "Off",
            1 => "Available",
            8 => "Active",
            _ => "Unknown"
        };
    }

    /// <summary>
    /// Checks if DRS is currently active
    /// </summary>
    public bool IsDrsActive => Drs == 8;

    /// <summary>
    /// Gets throttle as percentage string
    /// </summary>
    public string ThrottlePercent => $"{Throttle}%";

    /// <summary>
    /// Gets formatted speed with unit
    /// </summary>
    public string FormattedSpeed => $"{Speed} km/h";
}
