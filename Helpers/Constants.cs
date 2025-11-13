namespace F1TelemetryApp.Helpers;

/// <summary>
/// Application-wide constants and configuration values.
/// </summary>
public static class Constants
{
    /// <summary>
    /// OpenF1 API configuration
    /// </summary>
    public static class Api
    {
        /// <summary>
        /// Base URL for OpenF1 API
        /// </summary>
        public const string BaseUrl = "https://api.openf1.org/v1/";

        /// <summary>
        /// HTTP request timeout in seconds
        /// </summary>
        public const int TimeoutSeconds = 30;

        /// <summary>
        /// Maximum number of retry attempts for failed requests
        /// </summary>
        public const int MaxRetries = 3;
    }

    /// <summary>
    /// Cache configuration
    /// </summary>
    public static class Cache
    {
        /// <summary>
        /// Cache duration for telemetry data (1 minute)
        /// </summary>
        public static readonly TimeSpan TelemetryCacheDuration = TimeSpan.FromMinutes(1);

        /// <summary>
        /// Cache duration for session data (1 hour)
        /// </summary>
        public static readonly TimeSpan SessionCacheDuration = TimeSpan.FromHours(1);

        /// <summary>
        /// Cache duration for historical results (1 day)
        /// </summary>
        public static readonly TimeSpan HistoricalCacheDuration = TimeSpan.FromDays(1);

        /// <summary>
        /// Cache duration for driver/team info (7 days)
        /// </summary>
        public static readonly TimeSpan DriverCacheDuration = TimeSpan.FromDays(7);
    }

    /// <summary>
    /// Application settings and preferences
    /// </summary>
    public static class Settings
    {
        /// <summary>
        /// Default year for viewing data (current year)
        /// </summary>
        public static int DefaultYear => DateTime.UtcNow.Year;

        /// <summary>
        /// Number of items per page for pagination
        /// </summary>
        public const int PageSize = 20;

        /// <summary>
        /// Enable or disable offline mode
        /// </summary>
        public const string OfflineModeKey = "offline_mode";

        /// <summary>
        /// Last selected year preference key
        /// </summary>
        public const string LastSelectedYearKey = "last_selected_year";
    }

    /// <summary>
    /// Navigation route names
    /// </summary>
    public static class Routes
    {
        public const string Main = "main";
        public const string SessionList = "sessions";
        public const string SessionDetail = "session_detail";
        public const string DriverList = "drivers";
        public const string DriverDetail = "driver_detail";
        public const string Telemetry = "telemetry";
        public const string Settings = "settings";
        public const string Intro = "intro";
    }

    /// <summary>
    /// Application metadata
    /// </summary>
    public static class App
    {
        public const string Name = "F1 Telemetry";
        public const string Version = "1.0.0";
        public const string Author = "F1 Telemetry Team";
    }
}
