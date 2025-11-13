# CLAUDE.md - AI Assistant Guide for F1 Telemetry App

> **Purpose**: This document provides AI assistants with comprehensive context about the F1 Telemetry App codebase structure, architecture patterns, development workflows, and coding conventions.

**Last Updated**: 2025-11-13
**Project Version**: 1.0.0
**Target Framework**: .NET 9.0 (net9.0-android)
**Architecture**: MVVM with Dependency Injection

---

## Table of Contents

1. [Project Overview](#project-overview)
2. [Codebase Structure](#codebase-structure)
3. [Architecture & Patterns](#architecture--patterns)
4. [Development Principles](#development-principles)
5. [File Conventions](#file-conventions)
6. [Common Workflows](#common-workflows)
7. [Key Classes & Interfaces](#key-classes--interfaces)
8. [Navigation System](#navigation-system)
9. [Data Flow](#data-flow)
10. [Testing Guidelines](#testing-guidelines)
11. [Common Pitfalls](#common-pitfalls)
12. [Quick Reference](#quick-reference)

---

## Project Overview

### What is This Project?

A cross-platform mobile application built with .NET MAUI that displays Formula 1 telemetry data, race results, driver information, and session schedules using the free OpenF1 API.

### Current Status

- **Phase**: MVP/Demo Ready (Phase 3 of development)
- **Platform Support**: Android (iOS/Windows planned)
- **Implementation Status**: Core features complete, production-ready features pending
- **Code Quality**: High (clean MVVM, proper DI, good separation of concerns)

### Tech Stack

- **Framework**: .NET MAUI (Multi-platform App UI)
- **Language**: C# 12 / .NET 9
- **UI**: XAML with compiled bindings
- **Data Source**: [OpenF1 API](https://openf1.org/) (Free, no auth required)
- **Charts**: Microcharts.Maui (SkiaSharp-based)
- **Architecture**: MVVM with dependency injection
- **Storage**: In-memory caching (SQLite planned)

### Key Features Implemented

✅ Session browsing (by year)
✅ Driver listing from recent race
✅ Session details with driver roster
✅ Weather information display
✅ Lap time analysis
✅ Telemetry visualization with charts
✅ Cache management
✅ Educational F1 content
✅ Dark mode support

---

## Codebase Structure

### Directory Layout

```
/home/user/f1-telemetry-app/
├── .claude/                          # Claude agent configurations
├── Converters/                       # XAML value converters
│   └── IsNotNullConverter.cs
├── Helpers/                          # Utility classes
│   └── Constants.cs                  # ⭐ Centralized constants (READ THIS FIRST)
├── Models/                           # 📦 Domain models (7 files)
│   ├── CarData.cs                    # Telemetry data point (~3.7Hz)
│   ├── Driver.cs                     # Driver entity with team info
│   ├── Lap.cs                        # Lap timing with sector times
│   ├── Meeting.cs                    # Grand Prix weekend
│   ├── Position.cs                   # Driver standings
│   ├── Session.cs                    # Practice/Qualifying/Race
│   └── Weather.cs                    # Track conditions
├── Platforms/                        # Platform-specific code
│   └── Android/                      # Android implementations
├── Resources/                        # Static resources
│   ├── Fonts/                        # OpenSans fonts
│   ├── Images/                       # Icons and images
│   └── Styles/                       # 🎨 XAML styles
│       ├── Colors.xaml               # Color palette
│       └── Styles.xaml               # Global styles
├── Services/                         # 🔧 Business logic layer (4 files)
│   ├── ICacheService.cs              # Cache interface
│   ├── CacheService.cs               # In-memory cache implementation
│   ├── IOpenF1ApiService.cs          # API service interface
│   └── OpenF1ApiService.cs           # HTTP API client
├── ViewModels/                       # 🧠 MVVM ViewModels (7 files)
│   ├── BaseViewModel.cs              # ⭐ Abstract base with INotifyPropertyChanged
│   ├── MainViewModel.cs              # Home page navigation
│   ├── IntroViewModel.cs             # Education page
│   ├── SessionListViewModel.cs       # Session browsing
│   ├── SessionDetailViewModel.cs     # Session details + navigation params
│   ├── DriverListViewModel.cs        # Driver grid
│   └── TelemetryViewModel.cs         # Charts and telemetry (most complex)
├── Views/                            # 📱 XAML UI pages (6 pages + code-behind)
│   ├── MainPage.xaml[.cs]            # Landing page
│   ├── IntroPage.xaml[.cs]           # F1 education (417 lines!)
│   ├── SessionListPage.xaml[.cs]     # Session list with filters
│   ├── SessionDetailPage.xaml[.cs]   # Session details
│   ├── DriverListPage.xaml[.cs]      # Driver grid
│   ├── TelemetryPage.xaml[.cs]       # Telemetry charts
│   └── SettingsPage.xaml[.cs]        # App settings
├── App.xaml[.cs]                     # Application entry + resources
├── AppShell.xaml[.cs]                # 🧭 Navigation shell (TabBar + routes)
├── MauiProgram.cs                    # ⭐ DI container setup
├── F1TelemetryApp.csproj             # Project configuration
├── ARCHITECTURE.md                   # 📖 Detailed architecture docs
├── guide.md                          # 📖 Development principles
└── README.md                         # 📖 Project overview
```

### File Statistics

- **39 C# files** (excluding platform boilerplate)
- **12 XAML files**
- **7 Domain Models** (complete)
- **4 Service files** (2 interfaces, 2 implementations)
- **7 ViewModels** (all functional)
- **6 View Pages** (all implemented)

---

## Architecture & Patterns

### MVVM Implementation

```
┌──────────┐     binds to    ┌──────────────┐     calls     ┌─────────┐
│   View   │ ◄───────────────│  ViewModel   │ ◄─────────────│ Service │
│  (XAML)  │                 │   (Logic)    │               │  (API)  │
└──────────┘                 └──────────────┘               └─────────┘
                                    │
                                    │ uses
                                    ▼
                              ┌──────────┐
                              │  Model   │
                              │  (Data)  │
                              └──────────┘
```

### Dependency Injection

**Registration Location**: `MauiProgram.cs:27-46`

```csharp
// Services - Singleton lifecycle
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddHttpClient<IOpenF1ApiService, OpenF1ApiService>();

// ViewModels - Transient lifecycle (new instance per navigation)
builder.Services.AddTransient<MainViewModel>();
builder.Services.AddTransient<SessionListViewModel>();
// ... etc

// Views - Transient lifecycle
builder.Services.AddTransient<MainPage>();
builder.Services.AddTransient<SessionListPage>();
// ... etc
```

**Why This Matters**:
- Services are **singletons** (shared state, cache persistence)
- ViewModels are **transient** (fresh state per page visit)
- Views are **transient** (matched to ViewModel lifecycle)

### View-ViewModel Connection Pattern

**Standard Pattern** (used in all pages):

```csharp
// View code-behind (e.g., SessionListPage.xaml.cs)
public SessionListPage(SessionListViewModel viewModel)
{
    InitializeComponent();
    BindingContext = viewModel;  // ⭐ Connect ViewModel
}
```

```xml
<!-- XAML (e.g., SessionListPage.xaml) -->
<ContentPage xmlns="..."
             xmlns:vm="clr-namespace:F1TelemetryApp.ViewModels"
             x:DataType="vm:SessionListViewModel">  <!-- ⭐ Compiled bindings -->
    <Label Text="{Binding Title}" />  <!-- Binds to ViewModel.Title -->
</ContentPage>
```

### Service Layer Pattern

**All services follow interface-based design**:

```csharp
// Interface (contract)
public interface IOpenF1ApiService
{
    Task<List<Session>> GetSessionsAsync(int year);
    Task<List<Driver>> GetDriversAsync(int sessionKey);
    // ...
}

// Implementation
public class OpenF1ApiService : IOpenF1ApiService
{
    private readonly HttpClient _httpClient;

    public OpenF1ApiService(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri(Constants.Api.BaseUrl);
        _httpClient.Timeout = TimeSpan.FromSeconds(Constants.Api.TimeoutSeconds);
    }

    public async Task<List<Session>> GetSessionsAsync(int year)
    {
        // Implementation...
    }
}
```

**Why This Matters**:
- Easy to mock for testing
- Easy to swap implementations
- Follows Dependency Inversion Principle

### BaseViewModel Pattern

**All ViewModels inherit from `BaseViewModel`** (`ViewModels/BaseViewModel.cs:10`):

```csharp
public abstract class BaseViewModel : INotifyPropertyChanged
{
    // Common properties
    public bool IsBusy { get; set; }
    public bool IsNotBusy => !IsBusy;
    public string Title { get; set; }
    public string ErrorMessage { get; set; }
    public bool HasError => !string.IsNullOrEmpty(ErrorMessage);

    // Helper methods
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    protected void OnPropertyChanged([CallerMemberName] string? propertyName = null)
    protected void ClearError()
    protected void SetError(string message)
}
```

**When Creating New ViewModels**:
1. Always inherit from `BaseViewModel`
2. Use `SetProperty()` for property setters (automatic change notification)
3. Set `IsBusy = true` during async operations
4. Use `SetError()` / `ClearError()` for error handling

---

## Development Principles

### Strictly Enforced Principles (from `guide.md`)

#### 1. KISS (Keep It Simple, Stupid)
- Prefer straightforward, clear solutions
- Avoid over-engineering and unnecessary complexity

#### 2. YAGNI (You Aren't Gonna Need It)
- Avoid speculative features
- Implement only what's needed **now**

#### 3. SOLID Principles
- **S**ingle Responsibility Principle
- **O**pen/Closed Principle
- **L**iskov Substitution Principle
- **I**nterface Segregation Principle
- **D**ependency Inversion Principle

#### 4. DRY (Don't Repeat Yourself)
- Avoid duplication
- Centralize logic when it makes sense

### Forbidden Patterns (DO NOT DO THIS)

❌ Add "just in case" features
❌ Create abstractions without immediate, concrete use
❌ Mix multiple responsibilities in one module/class
❌ Implement future requirements before they're needed
❌ Prematurely optimize

### Response Structure (Always Follow)

When proposing work or reporting changes:

1. **Requirement Clarification**
2. **Core Solution Design**
3. **Implementation Details**
4. **Key Design Decisions**
5. **Validation Results**

---

## File Conventions

### Naming Conventions

| Type | Convention | Example |
|------|-----------|---------|
| Models | PascalCase, singular | `Driver.cs`, `Session.cs` |
| Services | `I{Name}Service.cs` (interface)<br>`{Name}Service.cs` (impl) | `IOpenF1ApiService.cs`<br>`OpenF1ApiService.cs` |
| ViewModels | `{Name}ViewModel.cs` | `SessionListViewModel.cs` |
| Views | `{Name}Page.xaml[.cs]` | `SessionListPage.xaml` |
| Helpers | `{Purpose}.cs` | `Constants.cs` |
| Converters | `{Purpose}Converter.cs` | `IsNotNullConverter.cs` |

### Code Structure Patterns

#### Model Structure (Example: `Models/Driver.cs`)

```csharp
namespace F1TelemetryApp.Models;

/// <summary>
/// Represents a driver in a specific session.
/// Maps to OpenF1 API /drivers endpoint.
/// </summary>
public class Driver
{
    // Properties matching API response (PascalCase)
    public int SessionKey { get; set; }
    public int DriverNumber { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string TeamName { get; set; } = string.Empty;
    public string TeamColour { get; set; } = string.Empty;

    // Computed properties for UI (read-only)
    public Color GetTeamColor()
    {
        // Helper method for UI
    }
}
```

**Key Points**:
- XML documentation comments required
- Properties match API response exactly
- Computed properties are methods or read-only properties
- String properties default to `string.Empty` (nullable enabled)

#### ViewModel Structure Pattern

```csharp
namespace F1TelemetryApp.ViewModels;

public class SessionListViewModel : BaseViewModel
{
    private readonly IOpenF1ApiService _apiService;
    private readonly ICacheService _cacheService;

    // Constructor injection
    public SessionListViewModel(IOpenF1ApiService apiService, ICacheService cacheService)
    {
        _apiService = apiService;
        _cacheService = cacheService;
        Title = "Sessions";  // Set page title

        // Initialize commands
        LoadSessionsCommand = new Command(async () => await LoadSessionsAsync());
    }

    // Observable collections for data binding
    public ObservableCollection<Session> Sessions { get; } = new();

    // Commands
    public Command LoadSessionsCommand { get; }

    // Private async methods
    private async Task LoadSessionsAsync()
    {
        if (IsBusy) return;

        IsBusy = true;
        ClearError();

        try
        {
            // Check cache first
            var cacheKey = $"sessions_{year}";
            var cached = _cacheService.Get<List<Session>>(cacheKey);

            if (cached != null)
            {
                UpdateSessions(cached);
                return;
            }

            // Fetch from API
            var sessions = await _apiService.GetSessionsAsync(year);

            // Update cache
            _cacheService.Set(cacheKey, sessions, Constants.Cache.SessionCacheDuration);

            // Update UI
            UpdateSessions(sessions);
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

    private void UpdateSessions(List<Session> sessions)
    {
        Sessions.Clear();
        foreach (var session in sessions)
        {
            Sessions.Add(session);
        }
    }
}
```

**Key Patterns**:
1. Constructor injection of services
2. Set `Title` in constructor
3. Initialize commands in constructor
4. Use `ObservableCollection<T>` for lists
5. Always check `IsBusy` before operations
6. Always use try-catch with `SetError()`
7. Always set `IsBusy = false` in finally
8. Check cache before API calls

#### View Code-Behind Pattern

```csharp
namespace F1TelemetryApp.Views;

public partial class SessionListPage : ContentPage
{
    public SessionListPage(SessionListViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    // Optional: Page lifecycle methods
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Trigger data load when page appears
        if (BindingContext is SessionListViewModel vm)
        {
            vm.LoadSessionsCommand.Execute(null);
        }
    }
}
```

**Key Patterns**:
1. Constructor injection of ViewModel
2. Set `BindingContext` in constructor
3. Minimal logic in code-behind (delegate to ViewModel)
4. Use `OnAppearing()` for data loading

---

## Common Workflows

### Adding a New Feature (Complete Workflow)

#### 1. Add Model (if needed)

```bash
# Create model file
touch Models/TeamStanding.cs
```

```csharp
// Models/TeamStanding.cs
namespace F1TelemetryApp.Models;

/// <summary>
/// Represents constructor championship standings.
/// </summary>
public class TeamStanding
{
    public int Position { get; set; }
    public string TeamName { get; set; } = string.Empty;
    public int Points { get; set; }
}
```

#### 2. Add Service Method (if needed)

```csharp
// Services/IOpenF1ApiService.cs
public interface IOpenF1ApiService
{
    // ... existing methods
    Task<List<TeamStanding>> GetTeamStandingsAsync(int year);
}

// Services/OpenF1ApiService.cs
public async Task<List<TeamStanding>> GetTeamStandingsAsync(int year)
{
    try
    {
        var response = await _httpClient.GetStringAsync($"standings?year={year}");
        return JsonSerializer.Deserialize<List<TeamStanding>>(response) ?? new();
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Error fetching team standings: {ex.Message}");
        return new List<TeamStanding>();
    }
}
```

#### 3. Create ViewModel

```csharp
// ViewModels/TeamStandingsViewModel.cs
namespace F1TelemetryApp.ViewModels;

public class TeamStandingsViewModel : BaseViewModel
{
    private readonly IOpenF1ApiService _apiService;

    public TeamStandingsViewModel(IOpenF1ApiService apiService)
    {
        _apiService = apiService;
        Title = "Team Standings";
        LoadStandingsCommand = new Command(async () => await LoadStandingsAsync());
    }

    public ObservableCollection<TeamStanding> Standings { get; } = new();
    public Command LoadStandingsCommand { get; }

    private async Task LoadStandingsAsync()
    {
        if (IsBusy) return;
        IsBusy = true;
        ClearError();

        try
        {
            var standings = await _apiService.GetTeamStandingsAsync(DateTime.UtcNow.Year);
            Standings.Clear();
            foreach (var standing in standings)
            {
                Standings.Add(standing);
            }
        }
        catch (Exception ex)
        {
            SetError($"Failed to load standings: {ex.Message}");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
```

#### 4. Create View

```xml
<!-- Views/TeamStandingsPage.xaml -->
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             xmlns:vm="clr-namespace:F1TelemetryApp.ViewModels"
             x:Class="F1TelemetryApp.Views.TeamStandingsPage"
             x:DataType="vm:TeamStandingsViewModel"
             Title="{Binding Title}">

    <Grid RowDefinitions="Auto,*">
        <!-- Loading Indicator -->
        <ActivityIndicator Grid.Row="0" IsRunning="{Binding IsBusy}" />

        <!-- Error Message -->
        <Label Grid.Row="0"
               Text="{Binding ErrorMessage}"
               IsVisible="{Binding HasError}"
               TextColor="Red" />

        <!-- Data List -->
        <CollectionView Grid.Row="1"
                       ItemsSource="{Binding Standings}"
                       IsVisible="{Binding IsNotBusy}">
            <CollectionView.ItemTemplate>
                <DataTemplate x:DataType="models:TeamStanding">
                    <Grid ColumnDefinitions="Auto,*,Auto" Padding="10">
                        <Label Grid.Column="0" Text="{Binding Position}" />
                        <Label Grid.Column="1" Text="{Binding TeamName}" />
                        <Label Grid.Column="2" Text="{Binding Points}" />
                    </Grid>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>
    </Grid>
</ContentPage>
```

```csharp
// Views/TeamStandingsPage.xaml.cs
namespace F1TelemetryApp.Views;

public partial class TeamStandingsPage : ContentPage
{
    public TeamStandingsPage(TeamStandingsViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (BindingContext is TeamStandingsViewModel vm)
        {
            vm.LoadStandingsCommand.Execute(null);
        }
    }
}
```

#### 5. Register in DI Container

```csharp
// MauiProgram.cs
builder.Services.AddTransient<TeamStandingsViewModel>();
builder.Services.AddTransient<TeamStandingsPage>();
```

#### 6. Add Route and Navigation

```csharp
// Helpers/Constants.cs
public static class Routes
{
    // ... existing routes
    public const string TeamStandings = "team_standings";
}

// AppShell.xaml.cs
public AppShell()
{
    InitializeComponent();

    // ... existing routes
    Routing.RegisterRoute(Constants.Routes.TeamStandings, typeof(TeamStandingsPage));
}
```

#### 7. Navigate to New Page

```csharp
// From any ViewModel
await Shell.Current.GoToAsync(Constants.Routes.TeamStandings);
```

---

## Key Classes & Interfaces

### Core Services

#### IOpenF1ApiService (`Services/IOpenF1ApiService.cs`)

**Purpose**: Contract for OpenF1 API access

**Methods**:
```csharp
Task<List<Session>> GetSessionsAsync(int year);
Task<Session?> GetSessionByKeyAsync(int sessionKey);
Task<List<Driver>> GetDriversAsync(int sessionKey);
Task<List<Lap>> GetLapsAsync(int sessionKey, int driverNumber);
Task<List<CarData>> GetCarDataAsync(int sessionKey, int driverNumber);
Task<List<Weather>> GetWeatherAsync(int sessionKey);
Task<List<Position>> GetPositionsAsync(int sessionKey);
Task<List<Meeting>> GetMeetingsAsync(int year);
```

**Implementation Notes**:
- Base URL: `https://api.openf1.org/v1/`
- Timeout: 30 seconds
- No authentication required
- Returns empty list on error (not null)
- Uses `JsonSerializer` with case-insensitive deserialization

#### ICacheService (`Services/ICacheService.cs`)

**Purpose**: Generic caching interface

**Methods**:
```csharp
T? Get<T>(string key);
void Set<T>(string key, T value, TimeSpan expiration);
void Remove(string key);
void Clear();
bool Exists(string key);
```

**Implementation**: In-memory cache with `ConcurrentDictionary` (thread-safe)

**Cache Durations** (from `Constants.Cache`):
- Telemetry: 1 minute
- Sessions: 1 hour
- Historical: 1 day
- Drivers: 7 days

### Constants Class (`Helpers/Constants.cs`)

**⭐ ALWAYS CHECK THIS FILE FIRST** when:
- Adding API endpoints
- Modifying cache durations
- Adding navigation routes
- Changing app settings

**Structure**:
```csharp
Constants.Api.BaseUrl                    // "https://api.openf1.org/v1/"
Constants.Api.TimeoutSeconds             // 30
Constants.Api.MaxRetries                 // 3

Constants.Cache.TelemetryCacheDuration   // 1 minute
Constants.Cache.SessionCacheDuration     // 1 hour
Constants.Cache.HistoricalCacheDuration  // 1 day
Constants.Cache.DriverCacheDuration      // 7 days

Constants.Routes.Main                    // "main"
Constants.Routes.SessionDetail           // "session_detail"
Constants.Routes.Telemetry               // "telemetry"
// ... etc

Constants.Settings.DefaultYear           // DateTime.UtcNow.Year
Constants.Settings.PageSize              // 20
```

---

## Navigation System

### Navigation Structure

```
AppShell (TabBar)
├── Tab: Home (main) → MainPage
├── Tab: Sessions (sessions) → SessionListPage
├── Tab: Drivers (drivers) → DriverListPage
└── Tab: Settings (settings) → SettingsPage

Registered Routes (push navigation)
├── intro → IntroPage
├── session_detail → SessionDetailPage
└── telemetry → TelemetryPage
```

### Navigation Patterns

#### Tab Navigation (Shell Routes)

```csharp
// Navigate to a tab
await Shell.Current.GoToAsync($"//{Constants.Routes.SessionList}");
```

#### Push Navigation (Modal/Detail Pages)

```csharp
// Navigate to detail page
await Shell.Current.GoToAsync(Constants.Routes.SessionDetail);

// With query parameters
await Shell.Current.GoToAsync($"{Constants.Routes.SessionDetail}?sessionKey={sessionKey}");

// With multiple parameters
await Shell.Current.GoToAsync(
    $"{Constants.Routes.Telemetry}?sessionKey={sessionKey}&driverNumber={driverNumber}"
);
```

#### Receiving Navigation Parameters

**ViewModel must implement `IQueryAttributable`**:

```csharp
public class SessionDetailViewModel : BaseViewModel, IQueryAttributable
{
    private int _sessionKey;

    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("sessionKey"))
        {
            _sessionKey = int.Parse(query["sessionKey"].ToString()!);
            _ = LoadSessionDetailsAsync();  // Trigger data load
        }
    }

    private async Task LoadSessionDetailsAsync()
    {
        // Use _sessionKey to fetch data
    }
}
```

#### Back Navigation

```csharp
// Go back one page
await Shell.Current.GoToAsync("..");

// Go back to root of current tab
await Shell.Current.GoToAsync("//");
```

---

## Data Flow

### Complete Data Flow Example: Loading Session List

```
User Opens Sessions Tab
        │
        ▼
SessionListPage.OnAppearing()
        │
        ▼
SessionListViewModel.LoadSessionsCommand.Execute()
        │
        ▼
LoadSessionsAsync()
        │
        ├─► Set IsBusy = true
        ├─► ClearError()
        │
        ▼
Check ICacheService.Get<List<Session>>("sessions_2025")
        │
        ├─► Cache Hit? ──Yes──► UpdateSessions(cached) ──► IsBusy = false ──► UI Updates
        │
        └─► Cache Miss? ──No───┐
                               │
                               ▼
IOpenF1ApiService.GetSessionsAsync(2025)
                               │
                               ▼
HttpClient GET https://api.openf1.org/v1/sessions?year=2025
                               │
                               ▼
Deserialize JSON → List<Session>
                               │
                               ▼
ICacheService.Set("sessions_2025", sessions, 1 hour)
                               │
                               ▼
UpdateSessions(sessions)
                               │
                               ▼
Sessions.Clear() + Sessions.Add(each)
                               │
                               ▼
IsBusy = false
                               │
                               ▼
UI Automatically Updates (ObservableCollection binding)
```

### Error Handling Flow

```
Exception Thrown
        │
        ▼
Caught in try-catch
        │
        ▼
SetError("User-friendly message")
        │
        ├─► ErrorMessage property set
        ├─► HasError becomes true
        │
        ▼
UI shows error label (IsVisible="{Binding HasError}")
        │
        ▼
finally: IsBusy = false
```

### Chart Generation Flow (TelemetryViewModel)

```
LoadTelemetryAsync()
        │
        ├─► Fetch Laps from API/Cache
        ├─► Fetch CarData from API/Cache
        │
        ▼
GenerateLapTimesChart()
        │
        ├─► Filter valid laps (where LapDuration != null)
        ├─► Transform to ChartEntry[] with lap number + duration
        │
        ▼
Create LineChart instance
        │
        ├─► Set Entries = chartEntries
        ├─► Set LabelTextSize, LineSize, PointSize
        ├─► Set ValueLabelOrientation
        │
        ▼
Set LapTimesChart property
        │
        ▼
ChartView.Chart binding updates → Microcharts renders
```

---

## Testing Guidelines

### Current State

❌ **No tests currently implemented** (0% coverage)

### Recommended Testing Strategy

#### Unit Tests (ViewModels)

**Example: SessionListViewModel Tests**

```csharp
[Fact]
public async Task LoadSessionsAsync_CacheHit_ShouldNotCallApi()
{
    // Arrange
    var mockCache = new Mock<ICacheService>();
    var mockApi = new Mock<IOpenF1ApiService>();
    var cachedSessions = new List<Session> { new Session { SessionName = "Test" } };

    mockCache.Setup(c => c.Get<List<Session>>(It.IsAny<string>()))
             .Returns(cachedSessions);

    var vm = new SessionListViewModel(mockApi.Object, mockCache.Object);

    // Act
    await vm.LoadSessionsCommand.Execute(null);

    // Assert
    mockApi.Verify(a => a.GetSessionsAsync(It.IsAny<int>()), Times.Never);
    Assert.Single(vm.Sessions);
}
```

#### Integration Tests (Services)

**Example: OpenF1ApiService Tests**

```csharp
[Fact]
public async Task GetSessionsAsync_ValidYear_ReturnsData()
{
    // Arrange
    var httpClient = new HttpClient();
    var service = new OpenF1ApiService(httpClient);

    // Act
    var sessions = await service.GetSessionsAsync(2024);

    // Assert
    Assert.NotEmpty(sessions);
    Assert.All(sessions, s => Assert.Equal(2024, s.Year));
}
```

### Testing Best Practices

1. **Mock services in ViewModel tests** (use Moq or NSubstitute)
2. **Test error handling** (verify SetError is called)
3. **Test IsBusy states** (verify true during operation, false after)
4. **Test caching logic** (cache hit vs miss scenarios)
5. **Integration tests for API** (use real endpoints sparingly)

---

## Common Pitfalls

### ❌ Pitfall 1: Forgetting IsBusy Guards

**Wrong**:
```csharp
private async Task LoadDataAsync()
{
    IsBusy = true;
    var data = await _apiService.GetDataAsync();
    UpdateUI(data);
    IsBusy = false;
}
```

**Right**:
```csharp
private async Task LoadDataAsync()
{
    if (IsBusy) return;  // ⭐ Guard against multiple concurrent loads

    IsBusy = true;
    try
    {
        var data = await _apiService.GetDataAsync();
        UpdateUI(data);
    }
    finally
    {
        IsBusy = false;  // ⭐ Always in finally block
    }
}
```

### ❌ Pitfall 2: Not Using SetProperty

**Wrong**:
```csharp
private string _title;
public string Title
{
    get => _title;
    set => _title = value;  // ❌ UI won't update!
}
```

**Right**:
```csharp
private string _title = string.Empty;
public string Title
{
    get => _title;
    set => SetProperty(ref _title, value);  // ✅ Triggers PropertyChanged
}
```

### ❌ Pitfall 3: Modifying ObservableCollection Incorrectly

**Wrong**:
```csharp
Sessions = new ObservableCollection<Session>(newSessions);  // ❌ Breaks binding!
```

**Right**:
```csharp
Sessions.Clear();
foreach (var session in newSessions)
{
    Sessions.Add(session);
}
```

### ❌ Pitfall 4: Not Registering in DI Container

**Symptom**: `NullReferenceException` when navigating to page

**Fix**: Always register in `MauiProgram.cs`:
```csharp
builder.Services.AddTransient<YourViewModel>();
builder.Services.AddTransient<YourPage>();
```

### ❌ Pitfall 5: Not Registering Routes

**Symptom**: Navigation fails silently or crashes

**Fix**: Register in `AppShell.xaml.cs`:
```csharp
Routing.RegisterRoute(Constants.Routes.YourRoute, typeof(YourPage));
```

### ❌ Pitfall 6: Hardcoding Values

**Wrong**:
```csharp
await Shell.Current.GoToAsync("session_detail");  // ❌ Magic string
var timeout = TimeSpan.FromSeconds(30);          // ❌ Magic number
```

**Right**:
```csharp
await Shell.Current.GoToAsync(Constants.Routes.SessionDetail);
var timeout = TimeSpan.FromSeconds(Constants.Api.TimeoutSeconds);
```

### ❌ Pitfall 7: Not Handling Errors

**Wrong**:
```csharp
private async Task LoadDataAsync()
{
    var data = await _apiService.GetDataAsync();  // ❌ No error handling
    UpdateUI(data);
}
```

**Right**:
```csharp
private async Task LoadDataAsync()
{
    try
    {
        var data = await _apiService.GetDataAsync();
        UpdateUI(data);
    }
    catch (Exception ex)
    {
        SetError($"Failed to load data: {ex.Message}");
    }
}
```

---

## Quick Reference

### Commands Cheat Sheet

```bash
# Build project
dotnet build

# Run on Android
dotnet build -t:Run -f net9.0-android

# Restore dependencies
dotnet restore

# Clean build artifacts
dotnet clean

# Format code
dotnet format
```

### File Creation Quick Commands

```bash
# Create new Model
touch Models/NewModel.cs

# Create new Service (interface + implementation)
touch Services/INewService.cs Services/NewService.cs

# Create new ViewModel
touch ViewModels/NewViewModel.cs

# Create new View
touch Views/NewPage.xaml Views/NewPage.xaml.cs
```

### Code Snippets

#### Command Property

```csharp
public Command LoadDataCommand { get; }

// In constructor:
LoadDataCommand = new Command(async () => await LoadDataAsync());
```

#### ObservableCollection

```csharp
public ObservableCollection<MyModel> Items { get; } = new();
```

#### Navigation with Parameters

```csharp
await Shell.Current.GoToAsync($"{Constants.Routes.Detail}?id={item.Id}");
```

#### Implementing IQueryAttributable

```csharp
public class MyViewModel : BaseViewModel, IQueryAttributable
{
    public void ApplyQueryAttributes(IDictionary<string, object> query)
    {
        if (query.ContainsKey("id"))
        {
            var id = int.Parse(query["id"].ToString()!);
            _ = LoadDetailsAsync(id);
        }
    }
}
```

### OpenF1 API Endpoints Reference

| Endpoint | Purpose | Example |
|----------|---------|---------|
| `/sessions?year=2024` | Get all sessions for a year | Sessions list |
| `/sessions?session_key=9158` | Get specific session | Session details |
| `/drivers?session_key=9158` | Get drivers in session | Driver roster |
| `/laps?session_key=9158&driver_number=1` | Get lap data | Lap times |
| `/car_data?session_key=9158&driver_number=1` | Get telemetry | Speed, throttle, etc. |
| `/weather?session_key=9158` | Get weather data | Track conditions |
| `/meetings?year=2024` | Get Grand Prix weekends | Race calendar |
| `/position?session_key=9158` | Get driver positions | Race positions |

### Cache Key Patterns

```csharp
// Session cache keys
$"sessions_{year}"
$"session_{sessionKey}"

// Driver cache keys
$"drivers_{sessionKey}"
$"driver_{sessionKey}_{driverNumber}"

// Telemetry cache keys
$"laps_{sessionKey}_{driverNumber}"
$"cardata_{sessionKey}_{driverNumber}"

// Weather cache keys
$"weather_{sessionKey}"
```

---

## Implementation Status Summary

### ✅ Fully Implemented

- MVVM architecture with BaseViewModel
- Dependency injection setup
- All 7 domain models
- All 8 OpenF1 API endpoints
- All 7 ViewModels
- All 6 Views
- Navigation system (tabs + routes)
- In-memory caching
- Chart visualization (Microcharts)
- Dark mode support
- Error handling (basic)

### ⚠️ Partially Implemented

- Caching (no persistent storage)
- Error handling (no retry logic)
- Logging (console only, no framework)

### ❌ Not Implemented

- SQLite persistence
- Unit/integration tests
- Retry policies (Polly)
- Driver detail page (route exists, page doesn't)
- Real-time data streaming
- Background synchronization
- iOS/Windows builds
- Accessibility features
- Localization

---

## Getting Help

### Documentation Files

1. **README.md** - Project overview, setup instructions
2. **ARCHITECTURE.md** - Detailed architecture documentation
3. **guide.md** - Development principles and workflow
4. **CLAUDE.md** (this file) - AI assistant guide

### When Adding Features

1. Check if similar feature exists (e.g., SessionListViewModel for reference)
2. Follow the "Adding a New Feature" workflow above
3. Always use existing patterns (BaseViewModel, service interfaces, etc.)
4. Update Constants.cs for new routes/settings
5. Register everything in MauiProgram.cs

### When Debugging

1. Check DI registration (MauiProgram.cs)
2. Check route registration (AppShell.xaml.cs)
3. Verify BindingContext is set
4. Check `x:DataType` in XAML
5. Look for console errors (no proper logging yet)

### When Reviewing Code

Use the structured response format:
1. Requirement Clarification
2. Core Solution Design
3. Implementation Details
4. Key Design Decisions
5. Validation Results

---

## Revision History

| Date | Version | Changes |
|------|---------|---------|
| 2025-11-13 | 1.0.0 | Initial comprehensive documentation |

---

**End of CLAUDE.md**

*This document is maintained by the development team and should be updated whenever significant architectural changes are made.*
