namespace F1TelemetryApp.ViewModels;

/// <summary>
/// ViewModel for the Introduction/Help page.
/// Provides information about Formula One, telemetry, and app usage.
/// </summary>
public class IntroViewModel : BaseViewModel
{
    public IntroViewModel()
    {
        Title = "Introduction to F1 Telemetry";
    }

    // Properties for expandable sections could be added here if needed
    // For now, keeping it simple with static content in XAML
}
