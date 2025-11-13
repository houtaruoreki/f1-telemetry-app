namespace F1TelemetryApp.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using F1TelemetryApp.Helpers;
using F1TelemetryApp.Models;
using F1TelemetryApp.Services;

/// <summary>
/// ViewModel for displaying a list of F1 drivers.
/// Shows drivers from the most recent session.
/// </summary>
public class DriverListViewModel : BaseViewModel
{
    private readonly IOpenF1ApiService _apiService;
    private readonly ICacheService _cacheService;
    private int _selectedYear;

    public DriverListViewModel(IOpenF1ApiService apiService, ICacheService cacheService)
    {
        _apiService = apiService;
        _cacheService = cacheService;
        _selectedYear = Constants.Settings.DefaultYear;

        Title = "Drivers";
        Drivers = new ObservableCollection<Driver>();

        LoadDriversCommand = new Command(async () => await LoadDriversAsync());
        RefreshCommand = new Command(async () => await RefreshDriversAsync());
    }

    public ObservableCollection<Driver> Drivers { get; }

    public int SelectedYear
    {
        get => _selectedYear;
        set
        {
            if (SetProperty(ref _selectedYear, value))
            {
                _ = LoadDriversAsync();
            }
        }
    }

    public ICommand LoadDriversCommand { get; }
    public ICommand RefreshCommand { get; }

    /// <summary>
    /// Loads drivers from the most recent session
    /// </summary>
    public async Task LoadDriversAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            // Check cache first
            var cacheKey = $"drivers_list_{SelectedYear}";
            var cachedDrivers = _cacheService.Get<List<Driver>>(cacheKey);

            if (cachedDrivers != null)
            {
                UpdateDriversList(cachedDrivers);
                return;
            }

            // Get most recent session for the year
            var sessions = await _apiService.GetSessionsAsync(SelectedYear);
            var recentSession = sessions
                .Where(s => s.SessionType == "Race")
                .OrderByDescending(s => s.DateStart)
                .FirstOrDefault();

            if (recentSession == null)
            {
                SetError($"No sessions found for {SelectedYear}");
                return;
            }

            // Get drivers from that session
            var drivers = await _apiService.GetDriversAsync(recentSession.SessionKey);

            if (drivers.Any())
            {
                // Cache the results
                _cacheService.Set(cacheKey, drivers, Constants.Cache.DriverCacheDuration);
                UpdateDriversList(drivers);
            }
            else
            {
                SetError("No drivers found");
            }
        }
        catch (Exception ex)
        {
            SetError($"Failed to load drivers: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Refreshes drivers by clearing cache and reloading
    /// </summary>
    private async Task RefreshDriversAsync()
    {
        var cacheKey = $"drivers_list_{SelectedYear}";
        _cacheService.Remove(cacheKey);
        await LoadDriversAsync();
    }

    /// <summary>
    /// Updates the observable collection with new drivers
    /// </summary>
    private void UpdateDriversList(List<Driver> drivers)
    {
        Drivers.Clear();

        // Sort by driver number
        var sortedDrivers = drivers.OrderBy(d => d.DriverNumber);

        foreach (var driver in sortedDrivers)
        {
            Drivers.Add(driver);
        }
    }
}
