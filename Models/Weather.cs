namespace F1TelemetryApp.Models;

/// <summary>
/// Represents weather conditions at the circuit.
/// Maps to OpenF1 API /weather endpoint.
/// Updated approximately every minute.
/// </summary>
public class Weather
{
    /// <summary>
    /// Unique session identifier
    /// </summary>
    public int SessionKey { get; set; }

    /// <summary>
    /// Timestamp of weather reading
    /// </summary>
    public DateTime Date { get; set; }

    /// <summary>
    /// Air temperature in Celsius
    /// </summary>
    public double AirTemperature { get; set; }

    /// <summary>
    /// Track surface temperature in Celsius
    /// </summary>
    public double TrackTemperature { get; set; }

    /// <summary>
    /// Humidity percentage (0-100)
    /// </summary>
    public int Humidity { get; set; }

    /// <summary>
    /// Atmospheric pressure in millibars
    /// </summary>
    public double Pressure { get; set; }

    /// <summary>
    /// Rainfall indicator (true if raining)
    /// </summary>
    public bool Rainfall { get; set; }

    /// <summary>
    /// Wind direction in degrees (0-360)
    /// </summary>
    public int WindDirection { get; set; }

    /// <summary>
    /// Wind speed in m/s
    /// </summary>
    public double WindSpeed { get; set; }

    /// <summary>
    /// Gets formatted air temperature
    /// </summary>
    public string FormattedAirTemp => $"{AirTemperature:F1}°C";

    /// <summary>
    /// Gets formatted track temperature
    /// </summary>
    public string FormattedTrackTemp => $"{TrackTemperature:F1}°C";

    /// <summary>
    /// Gets formatted humidity
    /// </summary>
    public string FormattedHumidity => $"{Humidity}%";

    /// <summary>
    /// Gets weather condition description
    /// </summary>
    public string Condition => Rainfall ? "Rainy" : "Dry";

    /// <summary>
    /// Gets wind direction as compass bearing
    /// </summary>
    public string WindDirectionCompass
    {
        get
        {
            var directions = new[] { "N", "NE", "E", "SE", "S", "SW", "W", "NW" };
            var index = (int)Math.Round(WindDirection / 45.0) % 8;
            return directions[index];
        }
    }

    /// <summary>
    /// Gets formatted wind speed
    /// </summary>
    public string FormattedWindSpeed => $"{WindSpeed:F1} m/s {WindDirectionCompass}";
}
