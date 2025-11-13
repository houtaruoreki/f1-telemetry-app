namespace F1TelemetryApp.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using F1TelemetryApp.Helpers;
using F1TelemetryApp.Models;
using F1TelemetryApp.Services;

/// <summary>
/// ViewModel for displaying telemetry data for a specific driver.
/// Shows laps, car data, and performance metrics.
/// </summary>
public class TelemetryViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IOpenF1ApiService _apiService;
    private readonly ICacheService _cacheService;
    private int _sessionKey;
    private int _driverNumber;
    private Driver? _driver;
    private Lap? _selectedLap;

    public TelemetryViewModel(IOpenF1ApiService apiService, ICacheService cacheService)
    {
        _apiService = apiService;
        _cacheService = cacheService;

        Title = "Telemetry";
        Laps = new ObservableCollection<Lap>();
        CarData = new ObservableCollection<CarData>();

        LoadTelemetryCommand = new Command(async () => await LoadTelemetryAsync());
        LoadLapDataCommand = new Command<Lap>(async (lap) => await LoadLapDataAsync(lap));
    }

    public Driver? Driver
    {
        get => _driver;
        set => SetProperty(ref _driver, value);
    }

    public Lap? SelectedLap
    {
        get => _selectedLap;
        set
        {
            if (SetProperty(ref _selectedLap, value) && value != null)
            {
                _ = LoadLapDataAsync(value);
            }
        }
    }

    public ObservableCollection<Lap> Laps { get; }
    public ObservableCollection<CarData> CarData { get; }

    public ICommand LoadTelemetryCommand { get; }
    public ICommand LoadLapDataCommand { get; }

    /// <summary>
    /// Receives navigation parameters
    /// </summary>
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("sessionKey") && query.ContainsKey("driverNumber"))
        {
            _sessionKey = int.Parse(query["sessionKey"].ToString()!);
            _driverNumber = int.Parse(query["driverNumber"].ToString()!);
            _ = LoadTelemetryAsync();
        }
    }

    /// <summary>
    /// Loads telemetry data including laps and driver info
    /// </summary>
    private async Task LoadTelemetryAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            // Load driver info
            await LoadDriverInfoAsync();

            // Load laps
            await LoadLapsAsync();
        }
        catch (Exception ex)
        {
            SetError($"Failed to load telemetry: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Loads driver information
    /// </summary>
    private async Task LoadDriverInfoAsync()
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

            Driver = drivers.FirstOrDefault(d => d.DriverNumber == _driverNumber);

            if (Driver != null)
            {
                Title = $"{Driver.BroadcastName} - Telemetry";
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load driver info: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads lap data for the driver
    /// </summary>
    private async Task LoadLapsAsync()
    {
        try
        {
            var cacheKey = $"laps_{_sessionKey}_{_driverNumber}";
            var cachedLaps = _cacheService.Get<List<Lap>>(cacheKey);

            List<Lap> laps;
            if (cachedLaps != null)
            {
                laps = cachedLaps;
            }
            else
            {
                laps = await _apiService.GetLapsAsync(_sessionKey, _driverNumber);
                _cacheService.Set(cacheKey, laps, Constants.Cache.SessionCacheDuration);
            }

            Laps.Clear();
            foreach (var lap in laps.OrderBy(l => l.LapNumber))
            {
                Laps.Add(lap);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load laps: {ex.Message}");
        }
    }

    /// <summary>
    /// Loads detailed car data for a specific lap
    /// </summary>
    private async Task LoadLapDataAsync(Lap? lap)
    {
        if (lap == null)
            return;

        try
        {
            IsBusy = true;

            var cacheKey = $"cardata_{_sessionKey}_{_driverNumber}";
            var cachedCarData = _cacheService.Get<List<CarData>>(cacheKey);

            List<CarData> carData;
            if (cachedCarData != null)
            {
                carData = cachedCarData;
            }
            else
            {
                carData = await _apiService.GetCarDataAsync(_sessionKey, _driverNumber);
                _cacheService.Set(cacheKey, carData, Constants.Cache.TelemetryCacheDuration);
            }

            // Filter car data for the selected lap (simplified - actual implementation would need lap timestamps)
            CarData.Clear();
            var lapData = carData
                .OrderBy(cd => cd.Date)
                .Take(100); // Take first 100 data points as sample

            foreach (var data in lapData)
            {
                CarData.Add(data);
            }
        }
        catch (Exception ex)
        {
            SetError($"Failed to load car data: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
