namespace F1TelemetryApp.ViewModels;

using System.Windows.Input;
using F1TelemetryApp.Helpers;

/// <summary>
/// ViewModel for the main/home page.
/// Provides navigation to main app sections.
/// </summary>
public class MainViewModel : BaseViewModel
{
    public MainViewModel()
    {
        Title = "F1 Telemetry";

        // Initialize commands
        NavigateToIntroCommand = new Command(async () => await NavigateToIntro());
        NavigateToSessionsCommand = new Command(async () => await NavigateToSessions());
        NavigateToDriversCommand = new Command(async () => await NavigateToDrivers());
        NavigateToSettingsCommand = new Command(async () => await NavigateToSettings());
    }

    public ICommand NavigateToIntroCommand { get; }
    public ICommand NavigateToSessionsCommand { get; }
    public ICommand NavigateToDriversCommand { get; }
    public ICommand NavigateToSettingsCommand { get; }

    private async Task NavigateToIntro()
    {
        await Shell.Current.GoToAsync(Constants.Routes.Intro);
    }

    private async Task NavigateToSessions()
    {
        await Shell.Current.GoToAsync($"//{Constants.Routes.SessionList}");
    }

    private async Task NavigateToDrivers()
    {
        await Shell.Current.GoToAsync($"//{Constants.Routes.DriverList}");
    }

    private async Task NavigateToSettings()
    {
        await Shell.Current.GoToAsync($"//{Constants.Routes.Settings}");
    }
}
