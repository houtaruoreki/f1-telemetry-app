namespace F1TelemetryApp.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using F1TelemetryApp.Helpers;
using F1TelemetryApp.Models;
using F1TelemetryApp.Services;

/// <summary>
/// ViewModel for displaying detailed information about a specific session.
/// Shows drivers, weather, and allows navigation to telemetry view.
/// </summary>
public class SessionDetailViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IOpenF1ApiService _apiService;
    private readonly ICacheService _cacheService;
    private Session? _session;
    private Weather? _currentWeather;
    private int _sessionKey;

    public SessionDetailViewModel(IOpenF1ApiService apiService, ICacheService cacheService)
    {
        _apiService = apiService;
        _cacheService = cacheService;

        Title = "Session Details";
        Drivers = new ObservableCollection<Driver>();

        LoadSessionDataCommand = new Command(async () => await LoadSessionDataAsync());
        ViewTelemetryCommand = new Command<Driver>(async (driver) => await ViewTelemetry(driver));
    }

    public Session? Session
    {
        get => _session;
        set => SetProperty(ref _session, value);
    }

    public Weather? CurrentWeather
    {
        get => _currentWeather;
        set => SetProperty(ref _currentWeather, value);
    }

    public ObservableCollection<Driver> Drivers { get; }

    public ICommand LoadSessionDataCommand { get; }
    public ICommand ViewTelemetryCommand { get; }

    /// <summary>
    /// Receives navigation parameters
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("sessionKey"))
        {
            _sessionKey = int.Parse(query["sessionKey"].ToString()!);
            _ = LoadSessionDataAsync();
        }
    }

    /// <summary>
    /// Loads session details, drivers, and weather
    /// </summary>
    private async Task LoadSessionDataAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            // Load session details
            var cacheKey = $"session_{_sessionKey}";
            var cachedSession = _cacheService.Get<Session>(cacheKey);

            if (cachedSession != null)
            {
                Session = cachedSession;
            }
            else
            {
                Session = await _apiService.GetSessionByKeyAsync(_sessionKey);
                if (Session != null)
                {
                    _cacheService.Set(cacheKey, Session, Constants.Cache.SessionCacheDuration);
                }
            }

            if (Session == null)
            {
                SetError("Session not found");
                return;
            }

            Title = $"{Session.SessionName} - {Session.CircuitShortName}";

            // Load drivers
            await LoadDriversAsync();

            // Load weather
            await LoadWeatherAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to load session data: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Loads drivers for this session
    /// </summary>
    private async Task LoadDriversAsync()
    {
        try
        {
            var cacheKey = $"drivers_{_sessionKey}";
            var cachedDrivers = _cacheService.Get<List<Driver>>(cacheKey);

            List<Driver> drivers;
            if (cachedDrivers != null)
            {
                drivers = cachedDrivers;
            }
            else
            {
                drivers = await _apiService.GetDriversAsync(_sessionKey);
                _cacheService.Set(cacheKey, drivers, Constants.Cache.SessionCacheDuration);
            }

            Drivers.Clear();
            foreach (var driver in drivers.OrderBy(d => d.DriverNumber))
            {
                Drivers.Add(driver);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load drivers: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads latest weather for this session
    /// </summary>
    private async Task LoadWeatherAsync()
    {
        try
        {
            var cacheKey = $"weather_{_sessionKey}";
            var cachedWeather = _cacheService.Get<List<Weather>>(cacheKey);

            List<Weather> weatherData;
            if (cachedWeather != null)
            {
                weatherData = cachedWeather;
            }
            else
            {
                weatherData = await _apiService.GetWeatherAsync(_sessionKey);
                _cacheService.Set(cacheKey, weatherData, Constants.Cache.TelemetryCacheDuration);
            }

            // Get most recent weather reading
            CurrentWeather = weatherData.OrderByDescending(w => w.Date).FirstOrDefault();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load weather: {ex.Message}");
        }
    }

    /// <summary>
    /// Navigates to telemetry view for selected driver
    /// </summary>
    private async Task ViewTelemetry(Driver? driver)
    {
        if (driver == null)
            return;

        await Shell.Current.GoToAsync($"{Constants.Routes.Telemetry}?sessionKey={_sessionKey}&driverNumber={driver.DriverNumber}");
    }
}
