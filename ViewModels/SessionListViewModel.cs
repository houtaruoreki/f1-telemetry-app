namespace F1TelemetryApp.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using F1TelemetryApp.Helpers;
using F1TelemetryApp.Models;
using F1TelemetryApp.Services;

/// <summary>
/// ViewModel for displaying a list of F1 sessions.
/// Allows filtering by year and navigation to session details.
/// </summary>
public class SessionListViewModel : BaseViewModel
{
    private readonly IOpenF1ApiService _apiService;
    private readonly ICacheService _cacheService;
    private int _selectedYear;
    private int _selectedYearIndex;

    public SessionListViewModel(IOpenF1ApiService apiService, ICacheService cacheService)
    {
        _apiService = apiService;
        _cacheService = cacheService;
        _selectedYear = Constants.Settings.DefaultYear;
        _selectedYearIndex = 0; // Default to current year

        Title = "Sessions";
        Sessions = new ObservableCollection<Session>();

        LoadSessionsCommand = new Command(async () => await LoadSessionsAsync());
        RefreshCommand = new Command(async () => await RefreshSessionsAsync());
        SessionSelectedCommand = new Command<Session>(async (session) => await OnSessionSelected(session));

        // Auto-load sessions on startup
        _ = LoadSessionsAsync();
    }

    public ObservableCollection<Session> Sessions { get; }

    /// <summary>
    /// Selected year index in the picker (0 = 2024, 1 = 2023, etc.)
    /// </summary>
    public int SelectedYearIndex
    {
        get => _selectedYearIndex;
        set
        {
            if (SetProperty(ref _selectedYearIndex, value))
            {
                // Calculate the actual year based on index (2024 - index)
                SelectedYear = 2024 - value;
            }
        }
    }

    /// <summary>
    /// Currently selected year for filtering sessions
    /// </summary>
    public int SelectedYear
    {
        get => _selectedYear;
        set
        {
            if (SetProperty(ref _selectedYear, value))
            {
                _ = LoadSessionsAsync();
            }
        }
    }

    public ICommand LoadSessionsCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SessionSelectedCommand { get; }

    /// <summary>
    /// Loads sessions for the selected year
    /// </summary>
    public async Task LoadSessionsAsync()
    {
        if (IsBusy)
            return;

        try
        {
            IsBusy = true;
            ClearError();

            // Check cache first
            var cacheKey = $"sessions_{SelectedYear}";
            var cachedSessions = _cacheService.Get<List<Session>>(cacheKey);

            if (cachedSessions != null)
            {
                UpdateSessionsList(cachedSessions);
                return;
            }

            // Fetch from API
            var sessions = await _apiService.GetSessionsAsync(SelectedYear);

            if (sessions.Any())
            {
                // Cache the results
                _cacheService.Set(cacheKey, sessions, Constants.Cache.SessionCacheDuration);
                UpdateSessionsList(sessions);
            }
            else
            {
                SetError($"No sessions found for {SelectedYear}");
            }
        }
        catch (Exception ex)
        {
            SetError($"Failed to load sessions: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>
    /// Refreshes sessions by clearing cache and reloading
    /// </summary>
    private async Task RefreshSessionsAsync()
    {
        var cacheKey = $"sessions_{SelectedYear}";
        _cacheService.Remove(cacheKey);
        await LoadSessionsAsync();
    }

    /// <summary>
    /// Handles session selection and navigates to details
    /// </summary>
    private async Task OnSessionSelected(Session? session)
    {
        if (session == null)
            return;

        await Shell.Current.GoToAsync($"{Constants.Routes.SessionDetail}?sessionKey={session.SessionKey}");
    }

    /// <summary>
    /// Updates the observable collection with new sessions
    /// </summary>
    private void UpdateSessionsList(List<Session> sessions)
    {
        Sessions.Clear();

        // Sort by date descending (most recent first)
        var sortedSessions = sessions.OrderByDescending(s => s.DateStart);

        foreach (var session in sortedSessions)
        {
            Sessions.Add(session);
        }
    }
}
