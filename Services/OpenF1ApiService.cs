namespace F1TelemetryApp.Services;

using System.Net.Http.Json;
using System.Text.Json;
using F1TelemetryApp.Helpers;
using F1TelemetryApp.Models;

/// <summary>
/// Implementation of OpenF1 API service.
/// Handles HTTP communication with the OpenF1 API.
/// </summary>
public class OpenF1ApiService : IOpenF1ApiService
{
    private readonly HttpClient _httpClient;
    private readonly JsonSerializerOptions _jsonOptions;

    public OpenF1ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(Constants.Api.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(Constants.Api.TimeoutSeconds);

        // Configure JSON options for case-insensitive property matching
        _jsonOptions = new JsonSerializerOptions
        {
            PropertyNameCaseInsensitive = true
        };
    }

    public async Task<List<Session>> GetSessionsAsync(int year, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Session>>(
                $"sessions?year={year}",
                _jsonOptions,
                ct
            );
            return response ?? new List<Session>();
        }
        catch (Exception ex)
        {
            // Log error (logging infrastructure to be added later)
            Console.WriteLine($"Error fetching sessions: {ex.Message}");
            return new List<Session>();
        }
    }

    public async Task<Session?> GetSessionByKeyAsync(int sessionKey, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Session>>(
                $"sessions?session_key={sessionKey}",
                _jsonOptions,
                ct
            );
            return response?.FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching session {sessionKey}: {ex.Message}");
            return null;
        }
    }

    public async Task<List<Driver>> GetDriversAsync(int sessionKey, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Driver>>(
                $"drivers?session_key={sessionKey}",
                _jsonOptions,
                ct
            );
            return response ?? new List<Driver>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching drivers: {ex.Message}");
            return new List<Driver>();
        }
    }

    public async Task<List<Lap>> GetLapsAsync(int sessionKey, int driverNumber, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Lap>>(
                $"laps?session_key={sessionKey}&driver_number={driverNumber}",
                _jsonOptions,
                ct
            );
            return response ?? new List<Lap>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching laps: {ex.Message}");
            return new List<Lap>();
        }
    }

    public async Task<List<CarData>> GetCarDataAsync(int sessionKey, int driverNumber, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<CarData>>(
                $"car_data?session_key={sessionKey}&driver_number={driverNumber}",
                _jsonOptions,
                ct
            );
            return response ?? new List<CarData>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching car data: {ex.Message}");
            return new List<CarData>();
        }
    }

    public async Task<List<Weather>> GetWeatherAsync(int sessionKey, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Weather>>(
                $"weather?session_key={sessionKey}",
                _jsonOptions,
                ct
            );
            return response ?? new List<Weather>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching weather: {ex.Message}");
            return new List<Weather>();
        }
    }

    public async Task<List<Position>> GetPositionsAsync(int sessionKey, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Position>>(
                $"position?session_key={sessionKey}",
                _jsonOptions,
                ct
            );
            return response ?? new List<Position>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching positions: {ex.Message}");
            return new List<Position>();
        }
    }

    public async Task<List<Meeting>> GetMeetingsAsync(int year, CancellationToken ct = default)
    {
        try
        {
            var response = await _httpClient.GetFromJsonAsync<List<Meeting>>(
                $"meetings?year={year}",
                _jsonOptions,
                ct
            );
            return response ?? new List<Meeting>();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching meetings: {ex.Message}");
            return new List<Meeting>();
        }
    }
}
