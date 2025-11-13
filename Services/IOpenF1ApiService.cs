namespace F1TelemetryApp.Services;

using F1TelemetryApp.Models;

/// <summary>
/// Service interface for accessing OpenF1 API endpoints.
/// Provides methods to retrieve F1 telemetry and session data.
/// </summary>
public interface IOpenF1ApiService
{
    /// <summary>
    /// Gets list of sessions for a specific year
    /// </summary>
    /// <param name="year">Year to filter sessions (e.g., 2024)</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of sessions</returns>
    Task<List<Session>> GetSessionsAsync(int year, CancellationToken ct = default);

    /// <summary>
    /// Gets detailed information about a specific session
    /// </summary>
    /// <param name="sessionKey">Unique session identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>Session details or null if not found</returns>
    Task<Session?> GetSessionByKeyAsync(int sessionKey, CancellationToken ct = default);

    /// <summary>
    /// Gets list of drivers participating in a session
    /// </summary>
    /// <param name="sessionKey">Unique session identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of drivers</returns>
    Task<List<Driver>> GetDriversAsync(int sessionKey, CancellationToken ct = default);

    /// <summary>
    /// Gets lap data for a specific driver in a session
    /// </summary>
    /// <param name="sessionKey">Unique session identifier</param>
    /// <param name="driverNumber">Driver's racing number</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of laps</returns>
    Task<List<Lap>> GetLapsAsync(int sessionKey, int driverNumber, CancellationToken ct = default);

    /// <summary>
    /// Gets telemetry data for a specific driver in a session
    /// </summary>
    /// <param name="sessionKey">Unique session identifier</param>
    /// <param name="driverNumber">Driver's racing number</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of telemetry data points</returns>
    Task<List<CarData>> GetCarDataAsync(int sessionKey, int driverNumber, CancellationToken ct = default);

    /// <summary>
    /// Gets weather data for a session
    /// </summary>
    /// <param name="sessionKey">Unique session identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of weather readings</returns>
    Task<List<Weather>> GetWeatherAsync(int sessionKey, CancellationToken ct = default);

    /// <summary>
    /// Gets driver positions during a session
    /// </summary>
    /// <param name="sessionKey">Unique session identifier</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of position updates</returns>
    Task<List<Position>> GetPositionsAsync(int sessionKey, CancellationToken ct = default);

    /// <summary>
    /// Gets meetings (Grand Prix weekends) for a specific year
    /// </summary>
    /// <param name="year">Year to filter meetings</param>
    /// <param name="ct">Cancellation token</param>
    /// <returns>List of meetings</returns>
    Task<List<Meeting>> GetMeetingsAsync(int year, CancellationToken ct = default);
}
