# F1 Telemetry App

A cross-platform mobile application for viewing Formula 1 telemetry data, race results, driver information, and session schedules. Built with .NET MAUI targeting Android.

## Features (Planned)

- **Live Race Telemetry**: Real-time speed, position, tire data during sessions
- **Historical Results**: Past race results, standings, and lap times
- **Driver & Team Information**: Profiles, statistics, and season standings
- **Session Schedules**: Race calendar with practice, qualifying, and race times
- **Telemetry Visualization**: Charts and graphs for data analysis
- **Offline Support**: Local caching for historical data access

## Tech Stack

- **Framework**: .NET MAUI (Multi-platform App UI)
- **Language**: C# 12 / .NET 9
- **Target Platform**: Android (iOS/Windows support planned)
- **Data Source**: [OpenF1 API](https://openf1.org/) - Free historical F1 data
- **Local Storage**: SQLite (planned)
- **Architecture**: MVVM with dependency injection

## Prerequisites

- [.NET 9 SDK](https://dotnet.microsoft.com/download)
- .NET MAUI workload installed
- Android SDK (for Android development)
- IDE: Visual Studio 2022 or VS Code with C# Dev Kit

### Installing .NET MAUI Workload

```bash
dotnet workload install maui
```

## Getting Started

### Clone the Repository

```bash
git clone <repository-url>
cd f1
```

### Restore Dependencies

```bash
dotnet restore
```

### Build the Project

```bash
dotnet build
```

### Run on Android

```bash
dotnet build -t:Run -f net9.0-android
```

Or use your IDE's built-in Android emulator/device support.

## Project Structure

```
F1TelemetryApp/
├── App.xaml                 # Application resources and styles
├── AppShell.xaml            # Shell navigation structure
├── MainPage.xaml            # Initial page
├── Platforms/               # Platform-specific code
│   └── Android/             # Android-specific implementations
├── Resources/               # Images, fonts, styles
└── (To be added)
    ├── Models/              # Domain models (Driver, Session, Telemetry)
    ├── Services/            # API clients, data repositories
    ├── ViewModels/          # MVVM view models
    ├── Views/               # UI pages and components
    └── Data/                # Database context and entities
```

## Development Principles

This project follows these software engineering principles:

- **KISS** (Keep It Simple, Stupid): Straightforward, clear solutions
- **YAGNI** (You Aren't Gonna Need It): Implement only current requirements
- **SOLID**: Single responsibility, open/closed, Liskov substitution, interface segregation, dependency inversion
- **DRY** (Don't Repeat Yourself): Centralized, reusable logic

## API Information

### OpenF1 API

- **Base URL**: `https://api.openf1.org/v1/`
- **Authentication**: None required for historical data
- **Rate Limits**: No rate limits
- **Documentation**: [openf1.org](https://openf1.org/)

### Available Endpoints

- `/car_data` - Vehicle telemetry (~3.7 Hz)
- `/drivers` - Driver information per session
- `/laps` - Detailed lap data with sector times
- `/meetings` - Grand Prix and testing weekends
- `/sessions` - Practice, qualifying, race details
- `/weather` - Track conditions and weather data
- And 11+ more endpoints...

## Contributing

1. Create a feature branch: `git checkout -b feature/your-feature`
2. Follow the coding principles outlined in `guide.md`
3. Write clean, documented code with tests
4. Commit with clear messages
5. Open a pull request

## Roadmap

### Phase 1: Foundation (Current)
- [x] Project setup and configuration
- [x] Git repository initialization
- [ ] Define architecture and folder structure
- [ ] API client implementation
- [ ] Domain models

### Phase 2: Core Features
- [ ] Session list and details
- [ ] Driver and team information
- [ ] Historical race results
- [ ] Basic telemetry display

### Phase 3: Data & Persistence
- [ ] SQLite integration
- [ ] Caching strategy
- [ ] Offline support

### Phase 4: Visualization
- [ ] Telemetry charts and graphs
- [ ] Interactive data exploration
- [ ] Performance comparisons

### Phase 5: Polish
- [ ] UI/UX refinements
- [ ] Testing and bug fixes
- [ ] Performance optimization
- [ ] Documentation

## License

MIT License (or your preferred license)

## Acknowledgments

- [OpenF1](https://openf1.org/) for providing free F1 data
- .NET MAUI team for the excellent framework
- Formula 1 community for inspiration
