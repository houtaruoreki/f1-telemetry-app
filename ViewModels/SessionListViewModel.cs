namespace F1TelemetryApp.ViewModels;

using System.Collections.ObjectModel;
using System.Windows.Input;
using F1TelemetryApp.Helpers;
using F1TelemetryApp.Models;
using F1TelemetryApp.Services;

/// <summary>
/// ViewModel for displaying a list of F1 sessions.
/// Allows filtering by year and session type, with grouping by Grand Prix.
/// </summary>
public class SessionListViewModel : BaseViewModel
{
    private readonly IOpenF1ApiService _apiService;
    private readonly ICacheService _cacheService;
    private int _selectedYear;
    private int _selectedYearIndex;
    private string _selectedFilter = "All";
    private List<Session> _allSessions = new();

    public SessionListViewModel(IOpenF1ApiService apiService, ICacheService cacheService)
    {
        _apiService = apiService;
        _cacheService = cacheService;
        _selectedYear = Constants.Settings.DefaultYear;
        _selectedYearIndex = 0; // Default to 2024

        Title = "Sessions";
        SessionGroups = new ObservableCollection<SessionGroup>();
        FilterOptions = new ObservableCollection<string> { "All", "Race", "Qualifying", "Practice", "Sprint" };

        LoadSessionsCommand = new Command(async () => await LoadSessionsAsync());
        RefreshCommand = new Command(async () => await RefreshSessionsAsync());
        SessionSelectedCommand = new Command<Session>(async (session) => await OnSessionSelected(session));
        ToggleGroupCommand = new Command<SessionGroup>(ToggleGroup);

        // Auto-load sessions on startup
        _ = LoadSessionsAsync();
    }

    public ObservableCollection<SessionGroup> SessionGroups { get; }
    public ObservableCollection<string> FilterOptions { get; }

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

    /// <summary>
    /// Currently selected filter (All, Race, Qualifying, etc.)
    /// </summary>
    public string SelectedFilter
    {
        get => _selectedFilter;
        set
        {
            if (SetProperty(ref _selectedFilter, value))
            {
                ApplyFilter();
            }
        }
    }

    public ICommand LoadSessionsCommand { get; }
    public ICommand RefreshCommand { get; }
    public ICommand SessionSelectedCommand { get; }
    public ICommand ToggleGroupCommand { get; }

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
                _allSessions = cachedSessions;
                ApplyFilter();
                return;
            }

            // Fetch from API
            var sessions = await _apiService.GetSessionsAsync(SelectedYear);

            if (sessions.Any())
            {
                // Cache the results
                _cacheService.Set(cacheKey, sessions, Constants.Cache.SessionCacheDuration);
                _allSessions = sessions;
                ApplyFilter();
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
    /// Toggles the expanded state of a session group
    /// </summary>
    private void ToggleGroup(SessionGroup? group)
    {
        if (group == null)
            return;

        group.IsExpanded = !group.IsExpanded;
        OnPropertyChanged(nameof(SessionGroups));
    }

    /// <summary>
    /// Applies the selected filter and groups sessions
    /// </summary>
    private void ApplyFilter()
    {
        // Filter sessions based on selected filter
        var filteredSessions = SelectedFilter == "All"
            ? _allSessions
            : _allSessions.Where(s => s.SessionType?.Contains(SelectedFilter, StringComparison.OrdinalIgnoreCase) == true).ToList();

        // Group sessions by meeting
        var grouped = filteredSessions
            .OrderByDescending(s => s.DateStart)
            .GroupBy(s => new
            {
                s.MeetingKey,
                Location = s.Location ?? "Unknown Location",
                CircuitName = s.CircuitShortName ?? s.CircuitKey ?? "Unknown Circuit",
                CountryName = s.CountryName ?? s.CountryCode ?? "",
                s.DateStart
            })
            .Select(g =>
            {
                var group = new SessionGroup(
                    g.First().MeetingOfficialName ?? $"{g.Key.CountryName} Grand Prix",
                    g.Key.Location,
                    g.Key.CircuitName,
                    g.Key.DateStart
                );

                // Add sessions to the group, sorted within each group
                var sessions = g.OrderBy(s => GetSessionOrder(s.SessionType)).ToList();
                foreach (var session in sessions)
                {
                    group.Add(session);
                }

                return group;
            });

        // Update the observable collection
        SessionGroups.Clear();
        foreach (var group in grouped)
        {
            SessionGroups.Add(group);
        }
    }

    /// <summary>
    /// Returns the sort order for session types
    /// </summary>
    private static int GetSessionOrder(string? sessionType)
    {
        return sessionType?.ToLower() switch
        {
            var s when s?.Contains("practice") == true => 1,
            var s when s?.Contains("qualifying") == true => 2,
            var s when s?.Contains("sprint") == true => 3,
            var s when s?.Contains("race") == true => 4,
            _ => 5
        };
    }
}
