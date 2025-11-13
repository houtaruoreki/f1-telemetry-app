namespace F1TelemetryApp.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using F1TelemetryApp.Helpers;
using F1TelemetryApp.Models;
using F1TelemetryApp.Services;
using Microcharts;
using SkiaSharp;

/// <summary>
/// ViewModel for displaying telemetry data for a specific driver.
/// Shows laps, car data, and performance metrics with charts.
/// </summary>
public class TelemetryViewModel : BaseViewModel, IQueryAttributable
{
    private readonly IOpenF1ApiService _apiService;
    private readonly ICacheService _cacheService;
    private int _sessionKey;
    private int _driverNumber;
    private Driver? _driver;
    private Lap? _selectedLap;
    private Chart? _lapTimesChart;
    private Chart? _sectorComparisonChart;
    private Chart? _speedChart;
    private string? _fastestLapTime;
    private string? _averageLapTime;
    private int _totalLaps;

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

    public Chart? LapTimesChart
    {
        get => _lapTimesChart;
        set => SetProperty(ref _lapTimesChart, value);
    }

    public Chart? SectorComparisonChart
    {
        get => _sectorComparisonChart;
        set => SetProperty(ref _sectorComparisonChart, value);
    }

    public Chart? SpeedChart
    {
        get => _speedChart;
        set => SetProperty(ref _speedChart, value);
    }

    public string? FastestLapTime
    {
        get => _fastestLapTime;
        set => SetProperty(ref _fastestLapTime, value);
    }

    public string? AverageLapTime
    {
        get => _averageLapTime;
        set => SetProperty(ref _averageLapTime, value);
    }

    public int TotalLaps
    {
        get => _totalLaps;
        set => SetProperty(ref _totalLaps, value);
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

            // Generate charts and statistics
            GenerateCharts(laps);
            CalculateStatistics(laps);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to load laps: {ex.Message}");
        }
    }

    /// <summary>
    /// Generates visual charts from lap data
    /// </summary>
    private void GenerateCharts(List<Lap> laps)
    {
        if (!laps.Any())
            return;

        var validLaps = laps.Where(l => l.LapDuration.HasValue).ToList();
        if (!validLaps.Any())
            return;

        // Generate Lap Times Chart (Line Chart)
        var lapTimeEntries = validLaps
            .OrderBy(l => l.LapNumber)
            .Select(l => new ChartEntry((float)l.LapDuration!.Value)
            {
                Label = $"L{l.LapNumber}",
                ValueLabel = l.FormattedLapTime,
                Color = SKColor.Parse("#2c3e50")
            })
            .ToList();

        LapTimesChart = new LineChart
        {
            Entries = lapTimeEntries,
            LineMode = LineMode.Straight,
            LineSize = 4,
            PointMode = PointMode.Circle,
            PointSize = 10,
            LabelTextSize = 32,
            ValueLabelTextSize = 32,
            BackgroundColor = SKColors.Transparent,
            LabelOrientation = Orientation.Horizontal
        };

        // Generate Sector Comparison Chart (Bar Chart) - Average sectors
        if (validLaps.Any(l => l.DurationSector1.HasValue && l.DurationSector2.HasValue && l.DurationSector3.HasValue))
        {
            var avgSector1 = validLaps.Where(l => l.DurationSector1.HasValue).Average(l => l.DurationSector1!.Value);
            var avgSector2 = validLaps.Where(l => l.DurationSector2.HasValue).Average(l => l.DurationSector2!.Value);
            var avgSector3 = validLaps.Where(l => l.DurationSector3.HasValue).Average(l => l.DurationSector3!.Value);

            var sectorEntries = new List<ChartEntry>
            {
                new ChartEntry((float)avgSector1)
                {
                    Label = "Sector 1",
                    ValueLabel = $"{avgSector1:F3}s",
                    Color = SKColor.Parse("#3498db")
                },
                new ChartEntry((float)avgSector2)
                {
                    Label = "Sector 2",
                    ValueLabel = $"{avgSector2:F3}s",
                    Color = SKColor.Parse("#e74c3c")
                },
                new ChartEntry((float)avgSector3)
                {
                    Label = "Sector 3",
                    ValueLabel = $"{avgSector3:F3}s",
                    Color = SKColor.Parse("#2ecc71")
                }
            };

            SectorComparisonChart = new BarChart
            {
                Entries = sectorEntries,
                LabelTextSize = 36,
                ValueLabelTextSize = 36,
                BackgroundColor = SKColors.Transparent,
                LabelOrientation = Orientation.Horizontal
            };
        }
    }

    /// <summary>
    /// Calculates lap statistics
    /// </summary>
    private void CalculateStatistics(List<Lap> laps)
    {
        var validLaps = laps.Where(l => l.LapDuration.HasValue).ToList();

        TotalLaps = laps.Count;

        if (validLaps.Any())
        {
            var fastest = validLaps.MinBy(l => l.LapDuration!.Value);
            FastestLapTime = fastest?.FormattedLapTime ?? "N/A";

            var average = validLaps.Average(l => l.LapDuration!.Value);
            var avgTime = TimeSpan.FromSeconds(average);
            AverageLapTime = $"{(int)avgTime.TotalMinutes}:{avgTime.Seconds:D2}.{avgTime.Milliseconds:D3}";
        }
        else
        {
            FastestLapTime = "N/A";
            AverageLapTime = "N/A";
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
                .Take(100) // Take first 100 data points as sample
                .ToList();

            foreach (var data in lapData)
            {
                CarData.Add(data);
            }

            // Generate speed chart from car data
            GenerateSpeedChart(lapData);
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

    /// <summary>
    /// Generates speed chart from telemetry data
    /// </summary>
    private void GenerateSpeedChart(List<CarData> carData)
    {
        if (!carData.Any())
        {
            SpeedChart = null;
            return;
        }

        var speedEntries = carData
            .Select((data, index) => new ChartEntry(data.Speed)
            {
                Label = $"{index}",
                ValueLabel = $"{data.Speed}",
                Color = data.Brake ? SKColor.Parse("#e74c3c") : SKColor.Parse("#3498db")
            })
            .ToList();

        SpeedChart = new LineChart
        {
            Entries = speedEntries,
            LineMode = LineMode.Spline,
            LineSize = 3,
            PointMode = PointMode.None,
            LabelTextSize = 28,
            ValueLabelTextSize = 0, // Hide value labels for cleaner look
            BackgroundColor = SKColors.Transparent,
            LabelOrientation = Orientation.Horizontal,
            MinValue = 0,
            MaxValue = speedEntries.Max(e => e.Value) + 20
        };
    }
}
