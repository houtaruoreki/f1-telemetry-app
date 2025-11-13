namespace F1TelemetryApp.Views;

using F1TelemetryApp.ViewModels;

/// <summary>
/// Introduction and help page explaining F1, telemetry, and app usage.
/// </summary>
public partial class IntroPage : ContentPage
{
    public IntroPage(IntroViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;
    }
}
