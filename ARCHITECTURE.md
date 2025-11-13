# Architecture Overview

## Design Pattern: MVVM (Model-View-ViewModel)

This application follows the Model-View-ViewModel pattern with clean architecture principles.

## Project Structure

```
F1TelemetryApp/
│
├── Models/                          # Domain models (pure C# objects)
│   ├── Driver.cs                    # Driver entity
│   ├── Team.cs                      # Team/Constructor entity
│   ├── Session.cs                   # Practice/Qualifying/Race session
│   ├── Meeting.cs                   # Grand Prix weekend
│   ├── Lap.cs                       # Lap data with sector times
│   ├── CarData.cs                   # Telemetry data point
│   ├── Weather.cs                   # Weather conditions
│   └── Position.cs                  # Driver position/standings
│
├── Services/                        # Business logic and data access
│   ├── IOpenF1ApiService.cs         # API service interface
│   ├── OpenF1ApiService.cs          # API client implementation
│   ├── ICacheService.cs             # Caching interface
│   ├── CacheService.cs              # In-memory/persistent caching
│   └── IConnectivityService.cs      # Network status checking
│
├── Data/                            # Data persistence layer
│   ├── Entities/                    # Database entities (if different from models)
│   ├── Repositories/                # Repository pattern implementations
│   │   ├── ISessionRepository.cs
│   │   └── SessionRepository.cs
│   └── AppDbContext.cs              # SQLite database context (future)
│
├── ViewModels/                      # MVVM ViewModels
│   ├── BaseViewModel.cs             # Common ViewModel functionality
│   ├── MainViewModel.cs             # Main page ViewModel
│   ├── SessionListViewModel.cs      # Session list
│   ├── SessionDetailViewModel.cs    # Individual session details
│   ├── DriverListViewModel.cs       # Driver list
│   └── TelemetryViewModel.cs        # Telemetry visualization
│
├── Views/                           # XAML UI pages
│   ├── MainPage.xaml                # Home/Dashboard
│   ├── SessionListPage.xaml         # List of sessions
│   ├── SessionDetailPage.xaml       # Session details
│   ├── DriverListPage.xaml          # Drivers and teams
│   ├── TelemetryPage.xaml           # Telemetry charts
│   └── SettingsPage.xaml            # App settings
│
├── Helpers/                         # Utility classes
│   ├── Constants.cs                 # App-wide constants
│   ├── HttpClientFactory.cs         # HTTP client configuration
│   └── DateTimeHelpers.cs           # Date/time utilities
│
├── Resources/                       # Static resources
│   ├── Styles/                      # XAML styles and themes
│   ├── Images/                      # Images and icons
│   └── Fonts/                       # Custom fonts
│
├── Platforms/                       # Platform-specific code
│   └── Android/                     # Android implementations
│
├── App.xaml[.cs]                    # Application entry and DI setup
├── AppShell.xaml[.cs]               # Navigation shell
└── MauiProgram.cs                   # Service registration and configuration
```

## Architecture Layers

### 1. Presentation Layer (Views + ViewModels)
- **Views**: XAML files defining UI structure
- **ViewModels**: Business logic, state management, data binding
- **Communication**: INotifyPropertyChanged, Commands, ObservableCollections

### 2. Business Logic Layer (Services)
- **API Services**: Handle HTTP communication with OpenF1 API
- **Cache Services**: Manage data caching and offline access
- **Domain Services**: Complex business rules and data transformation

### 3. Data Access Layer (Repositories + Data)
- **Repositories**: Abstract data source (API, database, cache)
- **Database Context**: SQLite for local persistence (future)
- **Entities**: Data transfer objects for storage

## Dependency Flow

```
View → ViewModel → Service → Repository → Data Source (API/DB)
```

### Dependency Injection

All services and ViewModels registered in `MauiProgram.cs`:

```csharp
builder.Services.AddSingleton<IOpenF1ApiService, OpenF1ApiService>();
builder.Services.AddSingleton<ICacheService, CacheService>();
builder.Services.AddTransient<MainViewModel>();
builder.Services.AddTransient<SessionListViewModel>();
```

## Data Flow Examples

### Loading Session List

1. **User** opens SessionListPage
2. **View** binds to SessionListViewModel
3. **ViewModel** calls `IOpenF1ApiService.GetSessionsAsync()`
4. **Service** checks `ICacheService` for cached data
5. **Service** fetches from API if cache miss or expired
6. **Service** updates cache and returns data
7. **ViewModel** updates `ObservableCollection<Session>`
8. **View** displays updated list via data binding

### Handling Offline Mode

1. **ViewModel** checks `IConnectivityService.IsConnected`
2. If offline, service only queries cache
3. UI shows cached data with "offline" indicator
4. On reconnection, background sync updates cache

## Key Design Decisions

### 1. Repository Pattern
- Abstracts data sources (API vs database)
- Enables easy testing with mock repositories
- Single source of truth for data access

### 2. MVVM Pattern
- Separation of concerns: UI logic from business logic
- Testable ViewModels without UI dependencies
- Data binding reduces boilerplate code

### 3. Interface-Based Design
- All services implement interfaces
- Dependency inversion principle
- Easy to swap implementations (testing, different APIs)

### 4. Caching Strategy
- **Short-lived cache**: Telemetry data (1-5 minutes)
- **Medium cache**: Session data (1 hour)
- **Long cache**: Historical results (1 day)
- **Persistent cache**: Driver/team info (SQLite)

### 5. Error Handling
- Service layer catches and logs HTTP errors
- ViewModels expose error states to UI
- User-friendly error messages
- Retry logic with exponential backoff

## API Client Design

### Base Service Pattern

```csharp
public interface IOpenF1ApiService
{
    Task<List<Session>> GetSessionsAsync(int year, CancellationToken ct = default);
    Task<Session> GetSessionByKeyAsync(int sessionKey, CancellationToken ct = default);
    Task<List<Driver>> GetDriversAsync(int sessionKey, CancellationToken ct = default);
    Task<List<Lap>> GetLapsAsync(int sessionKey, int driverNumber, CancellationToken ct = default);
    Task<List<CarData>> GetCarDataAsync(int sessionKey, int driverNumber, CancellationToken ct = default);
}
```

### HTTP Client Configuration
- Singleton HttpClient with proper disposal
- Base address configuration
- Timeout handling
- Retry policies (Polly library - future)

## Testing Strategy

### Unit Tests
- ViewModels with mocked services
- Service logic with mocked HTTP clients
- Model validation and transformations

### Integration Tests
- API client with real endpoints (limited)
- Repository with in-memory SQLite
- Cache behavior validation

## Performance Considerations

### Lazy Loading
- Load telemetry data only when viewing details
- Paginate large lists (race history)
- On-demand image loading for driver photos

### Memory Management
- Dispose ViewModels when leaving pages
- Clear collections when not visible
- Cache eviction policies

### UI Responsiveness
- All API calls are async
- Loading indicators for long operations
- Background data synchronization

## Security Considerations

### API Keys
- No authentication required for OpenF1 (historical data)
- Future: Secure storage for real-time API keys (if needed)

### Data Validation
- Validate all API responses
- Sanitize user inputs (search, filters)
- Handle malformed data gracefully

## Future Enhancements

### Phase 1 (Current)
- Basic API integration
- Simple data display
- In-memory caching

### Phase 2
- SQLite persistence
- Offline-first architecture
- Background synchronization

### Phase 3
- Telemetry visualization (charts)
- Advanced filtering and search
- User preferences

### Phase 4
- Real-time data (paid OpenF1 tier)
- Push notifications for race events
- Social features (sharing data)
